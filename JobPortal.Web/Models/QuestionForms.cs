 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using JobPortal.Web.Models;
using JobPortal.Models;

namespace HelloKisan.Areas.WebAdmin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("WebAdmin")]
    public class QuestionFormsController : Controller
    {
        private readonly IDForms _dForms;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDFormCategory _formCategory;
        public QuestionFormsController(IDForms dForms, IWebHostEnvironment webHostEnvironment, IDFormCategory formCategory)
        {
            _webHostEnvironment = webHostEnvironment;
            _dForms = dForms;
             _formCategory=formCategory;
        }

        public IActionResult Index()
        {
            var lst = _dForms.GetAllForms(1, true);
            return View(lst);
        }

        public IActionResult Create()
        {
            DynamicForm form = new DynamicForm();
            return View(form);
        }

        [HttpPost]
        public IActionResult Create(DynamicForm form, IFormFile flImageUrl, IFormFile flImageUrlLeft, IFormFile flImageBGUrl)
        {
            try
            {
                string CommonCss =   JsonConvert.SerializeObject(form.CommonCSS);
                form.CommonCssinStr = CommonCss;
                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/dforms/");

                string ImageRight = form.ImageUrl == null ? "" : form.ImageUrl;

                string ImageLeft = form.ImageUrlLeft == null ? "" : form.ImageUrlLeft;

                string bgImageurl = form.BGUrl == null ? "" : form.BGUrl;

                if (flImageUrl != null && flImageUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrl.CopyTo(stream);
                        }

                        form.ImageUrl = newimagename;

                    }


                }
                else
                {
                    form.ImageUrl = ImageRight;
                }

                if (flImageUrlLeft != null && flImageUrlLeft.Length > 0)
                {
                    var ext = Path.GetExtension(flImageUrlLeft.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrlLeft.CopyTo(stream);
                        }

                        form.ImageUrlLeft = newimagename;

                    }


                }
                else
                {
                    form.ImageUrlLeft = ImageLeft;
                }


                if (flImageBGUrl != null && flImageBGUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flImageBGUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageBGUrl.CopyTo(stream);
                        }

                        form.BGUrl = newimagename;

                    }


                }
                else
                {
                    form.BGUrl = bgImageurl;
                }

                form.CategoryId = 1;
                int i = _dForms.InsertForm(form);
                if (i > 0)
                {
                    ViewBag.msg = "Form added successfully";
                    return Redirect("index");
                }
                else
                {
                    ViewBag.msg = "\"Form couldn't be added successfully.";
                }

            }
            catch (Exception ex)
            {

                //CardCommon.WriteErrorLog(ex.Message);
            }
            return View(form);
        }

        public IActionResult Edit(int? id)
        {
            ViewBag.msg = "";


            if (id.HasValue)
            {
                var form = _dForms.GetFormById(id.Value);
                //string CommonCss = JsonConvert.SerializeObject(form.CommonCSS);
                //form.CommonCssinStr = CommonCss;
                if (!string.IsNullOrEmpty(form.CommonCssinStr))
                {
                    CommonCSS commonCss = JsonConvert.DeserializeObject<CommonCSS>(form.CommonCssinStr);

                    form.CommonCSS = commonCss;
                }
                else
                {
                    form.CommonCSS = new CommonCSS();
                }
                return View(form);
            }
            else
            {
                return View("Index");
            }
        }

        [HttpPost]
        public IActionResult Edit(DynamicForm form, IFormFile flImageUrl, IFormFile flImageUrlLeft, IFormFile flImageBGUrl)
        {
            try
            {

                string CommonCss = JsonConvert.SerializeObject(form.CommonCSS);
                form.CommonCssinStr = CommonCss;
                var allowedExtensions = new[] { ".jpg", ".png", ".jpg", ".jpeg" };
                var paperBasePath = Path.Combine(_webHostEnvironment.WebRootPath, "images/dforms/");

                string ImageRight = form.ImageUrl == null ? "" : form.ImageUrl;

                string ImageLeft = form.ImageUrlLeft == null ? "" : form.ImageUrlLeft;

                string bgImageurl = form.BGUrl == null ? "" : form.BGUrl;


                if (flImageUrl != null && flImageUrl.Length > 0)
                {

                    var ext = Path.GetExtension(flImageUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {

                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrl.CopyTo(stream);
                        }

                        form.ImageUrl = newimagename;

                    }


                }

                if (flImageUrlLeft != null && flImageUrlLeft.Length > 0)
                {
                    var ext = Path.GetExtension(flImageUrlLeft.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageUrlLeft.CopyTo(stream);
                        }

                        form.ImageUrlLeft = newimagename;

                    }


                }
                else
                {
                    form.ImageUrlLeft = ImageLeft;
                }


                if (flImageBGUrl != null && flImageBGUrl.Length > 0)
                {
                    var ext = Path.GetExtension(flImageBGUrl.FileName).ToLower();
                    var newimagename = Guid.NewGuid().ToString() + ext;
                    if (allowedExtensions.Contains(ext)) //check what type of extension  
                    {
                        var filePath = Path.Combine(paperBasePath, newimagename);
                        // Save the file
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            flImageBGUrl.CopyTo(stream);
                        }

                        form.BGUrl = newimagename;

                    }


                }
                else
                {
                    form.BGUrl = bgImageurl;
                }



                int i = _dForms.UpdateForm(form);
                if (i > 0)
                {
                    ViewBag.msg = "Form update successfully";
                    return RedirectToAction("index", "QuestionForms");
                }
                else
                {
                    ViewBag.msg = "Form couldn't be updated.";
                }

            }
            catch (Exception ex)
            {

                // CardCommon.WriteErrorLog(ex.Message);
            }
            return View(form);
        }


        public IActionResult ManageForm(int? id, int? sheetId, int? blockId, int? answerId)
        { //questionId
            DynamicForm model = new DynamicForm();
            ViewBag.Url = Request.Host.Value; //.GetDisplayUrl();
            ViewBag.CurrentSection = sheetId.HasValue ? sheetId.Value : 0;
            ViewBag.CurrentQuestion = blockId.HasValue ? blockId.Value : 0;
            if (id.HasValue && id.Value > 0)
            {
                model = _dForms.GetFormById(id.Value);

                try
                {
                    if (!string.IsNullOrEmpty(model.CommonCssinStr))
                    {
                        CommonCSS commonCss = JsonConvert.DeserializeObject<CommonCSS>(model.CommonCssinStr);

                        model.CommonCSS = commonCss;
                    }
                    else
                    {
                        model.CommonCSS = new CommonCSS();
                    }
                }
                catch
                {


                }


                var obj = _dForms.GetAllSectionByFormId(id.Value, true);
                if (obj != null)
                {
                    foreach (var itm in obj)
                    {
                       var questinoslist = _dForms.GetAllQuestionBySectionId(itm.PKID, true);
                        
                        foreach (var qst in questinoslist)
                        {
                            qst.Answers = _dForms.GetAllAnswerByQuestionId(qst.PKID, true);
                        }
                        itm.Questions = questinoslist;
                    }
                    model.Sections = obj;
                }
                else
                {
                    model.Sections = new List<DynamicFormSection>();
                }

                if (sheetId.HasValue && sheetId.Value > 0)
                {
                    model.SectionId = sheetId.Value;
                    var current = model.Sections.Where(x => x.PKID == sheetId.Value).FirstOrDefault();
                    if (current != null)
                    {
                        model.CurrentSection = current;
                    }
                    else
                    {
                        model.CurrentSection = new DynamicFormSection { PKID = 0, FormId = id.Value, Section = "", Detail = "", Footer = "", Screen = 0, DisplayOrder = 0, IsActive = true };
                    }

                    var questionlst = _dForms.GetAllQuestionBySectionId(sheetId.Value, true);
                    if (questionlst != null)
                    {
                        model.CurrentSection.Questions = questionlst;
                        foreach (var qst in questionlst)
                        {
                            qst.Answers = _dForms.GetAllAnswerByQuestionId(qst.PKID, true);
                        }
                    }
                    else
                    {
                        model.CurrentSection.Questions = new List<DQuestion>();
                    }

                    if (blockId.HasValue && model.CurrentSection.Questions.Count > 0)
                    {

                        var question = (from m in questionlst where m.PKID == blockId.Value select m).FirstOrDefault();
                        if (question != null)
                        {
                            model.CurrentSection.CurrentQuestion = question;
                            model.CurrentSection.CurrentQuestion.Answers = _dForms.GetAllAnswerByQuestionId(question.PKID, true);
                        }
                        else
                        {
                            model.CurrentSection.CurrentQuestion = new DQuestion { PKID = 0, DisplayOrder = 1, IsActive = true, Marks = 10, SectionId = model.SectionId };
                        }

                        if (answerId.HasValue && model.CurrentSection.CurrentQuestion.Answers.Count > 0)
                        {
                            var answerlist = _dForms.GetAllAnswerByQuestionId(question.PKID, true);
                            var answer = (from m in answerlist where m.PKID == answerId.Value select m).FirstOrDefault();
                            if (answer != null)
                            {
                                model.CurrentSection.CurrentQuestion.CurrentAnswer = answer;

                            }
                            else
                            {
                                model.CurrentSection.CurrentQuestion.CurrentAnswer = new DQuestionAnswer { PKID = 0, DisplayOrder = 1, IsActive = true, Marks = 10, QuestionId = question.PKID };
                            }
                        }

                        //if (costitem != null)
                        //{
                        //    model.CostItem = costitem;
                        //    foreach (var itm in listItem)
                        //    {
                        //        if (int.Parse(itm.Value) == model.CostItem.UnitOperator)
                        //        {
                        //            itm.Selected = true;
                        //        }
                        //        else
                        //        {
                        //            itm.Selected = false;
                        //        }
                        //    }
                        //}
                        //else
                        //{
                        //    model.CostItem = new CostingModel { SheetDetailId = 0, SheetId = sheetId.Value };
                        //}
                    }
                }


            }


            return View(model);
        }

        [HttpPost]
        public IActionResult ManageForm(DynamicForm model, string hidType)
        {

            try
            {
                int questionId = 0;
                DynamicFormSection section = new DynamicFormSection();
                ViewBag.CurrentSection = model.SectionId ;
                ViewBag.CurrentQuestion =  0;
                section.PKID = model.SectionId;
                section.FormId = model.PKID;
                section.Section = !string.IsNullOrEmpty(model.CurrentSection.Section) ? model.CurrentSection.Section : "";
                section.Detail = !string.IsNullOrEmpty(model.CurrentSection.Detail) ? model.CurrentSection.Detail : "";
                section.Footer = !string.IsNullOrEmpty(model.CurrentSection.Footer) ? model.CurrentSection.Footer : "";
                section.Screen = model.CurrentSection.Screen;
                section.DisplayOrder = model.CurrentSection.DisplayOrder;
                section.IsActive = model.CurrentSection.IsActive;

                

                if (model.PKID > 0 && hidType == "1")
                {
                    var currentform = _dForms.GetFormById(model.PKID);
                    currentform.Title = model.Title;
                    currentform.Footer = model.Footer;
                    _dForms.UpdateForm(currentform);
                }




                if (model.SectionId == 0 && hidType == "2")
                {
                    model.SectionId = _dForms.InsertFormSection(section);
                    TempData["msg"] = "section added successfully";
                }
                else
                {
                    if (hidType == "2")
                    {
                        _dForms.UpdateFormSection(section);
                    }
                    if (model.CurrentSection.CurrentQuestion != null)
                    {
                        if (hidType == "3")
                        {
                            DQuestion question = new DQuestion();

                            model.CurrentSection.CurrentQuestion.SectionId = model.SectionId;
                            if (model.CurrentSection.CurrentQuestion.PKID > 0)
                            {

                                question = model.CurrentSection.CurrentQuestion;
                                _dForms.UpdateQuestion(question);
                                questionId = question.PKID;
                            }
                            else
                            {

                                question = model.CurrentSection.CurrentQuestion;
                                questionId = _dForms.InsertQuestion(question);

                            }
                            ViewBag.CurrentQuestion = questionId;
                        }
                        else if (model.CurrentSection.CurrentQuestion.CurrentAnswer != null && hidType == "4")
                        {
                            DQuestionAnswer answer = new DQuestionAnswer();
                            answer.QuestionId = model.CurrentSection.CurrentQuestion.PKID;
                            model.CurrentSection.CurrentQuestion.CurrentAnswer.QuestionId = model.CurrentSection.CurrentQuestion.PKID;
                            if (model.CurrentSection.CurrentQuestion.CurrentAnswer.PKID > 0)
                            {

                                answer = model.CurrentSection.CurrentQuestion.CurrentAnswer;
                                _dForms.UpdateAnswer(answer);
                            }
                            else
                            {

                                answer = model.CurrentSection.CurrentQuestion.CurrentAnswer;
                                _dForms.InsertAnswer(answer);
                            }
                        }

                    }


                    TempData["msg"] = "Sheet updated successfully";

                }

                if (questionId > 0)
                    return RedirectToAction("ManageForm", "QuestionForms", new { id = model.PKID, sheetId = model.SectionId, blockId = questionId });
                else
                    return RedirectToAction("ManageForm", "QuestionForms", new { id = model.PKID, sheetId = model.SectionId });
            }
            catch
            {
                return View(model);
            }


        }


        public IActionResult DeleteAnswer(int id)
        {
            JsonResultResponse jsonResultResponse = new JsonResultResponse();
            if (id > 0)
            {
                int sheetDetailId = _dForms.DeleteAnswer(id);
                jsonResultResponse.Message = "Answer deleted successfully";
                jsonResultResponse.Status = true;
            }
            else
            {
                jsonResultResponse.Message = "Answer couldn't be deleted";
                jsonResultResponse.Status = false;
            }

            return Json(jsonResultResponse); // Send data back as JSON
        }


        public IActionResult Categories()
        {
            var lst = _formCategory.GetAllCategories(true);
            return View(lst);
        }


        [HttpPost]
        public IActionResult Duplicate(int id)
        {

            var result = _dForms.Duplicate(id);
            if (result > 0)
            {

                return Json(new { success = true, message = "Record created successfully" });
            }

            return Json(new { success = false, message = "Error! can't created." });
        }
    }
}
