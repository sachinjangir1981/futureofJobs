namespace JobPortal.Web.Models
{
    public interface IDForms
    {
        List<DynamicForm> GetAllForms(int categoryId, bool isAdmin);
        DynamicForm GetFormById( int Id);
        DynamicForm GetFormByGuid(Guid FormId);
        int InsertForm(DynamicForm faq);
        int UpdateForm(DynamicForm faq);
        int DeleteForm(int id);
        int Duplicate(int id);



        List<DynamicFormSection> GetAllSectionByFormId(int formId, bool isAdmin);
        List<DynamicFormSection> GetAllSectionByFormGuid(Guid FormId, bool isAdmin);
        DynamicFormSection GetSectionById(int Id);
        int InsertFormSection(DynamicFormSection faq);
        int UpdateFormSection(DynamicFormSection faq);
        int DeleteFormSection(int id);



        List<DQuestion> GetAllQuestionBySectionId(int SectionId, bool isAdmin);
        DQuestion GetQuestionById(int Id);
        int InsertQuestion(DQuestion question);
        int UpdateQuestion(DQuestion question);
        int DeleteQuestion(int id);



        List<DQuestionAnswer> GetAllAnswerByQuestionId(int QuestionId, bool isAdmin);
        DQuestionAnswer GetAnswerById(int Id);
        int InsertAnswer(DQuestionAnswer answer);
        int UpdateAnswer(DQuestionAnswer answer);
        int DeleteAnswer(int id);


        int RecordAnswer(int sectionId, int questionId,int answerId, string answer, string userFileName, string sessionId,int userId);
        List<DAnswerRecord> GetAnswerRecordBySessionId(int formId, string sessionId);

        int DeleteAnswerBySessionId (string sessionId);
    }

    public interface IDFormCategory
    {
        List<DFormCategory> GetAllCategories(bool isAdmin);
        DFormCategory GetCategoryById(int Id);
        int InsertCategory(DFormCategory category);
        int UpdateCategory(DFormCategory category);
        int DeleteCategory(int id);

    }




   
}
