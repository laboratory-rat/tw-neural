using Domain.User;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Store
{
    public interface IUserSocialsStore : IBaseStore<EntityUserSocial>
    {
        Task<EntityUserSocial> GetTwitter(string userId);
    }

    public class UserSocialsStore : BaseStore<EntityUserSocial>, IUserSocialsStore
    {
        public UserSocialsStore(ApiDbContext context) : base(context, x => x.UserSocials) { }

        public async Task<EntityUserSocial> GetTwitter(string userId)
        {
            return await GetFirst(x => x.Type == EntityUserSocialType.Twitter && x.UserId == userId);
        }
    }
}
