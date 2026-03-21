using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using Dapper;
using Microsoft.SqlServer.Server;
using Microsoft.IdentityModel.Tokens;

namespace JobPortal.Infrastructure.Services
{
    public interface ICompareServices
    {
        Task<CompareFormData> CompareData(int formId, string UserId, string profileId);
        Task<IEnumerable<UserListForCompare>> GetUserListByFormId(int formId, bool isDataEntryOnly = false);

        Task<IEnumerable<RatingScore>> GetRatingScore(int formId, string UserId);
    }

    public class CompareServices : ICompareServices
    {
        private readonly AppDbContext _context;
        public CompareServices(AppDbContext db) => _context = db;
        public async Task<CompareFormData> CompareData(int formId, string UserId, string profileId)
        {
            var data = new CompareFormData();
            var connection = _context.Database.GetDbConnection();

            // The stored procedure must return two SELECT statements in order.
            string storedProc = "dbo.CompareForms";

            // Check connection state and open if necessary
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandText = storedProc;
                command.CommandType = CommandType.StoredProcedure; // Use CommandType.StoredProcedure if preferred

                var parameters = new DynamicParameters();
                parameters.Add("@FormId", formId);
                if (!string.IsNullOrEmpty(profileId))
                {
                    parameters.Add("@ProfileId", profileId);
                }
                if (!string.IsNullOrEmpty(UserId))
                {
                    parameters.Add("@UserIds", UserId);
                }
                // Add parameter(s) safely
                var param = command.CreateParameter();
                param.ParameterName = "@FormId";
                param.Value = formId;
                command.Parameters.Add(param);

                if (!string.IsNullOrEmpty(UserId))
                {
                    var param2 = command.CreateParameter();
                    param2.ParameterName = "@UserIds";
                    param2.Value = UserId;
                    command.Parameters.Add(param2);
                }

                using (var multi = await connection.QueryMultipleAsync("CompareForms", parameters, commandType: CommandType.StoredProcedure))
                {
                    data.FormDetails = (await multi.ReadAsync<FormDetail>()).ToList();
                    data.FormDatas = (await multi.ReadAsync<FormData>()).ToList();  // ❌ if no rows, still must read
                    data.Users = (await multi.ReadAsync<UserListForCompare2>()).ToList();
                }

                //// Execute the command and get the data reader
                //using (var reader = await command.ExecuteReaderAsync())
                //{
                //    // 1. Process the FIRST Result Set (Customers)
                //    data.FormDetails = await MapFormDetailAsync(reader);

                //    // 2. Advance to the SECOND Result Set (Orders)
                //    if (await reader.NextResultAsync())
                //    {
                //        data.FormDatas = await MapFormDataAsync(reader);
                //    }

                //    // 3. Advance to the SECOND Result Set (Users)
                //    if (await reader.NextResultAsync())
                //    {
                //        data.Users = await MapUserDataAsync2(reader);
                //    }
                //}
            }

            // You may or may not want to explicitly close the connection, 
            // depending on your DbContext's connection management.
            // The DbContext typically handles closing the connection when it's disposed.
            // await connection.CloseAsync(); 

            return data;
        }

        public async Task<IEnumerable<UserListForCompare>> GetUserListByFormId(int formId, bool isDataEntryOnly = false)
        {

            var connection = _context.Database.GetDbConnection();

            // The stored procedure must return two SELECT statements in order.
            string storedProc = "dbo.GetUserListforCompareForms";

            // Check connection state and open if necessary
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }



            var parameters = new DynamicParameters();
            parameters.Add("@FormId", formId);
            parameters.Add("@IsDataEntryOnly", isDataEntryOnly);

            var data = await connection.QueryAsync<UserListForCompare>(storedProc, parameters, commandType: CommandType.StoredProcedure);

            // Execute the command and get the data reader
            //using (var reader = await command.q())
            //{
            //    // 1. Process the FIRST Result Set (Customers)
            //    data = await MapUserDataAsync(reader);


            //}


            // You may or may not want to explicitly close the connection, 
            // depending on your DbContext's connection management.
            // The DbContext typically handles closing the connection when it's disposed.
            // await connection.CloseAsync(); 

            return data;
        }

        public async Task<IEnumerable<RatingScore>> GetRatingScore(int formId, string userId)
        {
            try
            {

           
            var connection = _context.Database.GetDbConnection();

            // The stored procedure must return two SELECT statements in order.
            string storedProc = "dbo.Rating_Master";

            // Check connection state and open if necessary
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }

            var parameters = new DynamicParameters();
            parameters.Add("@action", "GetRatingsByRatingUserIdandFormId");
            parameters.Add("@FormId", formId);
            parameters.Add("@UserId", userId);

            var data = await connection.QueryAsync<RatingScore>(storedProc, parameters, commandType: CommandType.StoredProcedure);
            return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        private async Task<List<FormData>> MapFormDataAsync(DbDataReader reader)
        {
            var customers = new List<FormData>();
            while (await reader.ReadAsync())
            {
                customers.Add(new FormData
                {
                    // Column 0 in result set -> Id property
                    QuestionId = reader.GetInt32(reader.GetOrdinal("QuestionId")),
                    AnswerId = reader.GetInt32(reader.GetOrdinal("AnswerId")),
                    // Column 1 in result set -> Name property
                    AnswerText = reader.GetString(reader.GetOrdinal("AnswerText")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }
            return customers;
        }

        private async Task<List<UserListForCompare2>> MapUserDataAsync2(DbDataReader reader)
        {
            var users = new List<UserListForCompare2>();
            while (await reader.ReadAsync())
            {
                users.Add(new UserListForCompare2
                {
                    // Column 0 in result set -> Id property
                    UserId = reader.GetGuid(reader.GetOrdinal("UserId")),
                    UserName = reader.GetString(reader.GetOrdinal("UserName")),

                });
            }
            return users;
        }

        private async Task<List<FormDetail>> MapFormDetailAsync(DbDataReader reader)
        {
            var orders = new List<FormDetail>();
            while (await reader.ReadAsync())
            {
                orders.Add(new FormDetail
                {
                    // Map columns similarly...
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    FormName = reader.GetString(reader.GetOrdinal("FormName")),
                    SectionId = reader.GetInt32(reader.GetOrdinal("SectionId")),
                    Title = reader.GetString(reader.GetOrdinal("Title")),
                    QuestionId = reader.GetInt32(reader.GetOrdinal("QuestionId")),
                    QuestionText = reader.GetString(reader.GetOrdinal("QuestionText"))
                });
            }
            return orders;
        }

        private async Task<List<UserListForCompare>> MapUserDataAsync(DbDataReader reader)
        {
            var customers = new List<UserListForCompare>();
            while (await reader.ReadAsync())
            {
                customers.Add(new UserListForCompare
                {

                    UserId = reader.GetGuid(reader.GetOrdinal("UserId").ToString()).ToString(),
                    UserName = reader.GetString(reader.GetOrdinal("UserName"))
                });
            }
            return customers;
        }
    }
}
