using Dapper;
using Dapper_ORM.Services;
using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace JobPortal.Web.Models
{

    
    public class OneMinuteItem
    {
        public int PKID { get; set; }

        public string Title { get; set; }
        public string NewsSubject { get; set; }
        public string Discussion { get; set; } = "";
        public string FeedBackLink { get; set; }

        public string MediaRef { get; set; }
        public string CoverPhoto { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public int LanguageId { get; set; } = 1;

        public string Col5 { get; set; } = "";
         
    }

    public interface IOneMinute
    {
        int Add(OneMinuteItem customer, int recType = 1);

        int Update(OneMinuteItem customer, int recType = 1);
        List<OneMinuteItem> GetAllOneMinuteItem();
        List<OneMinuteItem> GetAllOneMinuteItem(int catId);
        OneMinuteItem GetOneMinuteItemById(int id);
        int DeleteById(int pkid);
    }

    public class OneMinuteServices : IOneMinute
    {
        private readonly IDapper _dapper;
        public OneMinuteServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public int Add(OneMinuteItem customer, int  recType=1)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("action", "INSERT", DbType.String);
            dbparams.Add("NewsSubject", customer.NewsSubject == null ? "" : customer.NewsSubject, DbType.String);
            dbparams.Add("Discussion", customer.Discussion == null ? "" : customer.Discussion, DbType.String);
            dbparams.Add("FeedBackLink", customer.FeedBackLink == null ? "" : customer.FeedBackLink, DbType.String);
            dbparams.Add("MediaRef", customer.MediaRef == null ? "" : customer.MediaRef, DbType.String);
            dbparams.Add("IsActive", customer.IsActive, DbType.Boolean);
            dbparams.Add("DisplayOrder", customer.DisplayOrder, DbType.Int32);
            dbparams.Add("CoverPhoto", customer.CoverPhoto, DbType.String);
            dbparams.Add("Title", customer.Title == null ? "" : customer.Title,DbType.String);
            dbparams.Add("LanguageId", recType, DbType.Int32);
            dbparams.Add("Col5", customer.Col5, DbType.String);

            return _dapper.Execute($"MediaCard_Master", dbparams, commandType: CommandType.StoredProcedure);

        }
        public int Update(OneMinuteItem customer, int recType)
        {
            var dbparams = new DynamicParameters();

            dbparams.Add("action", "UPDATE", DbType.String);
            dbparams.Add("pkid", customer.PKID, DbType.String);
            dbparams.Add("NewsSubject", customer.NewsSubject == null ? "" : customer.NewsSubject, DbType.String);
            dbparams.Add("Discussion", customer.Discussion == null ? "" : customer.Discussion, DbType.String);
            dbparams.Add("FeedBackLink", customer.FeedBackLink == null ? "" : customer.FeedBackLink, DbType.String);
            dbparams.Add("MediaRef", customer.MediaRef == null ? "" : customer.MediaRef, DbType.String);
            dbparams.Add("LanguageId", recType, DbType.Int32);
            dbparams.Add("IsActive", customer.IsActive, DbType.Boolean);
            dbparams.Add("DisplayOrder", customer.DisplayOrder, DbType.Int32);
            dbparams.Add("CoverPhoto", customer.CoverPhoto, DbType.String);
            dbparams.Add("Title", customer.Title == null ? "" : customer.Title, DbType.String);
            dbparams.Add("Col5", customer.Col5, DbType.String);

            return _dapper.Execute($"MediaCard_Master", dbparams, commandType: CommandType.StoredProcedure);

        }

        public int DeleteById(int pkid)
        {
            var dbparams = new DynamicParameters();

            dbparams.Add("action", "Delete", DbType.String);
            dbparams.Add("pkid", pkid, DbType.String);
            return _dapper.Execute("MediaCard_Master",  dbparams, commandType: CommandType.StoredProcedure);
            
        }
        public List<OneMinuteItem> GetAllOneMinuteItem()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SELECTALLFRONT", DbType.String);
            var result = _dapper.GetAll<OneMinuteItem>($"MediaCard_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<OneMinuteItem> GetAllOneMinuteItem(int catId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SELECTALL", DbType.String);
            dbparams.Add("LanguageId", catId, DbType.Int32);
            var result = _dapper.GetAll<OneMinuteItem>($"MediaCard_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public OneMinuteItem GetOneMinuteItemById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SELECTBYID", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            var result = _dapper.Get<OneMinuteItem>($"MediaCard_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}
