using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using SiteCore.Domain;

namespace SiteCore.AuthenticationAPI.Repository
{
    public interface ICreateCommand
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool RegisterNewUser(User user);
    }
}