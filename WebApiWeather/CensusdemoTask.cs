using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Web.Mvc;
using WebApiWeather.EF;
using System.Linq;

namespace WebApiWeather
{
    public class CensusdemoTask
    {
        System.Threading.Timer timer;
        public CensusdemoTask()
        {
            //3秒执行一次  
            timer = new System.Threading.Timer(GetJDWeather, null, 0, 1000 * 30);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void GetJDWeather(object obj)
        {

            try

            {
                var resultContent = "";
                //自己申请
                //jdkey:ccba4c1751078e7be9caa133a3c2439c
                //https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c
                using (HttpClient client = new HttpClient())
                {
                    //client.BaseAddress = new Uri("http://localhost:2810");
                    //var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("city", "cityid"), new KeyValuePair<string, string>("Password", "fob123") });
                    var result = client.PostAsync("https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c", null).Result;
                    resultContent = result.Content.ReadAsStringAsync().Result;
                    //Console.WriteLine(resultContent);
                }
                using (DB_TestEntities db = new DB_TestEntities())
                {
                    var data = db.Tb_WeatherForecast.SingleOrDefault(m=>m.ID>0);
                    if (data!=null)
                    {
                        data.Config = resultContent;
                        data.Verion += 1;
                        data.UpdateDate = DateTime.Now;
                        db.Entry(data).State = System.Data.Entity.EntityState.Modified;
                       

                    }
                    else
                    {
                        var wf = new Tb_WeatherForecast
                        {
                            Config = resultContent,
                            Verion =1,
                            SaveDate = DateTime.Now
                           
                        };
                        db.Tb_WeatherForecast.Add(wf);
                       

                    }
                    db.SaveChanges();
                }

            }
            catch (Exception)
            {

                //throw;
            }

            //return null;
        }
    }
}