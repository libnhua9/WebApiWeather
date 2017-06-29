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
            //一个小时执行一次  
            timer = new System.Threading.Timer(GetJDWeather, null, 0, 1000 * 60 * 1);
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void GetJDWeather(object obj)
        {
            try
            {
                var resultContent = "";
                //https://way.jd.com/jisuapi/weather?city=深圳&cityid=111&citycode=101260301&appkey=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c
                using (DB_TestEntities db = new DB_TestEntities())
                {
                    var data = db.Tb_WeatherForecast.ToList();
                    foreach (var item in data)
                    {
                        using (HttpClient client = new HttpClient())
                        {
                            var url = "https://way.jd.com/jisuapi/weather?";
                            var postData = $"city={item.City}&cityid=111&citycode=101260301&appkey=ccba4c1751078e7be9caa133a3c2439c";
                            var result = client.PostAsync(url + postData, null).Result;
                            resultContent = result.Content.ReadAsStringAsync().Result;
                            //Console.WriteLine(resultContent);
                        }
                        if (item != null)
                        {
                            item.Config = resultContent;
                            item.Verion += 1;
                            item.UpdateDate = DateTime.Now;
                            db.Entry(item).State = System.Data.Entity.EntityState.Modified;
                        }
                    }
                    db.SaveChanges();
                }

            }
            catch (Exception)
            {

                //throw;
            }
        }
    }
}