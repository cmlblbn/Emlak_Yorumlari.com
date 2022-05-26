using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Xml.Serialization;
using Emalk_Yorumlari_Redis;
using Emlak_Yorumlari.Models;
using Emlak_Yorumlari_Entities;
using Emlak_Yorumlari_Entities.Models;
using Emlak_Yorumlari_WebApp.ViewModels;

namespace Emlak_Yorumlari_WebApp.Controllers
{
    public class PlacesProfileController : Controller
    {
        // GET: PlacesProfile
        private MyContext db = new MyContext();

        private RedisManager redis = new RedisManager();

        public static bool checkBadWords(string comment)
        {
            string path = @"C:\Users\Cemal\Desktop\Emlak_Yorumlari.com\slang-bad_words\badWords.txt";
            string[] readText = System.IO.File.ReadAllLines(path);
            var commentList = comment.Split(' ');

            foreach(var commentWord in commentList)
            {
                foreach(var data in readText)
                {
                    if(commentWord == data)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        public static bool checkSlangWords(string comment)
        {
            string path = @"C:\Users\Cemal\Desktop\Emlak_Yorumlari.com\slang-bad_words\slangClean.txt";
            string[] readText = System.IO.File.ReadAllLines(path);
            var commentList = comment.Split(' ');

            foreach (var commentWord in commentList)
            {
                foreach (var data in readText)
                {
                    if (commentWord == data)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static float[] PlaceScoresCalculator(Place place)
        {
            float[] scores = new float[3];
            MyContext puanSorgu = new MyContext();
            IQueryable<Survey> q1 = puanSorgu.Surveys.Where(x => x.question_id == 1 && x.place_id == place.place_id);
            IQueryable<Survey> q2 = puanSorgu.Surveys.Where(x => x.question_id == 2 && x.place_id == place.place_id);
            IQueryable<Survey> q3 = puanSorgu.Surveys.Where(x => x.question_id == 3 && x.place_id == place.place_id);

            var sorguList = q1.ToList();




            float q1_mainscore = 0;
            float q2_mainscore = 0;
            float q3_mainscore = 0;


            foreach (var point in q1)
            {
                if (sorguList.Count == 0 )
                {
                    q1_mainscore = 0;
                    break;
                }
                q1_mainscore = q1_mainscore + point.score;
               
            }
            foreach (var point in q2)
            {
                if (sorguList.Count == 0)
                {
                    q2_mainscore = 0;
                    break;
                }
                q2_mainscore = q2_mainscore + point.score;
              
            }
            foreach (var point in q3)
            {

                if (sorguList.Count == 0)
                {
                    q3_mainscore = 0;
                    break;
                }
                q3_mainscore = q3_mainscore + point.score;
                
            }




            q1_mainscore = q1_mainscore / q1.Count();
            q2_mainscore = q2_mainscore / q2.Count();
            q3_mainscore = q3_mainscore / q3.Count();

            if (Single.IsNaN(q1_mainscore))
            {
                q1_mainscore = 0;
                q2_mainscore = 0;
                q3_mainscore = 0;
            }

            q1_mainscore = (float)Math.Round(q1_mainscore * 100f) / 100f;
            q2_mainscore = (float)Math.Round(q2_mainscore * 100f) / 100f;
            q3_mainscore = (float)Math.Round(q3_mainscore * 100f) / 100f;
            scores[0] = q1_mainscore;
            scores[1] = q2_mainscore;
            scores[2] = q3_mainscore;


            return scores;
        }

        public static float mainscoresCalculator(Place place)
        {
            float mainScores = 0;
            MyContext get_q = new MyContext();
            var questionsAnswers = get_q.Surveys.Where(x => x.place_id == place.place_id && x.IsActive).ToList();
            var usercount = place.comments.Count();
            float get_score = 0;
            foreach(var q in questionsAnswers)
            {
                if(q.question_definitioın.question_type_id == 2)
                {
                    get_score = q.score + get_score;
                }
            }
            var q_count = get_q.Question_Definitions.Where(x => x.question_type_id == 2).ToList();
            if(usercount == 0)
            {
                mainScores = 0;
            }
            else
            {
                mainScores = get_score / q_count.Count;
                mainScores = mainScores / usercount;
            }

            mainScores = (float)Math.Round(mainScores * 100f) / 100f;

            return mainScores;
        }

        public static Tuple<int,int> minmaxRentCalculator(int averageRent)
        {
            int min = 0;
            int max = 0;

            int hundredsDigit = averageRent % 100;

            if(averageRent > 100)
            {
                averageRent -= hundredsDigit;
                max = averageRent + 400;
                min = averageRent - 400;
            }
            else if(averageRent > 0 && averageRent <= 100){
                int precision = 100 - averageRent;
                averageRent += precision;
                min = averageRent - 100;
                max = averageRent + 100;
            }

            return new Tuple<int,int>(min,max);
        }

        public static List<string> GetUserScore(User sorguUser,Place place)
        {
            List<string> scores = new List<string>(4);
            MyContext sonSorgu = new MyContext();
            IQueryable<Survey> userPuan = sonSorgu.Surveys.Where(x => x.place_id == place.place_id && x.user_id == sorguUser.user_id);

            foreach (var puan in userPuan)
            {
                if (puan.question_id == 1)
                {
                    scores.Insert(0,puan.score.ToString());
                }
                else if(puan.question_id == 2)
                {
                    scores.Insert(1,puan.score.ToString());
                }
                else if(puan.question_id == 3)
                {
                    scores.Insert(2,puan.score.ToString());
                }
            }


            return scores;
        }

        public Dictionary<string, List<string>> commentHelper(MyContext db,Place place)
        {

            Dictionary<string, List<string>> commentsAndPoints = new Dictionary<string, List<string>>();
            MyContext yorumSorgu = new MyContext();
            User sorguUser = new User();

            IQueryable<Comment> comments = db.Comments.Where(x => x.place_id == place.place_id);


            string Commentusername = "";
            foreach (var comment in comments)
            {
                sorguUser = yorumSorgu.Users.Where(x => x.user_id == comment.user_id).FirstOrDefault();
                Commentusername = sorguUser.username;
                var userscoreComment = GetUserScore(sorguUser, place);
                userscoreComment.Insert(3,comment.text);
                commentsAndPoints.Add(Commentusername, userscoreComment);
            }

            return commentsAndPoints;
        }

        public static string getMinMaxRent(int? placeId)
        {
            string minMaxstr = "";
            if (placeId != null)
            {
                MyContext dbContext = new MyContext();
                var dataList = dbContext.Place_Statistics.Where(x => x.place_id == placeId).ToList();
                var stat = dataList.LastOrDefault();
                Tuple<int, int> minMaxRent;
                if (stat != null)
                {
                    minMaxRent = minmaxRentCalculator(stat.average_rent);

                }
                else
                {
                    minMaxRent = new Tuple<int, int>(0,0);

                }
                

                minMaxstr = minMaxRent.Item1.ToString() + " - " + minMaxRent.Item2.ToString();
                dbContext = null;
            }

            return minMaxstr;
        }

        static async Task<string> SendURI(Uri u, HttpContent c)
        {
            string text = string.Empty;
            using (var client = new HttpClient())
            {
                Task<string> response;
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Method = HttpMethod.Post,
                    RequestUri = u,
                    Content = c
                };

                HttpResponseMessage result = await client.SendAsync(request);
                if (result.IsSuccessStatusCode)
                {
                    response = result.Content.ReadAsStringAsync();
                    text = response.Result;
                    return text;
                }
            }
            return text;
        }

        public class prediction
        {
            public string response { get; set; }
        }

        public ActionResult PlaceProfile(int? placeId)
        {

            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            
            if (placeId != null)
            {
                placeId = Convert.ToInt32(placeId);
            }

            string username = "";
            if (Session["User"] == null)
            {
                username = Session["Admin"].ToString();
            }
            else
            {
                username = Session["User"].ToString();
            }

            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();
            Comment kisisorgu = new Comment();
            kisisorgu = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId).FirstOrDefault();
            var kisisorguLog = db.Comment_Logs.Where(x => x.user_id == user.user_id && x.place_id == placeId).FirstOrDefault();
            if (kisisorgu != null || kisisorguLog != null)
            {
                TempData["post"] = "ok";
            }
            else
            {
                TempData["post"] = "not";
            }

            Question_Definition questionName1 = new Question_Definition();
            questionName1 = db.Question_Definitions.Where(x => x.question_id == 1).FirstOrDefault();
            Question_Definition questionName2 = new Question_Definition();
            questionName2 = db.Question_Definitions.Where(x => x.question_id == 2).FirstOrDefault();
            Question_Definition questionName3 = new Question_Definition();
            questionName3 = db.Question_Definitions.Where(x => x.question_id == 3).FirstOrDefault();

            PlaceWithoutSurveys model = new PlaceWithoutSurveys();
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            
            if (place.IsActive == false && Session["Admin"] == null)
            {
                return RedirectToAction("DeActivatedProfileError");
            }
            redis.Remove(place.place_id.ToString());
            var scores = PlaceScoresCalculator(place);

            model.guven_puani_mainscore = scores[0];
            model.aktivite_alani_mainscore = scores[1];
            model.yonetim_memnuniyeti_mainscore = scores[2];


            model.questions = new List<Question_Definition>();
            model.scores = new Dictionary<string, string>();
            model.combobox_answers = new Dictionary<string, List<string>>();

            MyContext comboboxGetanswer = new MyContext();
            model.questions = db.Question_Definitions.Where(x=>x.IsActive).ToList();

            foreach(var data in model.questions)
            {
                model.scores.Add(data.question_id.ToString(), "0");
                if(data.question_type_id == 1)
                {
                    var answers = comboboxGetanswer.Combobox_Answers.Where(x => x.question_id == data.question_id && x.IsActive).ToList();
                    model.combobox_answers.Add(data.question_id.ToString(), new List<string>());
                    for (int i = 0; i < answers.Count; i++)
                    {           
                        model.combobox_answers[data.question_id.ToString()].Add(answers[i].question_answer);
                    }

                }

            }
            

            Dictionary<string, List<string>> commentsAndPoints;
            if (redis.IsSet(place.place_id.ToString()))
            {
                var getJson = redis.getKey(place.place_id.ToString());
                var getDictionary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(getJson);
                commentsAndPoints = getDictionary;
            }
            else
            {
                commentsAndPoints = commentHelper(db, place);
                var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                redis.setKey(place.place_id.ToString(), getJson, 3600);
            }

            model.commentsAndPoints = commentsAndPoints;
            model.guven_puani_score = 0;
            model.aktivite_alani_score = 0;
            model.yonetim_memnuniyeti_score = 0;
            model.comment = "";
            model.question1_name = questionName1.question_name;
            model.question2_name = questionName2.question_name;
            model.question3_name = questionName3.question_name;
            model.place = place;
            model.user = user;
            model.mainScore = mainscoresCalculator(place);
            model.minMaxRentStr = getMinMaxRent(place.place_id);




            

            Adress_Description mahalle = new Adress_Description();
            Adress_Description ilce = new Adress_Description();
            Adress_Description sehir = new Adress_Description();
            MyContext sorgu = new MyContext();

            string birlesmisAdres;

            birlesmisAdres = "";
            mahalle = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == model.place.adress_desc_id).FirstOrDefault();
            ilce = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == mahalle.parent_id).FirstOrDefault();
            sehir = sorgu.Adress_Descriptions.Where(x => x.adress_desc_id == ilce.parent_id).FirstOrDefault();

            birlesmisAdres = birlesmisAdres + sehir.adress_name + " / ";
            birlesmisAdres = birlesmisAdres + ilce.adress_name + " / ";
            birlesmisAdres = birlesmisAdres + mahalle.adress_name;

            ViewBag.TamAdres = birlesmisAdres;

            return View(model);
        }

        [HttpPost]
        public ActionResult PlaceProfile(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null && Session["Admin"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "ok";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = " ";
            if(Session["Admin"] != null)
            {
                username = Session["Admin"].ToString();
            }
            else
            {
                username = Session["User"].ToString();
            }

            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            //Communicate with the fastApi to get a predict from AI
            if(model.comment != null)
            {
                Uri u = new Uri("http://localhost:4444/predict/");

                var willPredict = new Dictionary<string, string>
                    {
                      {"text",model.comment}
                    };

                string strWillPredict = Newtonsoft.Json.JsonConvert.SerializeObject(willPredict);

                HttpContent c = new StringContent(strWillPredict, Encoding.UTF8, "application/json");
                var t = Task.Run(() => SendURI(u, c));
                t.Wait();
                var predictionobj = Newtonsoft.Json.JsonConvert.DeserializeObject<prediction>(t.Result);
                model.prediction = predictionobj.response;
            }



            if (model.prediction == "olumsuz")
            {
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey_Log addQuiz = new Survey_Log();
                    Survey quiz = new Survey();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        quiz.user_id = user.user_id;
                        addQuiz.user = user;
                        quiz.user = user;
                        addQuiz.place_id = place.place_id;
                        quiz.place_id = place.place_id;
                        addQuiz.place = place;
                        quiz.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        quiz.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        quiz.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                            quiz.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                            quiz.score = int.Parse(data.Value);
                        }
                        addQuiz.toxic_type = 1;
                        addQuiz.createdOn = DateTime.Now;
                        quiz.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;
                        quiz.IsActive = true;

                        db.Survey_Logs.Add(addQuiz);
                        db.Surveys.Add(quiz);
                        db.SaveChanges();
                    }



                }

                Comment comment = new Comment();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;

                db.Comments.Add(comment);

                Comment_Log logComment = new Comment_Log();
                logComment.user_id = user.user_id;
                logComment.user = user;
                logComment.place_id = place.place_id;
                logComment.place = place;
                logComment.text = model.comment;
                logComment.toxic_type = 1;
                logComment.createdOn = DateTime.Now;
                logComment.IsActive = true;
                logComment.isLabeled = false;

                db.Comment_Logs.Add(logComment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                }
                return RedirectToAction("PlaceProfile", new { placeId = place.place_id });
            }
            else if(model.prediction == "küfürlü"){
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey_Log addQuiz = new Survey_Log();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        addQuiz.user = user;
                        addQuiz.place_id = place.place_id;
                        addQuiz.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                        }
                        addQuiz.toxic_type = 2;
                        addQuiz.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;

                        db.Survey_Logs.Add(addQuiz);
                        db.SaveChanges();
                    }


                }

