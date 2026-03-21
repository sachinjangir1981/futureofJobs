using Dapper;
using Dapper_ORM.Services;
using JobPortal.Domain.Models;
using JobPortal.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
 


namespace JobPortal.Models
{
    public class Questions
    {
        public int PKID { get; set; }
        [Required]
     
        [DataType(DataType.MultilineText)]
        public string Question { get; set; }
        [Required]
        public int DisplayOrder { get; set; }
        public int CategoryId { get; set; }
        public bool IsActive { get; set; }
        public string CategoryNames { get; set; }
    }

    public class Answers
    {
        public int PKID { get; set; }
        public int QuestionId { get; set; }
       
        public string Answer { get; set; }
        public int DisplayOrder { get; set; }
 
    }


    public class QuestionforServey
    {
        public int PKID { get; set; }
        public string Question { get; set; }
        public string SubHeading { get; set; }
        public List<Answers> AnswerList { get; set; }
        public int TotalPages { get; set; }
        public int PageNo { get; set; }
        public int CategoryId { get; set; }

    }

    public class PollResult
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public string Answer { get; set; }
        public int AllAnswerCount { get; set; }
        public int TotalCount { get; set; }
        public string ColorCode { get; set; }
        public int Ratio { get; set; }

    }

    public class JsonResultResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public int CostSheetId { get; set; }
        public int SheetId { get; set; }
        public int SheetDetailId { get; set; }
    }
    public class Questionnaires
    {
        public int PKID { get; set; }

        public string Questionaire { get; set; }
        public string SubHeading { get; set; }
        public string Details { get; set; }
        public string ImageUrl { get; set; }
        public bool IsActive { get; set; }

        public bool IsSelected { get; set; }

        public int DisplayOrder { get; set; }

        public string ActionText { get; set; }

        public string BGImage { get; set; }

        public string BGColor { get; set; } = "#FFFFFF";


    }

    public class PollModel : IPollModel
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _iconfigure;
        private readonly IDapper _dapper;
        public PollModel(AppDbContext db, IConfiguration iconfigure, IDapper dapper)
        {
            _db = db;
            _iconfigure = iconfigure;
            this._dapper = dapper;
        }
        #region "Questions"
        public int AddQuestion(Questions question)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT");
            dbparams.Add("@Question", question.Question);
            dbparams.Add("@DisplayOrder", question.DisplayOrder);
            dbparams.Add("@IsActive", question.IsActive);
            var result = _dapper.ExecuteScalar($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure);
            if (result != null)
            {
                return int.Parse(result.ToString());
            }
            else
                return 0;
             
             

        }
        public int Updatequestion(Questions question)
        {
             
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE");
            dbparams.Add("@pkid", question.PKID);
            dbparams.Add("@Question", question.Question);
            dbparams.Add("@DisplayOrder", question.DisplayOrder);
            dbparams.Add("@IsActive", question.IsActive);
            var result = _dapper.Execute($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int UpdateQuestionCategory(int oldCategoryId, int newCategoryId, int questionId)
        {
            
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UpdateCategpory");
            dbparams.Add("@questionId", questionId);
            dbparams.Add("@NewCategoryId", newCategoryId);
            dbparams.Add("@CategoryId", oldCategoryId);
            var result = _dapper.Execute($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public List<Questions> GetAllQuestionList()
        {


            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL");
            var result = _dapper.GetAll<Questions>($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<Questions> GetAllQuestionListByCategoryId(int categoryId)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALLBYCATEGORYID");
            dbparams.Add("@CategoryId", categoryId);
            var result = _dapper.GetAll<Questions>($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public List<QuestionforServey> GetAllQuestionListForSurvey(int QuestionnairId, int pageNo, int questionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALLFRONT");
            dbparams.Add("@CategoryId", QuestionnairId);
            dbparams.Add("@PageNo", pageNo);
            dbparams.Add("@questionId", questionId);
            var result = _dapper.GetAll<QuestionforServey>($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }

        public Questions GetquestionById(int id)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYId");
            dbparams.Add("@PKID", id);
            var result = _dapper.Get<Questions>($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;

 
        }

        #endregion



        #region "Answer"
        public int AddAnswer(Answers answer)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT");
            dbparams.Add("@QuestionId", answer.QuestionId);
            dbparams.Add("@DisplayOrder", answer.DisplayOrder);
            dbparams.Add("@Answer", answer.Answer);
            var result = _dapper.Execute($"AnswerMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
             

        }
        public int UpdateAnswers(Answers answer)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE");
            dbparams.Add("@pkid", answer.PKID);
            dbparams.Add("@QuestionId", answer.QuestionId);
            dbparams.Add("@DisplayOrder", answer.DisplayOrder);
            dbparams.Add("@Answer", answer.Answer);
            var result = _dapper.Execute($"AnswerMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        public int DeleteAnswers(int id)
        {

            var dbparams = new DynamicParameters();
            
            
            var result = _dapper.Execute($"Delete from Answers where pkid="+ id.ToString(), dbparams, commandType: CommandType.Text);
            return result;
        }

        public List<Answers> GetAllAnswerByQuestionId(int questionId)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL");
            dbparams.Add("@QuestionId", questionId);

            var result = _dapper.GetAll<Answers>($"AnswerMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }



        public Answers GetAnswerById(int id)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYId");
            dbparams.Add("@PKID", id);
            var result = _dapper.Get<Answers>($"AnswerMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        #endregion


        #region "Poll"
        public List<PollResult> PollAddorUpdate(int questionId, int answerId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERTORUPDATE");
            dbparams.Add("@Questionid", questionId);
            dbparams.Add("@AnswerId", answerId);

            var result = _dapper.GetAll<PollResult>($"PollResultMaster", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;
        }



        public PollResult SelectByQuestionId(int questionId)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SelecgtByQuestionId");
            dbparams.Add("@Questionid", questionId);
            var result = _dapper.Get<PollResult>($"PollResultMaster", dbparams, commandType: CommandType.StoredProcedure);
            return result;
        }

        #endregion

        #region "Questionnaires"
        public int AddQuestionnaires(Questionnaires ques)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "INSERT");
            dbparams.Add("@Questionaire", ques.Questionaire);
            dbparams.Add("@SubHeading", ques.SubHeading);
            dbparams.Add("@IsActive", ques.IsActive);
            dbparams.Add("@Details", ques.Details);
            dbparams.Add("@ImageUrl", ques.ImageUrl);
            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
 
        }
        public int UpdateQuestionnaires(Questionnaires ques)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "UPDATE");
            dbparams.Add("@pkid", ques.PKID);
            dbparams.Add("@Questionaire", ques.Questionaire);
            dbparams.Add("@SubHeading", ques.SubHeading);
            dbparams.Add("@IsActive", ques.IsActive);
            dbparams.Add("@Details", ques.Details);
            dbparams.Add("@ImageUrl", ques.ImageUrl);
            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
             
        }

        public int DeleteQuestionnaires(int id)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@PKID", id);

            var result = _dapper.Execute($"DELETE  Questionnaires    WHERE PKID=@PKID", dbparams, commandType: CommandType.Text);
            return result;
             
        }

        public List<Questionnaires> GetAllQuestionnaires(int questionId)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTALL");
            dbparams.Add("@QuestionId", questionId);
            var result = _dapper.GetAll<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure).ToList();
            return result;

        }



        public Questionnaires GetQuestionnairesById(int id)
        {
            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "SELECTBYID");
            dbparams.Add("@PKID", id);
            var result = _dapper.Get<Questionnaires>($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
 
        }


        public int DeleteQuestionCategoryMapping(int questionId)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "DeleteMapping");
            dbparams.Add("@QuestionId", questionId);

            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;

             

        }


        public int AddQuestionCategoryMapping(int questionId, int categoryId, int displayOrder)
        {

            var dbparams = new DynamicParameters();
            dbparams.Add("@action", "AddMapping");
            dbparams.Add("@QuestionId", questionId);
            dbparams.Add("@PKID", categoryId);
            dbparams.Add("@DisplayOrder", displayOrder);

            var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
            return result;
             

        }


        public int UpdateDisplayOrder(int categoryId, int questionId, int displayOrder)
        {
            if (categoryId > 0)
            {

                var dbparams = new DynamicParameters();
                dbparams.Add("@action", "UPDATEDISPLAYORDER");
                dbparams.Add("@pkid", categoryId);
                dbparams.Add("@QuestionId", questionId);
                dbparams.Add("@DisplayOrder", displayOrder);

                var result = _dapper.Execute($"Questionnaire_Master", dbparams, commandType: CommandType.StoredProcedure);
                return result;

            }
            else
            {
                var dbparams = new DynamicParameters();
                dbparams.Add("@action", "UPDATEDISPLAYORDER");
                dbparams.Add("@pkid", questionId);
                dbparams.Add("@DisplayOrder", displayOrder);

                var result = _dapper.Execute($"QuestionMaster", dbparams, commandType: CommandType.StoredProcedure);
                return result;
            }
        }

        #endregion
    }
}