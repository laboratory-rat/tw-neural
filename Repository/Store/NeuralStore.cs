using Domain.Neural;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Store
{
    public interface INeuralStore : IBaseStore<NeuralNet>
    {

    }

    public class NeuralStore : BaseStore<NeuralNet>, INeuralStore
    {
        public NeuralStore(ApiDbContext context) : base(context, x => x.NeuralNets) { }
    }
}
