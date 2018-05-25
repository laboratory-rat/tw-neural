using Domain.General;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Collection
{
    public class TwitterSourceCollection : BaseEntity, IEntityGeneral
    {
        public string Title { get; set; }
        public string Comments { get; set; }

        public string UserId { get; set; }
        public EntityUser User { get; set; }

        public List<TwitterSource> Sorces { get; set; }
    }

    public class TwitterSource : BaseEntity, IEntityGeneral
    {
        public string ScreenName { get; set; }
        public long TwitterId { get; set; }

        public string Description { get; set; }
        public int StatusesCount { get; set; }

        public DateTime AccountCreatedAt { get; set; }

        public string Lang { get; set; }
        public string Location { get; set; }
        public bool GeoEnabled { get; set; }

        public string TimeZone { get; set; }

        public string ProfileImageUrl { get; set; }
        public string ProfileBackgroundImageUrl { get; set; }


        public string ProfileSidebarFillColor { get; set; }
        public string ProfileSidebarBorderColor { get; set; }
        public string ProfileLinkColor { get; set; }
        public string ProfileTextColor { get; set; }

        public string CollectionId { get; set; }
        public TwitterSourceCollection Collection { get; set; }

    }

}