                Comment_Log comment = new Comment_Log();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.toxic_type = 2;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;
                comment.isLabeled = false;

                db.Comment_Logs.Add(comment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                }

                return RedirectToAction("badSlangCommentDirection");
            }
            else
            {
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey addQuiz = new Survey();
                    Survey_Log quiz = new Survey_Log();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        quiz.user_id = user.user_id;
                        addQuiz.user = user;
                        quiz.user = user;
                        addQuiz.place_id = place.place_id;
                        quiz.place_id = place.place_id;
                        addQuiz.place = place;
                        quiz.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        quiz.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        quiz.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                            quiz.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                            quiz.score = int.Parse(data.Value);
                        }
                        addQuiz.createdOn = DateTime.Now;
                        quiz.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;
                        quiz.IsActive = true;
                        

                        db.Surveys.Add(addQuiz);
                        db.Survey_Logs.Add(quiz);
                        place.surveys.Add(addQuiz);
                        user.surveys.Add(addQuiz);
                        db.SaveChanges();
                    }


                }

                Comment comment = new Comment();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;

                Comment_Log logcomment = new Comment_Log();
                logcomment.user_id = user.user_id;
                logcomment.user = user;
                logcomment.place_id = place.place_id;
                logcomment.place = place;
                logcomment.text = model.comment;
                logcomment.createdOn = DateTime.Now;
                logcomment.IsActive = true;
                logcomment.isLabeled = false;

                db.Comment_Logs.Add(logcomment);
                db.Comments.Add(comment);
                place.comments.Add(comment);
                user.comments.Add(comment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                }
            }





            return RedirectToAction("PlaceProfile", new { placeId = place.place_id });


        }

        [HttpGet]
        public ActionResult EditComment()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return PartialView("~/Views/Shared/_PartialComment.cshtml");
        }

        [HttpPost]
        public ActionResult EditComment(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "ok";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            if (model.comment != null)
            {
                Uri u = new Uri("http://localhost:4444/predict/");

                var willPredict = new Dictionary<string, string>
                    {
                      {"text",model.comment}
                    };

                string strWillPredict = Newtonsoft.Json.JsonConvert.SerializeObject(willPredict);

                HttpContent c = new StringContent(strWillPredict, Encoding.UTF8, "application/json");
                var t = Task.Run(() => SendURI(u, c));
                t.Wait();
                var predictionobj = Newtonsoft.Json.JsonConvert.DeserializeObject<prediction>(t.Result);
                model.prediction = predictionobj.response;
            }

            if (model.prediction == "olumsuz")
            {
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey_Log addQuiz = new Survey_Log();
                    Survey survey = new Survey();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        survey.user_id = user.user_id;
                        addQuiz.user = user;
                        survey.user = user;
                        addQuiz.place_id = place.place_id;
                        survey.place_id = place.place_id;
                        addQuiz.place = place;
                        survey.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        survey.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        survey.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                            survey.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                            survey.score = int.Parse(data.Value);
                        }
                        addQuiz.toxic_type = 1;
                        addQuiz.createdOn = DateTime.Now;
                        survey.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;
                        survey.IsActive = true;

                        db.Surveys.Add(survey);
                        db.Survey_Logs.Add(addQuiz);
                        db.SaveChanges();
                    }


                }

                Comment comment = new Comment();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;

                db.Comments.Add(comment);

                Comment_Log logcomment = new Comment_Log();
                logcomment.user_id = user.user_id;
                logcomment.user = user;
                logcomment.place_id = place.place_id;
                logcomment.place = place;
                logcomment.text = model.comment;
                logcomment.toxic_type = 1;
                logcomment.createdOn = DateTime.Now;
                logcomment.IsActive = true;
                logcomment.isLabeled = false;

                db.Comment_Logs.Add(logcomment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                    var commentsAndPoints = commentHelper(db, place);
                    var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                    redis.setKey(place.place_id.ToString(), getJson, 3600);

                }
                return RedirectToAction("PlaceProfile", new { placeId = place.place_id });
            }
            else if (model.prediction == "küfürlü")
            {
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey_Log addQuiz = new Survey_Log();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        addQuiz.user = user;
                        addQuiz.place_id = place.place_id;
                        addQuiz.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                        }
                        addQuiz.toxic_type = 2;
                        addQuiz.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;

                        db.Survey_Logs.Add(addQuiz);
                        db.SaveChanges();
                    }


                }

                Comment_Log comment = new Comment_Log();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.toxic_type = 2;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;
                comment.isLabeled = false;

                db.Comment_Logs.Add(comment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                    var commentsAndPoints = commentHelper(db, place);
                    var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                    redis.setKey(place.place_id.ToString(), getJson, 3600);

                }

                return RedirectToAction("badSlangCommentDirection");
            }
            else
            {
                foreach (var data in model.scores)
                {
                    Question_Definition addQuestion = new Question_Definition();
                    int sorguKey = int.Parse(data.Key);
                    addQuestion = db.Question_Definitions.Where(x => x.question_id == sorguKey).FirstOrDefault();
                    Survey addQuiz = new Survey();
                    Survey_Log quiz = new Survey_Log();
                    if (addQuestion != null)
                    {
                        addQuiz.user_id = user.user_id;
                        quiz.user_id = user.user_id;
                        addQuiz.user = user;
                        quiz.user = user;
                        addQuiz.place_id = place.place_id;
                        quiz.place_id = place.place_id;
                        addQuiz.place = place;
                        quiz.place = place;
                        addQuiz.question_id = addQuestion.question_id;
                        quiz.question_id = addQuestion.question_id;
                        addQuiz.question_definitioın = addQuestion;
                        quiz.question_definitioın = addQuestion;
                        if (addQuestion.question_type_id == 1)
                        {
                            Combobox_Answer combo = new Combobox_Answer();
                            combo = db.Combobox_Answers.Where(x => x.question_answer == data.Value).FirstOrDefault();
                            addQuiz.score = combo.question_answer_id;
                            quiz.score = combo.question_answer_id;
                        }
                        else
                        {
                            addQuiz.score = int.Parse(data.Value);
                            quiz.score = int.Parse(data.Value);
                        }
                        addQuiz.createdOn = DateTime.Now;
                        quiz.createdOn = DateTime.Now;
                        addQuiz.IsActive = true;
                        quiz.IsActive = true;

                        db.Survey_Logs.AddOrUpdate(quiz);
                        db.Surveys.AddOrUpdate(addQuiz);
                        db.SaveChanges();
                    }


                }

                Comment comment = new Comment();
                comment.user_id = user.user_id;
                comment.user = user;
                comment.place_id = place.place_id;
                comment.place = place;
                comment.text = model.comment;
                comment.createdOn = DateTime.Now;
                comment.IsActive = true;

                Comment_Log logcomment = new Comment_Log();
                logcomment.user_id = user.user_id;
                logcomment.user = user;
                logcomment.place_id = place.place_id;
                logcomment.place = place;
                logcomment.text = model.comment;
                logcomment.createdOn = DateTime.Now;
                logcomment.IsActive = true;
                logcomment.isLabeled = false;

                db.Comment_Logs.AddOrUpdate(logcomment);
                db.Comments.AddOrUpdate(comment);
                db.SaveChanges();

                if (redis.IsSet(place.place_id.ToString()))
                {
                    redis.Remove(place.place_id.ToString());
                    var commentsAndPoints = commentHelper(db, place);
                    var getJson = Newtonsoft.Json.JsonConvert.SerializeObject(commentsAndPoints);
                    redis.setKey(place.place_id.ToString(), getJson, 3600);

                }
            }
          
            return RedirectToAction("PlaceProfile",new{placeId = place.place_id});
        }


        public ActionResult DeleteComment(PlaceWithoutSurveys model, int? placeId)
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            TempData["post"] = "not";
            Place place = new Place();
            place = db.Places.Where(x => x.place_id == placeId).FirstOrDefault();
            string username = Session["User"].ToString();
            User user = new User();
            user = db.Users.Where(x => x.username == username).FirstOrDefault();

            var SurveyList = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.IsActive).ToList();

            foreach(var survey in SurveyList)
            {
                db.Surveys.Remove(survey);
                db.SaveChanges();
            }
            Comment comment = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId).FirstOrDefault();
            db.Comments.Remove(comment);
            db.SaveChanges();



            //Survey quiz1 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 1).FirstOrDefault();
            //Survey quiz2 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 2).FirstOrDefault();
            //Survey quiz3 = db.Surveys.Where(x => x.user_id == user.user_id && x.place_id == placeId && x.question_id == 3).FirstOrDefault();
            //Comment comment = db.Comments.Where(x => x.user_id == user.user_id && x.place_id == placeId ).FirstOrDefault(); 
            //if (quiz1 != null && quiz2 != null && quiz3 != null && comment  != null)
            //{
            //    db.Surveys.Remove(quiz1);
            //    db.Surveys.Remove(quiz2);
            //    db.Surveys.Remove(quiz3);
            //    db.Comments.Remove(comment);
            //}

            //db.SaveChanges();

            if (redis.IsSet(place.place_id.ToString()))
            {
                redis.Remove(place.place_id.ToString());
            }


            return RedirectToAction("PlaceProfile",new{placeId = place.place_id});
        }

        public ActionResult DeActivatedProfileError()
        {
            return View();
        }

        public ActionResult genderChart(int? placeId)
        {
            if (redis.IsSet("genderChartxAxis" + placeId.ToString()) && redis.IsSet("genderChartxAxis" + placeId.ToString()))
            {

                var xAxisstr = redis.getKey("genderChartxAxis" + placeId.ToString());
                var yAxisstr = redis.getKey("genderChartyAxis" + placeId.ToString());

                var xAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(xAxisstr);
                var yAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(yAxisstr);

                Chart chartCache = new Chart(400, 500, theme: ChartTheme.Yellow);               
                chartCache.AddTitle("Sitede Yaşayanların Cinsiyet Dağılımı").AddSeries(name: "GenderStatus", chartType: "Doughnut",
                xValue: xAxis,
                yValues: yAxis).AddLegend();
                return View(chartCache);

            }

            string[] chartData_xAxis = new string[] { "Erkek", "Kadın", "Diğer" };
            int[] chartData_yAxis = new int[3];
            if (placeId != null)
            {
                var dataStats = db.Place_Statistics.Where(x => x.place_id == placeId).ToList();
                var dataStat = dataStats.LastOrDefault();
                if(dataStat != null)
                {
                    int dataCount = dataStat.male_count + dataStat.female_count + dataStat.otherSex_count;
                    chartData_yAxis[0] = dataStat.male_count;
                    chartData_yAxis[1] = dataStat.female_count;
                    chartData_yAxis[2] = dataStat.otherSex_count;
                    for (int i = 0; i < chartData_xAxis.Length; i++)
                    {
                        if (dataCount == 0)
                        {
                            chartData_xAxis[i] = chartData_xAxis[i] + " % 0";
                        }
                        else
                        {
                            float xDataElement = (float)chartData_yAxis[i] / dataCount;
                            xDataElement = (float)Math.Round(xDataElement * 100f) / 100f;
                            xDataElement = xDataElement * 100;
                            chartData_xAxis[i] = chartData_xAxis[i] + " % " + xDataElement.ToString();
                        }

                    }
                }
                else
                {
                    chartData_yAxis[0] = 0;
                    chartData_yAxis[1] = 0;
                    chartData_yAxis[2] = 0;
                }
    
            }
            Chart chart = new Chart(400, 500, theme: ChartTheme.Yellow).AddTitle("Sitede Yaşayanların Cinsiyet Dağılımı").AddSeries(name: "GenderStatus", chartType: "Doughnut",
            xValue: chartData_xAxis,
            yValues: chartData_yAxis).AddLegend();


            var xAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_xAxis);
            var yAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_yAxis);
            redis.setKey("genderChartxAxis" + placeId.ToString(), xAxisJson, 15);
            redis.setKey("genderChartyAxis" + placeId.ToString(), yAxisJson, 15);
            return View(chart);
            
        }

        public ActionResult EducationChart(int? placeId)
        {
            if (redis.IsSet("EducationChartxAxis" + placeId.ToString()) && redis.IsSet("EducationChartxAxis" + placeId.ToString()))
            {

                var xAxisstr = redis.getKey("EducationChartxAxis" + placeId.ToString());
                var yAxisstr = redis.getKey("EducationChartyAxis" + placeId.ToString());

                var xAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(xAxisstr);
                var yAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(yAxisstr);

                Chart chartCache = new Chart(400, 500, theme: ChartTheme.Yellow);
                chartCache.AddTitle("Sitede Yaşayanların Cinsiyet Dağılımı").AddSeries(name: "GenderStatus", chartType: "Doughnut",
                xValue: xAxis,
                yValues: yAxis).AddLegend();
                return View(chartCache);

            }

            string[] chartData_xAxis = new string[] { "İlkokul", "Orta okul", "Lise", "Üniversite", "Yüksek Lisans" };
            int[] chartData_yAxis = new int[5];
            if (placeId != null)
            {
                var dataStats = db.Place_Statistics.Where(x => x.place_id == placeId).ToList();
                var dataStat = dataStats.LastOrDefault();
                if(dataStat != null)
                {
                    int dataCount = dataStat.primarySchool_count + dataStat.middleSchool_count + dataStat.highSchool_count + dataStat.degree_count + dataStat.masterDegree_count;

                    chartData_yAxis[0] = dataStat.primarySchool_count;
                    chartData_yAxis[1] = dataStat.middleSchool_count;
                    chartData_yAxis[2] = dataStat.highSchool_count;
                    chartData_yAxis[3] = dataStat.degree_count;
                    chartData_yAxis[4] = dataStat.masterDegree_count;

                    for (int i = 0; i < chartData_xAxis.Length; i++)
                    {
                        if (dataCount == 0)
                        {
                            chartData_xAxis[i] = chartData_xAxis[i] + " % 0";
                        }
                        else
                        {
                            float xDataElement = (float)chartData_yAxis[i] / dataCount;
                            xDataElement = (float)Math.Round(xDataElement * 100f) / 100f;
                            xDataElement = xDataElement * 100;
                            chartData_xAxis[i] = chartData_xAxis[i] + " % " + xDataElement.ToString();
                        }

                    }
                }
                else
                {
                    chartData_yAxis[0] = 0;
                    chartData_yAxis[1] = 0;
                    chartData_yAxis[2] = 0;
                    chartData_yAxis[3] = 0;
                    chartData_yAxis[4] = 0;
                }

            }

            Chart chart = new Chart(400, 500, theme: ChartTheme.Yellow);
            chart.AddTitle("Sitede Yaşayanların Eğitim Durumu");
            chart.AddSeries(name: "EducationStatus", chartType: "Doughnut",
                xValue: chartData_xAxis,
                yValues: chartData_yAxis);
            chart.AddLegend();

            var xAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_xAxis);
            var yAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_yAxis);
            redis.setKey("EducationChartxAxis" + placeId.ToString(), xAxisJson, 15);
            redis.setKey("EducationChartyAxis" + placeId.ToString(), yAxisJson, 15);

            return View(chart);
            
        }

        public ActionResult maritalChart(int? placeId)
        {
            if (redis.IsSet("maritalChartxAxis" + placeId.ToString()) && redis.IsSet("maritalChartyAxis" + placeId.ToString()))
            {

                var xAxisstr = redis.getKey("maritalChartxAxis" + placeId.ToString());
                var yAxisstr = redis.getKey("maritalChartyAxis" + placeId.ToString());

                var xAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(xAxisstr);
                var yAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(yAxisstr);

                Chart chartCache = new Chart(400, 500, theme: ChartTheme.Yellow);
                chartCache.AddTitle("Sitede Yaşayanların Cinsiyet Dağılımı").AddSeries(name: "GenderStatus", chartType: "Doughnut",
                xValue: xAxis,
                yValues: yAxis).AddLegend();
                return View(chartCache);

            }

            string[] chartData_xAxis = new string[] { "Evli", "Bekar", "Boşanmış", "Dul" };
            int[] chartData_yAxis = new int[4];
            if (placeId != null)
            {
                var dataStats = db.Place_Statistics.Where(x => x.place_id == placeId).ToList();
                var dataStat = dataStats.LastOrDefault();
                if(dataStat != null)
                {
                    int dataCount = dataStat.married_count + dataStat.single_count + dataStat.divorced_count + dataStat.widow_count;
                    chartData_yAxis[0] = dataStat.married_count;
                    chartData_yAxis[1] = dataStat.single_count;
                    chartData_yAxis[2] = dataStat.divorced_count;
                    chartData_yAxis[3] = dataStat.widow_count;
                    for (int i = 0; i < chartData_xAxis.Length; i++)
                    {
                        if (dataCount == 0)
                        {
                            chartData_xAxis[i] = chartData_xAxis[i] + " % 0";
                        }
                        else
                        {
                            float xDataElement = (float)chartData_yAxis[i] / dataCount;
                            xDataElement = (float)Math.Round(xDataElement * 100f) / 100f;
                            xDataElement = xDataElement * 100;
                            chartData_xAxis[i] = chartData_xAxis[i] + " % " + xDataElement.ToString();
                        }

                    }
                }
                else
                {
                    chartData_yAxis[0] = 0;
                    chartData_yAxis[1] = 0;
                    chartData_yAxis[2] = 0;
                    chartData_yAxis[3] = 0;
                }
 
            }

            Chart chart = new Chart(400, 500, theme: ChartTheme.Yellow);
            chart.AddTitle("Sitede Yaşayanların Medeni Durumu");
            chart.AddSeries(name: "EducationStatus", chartType: "Doughnut",
                xValue: chartData_xAxis,
                yValues: chartData_yAxis);
            chart.AddLegend();

            var xAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_xAxis);
            var yAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_yAxis);
            redis.setKey("maritalChartxAxis" + placeId.ToString(), xAxisJson, 15);
            redis.setKey("maritalChartyAxis" + placeId.ToString(), yAxisJson, 15);

            return View(chart);
            
        }

        public ActionResult ageChart(int? placeId)
        {
            if (redis.IsSet("ageChartxAxis" + placeId.ToString()) && redis.IsSet("ageChartyAxis" + placeId.ToString()))
            {

                var xAxisstr = redis.getKey("ageChartxAxis" + placeId.ToString());
                var yAxisstr = redis.getKey("ageChartyAxis" + placeId.ToString());

                var xAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(xAxisstr);
                var yAxis = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(yAxisstr);

                Chart chartCache = new Chart(400, 500, theme: ChartTheme.Yellow);
                chartCache.AddTitle("Sitede Yaşayanların Cinsiyet Dağılımı").AddSeries(name: "GenderStatus", chartType: "Doughnut",
                xValue: xAxis,
                yValues: yAxis).AddLegend();
                return View(chartCache);

            }


            string[] chartData_xAxis = new string[] { "18'den az", "18-34 arası", "34-55 arası", "55 üstü" };
            int[] chartData_yAxis = new int[4];
            if (placeId != null)
            {
                var dataStats = db.Place_Statistics.Where(x => x.place_id == placeId).ToList();
                var dataStat = dataStats.LastOrDefault();
                if(dataStat != null)
                {
                    int dataCount = dataStat.age_lower_18 + dataStat.age_between_18_34 + dataStat.age_between_34_55 + dataStat.age_upper_55;
                    chartData_yAxis[0] = dataStat.age_lower_18;
                    chartData_yAxis[1] = dataStat.age_between_18_34;
                    chartData_yAxis[2] = dataStat.age_between_34_55;
                    chartData_yAxis[3] = dataStat.age_upper_55;
                    for (int i = 0; i < chartData_xAxis.Length; i++)
                    {
                        if (dataCount == 0)
                        {
                            chartData_xAxis[i] = chartData_xAxis[i] + " % 0";
                        }
                        else
                        {
                            float xDataElement = (float)chartData_yAxis[i] / dataCount;
                            xDataElement = (float)Math.Round(xDataElement * 100f) / 100f;
                            xDataElement = xDataElement * 100;
                            chartData_xAxis[i] = chartData_xAxis[i] + " % " + xDataElement.ToString();
                        }

                    }
                }
                else
                {
                    chartData_yAxis[0] = 0;
                    chartData_yAxis[1] = 0;
                    chartData_yAxis[2] = 0;
                    chartData_yAxis[3] = 0;
                }
 
            }

            Chart chart = new Chart(400, 500, theme: ChartTheme.Yellow);
            chart.AddTitle("Sitede Yaşayanların Yaş Aralıkları");
            chart.AddSeries(name: "EducationStatus", chartType: "Doughnut",
                xValue: chartData_xAxis,
                yValues: chartData_yAxis);
            chart.AddLegend();

            var xAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_xAxis);
            var yAxisJson = Newtonsoft.Json.JsonConvert.SerializeObject(chartData_yAxis);
            redis.setKey("ageChartxAxis" + placeId.ToString(), xAxisJson, 15);
            redis.setKey("ageChartyAxis" + placeId.ToString(), yAxisJson, 15);

            return View(chart);
            

        }

        public ActionResult rentDeltaChart(int? placeId, string type = "Line")
        {
            string[] chartData_xAxis = new string[] { "Geçen Ay", "Bu ay"};
            int[] chartData_yAxisMin = new int[2];
            int[] chartData_yAxisMax = new int[2];


            var dataStatsLastMonth = db.Place_Statistics.Where(x => x.place_id == placeId && x.createdOn.Month == (DateTime.Now.Month - 1)).ToList();
            var dataStatLastMonth = dataStatsLastMonth.LastOrDefault();


            var dataStatsToDay = db.Place_Statistics.Where(x => x.place_id == placeId && x.createdOn.Month == DateTime.Now.Month).ToList();
            var dataStatToDay = dataStatsToDay.LastOrDefault();


            var lastMonthMinMaxRent = minmaxRentCalculator(0);
            var toDayMinMaxRent = minmaxRentCalculator(0);

            if (dataStatLastMonth != null)
            {
                lastMonthMinMaxRent = minmaxRentCalculator(dataStatLastMonth.average_rent);
            }
            if(dataStatToDay != null)

            {
                toDayMinMaxRent = minmaxRentCalculator(dataStatToDay.average_rent);
            }

            chartData_yAxisMin[0] = lastMonthMinMaxRent.Item1;
            chartData_yAxisMin[1] = toDayMinMaxRent.Item1;

            chartData_yAxisMax[0] = lastMonthMinMaxRent.Item2;
            chartData_yAxisMax[1] = toDayMinMaxRent.Item2;

            Chart chart = new Chart(420, 500, theme: ChartTheme.Vanilla);
            chart.AddTitle("Sitedeki Aylık Kira Değişimi");
            chart.AddSeries(name: "Minimum Kira Değişimi", chartType: type,
                xValue: chartData_xAxis,
                yValues: chartData_yAxisMin);

            chart.AddSeries(name: "Maximum Kira Değişimi", chartType: type,
                xValue: chartData_xAxis,
                yValues: chartData_yAxisMax);

            chart.AddLegend();

            return View(chart);
        }

        public ActionResult badSlangCommentDirection()
        {
            return View();
        }



    }
}