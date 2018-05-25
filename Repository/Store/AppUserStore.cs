using Domain.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public interface IApiUserStore : IUserStore<EntityUser>
    {
        Task<EntityUser> FindByEmailAsync(string normalizedEmail);
        Task<int> CreateAsync(EntityUser user);
    }

    public class ApiUserStore : UserStore<EntityUser>
    {
        ApiDbContext _context => (ApiDbContext)Context;

        public ApiUserStore(DbContext context, IdentityErrorDescriber describer = null) : base(context, describer) { }

        public ApiUserStore(ApiDbContext context) : base(context) { }
    }
}
