using Domain.Collection;
using Domain.General;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Dictionary
{
    public class TwitterDictionary : BaseEntity, IEntityGeneral
    {
        public string Title { get; set; }

        public List<DictionaryLanguage> Languages { get; set; }

        public int MaxStatuses { get; set; }
        public int MaxDepth { get; set; }

        public DictionaryStatus Status { get; set; } = DictionaryStatus.New;

        public string CollectionId { get; set; }
        public TwitterSourceCollection Collection { get; set; }

        public string UserId { get; set; }
        public EntityUser User { get; set; }
    }

    public enum DictionaryStatus
    {
        New,
        Process,
        Ready,
        Failed,
        Undefined
    }
}
