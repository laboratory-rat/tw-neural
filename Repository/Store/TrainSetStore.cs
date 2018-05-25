using Domain.Neural;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Store
{
    public interface ITrainSetStore : IBaseStore<TrainSet>
    {

    }

    public class TrainSetStore : BaseStore<TrainSet>, ITrainSetStore
    {
        public TrainSetStore(ApiDbContext context) : base(context, x => x.TrainSets) { }
    }
}
