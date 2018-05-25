using Domain.Collection;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Repository.Store
{
    public interface ITwitterCollectionsStore : IBaseStore<TwitterSourceCollection>
    {
    }

    public class TwitterCollectionsStore : BaseStore<TwitterSourceCollection>, ITwitterCollectionsStore
    {
        public TwitterCollectionsStore(ApiDbContext context) : base(context, x => x.TwitterSouceCollections) { }


    }
}
