using JobPortal.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using JobPortal.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SqlServer.Server;
using System.Collections;

namespace JobPortal.Infrastructure.Services
{
    public interface IJLocationServices
    {
        public IEnumerable<State>  GetStateList();
        public IEnumerable<City> GetCityListByStateId(int iStateId);
    }
    public class JLocationServices : IJLocationServices
    {
        private readonly AppDbContext _db;

        public JLocationServices(AppDbContext db)
        {
            _db = db;
        }
        public  IEnumerable<State>  GetStateList()
        {
            var connection = _db.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@action", "GetStateList");
            parameters.Add("@CountryId", 1);

            if (connection.State == ConnectionState.Closed)
            {
                  connection.OpenAsync();
            }
              connection.OpenAsync();

            return   connection.Query<State>(
                "LocationMaster", parameters,
                commandType: CommandType.StoredProcedure
            );
        }
        public    IEnumerable<City> GetCityListByStateId(int iStateId)
        {
            var connection = _db.Database.GetDbConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@action", "GetCityList");
            parameters.Add("@StateId", iStateId);

            if (connection.State == ConnectionState.Closed)
            {
                  connection.OpenAsync();
            }
              connection.OpenAsync();

            return   connection.Query<City>(
                "LocationMaster", parameters,
                commandType: CommandType.StoredProcedure
            );
        }

         
    }
}
