using AutoMapper;
using Domain.Enums;
using Domain.Neural;
using Domain.User;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.TrainSet;
using Manager.General;
using MRNeural;
using MRNeural.Domain.Set;
using MRNeural.Tools;
using Newtonsoft.Json;
using Repository;
using Repository.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Tools.Storage;
using Tweetinvi;
using Tweetinvi.Parameters;
using TwitterConnector;

namespace Manager
{
    public class TrainSetManager : BaseManager
    {
        protected readonly ITrainSetStore _trainSets;
        protected readonly ITwitterCollectionsStore _twitterCollectionsStore;
        protected readonly ITwitterSourcesStore _twitterSourcesStore;
        protected readonly IUserSocialsStore _userSocialsStore;
        protected readonly VReader _vReader;
        protected readonly StorageBlobClient _storageBlobClient;

        protected readonly TwitterClient _twitterClient;

        protected const string CONTAINER_NAME = "trainsets";

        public TrainSetManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper, ITrainSetStore trainSetStore,
            ITwitterCollectionsStore twitterCollectionsStore, ITwitterSourcesStore twitterSourcesStore, IUserSocialsStore userSocialsStore,
            TwitterClient twitterClient, VReader vReader, StorageBlobClient storageBlobClient) : base(principal, userStore, mapper)
        {
            _trainSets = trainSetStore;
            _twitterCollectionsStore = twitterCollectionsStore;
            _twitterSourcesStore = twitterSourcesStore;
            _userSocialsStore = userSocialsStore;
            _twitterClient = twitterClient;
            _vReader = vReader;
            _storageBlobClient = storageBlobClient;
        }

        public async Task<ApiResponse<IdNameModel>> Create(TrainSetCreateModel model)
        {
            var user = await CurrentUser();
            if (user == null)
            {
                return Failed("No user");
            }

            var entity = new TrainSet
            {
                Name = model.Name,
                SourceId = model.SourceId,
                UserId = user.Id,
                Type = model.Type,
                MaxCount = model.MaxCount,
                MinCount = model.MinCount,
                InputWordsCount = 3,
            };

            entity = await _trainSets.Add(entity);
            if (entity == null)
                return Failed("Server error");

            return Ok(new IdNameModel
            {
                Id = entity.Id,
                Name = entity.Name
            });
        }

        public async Task<ApiResponse<ListModel<TrainSetDisplayModel>>> Get(int take, int skip)
        {
            take = Math.Max(1, take);
            take = Math.Min(250, take);

            skip = Math.Max(0, skip);

            var user = await CurrentUser();
            if (user == null)
                return Failed("Can not find user");

            var list = await _trainSets.GetCustom(x => x.UserId == user.Id, x => x.UpdatedTime, true, take, skip);
            if (list == null)
                list = new List<TrainSet>();

            var total = await _trainSets.Count(x => x.UserId == user.Id);

            var data = list.Select(x => (TrainSetDisplayModel)x).ToList();
            return Ok(new ListModel<TrainSetDisplayModel>(take, skip, total, data));
        }

