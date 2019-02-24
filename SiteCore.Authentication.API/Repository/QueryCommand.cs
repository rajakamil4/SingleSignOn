using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ApplicationBlocks.Data;

using SiteCore.Domain;


namespace SiteCore.AuthenticationAPI.Repository
{
    public class QueryCommand : ICommand
    {
        SqlConnection conn = null;
        public static readonly string _connection = System.Configuration.ConfigurationManager.ConnectionStrings["AuthenticationDB"].ConnectionString;
        public QueryCommand()
        {
            conn = new SqlConnection(_connection);
        }

        /// <summary>
        /// Get user by UserId Async
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<User> GetSingleUserByIdAsync(int? id)
        {
            var user = new User();

            SqlParameter param = new SqlParameter();
            param.ParameterName = "@UserId";
            param.Value = id;

            var asyncConnectionString = new SqlConnectionStringBuilder(_connection)
            {
                AsynchronousProcessing = true
            }.ToString();

            var connA = new SqlConnection(asyncConnectionString);

            try
            {               

                var spName = "sp_get_singleUser";
                
                if (connA.State != ConnectionState.Open)
                    await connA.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Parameters.Add(param);
                    cmd.Connection = connA;
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;                        

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {                            

                        while (await reader.ReadAsync())
                        {

                            user.UserId = Convert.ToInt32(reader["UserId"]);
                            user.Username = reader["Username"].ToString();
                            user.FirstName = reader["FirstName"].ToString();
                            user.LastName = reader["LastName"].ToString();
                            user.Email = reader["Email"].ToString();
                            user.Password = reader["Password"].ToString();
                            user.IsActive = Convert.ToBoolean(reader["IsActive"]);

                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connA.State == ConnectionState.Open)
                    connA.Close();

                user.Roles = this.GetRoles(id);
            }

            return await Task.FromResult<User>(user);
        }

        /// <summary>
        /// Get user by UserId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public User GetSingleUserById(int? id)
        {
            var user = new User();

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@UserId", SqlDbType.Int)
            };

            param[0].Value = id;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "sp_get_singleUser", param);

                while (reader.Read())
                {

                    user.UserId = Convert.ToInt32(reader["UserId"]);
                    user.Username = reader["Username"].ToString();
                    user.FirstName = reader["FirstName"].ToString();
                    user.LastName = reader["LastName"].ToString();
                    user.Email = reader["Email"].ToString();
                    user.Password = reader["Password"].ToString();
                    user.IsActive = Convert.ToBoolean(reader["IsActive"]);                    

                }
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                user.Roles = this.GetRoles(id);
            }

            return user;
        }

        /// <summary>
        /// Authenticate user
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool Authenticate(string username, string password)
        {
            bool status = false;

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@Username", SqlDbType.NVarChar, 50),
                new SqlParameter("@Password", SqlDbType.NVarChar, 50)
            };

            param[0].Value = username;
            param[1].Value = password;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "sp_authenticate", param);

                while (reader.Read())
                {
                    status = true;

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
                
            }

            return status;
        }

        /// <summary>
        /// Authenticate user async
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<bool> AuthenticateAsync(string username, string password)
        {
            bool status = false;
            
            var asyncConnectionString = new SqlConnectionStringBuilder(_connection)
            {
                AsynchronousProcessing = true
            }.ToString();

            var connA = new SqlConnection(asyncConnectionString);

            try
            {

                var spName = "sp_authenticate";

                if (connA.State != ConnectionState.Open)
                    await connA.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 20).Value = password;
                    cmd.Connection = connA;
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            status = true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connA.State == ConnectionState.Open)
                    connA.Close();                
            }

            return await Task.FromResult<bool>(status);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<Role> AuthenticateUserAsync(string username, string password)
        {
            var user = new User();
            Role role = new Role();
            bool status = false;
                        
            var asyncConnectionString = new SqlConnectionStringBuilder(_connection)
            {
                AsynchronousProcessing = true
            }.ToString();

            var connA = new SqlConnection(asyncConnectionString);

            try
            {

                var spName = "sp_authenticate";

                if (connA.State != ConnectionState.Open)
                    await connA.OpenAsync();

                using (var cmd = new SqlCommand())
                {
                    cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 50).Value = username;
                    cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 20).Value = password;
                    cmd.Connection = connA;
                    cmd.CommandText = spName;
                    cmd.CommandType = CommandType.StoredProcedure;

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        while (await reader.ReadAsync())
                        {
                            status = true;
                            user.UserId = Convert.ToInt32(reader["UserId"]);                            
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (connA.State == ConnectionState.Open)
                    connA.Close();

                if (status)
                {
                    var roles = this.GetRoles(user.UserId);
                    role.RoleId = roles[0].RoleId;
                    role.RoleName = roles[0].RoleName;
                }
            }

            return role;

        }
        /// <summary>
        /// Get Roles by userId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IList<Role> GetRoles(int? id)
        {
            var roles = new List<Role>();

            SqlParameter[] param = new SqlParameter[]
            {
                new SqlParameter("@UserId", SqlDbType.Int)
            };

            param[0].Value = id;

            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();

                SqlDataReader reader = SqlHelper.ExecuteReader(conn, CommandType.StoredProcedure, "sp_get_userRoles", param);

                while (reader.Read())
                {
                    var role = new Role();
                    
                    role.RoleId = Convert.ToInt32(reader["RoleId"]);
                    role.RoleName = Enum.GetName(typeof(RoleStatus),Convert.ToInt32(reader["RoleId"]));

                    roles.Add(role);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return roles;
        }

        
    }
}