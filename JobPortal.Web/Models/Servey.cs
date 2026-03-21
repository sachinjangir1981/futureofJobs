using Dapper;
using Dapper_ORM.Services;
using System.Data;

namespace JobPortal.Models
{
    public class ServeyServices : IServey
    {
        private readonly IDapper _dapper;
        public ServeyServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public List<QuestionforServey> GetAllQuestionListForSurvey(int QuestionnairId, int pageNo, int questionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALLFRONT", DbType.String);
            dbparams.Add("@CategoryId",QuestionnairId, DbType.Int32);
            dbparams.Add("@PageNo", pageNo, DbType.Int32);
            dbparams.Add("@questionId",questionId, DbType.Int32);
            var result = _dapper.GetAll<QuestionforServey>($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<Answers> GetAllAnswerByQuestionId(int questionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL", DbType.String);
             
            dbparams.Add("@QuestionId", questionId, DbType.Int32);
            var result = _dapper.GetAll<Answers>($"AnswerMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<PollResult> PollAddorUpdate(int questionId, int answerId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERTORUPDATE", DbType.String);
            dbparams.Add("@QuestionId", questionId, DbType.Int32);
            dbparams.Add("@AnswerId", answerId, DbType.Int32);
            var result = _dapper.GetAll<PollResult>($"PollResultMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public int AddServeyHeader(Questionnaires servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT", DbType.String);
            dbparams.Add("@Questionaire", servey.Questionaire, DbType.String);
            dbparams.Add("@SubHeading", servey.SubHeading, DbType.String);
            dbparams.Add("@Details", servey.Details, DbType.String);
            dbparams.Add("@ImageUrl", servey.ImageUrl, DbType.String);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@ActionText", servey.ActionText, DbType.String);
            dbparams.Add("@BGImage", servey.BGImage, DbType.String);
            dbparams.Add("@BGColor", servey.BGColor, DbType.String);
            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;


        }

        public int UpdateServeyHeader(Questionnaires servey)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE", DbType.String);
            dbparams.Add("@PKID", servey.PKID, DbType.Int32);
            dbparams.Add("@Questionaire", servey.Questionaire, DbType.String);
            dbparams.Add("@SubHeading", servey.SubHeading, DbType.String);
            dbparams.Add("@Details", servey.Details, DbType.String);
            dbparams.Add("@ImageUrl", servey.ImageUrl, DbType.String);
            dbparams.Add("@IsActive", servey.IsActive, DbType.Boolean);
            dbparams.Add("@DisplayOrder", servey.DisplayOrder, DbType.Int32);
            dbparams.Add("@ActionText", servey.ActionText, DbType.String);
            dbparams.Add("@BGImage", servey.BGImage, DbType.String);
            dbparams.Add("@BGColor", servey.BGColor, DbType.String);
            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int DeleteServeyHeader(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "Delete", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<Questionnaires> GetAllServeyHeader(bool IsAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL", DbType.String);
            dbparams.Add("@IsAdmin", IsAdmin, DbType.Boolean);
            var result = _dapper.GetAll<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public Questionnaires GetServeyHeaderById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYID", DbType.String);
            dbparams.Add("@PKID", id, DbType.Int32);
            var result = _dapper.Get<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        

        public List<Questionnaires> GetAllServeyListForGenZ()
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetAllServeyListForGenZ", DbType.String);
           
            var result = _dapper.GetAll<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<Questionnaires> GetAllServeyListForByText(string firsttext)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "GetAllServeyListForByText", DbType.String);
            dbparams.Add("@firsttext", firsttext, DbType.String);

            var result = _dapper.GetAll<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }


    }
}
