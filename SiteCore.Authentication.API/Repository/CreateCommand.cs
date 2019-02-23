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
    public class CreateCommand : ICreateCommand
    {
        SqlConnection conn = null;
        public static readonly string _connection = System.Configuration.ConfigurationManager.ConnectionStrings["AuthenticationDB"].ConnectionString;

        public CreateCommand()
        {
            conn = new SqlConnection(_connection);
        }

        /// <summary>
        /// Insert new user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool RegisterNewUser(User user)
        {
            int userId = 0;
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();


                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@Username", SqlDbType.NVarChar, 50, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.Username),
                    new SqlParameter("@FirstName", SqlDbType.NVarChar, 20, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.FirstName),
                    new SqlParameter("@LastName", SqlDbType.NVarChar, 20, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.LastName),
                    new SqlParameter("@Email", SqlDbType.NVarChar, 20, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.Email),
                    new SqlParameter("@Password", SqlDbType.NVarChar, 20, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.Password),
                    new SqlParameter("@IsActive", SqlDbType.Bit, 2, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.IsActive),
                    new SqlParameter("@ActivationCode", SqlDbType.NVarChar, 50, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, user.ActivationCode.ToString()),
                    new SqlParameter("@Id", SqlDbType.Int, 0, ParameterDirection.Output, true, 0, 0, null, DataRowVersion.Current, null),
                };

                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "sp_insert_newUser", param);

                userId = Convert.ToInt32(param[7].Value);
            }
            catch (SqlException ex)
            {
                return false;
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();

                foreach (Role role in user.Roles)
                {
                    UserRole ur = new UserRole();
                    ur.RoleId = role.RoleId;
                    ur.UserId = userId;

                    this.InsertUserRoles(ur, userId);
                }
            }

            return true;
        }

        /// <summary>
        /// Insert new UserRole
        /// </summary>
        /// <param name="userRole"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private bool InsertUserRoles(UserRole userRole, int userId)
        {
            try
            {
                if (conn.State != ConnectionState.Open)
                    conn.Open();


                SqlParameter[] param = new SqlParameter[]
                {
                    new SqlParameter("@RoleId", SqlDbType.Int, 0, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, userRole.RoleId),
                    new SqlParameter("@UserId", SqlDbType.Int, 0, ParameterDirection.Input, true, 0, 0, null, DataRowVersion.Current, userId)
                };

                SqlHelper.ExecuteNonQuery(conn, CommandType.StoredProcedure, "sp_insert_newUserRole", param);
            }
            catch (SqlException ex)
            {
                return false;
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

            return true;
        }


    }
}