using AutoMapper;
using Domain.Collection;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.Collections;
using Manager.General;
using Microsoft.Extensions.Logging;
using Repository;
using Repository.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tools;
using Tweetinvi;
using TwitterConnector;

namespace Manager
{
    public interface ICollectionsManager
    {
        Task<ApiResponse> Create(CollectionUpdateModel model);
        Task<ApiResponse<CollectionUpdateModel>> Get(string id);
        Task<ApiResponse<ListModel<CollectionDisplayShortModel>>> Get(int skip, int take);
        Task<ApiResponse> Update(CollectionUpdateModel model);
        Task<ApiResponse> Delete(string id);
    }

    public class CollectionsManager : BaseManager, ICollectionsManager
    {
        readonly protected ITwitterSourcesStore _twitterSourcesStore;
        readonly protected ITwitterCollectionsStore _twitterCollectionsStore;
        readonly protected TwitterClient _client;
        readonly protected IUserSocialsStore _userSocialsStore;

        public CollectionsManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper, ITwitterCollectionsStore twitterCollectionsStore, ITwitterSourcesStore twitterSourcesStore, TwitterClient client, IUserSocialsStore userSocialsStore) : base(principal, userStore, mapper)
        {
            _twitterSourcesStore = twitterSourcesStore;
            _twitterCollectionsStore = twitterCollectionsStore;
            _userSocialsStore = userSocialsStore;
            _client = client;
        }

        public async Task<ApiResponse> Create(CollectionUpdateModel model)
        {
            var user = await CurrentUser();
            var social = await _userSocialsStore.GetTwitter(user.Id);

            if (social == null)
            {
                Warning("No user twitter account");
                return Failed("No twitter account");
            }

            var entity = _mapper.Map<TwitterSourceCollection>(model);
            entity.UserId = _currentUser.Id;

            entity = await _twitterCollectionsStore.Add(entity);

            if (model.SourcesIds != null && model.SourcesIds.Any())
            {

                OAuthTwitter(_client, social);

                var sourcesList = new List<TwitterSource>();

                int skipFactor = 0;
                while (true)
                {
                    var usersToSearch = model.SourcesIds.Skip(50 * skipFactor).Take(50);
                    var uploadedUsers = User.UserFactory.GetUsersFromIds(usersToSearch);

                    foreach (var uploaded in uploadedUsers)
                    {
                        var u = _mapper.Map<TwitterSource>(uploaded);
                        u.Id = null;
                        u.TwitterId = uploaded.Id;
                        u.AccountCreatedAt = uploaded.CreatedAt;
                        u.CollectionId = entity.Id;

                        sourcesList.Add(u);
                    }

                    if (usersToSearch.Count() < 50)
                        break;

                    skipFactor++;
                }

                await _twitterSourcesStore.Add(sourcesList);
            }

            return Ok();
        }

        public async Task<ApiResponse<CollectionUpdateModel>> Get(string id)
        {
            var entity = await _twitterCollectionsStore.Get(id);
            if (entity == null)
            {
                return Failed("Not found");
            }

            if (entity.UserId != (await CurrentUser())?.Id)
            {
                return Failed("Access denied");
            }

            var model = _mapper.Map<CollectionUpdateModel>(entity);

            var sources = await _twitterSourcesStore.GetBy(x => x.CollectionId == entity.Id);
            if (sources != null && sources.Any())
            {
                model.SourcesIds = sources.Select(x => x.TwitterId).ToList();
            }
            else
            {
                model.SourcesIds = new List<long>();
            }

            return Ok(model);
        }

