using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Manager;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    public class BaseController : Controller
    {
        protected UserManager _userManager;

        public BaseController(UserManager managerUser)
        {
            _userManager = managerUser;
        }
    }
}