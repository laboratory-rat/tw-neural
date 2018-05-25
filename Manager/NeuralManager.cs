using AutoMapper;
using Domain.Neural;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.Neural;
using Manager.General;
using MRNeural.Domain.Net;
using MRNeural.Tools;
using Newtonsoft.Json;
using Repository;
using Repository.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tools.Storage;

namespace Manager
{
    public class NeuralManager : BaseManager
    {
        protected readonly INeuralStore _neuralStore;
        protected readonly StorageBlobClient _storageBlobClient;

        protected const string STORAGE_CONTAINER = "neurals";

        public NeuralManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper, INeuralStore neuralStore, StorageBlobClient storageBlobClient) : base(principal, userStore, mapper)
        {
            _neuralStore = neuralStore;
            _storageBlobClient = storageBlobClient;
        }

        public async Task<ApiResponse<IdNameModel>> Create(NetCreateModel model)
        {
            if (model == null) return Failed("No data get");

            var user = await CurrentUser();

            object net = null;
            if (model.FuncType == NeuralNetFuncType.SIGMOID_SIGMOID)
            {
                net = new SNeuralNet(model.InputLength, model.HiddenLayersCount, model.HiddenNeuronsCount, model.OutputLength, model.Skrew, model.Seed);
                ((SNeuralNet)net).Randomize();

            }
            else
            {
                net = new TNeuralNet(model.InputLength, model.HiddenLayersCount, model.HiddenNeuronsCount, model.OutputLength, model.Seed);
                ((TNeuralNet)net).Randomize();
            }



            if (net == null)
            {
                return Failed("Server error");
            }

            await _storageBlobClient.SetContainer(STORAGE_CONTAINER, true);
            var key = await _storageBlobClient.WriteText(JsonConvert.SerializeObject(net));

            if (string.IsNullOrWhiteSpace(key))
            {
                return Failed("Can not push neural net to the storage", "Try later");
            }

            var saveModel = new NeuralNet
            {
                IsTrained = false,
                OutputCount = model.OutputLength,
                TrainSet = null,
                NetFuncType = model.FuncType,
                Type = model.Type,
                Name = model.Name,
                InputCount = model.InputLength,
                HiddenLayersCount = model.HiddenLayersCount,
                HiddenCount = model.HiddenNeuronsCount,
                UserId = user.Id,
                Skrew = model.Skrew,
                Seed = model.Seed,
                StorageKey = key,
            };

            if(model.UseSeed && model.Seed.HasValue)
            {
                saveModel.Seed = model.Seed;
            }

            var saveResult = await _neuralStore.Add(saveModel);

            if (saveResult == null)
            {
                return Failed("Server error");
            }

            return Ok(new IdNameModel
            {
                Id = saveResult.Id,
                Name = saveResult.Name
            });
        }

        public async Task<ApiResponse<List<ShortInfoModel>>> Get(int take, int skip = 0)
        {
            var user = await CurrentUser();

            if (user == null)
                return Failed("Authorize first");

            var data = await _neuralStore.GetCustom(x => x.UserId == user.Id, x => x.UpdatedTime, true, take, skip);
            var count = await _neuralStore.Count();

            var response = data?.Select(x => new ShortInfoModel
            {
                Id = x.Id,
                Name = x.Name,
                UpdateTime = x.UpdatedTime,
                InputLength = x.InputCount,
                OutputLength = x.OutputCount,
            }).ToList() ?? new List<ShortInfoModel>();

            return Ok(response);
        }

        public async Task<ApiResponse<NeuralNetDisplayModel>> Get(string id)
        {
            var user = await CurrentUser();
            if (user == null)
                return Failed("Auth first");

            var net = await _neuralStore.GetFirst(x => x.UserId == user.Id && x.Id == id);
            if (net == null)
                return Failed("Not found");

            return Ok(new NeuralNetDisplayModel
            {
                Id = net.Id,
                HiddenNeuronsCount = net.HiddenCount,
                Seed = net.Seed,
                Skrew = net.Skrew,
                TypeString = net.Type.ToString(),
                HiddenLayersCount = net.HiddenLayersCount,
                InputLength = net.InputCount,
                Name = net.Name,
                OutputLength = net.OutputCount,
                UserId = user.Id,
                UserName = user.UserName,
                FuncTypeString = net.NetFuncType.ToString(),
                FuncType = net.NetFuncType,
                Type = net.Type
            });
        }

        public async Task<ApiResponse> Delete(string id)
        {
            var user = await CurrentUser();
            var net = await _neuralStore.GetFirst(x => x.Id == id && x.UserId == user.Id);

            var storageKey = net.StorageKey;

            if (net == null)
                return Failed("Not found");

            var r = await _neuralStore.DeleteSoft(id);
            if (r != 1)
                return Failed("Server error");

            if (!string.IsNullOrWhiteSpace(storageKey))
            {
                await _storageBlobClient.SetContainer(STORAGE_CONTAINER);
                await _storageBlobClient.Delete(storageKey);
            }

            return Ok();
        }
    }
}
