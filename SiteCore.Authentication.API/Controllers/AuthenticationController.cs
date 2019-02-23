using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Threading.Tasks;

using SiteCore.Domain;
using SiteCore.AuthenticationAPI.Repository;
using SiteCore.Helper;
using SiteCore.AuthenticationAPI.Helper;
using WebApi.OutputCache.V2;

namespace SiteCore.AuthenticationAPI.Controllers
{
    [AutoInvalidateCacheOutput]
    public class AuthenticationController : ApiController
    {               

        /// <summary>
        /// api/Authentication/GetUser/{id}
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Authentication/GetUser/{id}")]
        [CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
        public async Task<HttpResponseMessage> GetUser(int id)
        {
            ICommand command = new QueryCommand();

            User user = await command.GetSingleUserByIdAsync(id);

            if (user != null)
            {
                var message = "Success user " + user.Email;

                var response = Request.CreateResponse<ResponseMessage<User>>(HttpStatusCode.OK, new ResponseMessage<User> { Status = true, ReturnMessage = message, ReturnResult = user });
                
                return response;

            }

            return Request.CreateResponse<ResponseMessage>(HttpStatusCode.OK,
                    new ResponseMessage
                    {
                        Status = false,
                        ReturnMessage = "This user does not exist"
                    });            
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        
        [HttpGet]
        [Route("api/Authentication/Authenticate/{username}/{password}")]
        [CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
        public async Task<HttpResponseMessage> AuthenticateUser(string username, string password)
        {
            ICommand command = new QueryCommand();
            var status = await command.AuthenticateAsync(username, password);

            if (status)
            {
                var message = "Successfully authenticated";
                return Request.CreateResponse<ResponseMessage>(HttpStatusCode.OK,
                    new ResponseMessage
                    {
                        Status = true,
                        ReturnMessage = message
                    });

            }

            return Request.CreateResponse<ResponseMessage>(HttpStatusCode.OK,
                    new ResponseMessage
                    {
                        Status = false,
                        ReturnMessage = "This user does not exist"
                    });

        }

        [HttpPost]
        [Route("api/Authentication/Register")]
        [CacheOutput(ClientTimeSpan = 50, ServerTimeSpan = 50)]
        public HttpResponseMessage RegisterUser([FromBody]User user)
        {
            ICreateCommand command = new CreateCommand();
            var status = command.RegisterNewUser(user);

            if (status)
            {
                var message = "Successfully created new user";
                return Request.CreateResponse<ResponseMessage>(HttpStatusCode.OK,
                    new ResponseMessage
                    {
                        Status = true,
                        ReturnMessage = message
                    });

            }

            return Request.CreateResponse<ResponseMessage>(HttpStatusCode.OK,
                    new ResponseMessage
                    {
                        Status = false,
                        ReturnMessage = "Error on inserting new user"
                    });
        }

        
    }
}