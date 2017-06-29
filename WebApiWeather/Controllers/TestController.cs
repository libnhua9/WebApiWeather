using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using WebApiWeather.EF;
using System.Linq;
using System.Net;
using System.IO;
using System.Text;

namespace WebApiWeather.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }


        public string HttpPost(string Url, string postDataStr)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
                request.Method = "POST";
                request.Timeout = 6000000;
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = postDataStr.Length;
                StreamWriter writer = new StreamWriter(request.GetRequestStream(), Encoding.ASCII);
                writer.Write(postDataStr);
                writer.Flush();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                string encoding = response.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码
                }
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));
                string retString = reader.ReadToEnd();
                return retString;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        [HttpPost]
        public ActionResult GetJDWeather(string city)
        {
            var resultContent = "";
            var state = true;
            try

            {
                using (DB_TestEntities db = new DB_TestEntities())
                {
                    var data = db.Tb_WeatherForecast.SingleOrDefault(m => m.City == city);
                    if (data == null)
                    {

                        var url = "https://way.jd.com/jisuapi/weather?";
                        var postData = $"city={city}&cityid=111&citycode=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c";

                        using (HttpClient client = new HttpClient())
                        {
                            
                            //https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c

                           var result = client.PostAsync(url+postData, null).Result;
                            resultContent = result.Content.ReadAsStringAsync().Result;
                        }
                        var wf = new Tb_WeatherForecast
                        {
                            Config = resultContent,
                            SaveDate = DateTime.Now,
                            City=city,
                            Verion =1
                        };
                        db.Tb_WeatherForecast.Add(wf);
                        db.SaveChanges();
                    }
                    else
                    {
                        resultContent = data.Config;
                    }

                }
            }
            catch (Exception)
            {
                state = false;
                //throw;
            }
            var o = new
            {
                resultContent = resultContent,
                state = state
            };

            return Json(o, JsonRequestBehavior.AllowGet);
        }
    }
}