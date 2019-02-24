using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

using SiteCore.AuthenticationAPI.Repository;
using SiteCore.Domain;
using SiteCore.AuthenticationAPI.Controllers;
using SiteCore.Helper;

namespace SiteCore.AuthenticationAPI.Tests
{
    [TestClass]
    public class TestQuery
    {
        [TestMethod]
        public async void GetSingleUser()
        {
            string expectedEmail = "rajakamil@gmail.com";

            ICommand command = new QueryCommand();
            User user = await command.GetSingleUserByIdAsync(1);

            //AuthenticationController auth = new AuthenticationController();
            //HttpResponseMessage msg = await auth.GetUser(1);

            Assert.AreEqual(expectedEmail, user.Email);

        }

        [TestMethod]
        public void Authenticate()
        {
            User user = new User();
            user.Username = "rajakamil2";
            user.Password = "rajakamil123";

            string usernameApi = "Api1";
            string passwordApi = "pass123";

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usernameApi}:{passwordApi}"));

            var apiUri = "http://localhost/SiteCoreAPI";
            var client = new RestClient(apiUri);

            var request = new RestRequest(string.Format("/api/Authentication/Authenticate/{0}/{1}", user.Username, user.Password), Method.GET);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + encoded);
            request.RequestFormat = DataFormat.Json;

            IRestResponse<ResponseMessage> response = client.Execute<ResponseMessage>(request);
            
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }


        }

        [TestMethod]
        public void RegisterNewUser()
        {
            var role = new Role();

            User user = new User();
            user.Username = "rajakamil99";
            user.FirstName = "Raja Mohd99";
            user.LastName = "Kamil";
            user.Email = "rajakamil99@gmail.com";
            user.Password = "rajakamil123";
            user.IsActive = true;

            IList<Role> roles = new List<Role>();
            Role role1 = new Role();
            role1.RoleId = 2;
            role1.RoleName = "User";
            roles.Add(role1);

            user.Roles = roles;

            string usernameApi = "Api1";
            string passwordApi = "pass123";

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usernameApi}:{passwordApi}"));

            var apiUri = "http://localhost/SiteCoreAPI";
            var client = new RestClient(apiUri);

            var request = new RestRequest(string.Format("/api/Authentication/Register"), Method.POST);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + encoded);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(user);

            IRestResponse<bool> response2 = client.Execute<bool>(request);

            if (response2.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }

            
        }

        [TestMethod]
        public void AuthenticateUser()
        {
            var role = new Role();

            User user = new User();
            user.Username = "rajakamil2";
            user.Password = "rajakamil123";         

            string usernameApi = "Api1";
            string passwordApi = "pass123";

            string encoded = Convert.ToBase64String(Encoding.GetEncoding("ISO-8859-1").GetBytes($"{usernameApi}:{passwordApi}"));

            var apiUri = "http://localhost/SiteCoreAPI";
            var client = new RestClient(apiUri);

            var request = new RestRequest(string.Format("/api/Authentication/AuthenticateUserAsync/{0}/{1}", user.Username, user.Password), Method.GET);

            request.AddHeader("Content-Type", "multipart/form-data");
            request.AddHeader("Authorization", "Basic " + encoded);
            request.RequestFormat = DataFormat.Json;
            
            IRestResponse<ResponseMessage<Role>> response = client.Execute<ResponseMessage<Role>>(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Assert.IsTrue(true);
            }
            else
            {
                Assert.IsTrue(false);
            }


        }


    }
}
