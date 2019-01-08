using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VipcoSageX3.Models.Machines;
using VipcoSageX3.ViewModels;

namespace VipcoSageX3.Services
{
    public interface IUserService
    {
        UserViewModel Authenticate(string username, string password);

        Task<UserViewModel> AuthenticateAsync(string username, string password);
    }
}
