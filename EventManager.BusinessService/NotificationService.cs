using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EventManager.BusinessService
{
    public interface INotificationService
    {
        void NotifyAsync(string to, string message);
    }
    public class NotificationService : INotificationService
    {
        public void NotifyAsync(string to, string message)
        {
            var _apiKey = "NTcyNmI3MTEtZmQ4My00YTE4LTk5ZTEtMTliODZmNjcyYTBl";
            var _appId = "16df746e-2fab-4a86-88ca-bbcee8d8b76c";
            var request = WebRequest.Create("https://onesignal.com/api/v1/notifications") as HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("authorization", "Basic "+ _apiKey);

            var serializer = new JavaScriptSerializer();            
            var obj = new
            {
                app_id = _appId,
                contents = new { en = message },
                include_player_ids = new string[] { to }
            };
            var param = serializer.Serialize(obj);
            byte[] byteArray = Encoding.UTF8.GetBytes(param);

            string responseContent = null;

            try
            {
                using (var writer = request.GetRequestStream())
                {
                    writer.Write(byteArray, 0, byteArray.Length);
                }

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseContent = reader.ReadToEnd();
                    }
                }
            }
            catch (WebException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                System.Diagnostics.Debug.WriteLine(new StreamReader(ex.Response.GetResponseStream()).ReadToEnd());
            }
        }
    }
}