        public async Task<ApiResponse<ListModel<CollectionDisplayShortModel>>> Get(int skip, int take)
        {
            skip = Math.Min(0, skip);
            take = Math.Max(1, take);

            var userId = (await CurrentUser())?.Id;

            var list = new ListModel<CollectionDisplayShortModel>();
            var entities = await _twitterCollectionsStore.GetCustom(x => x.UserId == userId, x => x.UpdatedTime, true, take, skip);

            var totalCount = await _twitterCollectionsStore.Count();

            list = new ListModel<CollectionDisplayShortModel>(take, skip, totalCount, new List<CollectionDisplayShortModel>());

            if (entities != null && entities.Any())
            {
                list.Data = entities.Select(x => new CollectionDisplayShortModel
                {
                    Id = x.Id,
                    CreatedAt = x.CreatedTime.ToTimestamp(),
                    Title = x.Title
                })?.ToList();

                var tasks = new List<Task<Tuple<string, int>>>();
                foreach (var e in entities)
                {
                    tasks.Add(new TaskFactory().StartNew(() =>
                    {
                        var id = e.Id;
                        var s = _twitterSourcesStore.Count(x => x.CollectionId == id);
                        s.Wait();
                        return new Tuple<string, int>(id, s.Result);
                    }));
                }

                await Task.WhenAll(tasks);
                foreach (var t in tasks)
                {
                    var l = list.Data.FirstOrDefault(x => x.Id == t.Result.Item1);
                    if (l != null)
                    {
                        l.SourcesCount = t.Result.Item2;
                    }
                }
            }

            return Ok(list);
        }

        public async Task<ApiResponse> Update(CollectionUpdateModel model)
        {
            var entity = await _twitterCollectionsStore.Get(model.Id);
            if (entity == null)
            {
                return Failed("Not found");
            }

            if (entity.UserId != (await CurrentUser())?.Id)
            {
                return Failed("Access denied");
            }

            var social = _userSocialsStore.GetTwitter((await CurrentUser()).Id);
            if (social == null)
            {
                return Failed("Twitter account not found");
            }

            entity.Title = model.Title;
            entity.Comments = model.Comments;

            if ((await _twitterCollectionsStore.Update(entity)) != 1)
            {
                _logger.LogError("Update collection failed", entity);
                return Failed("Server unavaliable");
            }

            var sources = await _twitterSourcesStore.GetBy(x => x.CollectionId == entity.Id);

            var toDelete = new List<TwitterSource>(sources ?? new List<TwitterSource>());
            var toInsert = new List<long>();

            foreach (var newSource in model.SourcesIds)
            {
                var exists = toDelete.FirstOrDefault(x => x.TwitterId == newSource);
                if (exists != null)
                {
                    toDelete.Remove(exists);
                }
                else
                {
                    toInsert.Add(newSource);
                }
            }

            if (toDelete.Any())
            {
                if ((await _twitterSourcesStore.DeleteSoft(toDelete)) != toDelete.Count)
                {
                    _logger.LogError("Error deleting sources", toDelete);
                    return Failed("Service unavaliable");
                }
            }

            if (toInsert.Any())
            {
                var list = new List<TwitterSource>();

                int skipFactor = 0;
                while (true)
                {
                    var usersToSearch = model.SourcesIds.Skip(50 * skipFactor).Take(50);
                    var uploadedUsers = User.UserFactory.GetUsersFromIds(usersToSearch);

                    foreach (var uploaded in uploadedUsers)
                    {
                        var u = _mapper.Map<TwitterSource>(uploaded);
                        u.Id = null;
                        u.TwitterId = uploaded.Id;
                        u.AccountCreatedAt = uploaded.CreatedAt;
                        u.CollectionId = entity.Id;

                        list.Add(u);
                    }

                    if (usersToSearch.Count() < 50)
                        break;

                    skipFactor++;
                }

                await _twitterSourcesStore.Add(list);
            }


            return Ok();
        }

        public async Task<ApiResponse> Delete(string id)
        {
            var entity = await _twitterCollectionsStore.Get(id);
            if (entity == null)
            {
                return Failed("Not found");
            }

            if (entity.UserId != (await CurrentUser())?.Id)
            {
                return Failed("Access denied");
            }

            if (entity.State != Domain.General.EntityState.Deleted)
            {
                if ((await _twitterCollectionsStore.DeleteSoft(id)) != 1)
                {
                    _logger.LogError("Can not delete collection " + id);
                    return Failed("Service unavaliable");
                }
            }

            return Ok();
        }
    }
}
