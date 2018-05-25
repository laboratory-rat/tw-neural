using Domain.General;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.User
{
    public class EntityUserSocial : BaseEntity, IEntityGeneral
    {
        public EntityUserSocialType Type { get; set; }
        public long ExternalId { get; set; }
        public string Token { get; set; }
        public DateTime TokenExpires { get; set; }

        // for twitter
        public string TokenSecret { get; set; }
        public string ConsumerKey { get; set; }
        public string ConsumerSecret { get; set; }

        public string AccountName { get; set; }

        public string UserId { get; set; }
        public EntityUser User { get; set; }
    }

    public enum EntityUserSocialType
    {
        Undefined,
        Facebook,
        Google,
        Twitter
    }
}
