using Domain.Collection;
using Domain.Dictionary;
using Domain.General;
using Domain.Neural;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.User
{
    public class EntityUser : IdentityUser, IEntityGeneral
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Tel { get; set; }
        public string Locale { get; set; }
        public DateTime Birthday { get; set; }
        public string PictureUrl { get; set; }

        public List<EntityUserSocial> Socials { get; set; }

        public List<TwitterSourceCollection> TwitterCollections { get; set; }

        public List<TwitterDictionary> TwitterDictionaries { get; set; }

        public List<NeuralNet> NeuralNets { get; set; }

        public List<TrainSet> TrainSets { get; set; }

        public DateTime CreatedTime { get; set; }
        public DateTime UpdatedTime { get; set; }
        public EntityState State { get; set; } = EntityState.Active;

        public EntityUser()
        {

        }

        public EntityUser(string email, string first, string last) : base(email)
        {
            FirstName = first;
            LastName = last;
            Email = UserName = email;
        }


    }
}