        public async Task<ApiResponse> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Failed("Id is required");
            }

            var user = await CurrentUser();
            if (user == null)
            {
                return Failed("Access denied");
            }

            var entity = await _trainSets.GetBy(x => x.Id == id && x.UserId == user.Id);
            if (entity == null)
            {
                return Failed("Not found");
            }

            var r = await _trainSets.DeleteSoft(id);
            if (r == 1)
            {
                return Ok();
            }

            return Failed("Server error");
        }

        #region scheduled tasks

        public async Task ScheduleUpdate()
        {
            var allToUpdate = await _trainSets.GetCustom(x => x.ScheduleStatus == Domain.Enums.ScheduleStatus.New, x => x.UpdatedTime, true, 5, 0);
            if (allToUpdate == null || !allToUpdate.Any())
                return;

            List<Task> tasks = new List<Task>();
            foreach (var d in allToUpdate)
            {
                d.ScheduleStatus = Domain.Enums.ScheduleStatus.Pending;
                tasks.Add(new TaskFactory().StartNew(() =>
                {

                    var t = _trainSets.Update(d);
                    t.Wait();
                }));
            }

            await Task.WhenAll(tasks);

            foreach (var d in allToUpdate)
            {
                await ScheduleProcess(d);
            }
        }

        protected async Task ScheduleProcess(TrainSet data)
        {
            switch (data.Type)
            {
                case TrainSetSourceType.Twitter:
                    data = await ScheduleProcessTwitter(data);
                    break;
                default:
                    data = await ScheduleProcessNotImplemented(data);
                    break;
            }

            await _trainSets.Update(data);
        }

        protected async Task<TrainSet> ScheduleProcessTwitter(TrainSet data)
        {
            var collection = await _twitterCollectionsStore.Get(data.SourceId);
            if (collection == null)
            {
                data.SetFailed("Can not find source data");
                return data;
            }

            var sources = await _twitterSourcesStore.GetBy(x => x.CollectionId == collection.Id);
            if (sources == null || !sources.Any())
            {
                data.SetFailed("Can not find any twitter sources");
                return data;
            }

            var user = await _userStore.FindByIdAsync(collection.UserId);
            if (user == null)
            {
                data.SetFailed("Can not find user data");
                return data;
            }

            var userTwitter = await _userSocialsStore.GetTwitter(user.Id);
            if (userTwitter == null)
            {
                data.SetFailed("No twitter access token");
                return data;
            }

            try
            {
                OAuthTwitter(userTwitter);
            }
            catch
            {
                data.SetFailed("Error with twitter connections");
                return data;
            }

            // upload twitter data

            int min = data.MinCount;
            min = Math.Max(1, min);

            int max = data.MaxCount;
            max = Math.Max(100, max);
            max = Math.Min(10000, max);

            if (min > max)
            {
                var t = min;
                max = min;
                min = t;
            }

            int perSource = (int)Math.Ceiling((double)max / sources.Count);
            var entity = new TrainSetModel { };

            var rawData = new StringBuilder();
            int total = 0;

            var regex = new Regex("http[s]?://[A-Za-z0-9._-]*");

            foreach (var screen in sources)
            {
                long? lastId = null;
                int count = 0;
                var twetterUser = await UserAsync.GetUserFromId(screen.TwitterId);

                while (perSource > count)
                {
                    var @params = new UserTimelineParameters
                    {
                        MaximumNumberOfTweetsToRetrieve = 50,
                    };

                    if (lastId.HasValue)
                    {
                        @params.SinceId = lastId.Value;
                    }

                    var tweets = await TimelineAsync.GetUserTimeline(twetterUser, @params);
                    if (tweets == null || !tweets.Any())
                    {
                        break;
                    }

                    count += tweets.Count();
                    foreach (var t in tweets)
                    {
                        rawData.Append(regex.Replace(t.FullText, string.Empty));
                    }
                }

                total += count;
            }

            if (total < min)
            {
                data.SetFailed($"Not enough data avaliable. Avaliable : {total}. Minimum: {min}");
                return data;
            }

            WordBag wb = WordBag.CreateToWords(rawData.ToString(), data.InputWordsCount + 1);
            _vReader.UploadBinary();

            List<Tuple<string[], string[]>> stringList = new List<Tuple<string[], string[]>>();
            List<Tuple<double[], double[]>> doubleList = new List<Tuple<double[], double[]>>();

            foreach (var s in wb.Read())
            {
                var vectorList = new List<double[]>();
                var wordList = new List<string>();

                foreach (var ss in s)
                {
                    var word = _vReader.Vocab.GetRepresentationOrNullFor(ss);
                    if (word == null)
                        break;

                    vectorList.Add(ss.Select(x => (double)x).ToArray());
                    wordList.Add(ss);
                }

                if (vectorList.Count < s.Length)
                    continue;

                var tmpVector = new List<double>();
                foreach(var i in vectorList.Take(data.InputWordsCount))
                {
                    tmpVector.AddRange(i);
                }

                doubleList.Add(new Tuple<double[], double[]>(tmpVector.ToArray(), vectorList.Last().ToArray()));
                stringList.Add(new Tuple<string[], string[]>(wordList.Take(wordList.Count - 1).ToArray(), new string[1] { wordList.Last() }));
            }

            entity.Data = doubleList.ToArray();
            entity.StringSource = stringList.ToArray();

            string dataString = JsonConvert.SerializeObject(entity);

            await _storageBlobClient.SetContainer(CONTAINER_NAME, true);
            var storageKey = await _storageBlobClient.WriteText(dataString);

            if (string.IsNullOrWhiteSpace(storageKey))
            {
                data.SetFailed("Can not upload train set to storage");
                return data;
            }

            data.StorageKey = storageKey;
            data.SetReady();

            data.ExamplesCount = entity.Data.Count();

            return data;
        }

        protected async Task<TrainSet> ScheduleProcessNotImplemented(TrainSet data)
        {
            data.SetFailed("Not implimented yet");
            return data;
        }

        protected void OAuthTwitter(EntityUserSocial twitterSocial)
        {
            _twitterClient.OAuthUser(twitterSocial.ConsumerKey, twitterSocial.ConsumerSecret, twitterSocial.Token, twitterSocial.TokenSecret);
        }

        #endregion
    }
}
