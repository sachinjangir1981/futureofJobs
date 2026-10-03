using Dapper;
using Dapper_ORM.Services;
using JobPortal.Domain.Models;
using System.Data;

namespace JobPortal.Web.Models
{

    public interface IMyform
    {
        List<MyForms> GetAll(Guid userId);

        int GetFormSessionId(string sessionId, int formId, Guid userId);

        List<FormSession> GetAllFormVersionsByFormId(int formId, Guid userId);

        List<FormRoleMappings> GetAllRolesByFormId(int formId);

        int UpdateFormRoles(int formId, string roleIds);
    }

    public class FormSession
    {
        public int Id { get; set; }

        public string SessionId { get; set; }

        public string FormVersion { get; set; }

        public int FormId { get; set; }

        public Guid UserId { get; set; }

        public DateTime RcdInsTs { get; set; }

        public string CreatedDate { get; set; }
    }
    public class MyForms
    {
        public int Id { get; set; }
        public int IsFilled { get; set; }
        public string Title { get; set; }
    }

    public class FormRoleMappings
    {
        public int FormId { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }

        public int IsChecked { get; set; }
    }

    public class MyformServices : IMyform
    {
        private readonly IDapper _dapper;

        public MyformServices(IDapper dapper) => this._dapper = dapper;

        public List<MyForms> GetAll(Guid userId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@Userid",  userId, DbType.Guid);
            return this._dapper.GetAll<MyForms>("GelAllFormsForMyProfilePage", dbparams);
        }

        public int GetFormSessionId(string sessionId, int formId, Guid userId)
        {
            var dbparams = new DynamicParameters();

            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@SessionId", sessionId, DbType.String);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@Userid", userId, DbType.Guid);
            object obj = _dapper.ExecuteScalar("FormSessionMaster", dbparams);
            return obj == null ? 0 : int.Parse(obj.ToString());
        }

        public List<FormSession> GetAllFormVersionsByFormId(int formId, Guid userId)
        {
            var dbparams = new DynamicParameters();
            
            dbparams.Add("@action", "GetFormVersionsByFormId", DbType.String);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@Userid", userId, DbType.Guid);
            return _dapper.GetAll<FormSession>("FormSessionMaster", dbparams);
        }

        public List<FormRoleMappings> GetAllRolesByFormId(int formId)
        {
            var dbparams = new DynamicParameters();
           
            dbparams.Add("@action", "Get_Roles_By_FormId", DbType.String);
            dbparams.Add("@PKId", formId, DbType.Int32);
            return _dapper.GetAll<FormRoleMappings>("FormDetailMaster", dbparams);
        }

        public int UpdateFormRoles(int formId, string roleIds)
        {
            var dbparams = new DynamicParameters();
           
            dbparams.Add("@action", "UpdateFormRoleMapping", DbType.String);
            dbparams.Add("@RoleIds", roleIds, DbType.String);
            dbparams.Add("@PKId", formId, DbType.Int32);
            
            object obj = _dapper.ExecuteScalar("FormDetailMaster", dbparams);
            return obj == null ? 0 : int.Parse(obj.ToString());
        }

    }
}
