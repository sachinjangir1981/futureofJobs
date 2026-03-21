using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Web.Models
{
    public interface ILibraryCategory
    {

        int Add(LibraryCategoryModel servey);
        int Update(LibraryCategoryModel servey);
        int Delete(int id);
        List<LibraryCategoryModel> GetAll();

        List<LibraryCategoryModel> GetAllByParentId(int parentId);
        LibraryCategoryModel GetById(int id);

        List<LibraryCategoryModel> GetAllCaseCade();

        List<LibraryCategoryModel> GetAllParentCategoryByChildId(int childId);
    }
    public class LibraryCategoryServices : ILibraryCategory
    {
        private readonly IDapper _dapper;
        public LibraryCategoryServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public int Add(LibraryCategoryModel servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@Title", servey.Title, DbType.String);
            dbparams.Add("@ParentId", servey.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsCustomBreadcrumb", servey.IsCustomBreadcrumb, DbType.Boolean);
            dbparams.Add("@CustomBreadcrumb", servey.CustomBreadcrumb, DbType.String);
            var result = _dapper.Execute($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }

        public int Update(LibraryCategoryModel servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", servey.PKID, DbType.Int32);
            dbparams.Add("@Title", servey.Title, DbType.String);
            dbparams.Add("@ParentId", servey.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsCustomBreadcrumb", servey.IsCustomBreadcrumb, DbType.Boolean);
            dbparams.Add("@CustomBreadcrumb", servey.CustomBreadcrumb, DbType.String);
            var result = _dapper.Execute($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
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
            var result = _dapper.Execute($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int Duplicate(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Duplicate", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<LibraryCategoryModel> GetAll()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAll", DbType.String);
            var result = _dapper.GetAll<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<LibraryCategoryModel> GetAllByParentId(int parentId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAllByCategoryId", DbType.String);
            dbparams.Add("@ParentId", parentId, DbType.Int32);
            var result = _dapper.GetAll<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public LibraryCategoryModel GetById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public List<LibraryCategoryModel> GetAllCaseCade()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectCaseCade", DbType.String);
            var result = _dapper.GetAll<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }


        public List<LibraryCategoryModel> GetAllParentCategoryByChildId(int childId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetAllParentCategory", DbType.String);
            dbparams.Add("@chaildCategoryId", childId, DbType.Int32);
            var result = _dapper.GetAll<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

    }
}
