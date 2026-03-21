using Dapper;
using Dapper_ORM.Services;
using JobPortal.Domain.Models;
using System.Data;

namespace JobPortal.Models
{
    public class WpPost
    {
        public int RNUM { get; set; }
        public int PKID { get; set; }
        public string Title { get; set; }
        public string PostContent { get; set; }
        public int CategoryId { get; set; }
        public string CoverImage { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsHKContent { get; set; }
        public string OSource { get; set; }
        public DateTime PostDate { get; set; }
    }

    public class WpPostCategory
    {
        public int PKID { get; set; }
        public string CatName { get; set; }
        public int ParentCatId { get; set; }
        public string ImageUrl { get; set; }
        public string BgImageUrl { get; set; }
    }

    public interface IWpPostModel
    {
        int AddPost(WpPost obj);
        int UpdatePost(WpPost obj);
        List<WpPost> GetAllPosts(int categoryId = 167);
        WpPost SelectById(int id);
        int Delete(int id);

        int AddCategory(WpPostCategory obj);
        int UpdateCategory(WpPostCategory obj);
        List<WpPostCategory> GetAllCategory(int categoryId);
        WpPostCategory SelectCategoryById(int id);
        int DeleteCategory(int id);
    }

    public class WpPostModel : IWpPostModel
    {
        private readonly IDapper _dapper;
        public WpPostModel(IDapper dapper)
        {
            this._dapper = dapper;
        }
        public int AddPost(WpPost obj)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@Title", obj.Title, DbType.String);
            dbparams.Add("@PostContent", obj.PostContent, DbType.String);
            dbparams.Add("@CategoryId", obj.CategoryId, DbType.Int32);
            dbparams.Add("@CoverImage", obj.CoverImage, DbType.String);
            dbparams.Add("@DisplayOrder", obj.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsActive", obj.IsActive, DbType.Boolean);
            dbparams.Add("@OSource", obj.OSource, DbType.String);
            dbparams.Add("@IsHKContent", obj.IsHKContent, DbType.Boolean); 
            dbparams.Add("@PostDate", obj.PostDate, DbType.DateTime);
            var result = _dapper.Execute ($"WPPost_Master", dbparams, commandType: CommandType.StoredProcedure);
             return result;
        }
        public int UpdatePost(WpPost obj)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", obj.PKID, DbType.Int32);
            dbparams.Add("@Title", obj.Title, DbType.String);
            dbparams.Add("@PostContent", obj.PostContent, DbType.String);
            dbparams.Add("@CategoryId", obj.CategoryId, DbType.Int32);
            dbparams.Add("@CoverImage", obj.CoverImage, DbType.String);
            dbparams.Add("@DisplayOrder", obj.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsActive", obj.IsActive, DbType.Boolean);
            dbparams.Add("@OSource", obj.OSource, DbType.String);
            dbparams.Add("@IsHKContent", obj.IsHKContent, DbType.Boolean);
            dbparams.Add("@PostDate", obj.PostDate, DbType.DateTime);
            var result = _dapper.Execute($"WPPost_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<WpPost> GetAllPosts(int categoryId=167)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL", DbType.String);
            dbparams.Add("@CategoryId", categoryId, DbType.Int32);
            var result = _dapper.GetAll<WpPost>($"WPPost_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
            
        public WpPost SelectById(int id)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<WpPost>($"WPPost_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;

 
        }

        public int Delete(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"WPPost_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int AddCategory(WpPostCategory obj)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@CatName", obj.CatName, DbType.String);
            dbparams.Add("@ParentCatId", obj.ParentCatId, DbType.Int32);
            dbparams.Add("@ImageUrl", obj.ImageUrl, DbType.String);
            dbparams.Add("@BgImageUrl", obj.BgImageUrl, DbType.String);
          
            var result = _dapper.Execute($"WPPostCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public int UpdateCategory(WpPostCategory obj)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", obj.PKID, DbType.Int32);
            dbparams.Add("@CatName", obj.CatName, DbType.String);
            dbparams.Add("@ParentCatId", obj.ParentCatId, DbType.Int32);
            dbparams.Add("@ImageUrl", obj.ImageUrl, DbType.String);
            dbparams.Add("@BgImageUrl", obj.BgImageUrl, DbType.String);

            var result = _dapper.Execute($"WPPostCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public List<WpPostCategory> GetAllCategory(int categoryId = 167)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL", DbType.String);
            dbparams.Add("@ParentCatId", categoryId, DbType.Int32);
            var result = _dapper.GetAll<WpPostCategory>($"WPPostCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;

        }
        public WpPostCategory SelectCategoryById(int id)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<WpPostCategory>($"WPPostCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }
        public int DeleteCategory(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"WPPostCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
    }
}