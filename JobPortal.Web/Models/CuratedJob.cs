using Dapper;
using Dapper_ORM.Services;
using System.Data;


namespace JobPortal.Web.Models
{
    public class CuratedJob
    {
        public int PKID { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string DomainTag { get; set; } = string.Empty;
        public string SourceLink { get; set; } = string.Empty;
        public DateOnly PostedDate2 { get; set; }

        public DateTime PostedDate { get; set; }
        public bool IsActive { get; set; } = true;

    }

    public interface ICuratedJobs
    {

        int Add(CuratedJob model);
        int Update(CuratedJob model);
        int Delete(int id);
        List<CuratedJob> GetAll(bool isAdmin);
        CuratedJob GetById(int id);
    }

    public class CuratedJobServices : ICuratedJobs
    {
        private readonly IDapper _dapper;

        public CuratedJobServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public int Add(CuratedJob model)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Insert", DbType.String);
            dbparams.Add("@JobTitle", model.JobTitle, DbType.String);
            dbparams.Add("@CompanyName", model.CompanyName, DbType.String);
            dbparams.Add("@DomainTag", model.DomainTag, DbType.String);
            dbparams.Add("@SourceLink", model.SourceLink, DbType.String);
            dbparams.Add("@PostedDate", model.PostedDate, DbType.DateTime);
            dbparams.Add("@IsActive", model.IsActive, DbType.Boolean);
            var result = _dapper.Execute($"CuratedJobs_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }

        public int Update(CuratedJob model)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Update", DbType.String);
            dbparams.Add("@PKID", model.PKID, DbType.Int32);
            dbparams.Add("@JobTitle", model.JobTitle, DbType.String);
            dbparams.Add("@CompanyName", model.CompanyName, DbType.String);
            dbparams.Add("@DomainTag", model.DomainTag, DbType.String);
            dbparams.Add("@SourceLink", model.SourceLink, DbType.String);
            dbparams.Add("@PostedDate", model.PostedDate, DbType.DateTime);
            dbparams.Add("@IsActive", model.IsActive, DbType.Boolean);
            var result = _dapper.Execute($"CuratedJobs_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int Delete(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"CuratedJobs_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<CuratedJob> GetAll(bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAll", DbType.String);
            dbparams.Add("@IsAdmin", isAdmin, DbType.Boolean);
            var result = _dapper.GetAll<CuratedJob>($"CuratedJobs_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public CuratedJob GetById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<CuratedJob>($"CuratedJobs_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

    }
}
