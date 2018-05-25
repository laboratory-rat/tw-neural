using Domain.Collection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Store
{
    public interface ITwitterSourcesStore : IBaseStore<TwitterSource>
    {

    }

    public class TwitterSourcesStore : BaseStore<TwitterSource>, ITwitterSourcesStore
    {
        public TwitterSourcesStore(ApiDbContext context) : base(context, x => x.TwitterSources) { }
    }
}
