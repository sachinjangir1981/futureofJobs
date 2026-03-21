using Dapper;
using Dapper_ORM.Services;
using System;
using System.Data;

namespace JobPortal.Web.Models
{
    public class LeadershipReflection
    {
        public int PKID { get; set; }
        public string R1L1 { get; set; }
        public string R1L2 { get; set; }
        public string R1L3 { get; set; }
        public string R2L1 { get; set; }
        public string R2L2 { get; set; }
        public string R2L3 { get; set; }
        public string R3L1 { get; set; }
        public string R3L2 { get; set; }
        public string R3L3 { get; set; }
        public string R4L1 { get; set; }
        public string R4L2 { get; set; }
        public string R4L3 { get; set; }
        public DateTime RcdInsTs { get; set; }
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string InstituteName { get; set; }
        public DateOnly?  PostDate { get; set; }

        public DateTime PostDate2 { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public Guid uRId { get; set; }
        public int GridFormType { get; set; }

    }

    public interface ILeadershipReflection 
    {
        LeadershipReflection GetByURId(Guid uRId, int gridFormType = 1);

        LeadershipReflection GetByUserId(Guid userId, int gridFormType = 1);
        Guid Add(LeadershipReflection reflection);
        Guid Update(LeadershipReflection reflection);
    }


    public class LeadershipReflectionServices : ILeadershipReflection
    {
        private readonly IDapper _dapper;
        public LeadershipReflectionServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public Guid Add(LeadershipReflection reflection)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@R1L1", reflection.R1L1, DbType.String);
            dbparams.Add("@R1L2", reflection.R1L2, DbType.String);
            dbparams.Add("@R1L3", reflection.R1L3, DbType.String);
            dbparams.Add("@R2L1", reflection.R2L1, DbType.String);
            dbparams.Add("@R2L2", reflection.R2L2, DbType.String);
            dbparams.Add("@R2L3", reflection.R2L3, DbType.String);
            dbparams.Add("@R3L1", reflection.R3L1, DbType.String);
            dbparams.Add("@R3L2", reflection.R3L2, DbType.String);
            dbparams.Add("@R3L3", reflection.R3L3, DbType.String);
            dbparams.Add("@R4L1", reflection.R4L1, DbType.String);
            dbparams.Add("@R4L2", reflection.R4L2, DbType.String);
            dbparams.Add("@R4L3", reflection.R4L3, DbType.String);
            dbparams.Add("@UserId", reflection.UserId, DbType.Guid);
            dbparams.Add("@FullName", reflection.FullName, DbType.String);
            dbparams.Add("@InstituteName", reflection.InstituteName, DbType.String);
            dbparams.Add("@PostDate", reflection.PostDate, DbType.Date);
            dbparams.Add("@uRId", reflection.uRId, DbType.Guid);
            dbparams.Add("@GridFormType", reflection.GridFormType, DbType.Int32);
            var result = _dapper.ExecuteScalar($"LeadershipReflectionMasterSP", dbparams, commandType: CommandType.StoredProcedure);
            return Guid.Parse(result.ToString());


        }
        public Guid Update(LeadershipReflection reflection)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATEBYUSERID", DbType.String);
            dbparams.Add("@PKID", reflection.PKID, DbType.Int32);
            dbparams.Add("@R1L1", reflection.R1L1, DbType.String);
            dbparams.Add("@R1L2", reflection.R1L2, DbType.String);
            dbparams.Add("@R1L3", reflection.R1L3, DbType.String);
            dbparams.Add("@R2L1", reflection.R2L1, DbType.String);
            dbparams.Add("@R2L2", reflection.R2L2, DbType.String);
            dbparams.Add("@R2L3", reflection.R2L3, DbType.String);
            dbparams.Add("@R3L1", reflection.R3L1, DbType.String);
            dbparams.Add("@R3L2", reflection.R3L2, DbType.String);
            dbparams.Add("@R3L3", reflection.R3L3, DbType.String);
            dbparams.Add("@R4L1", reflection.R4L1, DbType.String);
            dbparams.Add("@R4L2", reflection.R4L2, DbType.String);
            dbparams.Add("@R4L3", reflection.R4L3, DbType.String);
            dbparams.Add("@UserId", reflection.UserId, DbType.Guid);
            dbparams.Add("@FullName", reflection.FullName, DbType.String);
            dbparams.Add("@InstituteName", reflection.InstituteName, DbType.String);
            dbparams.Add("@PostDate", reflection.PostDate, DbType.DateTime);
            dbparams.Add("@uRId", reflection.uRId, DbType.Guid);
            dbparams.Add("@GridFormType", reflection.GridFormType, DbType.Int32);
            var result = _dapper.ExecuteScalar($"LeadershipReflectionMasterSP", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Guid.Parse(result.ToString());
            }
            else
            {
                return Guid.Empty;
            }
        }

        //

        public LeadershipReflection GetByURId(Guid uRId, int gridFormType = 1)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYURID", DbType.String);
            dbparams.Add("@uRId", uRId, DbType.Guid);
            dbparams.Add("@GridFormType", gridFormType, DbType.Int32);
            var result = _dapper.Get<LeadershipReflection>($"LeadershipReflectionMasterSP", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                result.PostDate = DateOnly.FromDateTime(result.PostDate2);
            }
            return result;
        }
        public LeadershipReflection GetByUserId(Guid userId, int gridFormType=1)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYUSERID", DbType.String);
            dbparams.Add("@UserId", userId, DbType.Guid);
            dbparams.Add("@GridFormType", gridFormType, DbType.Int32);
            var result = _dapper.Get<LeadershipReflection>($"LeadershipReflectionMasterSP", dbparams, commandType: CommandType.StoredProcedure);
            if(result !=null)
            {
                result.PostDate = DateOnly.FromDateTime(result.PostDate2);
            }
            return result;
        }
    }
}
