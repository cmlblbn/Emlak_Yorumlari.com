using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;
using Emlak_Yorumlari.Models;
using System.Data.Entity.Migrations;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class AdminSurveyController : Controller
    {
        private MyContext db = new MyContext();
        // GET: AdminSurvey
        public ActionResult Adminsurvey()
        {
            AdminSurveyViewModel model = new AdminSurveyViewModel();
            model.question_list = new List<Question_Definition>();
            model.question_list = db.Question_Definitions.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult Adminsurvey(AdminSurveyViewModel model)
        {
            model.question_list = new List<Question_Definition>();
            model.question_list = db.Question_Definitions.ToList();
            if (model.questionName == null)
            {
                ModelState.AddModelError("", "Lütfen Soru Adı Giriniz!");
            }
            if(model.questionType == "1" && model.comboBoxAnswers == null)
            {
                ModelState.AddModelError("", "Combobox tipinde veri girişi yaparken cevap girmek zorundasınız!");
            }



            Question_Definition q_append = new Question_Definition();
            q_append.question_type_id = int.Parse(model.questionType);
            q_append.question_name = model.questionName;
            q_append.IsActive = true;
            db.Question_Definitions.Add(q_append);
            db.SaveChanges();

            if (model.comboBoxAnswers != null && model.questionType == "1")
            {

                Question_Definition q_find = new Question_Definition();
                MyContext updateDatabase = new MyContext();
                q_find = db.Question_Definitions.Where(x => x.question_name == model.questionName).FirstOrDefault();
                var answers = model.comboBoxAnswers.Split(' ');
                if (q_find != null)
                {
                    foreach (var answerName in answers)
                    {
                        Combobox_Answer answer = new Combobox_Answer();
                        answer.question_id = q_find.question_id;
                        answer.question_answer = answerName;
                        answer.IsActive = true;
                        updateDatabase.Combobox_Answers.Add(answer);
                        updateDatabase.SaveChanges();
                    }


                }



            }


            return RedirectToAction("Adminsurvey");
        }

        public ActionResult QuestionEdit(int? id)
        {
            if(id != null)
            {
                TempData["q_id"] = id;
                AdminSurveyViewModel model = new AdminSurveyViewModel();
                Question_Definition q_edit = new Question_Definition();
                List<Combobox_Answer> allAnswers = new List<Combobox_Answer>();
                q_edit = db.Question_Definitions.Where(x => x.question_id == id).FirstOrDefault();
                allAnswers = db.Combobox_Answers.Where(x => x.question_id == id).ToList();
                if (q_edit != null)
                {
                    model.questionName = q_edit.question_name;
                    model.questionType = q_edit.question_type_id.ToString();
                    model.question_list = new List<Question_Definition>();
                    model.question_list = db.Question_Definitions.ToList();
                    if(allAnswers != null)
                    {
                        int count = 0;
                        foreach(var data in allAnswers)
                        {
                            count++;
                            if (count == allAnswers.Count())
                            {
                                model.comboBoxAnswers = model.comboBoxAnswers + data.question_answer;
                                break;
                            }
                            model.comboBoxAnswers = model.comboBoxAnswers + data.question_answer + ' ';
                            
                        }
                        
                    }
                    return View(model);
                }


            }

            return RedirectToAction("Adminsurvey");
        }

        [HttpPost]
        public ActionResult QuestionEdit(AdminSurveyViewModel model)
        {
            if (model.questionName == null)
            {
                ModelState.AddModelError("", "Lütfen Soru Adı Giriniz!");
            }
            if (model.questionType == "1" && model.comboBoxAnswers == null)
            {
                ModelState.AddModelError("", "Combobox tipinde veri girişi yaparken cevap girmek zorundasınız!");
            }

            string id_s = TempData["q_id"].ToString();
            int id = int.Parse(id_s);
            if(id != null)
            {
                Question_Definition q_edit = new Question_Definition();
                q_edit = db.Question_Definitions.Where(x => x.question_id == id).FirstOrDefault();
                q_edit.question_type_id = int.Parse(model.questionType);
                q_edit.question_name = model.questionName;
                db.Question_Definitions.AddOrUpdate(q_edit);
                db.SaveChanges();
                if (model.comboBoxAnswers != null && model.questionType == "1")
                {

                    List<Combobox_Answer> q_find = new List<Combobox_Answer>();
                    MyContext updateDatabase = new MyContext();
                    q_find = db.Combobox_Answers.Where(x => x.question_id == q_edit.question_id).ToList();
                    var answers = model.comboBoxAnswers.Split(' ');
                    if (q_find != null)
                    {
                        int i = 0;
                        foreach (var answerName in answers)
                        {
                            
                            if (i + 1 > q_find.Count)
                            {
                                Combobox_Answer answer = new Combobox_Answer();
                                answer.question_id = id;
                                answer.question_answer = answerName;
                                answer.IsActive = true;
                                updateDatabase.Combobox_Answers.Add(answer);
                                updateDatabase.SaveChanges();
                                
                            }
                            else
                            {
                                q_find[i].question_answer = answerName;
                                updateDatabase.Combobox_Answers.AddOrUpdate(q_find[i]);
                                updateDatabase.SaveChanges();
                                i++;
                            }
                            if (answers.Count() < q_find.Count)
                            {
                                int j = q_find.Count();
                                while(j > answers.Count())
                                {
                                    
                                    db.Combobox_Answers.Remove(q_find[j-1]);
                                    q_find.Remove(q_find[j - 1]);
                                    db.SaveChanges();
                                    j--;
                                }
                            }
                           

                            
                        }
                    }
                }
                else
                {
                    model.comboBoxAnswers = "deneme";
                }
            }

            return RedirectToAction("Adminsurvey");
        }

        public ActionResult ActivateQuestion(int? id)
        {
            if(id != null)
            {
                Question_Definition q_activated = new Question_Definition();
                q_activated = db.Question_Definitions.Where(x => x.question_id == id).FirstOrDefault();
                if(q_activated != null)
                {
                    q_activated.IsActive = true;
                    db.SaveChanges();
                }

                Survey q_answer_activated = new Survey();
                q_answer_activated = db.Surveys.Where(x => x.question_id == id).FirstOrDefault();
                if(q_answer_activated != null)
                {
                    q_answer_activated.IsActive = true;
                    db.SaveChanges();
                }
            }
            
            return RedirectToAction("Adminsurvey");
        }

        public ActionResult DeActivateQuestion(int? id)
        {
            if (id != null)
            {
                Question_Definition q_activated = new Question_Definition();
                q_activated = db.Question_Definitions.Where(x => x.question_id == id).FirstOrDefault();
                if (q_activated != null)
                {
                    q_activated.IsActive = false;
                    db.SaveChanges();
                }

                Survey q_answer_activated = new Survey();
                q_answer_activated = db.Surveys.Where(x => x.question_id == id).FirstOrDefault();
                if (q_answer_activated != null)
                {
                    q_answer_activated.IsActive = false;
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Adminsurvey");
        }
    }
}