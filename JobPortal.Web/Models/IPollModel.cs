
namespace JobPortal.Models
{
    public interface IPollModel
    {
        int AddAnswer(Answers answer);
        int AddQuestion(Questions question);
        int AddQuestionCategoryMapping(int questionId, int categoryId, int displayOrder);
        int AddQuestionnaires(Questionnaires ques);
        int DeleteAnswers(int id);
        int DeleteQuestionCategoryMapping(int questionId);
        int DeleteQuestionnaires(int id);
        List<Answers> GetAllAnswerByQuestionId(int questionId);
        List<Questions> GetAllQuestionList();
        List<Questions> GetAllQuestionListByCategoryId(int categoryId);
        List<QuestionforServey> GetAllQuestionListForSurvey(int QuestionnairId, int pageNo, int questionId);
        List<Questionnaires> GetAllQuestionnaires(int questionId);
        Answers GetAnswerById(int id);
        Questions GetquestionById(int id);
        Questionnaires GetQuestionnairesById(int id);
        List<PollResult> PollAddorUpdate(int questionId, int answerId);
        PollResult SelectByQuestionId(int questionId);
        int UpdateAnswers(Answers answer);
        int UpdateDisplayOrder(int categoryId, int questionId, int displayOrder);
        int Updatequestion(Questions question);
        int UpdateQuestionCategory(int oldCategoryId, int newCategoryId, int questionId);
        int UpdateQuestionnaires(Questionnaires ques);
    }

    public interface IServey
    {
        List<QuestionforServey> GetAllQuestionListForSurvey(int QuestionnairId, int pageNo, int questionId);

        List<Answers> GetAllAnswerByQuestionId(int questionId);

        List<PollResult> PollAddorUpdate(int questionId, int answerId);
        int AddServeyHeader(Questionnaires servey);
        int UpdateServeyHeader(Questionnaires servey);
        int DeleteServeyHeader(int id);
        List<Questionnaires> GetAllServeyHeader(bool IsAdmin);
        Questionnaires GetServeyHeaderById(int id);

        List<Questionnaires> GetAllServeyListForGenZ();

        List<Questionnaires> GetAllServeyListForByText(string firsttext);
    }
}