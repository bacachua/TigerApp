using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EventManager.BusinessService
{
    public interface INotificationService
    {
        Task<bool> NotifyAsync(string _serverKey, string _senderId, string to, string title, string body);
    }
    public class NotificationService : INotificationService
    {
        public async Task<bool> NotifyAsync(string _serverKey, string _senderId, string to, string title, string body)
        {
            try
            {
                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", _serverKey);
                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", _senderId);
                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body }
                };
                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);
                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.SendAsync(httpRequest);
                        if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception e) { return false; }
        }
    }
}
