using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Configuration;
using System.Text;

using RestSharp;
using SiteCore.Domain;
using SiteCore.Helper;
using RestSharp.Deserializers;

namespace SiteCore.Web.Client.ClientHttp
{
    public static class ApiRepository
    {
        public static async Task<bool> AuthenticateAsync(User user)
        {
            var message = "Successfully created new user";
            ResponseMessage rm = new ResponseMessage()
            {
                ReturnMessage = message,
                Status = true
            };           

            string usernameApi = "Api1";
            string passwordApi = "pass123";
            
            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usernameApi}:{passwordApi}"));

            var apiUri = ConfigurationManager.AppSettings["auth.Api.Uri"].ToString();
            var client = new RestClient(apiUri);

            var request = new RestRequest(string.Format("/api/Authentication/Authenticate/{0}/{1}", user.Username, user.Password), Method.GET);
            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + encoded);

            IRestResponse<ResponseMessage> response2 = await client.ExecuteTaskAsync<ResponseMessage>(request);

            RestSharp.Deserializers.JsonDeserializer json = new JsonDeserializer();

            ResponseMessage<Role> responseRole = json.Deserialize<ResponseMessage<Role>>(response2);

            if (response2.StatusCode == HttpStatusCode.OK)
            {
                string ErrorMessage = String.Format("Error from API : {0}", response2.Data.ReturnMessage);

            }
            else
            {
                string ErrorMessage = String.Format("Error from API : {0}", response2.Data.ReturnMessage);                
            }

            return response2.Data.Status;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public static async Task<Role> AuthenticateUserAsync(User user)
        {
            var role = new Role();

            string usernameApi = "Api1";
            string passwordApi = "pass123";

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usernameApi}:{passwordApi}"));

            var apiUri = ConfigurationManager.AppSettings["auth.Api.Uri"].ToString();
            var client = new RestClient(apiUri);

            try
            {

                var request = new RestRequest(string.Format("/api/Authentication/AuthenticateUserAsync/{0}/{1}", user.Username, user.Password), Method.GET);
                request.AddHeader("Content-Type", "multipart/form-data");
                request.AddHeader("Authorization", "Basic " + encoded);

                IRestResponse<ResponseMessage<Role>> response2 = await client.ExecuteTaskAsync<ResponseMessage<Role>>(request);

                if (response2.StatusCode == HttpStatusCode.OK)
                {
                    role = response2.Data.ReturnResult;

                }
                else
                {
                    string ErrorMessage = String.Format("Error from API : {0}", response2.Data.ReturnMessage);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return role;
        }
    }
}