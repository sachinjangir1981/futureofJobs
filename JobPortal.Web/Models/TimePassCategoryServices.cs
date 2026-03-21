using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Web.Models
{
   
    public interface ITimePassCategory
    {

        int Add(TimePassCategories servey);
        int Update(TimePassCategories servey);
        int Delete(int id);
        List<TimePassCategories> GetAll();

        List<TimePassCategories> GetAllByParentId(int parentId);
        TimePassCategories GetById(int id);

        List<TimePassCategories> GetAllCaseCade(bool IsAdmin = true, bool IsFiveLine = false);

        List<TimePassCategories> GetAllParentCategoryByChildId(int childId);
    }
    public class TimePassCategoryServices : ITimePassCategory
    {
        private readonly IDapper _dapper;
        public TimePassCategoryServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public int Add(TimePassCategories servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@TPCategory", servey.TPCategory, DbType.String);
            dbparams.Add("@ParentId", servey.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsFiveLine", servey.IsFiveLine, DbType.Boolean);

            var result = _dapper.Execute($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }

        public int Update(TimePassCategories servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", servey.PKID, DbType.Int32);
            dbparams.Add("@TPCategory", servey.TPCategory, DbType.String);
            dbparams.Add("@ParentId", servey.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@IsFiveLine", servey.IsFiveLine, DbType.Boolean);

            var result = _dapper.Execute($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure);
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
            var result = _dapper.Execute($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int Duplicate(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Duplicate", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<TimePassCategories> GetAll()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAll", DbType.String);
            var result = _dapper.GetAll<TimePassCategories>($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<TimePassCategories> GetAllByParentId(int parentId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAllByCategoryId", DbType.String);
            dbparams.Add("@ParentId", parentId, DbType.Int32);
            var result = _dapper.GetAll<TimePassCategories>($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public TimePassCategories GetById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<TimePassCategories>($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public List<TimePassCategories> GetAllCaseCade(bool IsAdmin =true, bool IsFiveLine =false)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectCaseCade", DbType.String);
            dbparams.Add("IsFiveLine", IsFiveLine, DbType.Boolean);
            dbparams.Add("IsAdmin", IsAdmin, DbType.Boolean);
            var result = _dapper.GetAll<TimePassCategories>($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }


        public List<TimePassCategories> GetAllParentCategoryByChildId(int childId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetAllParentCategory", DbType.String);
            dbparams.Add("@chaildCategoryId", childId, DbType.Int32);
            var result = _dapper.GetAll<TimePassCategories>($"TimePassCategories_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

    }
}
