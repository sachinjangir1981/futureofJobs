using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Web.Models
{

    public class ExhibitorDto
    {
        public string Index { get; set; }
        public string Image { get; set; }
        public string Website { get; set; }
        public string Titles { get; set; }

        public string Category { get; set; }
    }
    public class LibraryModel
    {
        public int PKID { get; set; }

        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public string ImageUrl { get; set; } = "";
        public bool IsDelete { get; set; } = false;
        public int DisplayOrder { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public string FullPath { get; set; } = "";
        public int IsCategory { get; set; } = 0;
        public int ChildCategoryCount { get; set; } = 0;
        public int Level1 { get; set; } = 0;
        public int Level2 { get; set; } = 0;
        public int Level3 { get; set; } = 0;
    }
    public interface ILibrary
    {

        int AddToLibrary(LibraryModel servey);
        int UpdateToLibrary(LibraryModel servey);
        int DeleteLibrary(int id);
        int Duplicate(int id);

        int ImportExhibitorData(ExhibitorDto exhibitorDto);

        List<ExhibitorDto> SelectAllExhibitorData(int id);
        List<LibraryModel> GetAll();


        LibraryModel GetById(int id);

        LibraryModel SelectByTitle(string title);

        List<LibraryModel> GetAllLibraryAndCategory(int CategoryId);

        List<LibraryModel> GetAllLibraryItemByCategory(int CategoryId);

        List<LibraryCategoryModel> GetAllCategoryforLibrary(int CategoryId);

        int UpdateCategory(int id, int categoryId);
        int UpdateLevel(int id, int displayOrder, int level1, int level2, int level3);

        int DuplicateCategoryWithAllLevels(int id);
    }

    public class ShareItModel
    {
        public int PKID { get; set; }
        public string Subject { get; set; }
        public string URL { get; set; }

    }

    public class LibraryCategoryModel
    {

        public int PKID { get; set; }

        public string Title { get; set; }
        public int ParentId { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }  

        public string Path { get; set; } = "";

        public int Level { get; set; } = 0;

        public int RNum { get; set; } = 0;

        public string TLevel { get; set; } = "0";

        public int childCount { get; set; } = 0;

        public int LibraryItemCount { get; set; } = 0;

        public bool IsCustomBreadcrumb { get; set; }

        public string CustomBreadcrumb { get; set; } = "";
    }
    public class LibraryServices : ILibrary
    {
        private readonly IDapper _dapper;
        public LibraryServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public int ImportExhibitorData(ExhibitorDto exhibitorDto)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "insert", DbType.String);
            dbparams.Add("@col1", exhibitorDto.Index, DbType.String);
            dbparams.Add("@col2", exhibitorDto.Titles, DbType.String);
            dbparams.Add("@col3", exhibitorDto.Image, DbType.String);
            dbparams.Add("@col4", exhibitorDto.Category, DbType.String);
            dbparams.Add("@col5", exhibitorDto.Website, DbType.String);
            dbparams.Add("@col6","", DbType.String);
           
           
            var result = _dapper.Execute($"ImportDataSP", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }
        public int AddToLibrary(LibraryModel servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@Title", servey.Title, DbType.String);
            dbparams.Add("@Content", servey.Content, DbType.String);
            dbparams.Add("@ImageUrl", servey.ImageUrl, DbType.String);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@CategoryId", servey.CategoryId, DbType.Int32);
            dbparams.Add("@Level1", servey.Level1, DbType.Int32);
            dbparams.Add("@Level2", servey.Level2, DbType.Int32);
            dbparams.Add("@Level3", servey.Level3, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }

        public int UpdateToLibrary(LibraryModel servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", servey.PKID, DbType.Int32);
            dbparams.Add("@Title", servey.Title, DbType.String);
            dbparams.Add("@Content", servey.Content, DbType.String);
            dbparams.Add("@ImageUrl", servey.ImageUrl, DbType.String);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@CategoryId", servey.CategoryId, DbType.Int32);
            dbparams.Add("@Level1", servey.Level1, DbType.Int32);
            dbparams.Add("@Level2", servey.Level2, DbType.Int32);
            dbparams.Add("@Level3", servey.Level3, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }


        public int UpdateCategory(int id, int categoryId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UpdateCategory", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            dbparams.Add("@CategoryId", categoryId, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int UpdateLevel(int id, int displayOrder, int level1, int level2, int level3)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UpdateLevel", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            dbparams.Add("@DisplayOrder", displayOrder, DbType.Int32);
            dbparams.Add("@Level1", level1, DbType.Int32);
            dbparams.Add("@Level2", level2, DbType.Int32);
            dbparams.Add("@Level3", level3, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int DeleteLibrary(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int Duplicate(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Duplicate", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<LibraryModel> GetAll()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectAll", DbType.String);
            var result = _dapper.GetAll<LibraryModel>($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }



        public LibraryModel GetById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectById", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<LibraryModel>($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public LibraryModel SelectByTitle(string title)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectByTitle", DbType.String);
            dbparams.Add("@Title", title, DbType.String);
            var result = _dapper.Get<LibraryModel>($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<LibraryModel> GetAllLibraryAndCategory(int CategoryId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectCategoryandLibrary", DbType.String);
            dbparams.Add("@CategoryId", CategoryId, DbType.Int32);
            var result = _dapper.GetAll<LibraryModel>($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<LibraryModel> GetAllLibraryItemByCategory(int CategoryId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectByCategoryId", DbType.String);
            dbparams.Add("@CategoryId", CategoryId, DbType.Int32);
            var result = _dapper.GetAll<LibraryModel>($"LibraryMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<LibraryCategoryModel> GetAllCategoryforLibrary(int CategoryId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelectCaseCadeForLibrary", DbType.String);
            dbparams.Add("@chaildCategoryId", CategoryId, DbType.Int32);
            var result = _dapper.GetAll<LibraryCategoryModel>($"LibraryCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public int DuplicateCategoryWithAllLevels(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@ChildCategoryId", id, DbType.Int32);
            var result = _dapper.Execute($"DuplicateLibraryCategory", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<ExhibitorDto> SelectAllExhibitorData(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "selectall", DbType.String);
            dbparams.Add("@RecordType", id, DbType.Int32);
            var result = _dapper.GetAll<ExhibitorDto>($"ImportDataSP", dbparams, commandType: CommandType.StoredProcedure);
            return result;

        }
    }
}
