using Dapper;
using Dapper_ORM.Services;
 
using Microsoft.SqlServer.Server;
using System.Data;
using System.Linq;
using static System.Collections.Specialized.BitVector32;

namespace JobPortal.Web.Models
{
    public class DFormServices : IDForms
    {
        private readonly IDapper _dapper;
        public DFormServices(IDapper dapper)
        {

            this._dapper = dapper;
        }
        public int DeleteForm(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "DeleteForm", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            var result = _dapper.Execute($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int Duplicate(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("FormId", id, DbType.Int32);
            var result = _dapper.Execute($"DuplicateForms", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public List<DynamicForm> GetAllForms(int categoryId, bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectAll", DbType.String);
            dbparams.Add("isAdmin", isAdmin, DbType.Boolean);
            dbparams.Add("CategoryId", categoryId, DbType.Int32);
            var result = _dapper.GetAll<DynamicForm>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public DynamicForm GetFormById(int Id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectById", DbType.String);
            dbparams.Add("PKID", Id, DbType.Int32);
            var result = _dapper.Get<DynamicForm>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public DynamicForm GetFormByGuid(Guid FormId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectByGuid", DbType.String);
            dbparams.Add("FormGuid", FormId, DbType.Guid);
            var result = _dapper.Get<DynamicForm>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int InsertForm(DynamicForm form)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "InsertForm", DbType.String);
            dbparams.Add("Title", form.Title, DbType.String);
            dbparams.Add("Detail", form.Detail, DbType.String);
            dbparams.Add("Header", form.Header, DbType.String);
            dbparams.Add("Footer", form.Footer, DbType.String);
            dbparams.Add("ImageUrl", form.ImageUrl, DbType.String);
            dbparams.Add("DisplayOrder", form.DisplayOrder, DbType.Int32);
            dbparams.Add("CategoryId", form.CategoryId, DbType.Int32);
            dbparams.Add("IsActive", form.IsActive, DbType.Boolean);
            dbparams.Add("ImageUrlLeft", form.ImageUrlLeft, DbType.String);
            dbparams.Add("BGUrl", form.BGUrl, DbType.String);
            dbparams.Add("CommonCss", form.CommonCssinStr, DbType.String);
            var result = _dapper.ExecuteScalar($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }

        }

        public int UpdateForm(DynamicForm form)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "UpdateForm", DbType.String);
            dbparams.Add("PKID", form.PKID, DbType.Int32);
            dbparams.Add("Title", form.Title, DbType.String);
            dbparams.Add("Detail", form.Detail, DbType.String);
            dbparams.Add("Header", form.Header, DbType.String);
            dbparams.Add("Footer", form.Footer, DbType.String);
            dbparams.Add("ImageUrl", form.ImageUrl, DbType.String);
            dbparams.Add("DisplayOrder", form.DisplayOrder, DbType.Int32);
            dbparams.Add("CategoryId", form.CategoryId, DbType.Int32);
            dbparams.Add("IsActive", form.IsActive, DbType.Boolean);
            dbparams.Add("ImageUrlLeft", form.ImageUrlLeft, DbType.String);
            dbparams.Add("BGUrl", form.BGUrl, DbType.String);
            dbparams.Add("CommonCss", form.CommonCssinStr, DbType.String);
            var result = _dapper.Execute($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;

        }

        public int DeleteFormSection(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "DeleteSection", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            var result = _dapper.Execute($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
        public List<DynamicFormSection> GetAllSectionByFormId(int formId, bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectByFormId", DbType.String);
            dbparams.Add("isAdmin", isAdmin, DbType.Boolean);
            dbparams.Add("FormId", formId, DbType.Int32);
            var result = _dapper.GetAll<DynamicFormSection>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<DynamicFormSection> GetAllSectionByFormGuid(Guid FormId, bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectByFormGuid", DbType.String);
            dbparams.Add("IsActive", isAdmin, DbType.Boolean);
            dbparams.Add("FormGuid", FormId, DbType.Guid);
            var result = _dapper.GetAll<DynamicFormSection>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public DynamicFormSection GetSectionById(int Id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectBySectionId", DbType.String);
            dbparams.Add("PKID", Id, DbType.Int32);
            var result = _dapper.Get<DynamicFormSection>($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int InsertFormSection(DynamicFormSection section)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "InsertSection", DbType.String);
            dbparams.Add("FormId", section.FormId, DbType.Int32);
            dbparams.Add("Section", section.Section, DbType.String);
            dbparams.Add("Detail", section.Detail, DbType.String);
            dbparams.Add("Footer", section.Footer, DbType.String);
            dbparams.Add("Screen", section.Screen, DbType.Int32);
            dbparams.Add("DisplayOrder", section.DisplayOrder, DbType.Int32);
            dbparams.Add("IsActive", section.IsActive, DbType.Boolean);
            var result = _dapper.ExecuteScalar($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }

        }

        public int UpdateFormSection(DynamicFormSection section)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "UpdateSection", DbType.String);
            dbparams.Add("PKID", section.PKID, DbType.Int32);
            dbparams.Add("FormId", section.FormId, DbType.Int32);
            dbparams.Add("Section", section.Section, DbType.String);
            dbparams.Add("Detail", section.Detail, DbType.String);
            dbparams.Add("Footer", section.Footer, DbType.String);
            dbparams.Add("Screen", section.Screen, DbType.Int32);
            dbparams.Add("DisplayOrder", section.DisplayOrder, DbType.Int32);
            dbparams.Add("IsActive", section.IsActive, DbType.Boolean);
            var result = _dapper.Execute($"DynamicForm_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;

        }

        public List<DQuestion> GetAllQuestionBySectionId(int SectionId, bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectAll", DbType.String);
            dbparams.Add("isAdmin", isAdmin, DbType.Boolean);
            dbparams.Add("SectionId", SectionId, DbType.Int32);
            var result = _dapper.GetAll<DQuestion>($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public DQuestion GetQuestionById(int Id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectById", DbType.String);
            dbparams.Add("PKID", Id, DbType.Int32);
            var result = _dapper.Get<DQuestion>($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int InsertQuestion(DQuestion question)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "InsertQuestion", DbType.String);
            dbparams.Add("SectionId", question.SectionId, DbType.Int32);
            dbparams.Add("QuestionType", question.QuestionType, DbType.Int32);
            dbparams.Add("Question", question.Question, DbType.String);
            dbparams.Add("Answer", question.Answer, DbType.String);
            dbparams.Add("Marks", question.Marks, DbType.Decimal);
            dbparams.Add("DisplayOrder", question.DisplayOrder, DbType.Int32);
            dbparams.Add("Step", question.Step, DbType.Int32);
            dbparams.Add("IsActive", question.IsActive, DbType.Boolean);
            var result = _dapper.ExecuteScalar($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int UpdateQuestion(DQuestion question)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "UpdateQuestion", DbType.String);
            dbparams.Add("PKID", question.PKID, DbType.Int32);
            dbparams.Add("SectionId", question.SectionId, DbType.Int32);
            dbparams.Add("QuestionType", question.QuestionType, DbType.Int32);
            dbparams.Add("Question", question.Question, DbType.String);
            dbparams.Add("Answer", question.Answer, DbType.String);
            dbparams.Add("Marks", question.Marks, DbType.Decimal);
            dbparams.Add("DisplayOrder", question.DisplayOrder, DbType.Int32);
            dbparams.Add("Step", question.Step, DbType.Int32);
            dbparams.Add("IsActive", question.IsActive, DbType.Boolean);
            return _dapper.Execute($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);

        }

        public int DeleteQuestion(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "DeleteForm", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            return _dapper.Execute($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
        }

        public List<DQuestionAnswer> GetAllAnswerByQuestionId(int QuestionId, bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectByQuestionId", DbType.String);
            dbparams.Add("isAdmin", isAdmin, DbType.Boolean);
            dbparams.Add("QuestionId", QuestionId, DbType.Int32);
            var result = _dapper.GetAll<DQuestionAnswer>($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public DQuestionAnswer GetAnswerById(int Id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectByAnswerId", DbType.String);

            dbparams.Add("PKID", Id, DbType.Int32);
            var result = _dapper.Get<DQuestionAnswer>($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int InsertAnswer(DQuestionAnswer answer)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "InsertAnswer", DbType.String);
            dbparams.Add("QuestionId", answer.QuestionId, DbType.Int32);
            dbparams.Add("Answer", answer.Answer, DbType.String);
            dbparams.Add("Details", answer.Details, DbType.String);
            dbparams.Add("Marks", answer.Marks, DbType.Decimal);
            dbparams.Add("DisplayOrder", answer.DisplayOrder, DbType.Int32);
            dbparams.Add("IsActive", answer.IsActive, DbType.Boolean);
            dbparams.Add("RMin", answer.RMin, DbType.Int32);
            dbparams.Add("RMax", answer.RMax, DbType.Int32);
            dbparams.Add("RIncrement", answer.Rincrement, DbType.Int32);
            var result = _dapper.ExecuteScalar($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }
        }

        public int UpdateAnswer(DQuestionAnswer answer)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "UpdateAnswer", DbType.String);
            dbparams.Add("PKID", answer.PKID, DbType.Int32);
            dbparams.Add("QuestionId", answer.QuestionId, DbType.Int32);
            dbparams.Add("Answer", answer.Answer, DbType.String);
            dbparams.Add("Details", answer.Details, DbType.String);
            dbparams.Add("Marks", answer.Marks, DbType.Decimal);
            dbparams.Add("DisplayOrder", answer.DisplayOrder, DbType.Int32);
            dbparams.Add("IsActive", answer.IsActive, DbType.Boolean);
            dbparams.Add("RMin", answer.RMin, DbType.Int32);
            dbparams.Add("RMax", answer.RMax, DbType.Int32);
            dbparams.Add("RIncrement", answer.Rincrement, DbType.Int32);
            return _dapper.Execute($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);

        }

        public int DeleteAnswer(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "DeleteAnswer", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            return _dapper.Execute($"DynamicQuestion_Master", dbparams, commandType: CommandType.StoredProcedure);
        }

        public int RecordAnswer(int sectionId, int questionId, int answerId, string answer, string userFileName, string sessionId, int userId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "INSERT", DbType.String);
            dbparams.Add("SectionId", sectionId, DbType.Int32);
            dbparams.Add("QuestionId", questionId, DbType.Int32);
            dbparams.Add("AnswerId", answerId, DbType.Int32);
            dbparams.Add("Answer", answer, DbType.String);
            dbparams.Add("UserFileName", userFileName, DbType.String);
            dbparams.Add("SessionId", sessionId, DbType.String);
            dbparams.Add("UserId", userId, DbType.String);
            var result = _dapper.Execute($"RecordAnswer", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<DAnswerRecord> GetAnswerRecordBySessionId(int formId, string sessionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectBySessionId", DbType.String);
            dbparams.Add("SessionId", sessionId, DbType.String);
            dbparams.Add("FormId", formId, DbType.Int32);

            var result = _dapper.GetAll<DAnswerRecord>($"RecordAnswer", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public int DeleteAnswerBySessionId(string sessionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "DeleteBySessionId", DbType.String);
            dbparams.Add("SessionId", sessionId, DbType.String);
           
            var result = _dapper.Execute($"RecordAnswer", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
    }

    public class DFormCategoryServices : IDFormCategory
    {
        private readonly IDapper _dapper;
        public DFormCategoryServices(IDapper dapper)
        {

            this._dapper = dapper;
        }

        public List<DFormCategory> GetAllCategories(bool isAdmin)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectAll", DbType.String);
            dbparams.Add("IsAdmin", isAdmin, DbType.Boolean);
            var result = _dapper.GetAll<DFormCategory>($"DFormCategory_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public DFormCategory GetCategoryById(int Id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "SelectById", DbType.String);
            dbparams.Add("PKID", Id, DbType.Int32);
            var result = _dapper.Get<DFormCategory>($"DFormCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int InsertCategory(DFormCategory form)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "INSERT", DbType.String);
            dbparams.Add("@CategoryName", form.CategoryName, DbType.String);
            dbparams.Add("@ParentId", form.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", form.IsActive, DbType.Boolean);
            
            var result = _dapper.ExecuteScalar($"DFormCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            else
            {
                return 0;
            }

        }

        public int UpdateCategory(DFormCategory form)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "UPDATE", DbType.String);
            dbparams.Add("PKID", form.PKID, DbType.Int32);
            dbparams.Add("@CategoryName", form.CategoryName, DbType.String);
            dbparams.Add("@ParentId", form.ParentId, DbType.Int32);
            dbparams.Add("@IsActive", form.IsActive, DbType.Boolean);
            var result = _dapper.Execute($"DFormCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;

        }

        public int DeleteCategory(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("action", "Delete", DbType.String);
            dbparams.Add("PKID", id, DbType.Int32);
            var result = _dapper.Execute($"DFormCategory_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }
    }


  

}
