using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Web.Models
{
    public class UserLedger
    {
        public int PKID { get; set; }
        public Guid UserId { get; set; }
        public int RoleId { get; set; }
        public string Particular { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal TotalAmount { get; set; }
        public int CouponId { get; set; }
        public string RcdInsTs { get; set; }
        public string RcdUpdt { get; set; }

    }

    public class FormFee
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public string FormTitle { get; set; }
        public decimal FeeAmount { get; set; }

        public decimal FTFeeAmount { get; set; }

    }

    public class FormFeeChangeHistory
    {
        public int Id { get; set; }
        public string RcdIndsTs { get; set; }
        public decimal FeeAmount { get; set; }

        public decimal FTFeeAmount { get; set; }


    }

    public class UserFormFeeDetails
    {
        public int Id { get; set; }

        public int FormId { get; set; }

        public string FormName { get; set; }
        public string RcdIndsTs { get; set; }
        public decimal FeeAmount { get; set; }

        public decimal FTFeeAmount { get; set; }
        public decimal AvailableAmount { get; set; }
        public bool IsFormFilledByUser { get; set; }
    }


    public interface IFormFeeRepository
    {
        List<FormFee> GetAllFormFeesByRoleId(int roleId);
        int AddUpdateFormFee(FormFee formFee);

        List<FormFeeChangeHistory> GetFeeHistoryByRoleIdFormId(int roleId, int formId);

        UserFormFeeDetails GetUserFormFeeDetails(int roleId, int formId, Guid userId);

        int CheckforFeeSession(int formId, Guid userId, string sessionId);
        int AddNewFeeSession(int formId, Guid userId, string sessionId);
    }



    public class FormFeeRepository : IFormFeeRepository
    {
        private readonly IDapper _dapper;
        public FormFeeRepository(IDapper dapper)
        {

            this._dapper = dapper;
        }
        public List<FormFee> GetAllFormFeesByRoleId(int roleId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetAllFormFeesByRoleId", DbType.String);
            dbparams.Add("@RoleId", roleId, DbType.Int32);
            var result = _dapper.GetAll<FormFee>($"FormFee_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public int AddUpdateFormFee(FormFee formFee)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERTORUPDATE", DbType.String);
            dbparams.Add("@FormId", formFee.Id, DbType.Int32);
            dbparams.Add("@RoleId", formFee.RoleId, DbType.Int32);
            dbparams.Add("@Fees", formFee.FeeAmount, DbType.Decimal);
            dbparams.Add("@FTFee", formFee.FTFeeAmount, DbType.Decimal);
            var result = _dapper.Execute($"FormFee_Master", dbparams, commandType: CommandType.StoredProcedure);
            return Convert.ToInt32(result);
        }

        public List<FormFeeChangeHistory> GetFeeHistoryByRoleIdFormId(int roleId, int formId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetFeeHistoryByRoleIdFormId", DbType.String);
            dbparams.Add("@RoleId", roleId, DbType.Int32);
            dbparams.Add("@FormId", formId, DbType.Int32);
            var result = _dapper.GetAll<FormFeeChangeHistory>($"FormFee_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public UserFormFeeDetails GetUserFormFeeDetails(int roleId, int formId, Guid userid)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetFormChargesAndAvailalbeAmountByUserId", DbType.String);
            dbparams.Add("@RoleId", roleId, DbType.Int32);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@UserId", userid, DbType.Guid);
            var result = _dapper.GetAll<UserFormFeeDetails>($"FormFee_Master", dbparams, commandType: CommandType.StoredProcedure).FirstOrDefault();
           if(result== null)
            {
                result = new UserFormFeeDetails();
            }
            return result;
        }


        public int CheckforFeeSession(int formId, Guid userId, string sessionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "CHECK", DbType.String);
            dbparams.Add("@sessionId", sessionId, DbType.String);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@UserId", userId, DbType.Guid);
            var result = _dapper.ExecuteScalar($"FormSessionsMaster", dbparams, commandType: CommandType.StoredProcedure);
            return int.Parse(result.ToString());
        }


        public int AddNewFeeSession(int formId, Guid userId, string sessionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@sessionId", sessionId, DbType.String);
            dbparams.Add("@FormId", formId, DbType.Int32);
            dbparams.Add("@UserId", userId, DbType.Guid);
            var result = _dapper.ExecuteScalar($"FormSessionsMaster", dbparams, commandType: CommandType.StoredProcedure);
            return int.Parse(result.ToString());
        }
    }


    public interface IFormPaymentRepository
    {
        List<UserLedger> GetAllLedgerDetailByUserId(Guid userId);
        int AddCredit(UserLedger modal);
        int AddDebit(UserLedger modal);
        decimal GetUserCurrentBalance(Guid userId);
    }

    public class FormPaymentRepository : IFormPaymentRepository
    {
        private readonly IDapper _dapper;
        public FormPaymentRepository(IDapper dapper)
        {

            this._dapper = dapper;
        }
        public List<UserLedger> GetAllLedgerDetailByUserId(Guid userId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetLedgerDetailsByuserId", DbType.String);
            dbparams.Add("@UserId", userId, DbType.Guid);
            var result = _dapper.GetAll<UserLedger>($"UserLedger_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }
        public int AddCredit(UserLedger modal)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "CREDIT", DbType.String);
            dbparams.Add("@UserId", modal.UserId, DbType.Guid);
            dbparams.Add("@Particular", modal.Particular, DbType.String);
            dbparams.Add("@Credit", modal.Credit, DbType.Decimal);
            dbparams.Add("@CouponId", modal.CouponId, DbType.Int32);
            dbparams.Add("@RoleId", modal.RoleId, DbType.Int32);
            var result = _dapper.Execute($"UserLedger_Master", dbparams, commandType: CommandType.StoredProcedure);
            return Convert.ToInt32(result);
        }

        public int AddDebit(UserLedger modal)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "DEBIT", DbType.String);
            dbparams.Add("@UserId", modal.UserId, DbType.Guid);
            dbparams.Add("@Particular", modal.Particular, DbType.String);
            dbparams.Add("@Debit", modal.Debit, DbType.Decimal);
            dbparams.Add("@CouponId", modal.CouponId, DbType.Int32);
            dbparams.Add("@RoleId", modal.RoleId, DbType.Int32);
            var result = _dapper.Execute($"UserLedger_Master", dbparams, commandType: CommandType.StoredProcedure);
            return Convert.ToInt32(result);
        }

        public decimal GetUserCurrentBalance(Guid userId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetUserCurrentBalance", DbType.String);
            dbparams.Add("@UserId", userId, DbType.Guid);
            var result = _dapper.ExecuteScalar($"UserLedger_Master", dbparams, commandType: CommandType.StoredProcedure);
            return Convert.ToDecimal(result);
        }

    }
}
