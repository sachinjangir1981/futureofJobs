using JobPortal.Infrastructure.Data;
using JobPortal.Infrastructure.Services;
using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace JobPortal.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PollsController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IPollModel _polls;
        private readonly ILogger<PollsController> _logger;
        public PollsController(AppDbContext db, ILogger<PollsController> logger, IPollModel polls)
        {
            _db = db;
            _polls = polls;
            _logger = logger;
        }

        public IActionResult Index(int? id)
        {
            List<Questions> questionList;
            
            var lst =   _polls.GetAllQuestionnaires(0);

            var CategoryList = (from m in lst select new SelectListItem { Text = m.DisplayOrder.ToString() + ". " + m.Questionaire, Value = m.PKID.ToString(), Selected = m.IsSelected }).ToList();
            CategoryList.Insert(0, new SelectListItem { Value = "0", Text = "All" });
             id = id.HasValue == false ? 1 : id.Value;
            foreach (var itm in CategoryList)
            {
                if (id.HasValue)
                {
                    if (int.Parse(itm.Value) == id.Value)
                    {
                        itm.Selected = true;
                    }
                }
            }
            ViewBag.CategoryList = CategoryList;
            if (id.HasValue && id.Value > 0)
                questionList = _polls.GetAllQuestionListByCategoryId(id.Value);
            else
                questionList = _polls.GetAllQuestionList();

            return View(questionList);
        }

        public IActionResult ManageQuestion(int? id)
        {

            Questions question = new Questions();
            try
            {
                if (id.HasValue)
                {
                    question =   _polls.GetquestionById(id.Value);

                }
                var lst = _polls.GetAllQuestionnaires(id.HasValue ? id.Value : 0);

                ViewBag.CategoryList = (from m in lst select new SelectListItem { Text = m.DisplayOrder.ToString() + ". " + m.Questionaire, Value = m.PKID.ToString(), Selected = m.IsSelected }).ToList();



            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error in Manage Question");
                _logger.LogError(ex.StackTrace, "Error in Manage Question");
            }
            return View(question);
        }

        [HttpPost]
        public IActionResult ManageQuestion(int? id, Questions ques, IFormCollection coll)
        {
             
            ViewBag.CategoryList = _polls.GetAllQuestionnaires(id.HasValue ? id.Value : 0);
            try
            {

                if (ques != null)
                {

                    if (ques.PKID > 0)
                        _polls.Updatequestion(ques);
                    else
                        ques.PKID = _polls.AddQuestion(ques);
                }

                var categoy = coll["Category"] ;



                if (!string.IsNullOrEmpty(categoy) && ques.PKID > 0)
                {
                    _polls.DeleteQuestionCategoryMapping(ques.PKID);
                    foreach (var itm in categoy.ToString().Split(','))
                    {
                        _polls.AddQuestionCategoryMapping( ques.PKID, int.Parse(itm), ques.DisplayOrder);
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                return View(ques);

            }

        }


        public IActionResult ManageAnswers(int? id)
        {
             
            Questions question = new Questions();
            List<Answers> answerList = new List<Answers>();
            try
            {
                if (id.HasValue)
                {
                    question = _polls.GetquestionById(id.Value);
                    ViewBag.Question = question;
                    answerList = _polls.GetAllAnswerByQuestionId(id.Value);
                }

            }
            catch
            {
            }
            return View(answerList);
        }

        [HttpPost]
        public IActionResult ManageAnswers(int id, string Answer, int DisplayOrder)
        {
              
            try
            {
                ViewBag.QuestionId = id;

                Answer = Answer.Replace("<p>", "").Replace("</p>", "");

                _polls.AddAnswer(new Answers { Answer = Answer, QuestionId = id, DisplayOrder = DisplayOrder, PKID = 0 });




                var question = _polls.GetquestionById(id);
                ViewBag.Question = question;
                var answerList = _polls.GetAllAnswerByQuestionId(id);
                return View(answerList);
            }
            catch (Exception ex)
            {
                return View();

            }

        }

        [HttpPost]
        public JsonResult UpdateAnswer(int? id, string answer, int displayorder)
        {
             
             
            try
            {
                if (answer != null)
                {

                    if (id.HasValue && id.Value > 0)
                    {
                        _polls.UpdateAnswers(new Answers { Answer = answer, DisplayOrder = displayorder, PKID = id.Value });
                    }

                }


                return Json(new { msg = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = "error" });

            }

        }


        [HttpPost]
        public JsonResult DeleteAnswer(int id)
        {
            
            try
            {


                if (id > 0)
                {
                    _polls.DeleteAnswers(id);
                }




                return Json(new { msg = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = "error" });

            }

        }


        [HttpPost]
        public JsonResult UpdateDisplayOrder(int id, int questionId, int displayorder)
        {
            
            try
            {

                _polls.UpdateDisplayOrder(id, questionId, displayorder);

                return Json(new { msg = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = "error" });

            }

        }


        [HttpPost]
        public IActionResult UpdateAnswerList(IFormCollection coll)
        {
           
            string answer = coll["hidAnswer"].ToString();
            string id = coll["hidPKID"].ToString();
            string displayorder = coll["hidDisplayOrder"].ToString();
            string questionId = coll["hidQuestionId"].ToString();

            //ViewBag.CategoryList = obj.GetAllQuestionnaires(id.HasValue ? id.Value : 0);
            try
            {



                if (answer != null)
                {

                    if (int.Parse(id) > 0)
                    {
                        _polls.UpdateAnswers(new Answers { Answer = answer, DisplayOrder = int.Parse(displayorder), PKID = int.Parse(id) });
                    }

                }
                return RedirectToAction("ManageAnswers", new { @id = questionId });
            }
            catch (Exception ex)
            {

                return RedirectToAction("ManageAnswers");

            }

        }


        [HttpPost]
        public JsonResult UpdateToNewCategory(int questionId, int categoryId, int newCategoryId)
        {
           
            try
            {

                _polls.UpdateQuestionCategory(categoryId, newCategoryId, questionId);

                return Json(new { msg = "success" });
            }
            catch (Exception ex)
            {
                return Json(new { msg = "error" });

            }

        }
    }
}
