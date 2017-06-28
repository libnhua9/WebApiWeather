using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Mvc;
using WebApiWeather.EF;

namespace WebApiWeather.Controllers
{
    public class TestController : Controller
    {
        // GET: Test
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetJDWeather()
        {

            try

            {
                var resultContent = "";
                //自己申请
                //jdkey:ccba4c1751078e7be9caa133a3c2439c
                //https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("http://localhost:2810");
                    //var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("city", "cityid"), new KeyValuePair<string, string>("Password", "fob123") });
                    var result = client.PostAsync("https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c", null).Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                    Console.WriteLine(resultContent);
                }
                using (DB_TestEntities db = new DB_TestEntities())
                {
                    Tb_WeatherForecast wf = new Tb_WeatherForecast
                    {
                        Config = resultContent,
                        Verion = +1
                    };
                    db.Tb_WeatherForecast.Add(wf);
                    db.SaveChanges();
                }
                
            }
            catch (Exception)
            {

                throw;
            }
           
            return null;
        }
    }
}