using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PushSharp.Apple;
using PushSharp.Core;
using PushSharp.Google;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace BagGomla.Utils
{
    public class PushManager
    {
        //Android
        private static string UserServerKey = ConfigurationManager.AppSettings["USER_SERVER_API_KEY"];
        private static string UserSenderId = ConfigurationManager.AppSettings["USER_SERVER_SENDER_ID"];

        //IOS
        private static string UserFullPath = HostingEnvironment.MapPath("~") + "/Files/UserCertificate.p12";
        private static string certPassword = "Project1234";
    
        public static void pushToAndroidDevice(String token, string title,string message)
        {
            var Data = new
            {
                to = token,
                priority = "high",
                content_available = true,
                notification = new
                {
                    body = message,
                    title = title,
                    sound = "default"
                },
                data = new
                {
                    id = "-1",
                }
            };
            var Serializer = new JavaScriptSerializer();
            var Json = Serializer.Serialize(Data);
            var byteArray = Encoding.UTF8.GetBytes(Json);

            try
            {
                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");
                tRequest.Method = "post";
                tRequest.ContentType = "application/json";
                tRequest.Headers.Add(string.Format("Authorization: key={0}", UserServerKey));
                tRequest.Headers.Add(string.Format("Sender: id={0}", UserSenderId));
                tRequest.ContentLength = byteArray.Length;

                using (Stream dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                    using (WebResponse tResponse = tRequest.GetResponse())
                    {
                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {
                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {
                                string sResponseFromServer = tReader.ReadToEnd();
                                string str = sResponseFromServer;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }


        public static void pushToIphoneDevice(String token, string title, string message, bool IsPassenger = false)
        {
            var config = new ApnsConfiguration(ApnsConfiguration.ApnsServerEnvironment.Sandbox, UserFullPath, certPassword);

            // Create a new broker
            var apnsBroker = new ApnsServiceBroker(config);

            apnsBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {

                aggregateEx.Handle(ex =>
                {

                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        Console.WriteLine($"GCM Notification Failed: ID={gcmNotification.MessageId}, Desc={description}");
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Console.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            Console.WriteLine($"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}");
                        }
                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Console.WriteLine($"Device RegistrationId Expired: {oldId}");

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Console.WriteLine($"Device RegistrationId Changed To: {newId}");
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        Console.WriteLine($"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}");
                    }
                    else
                    {
                        Console.WriteLine("GCM Notification Failed for some unknown reason");
                    }

                    // Mark it as handled
                    return true;
                });
            };

            apnsBroker.OnNotificationSucceeded += (notification) =>
            {
                Console.WriteLine("Apple Notification Sent!");
            };


            // Start the broker
            apnsBroker.Start();

            var Data = new
            {
                aps = new
                {
                    alert = new
                    {
                        title = title,
                        body = message,
                    },
                    badge = 1,
                    sound = "default",
                },
                Id = -1,
            };
            var modelToJson = JsonConvert.SerializeObject(Data);
            var Payloadd = JObject.Parse(modelToJson);

            // Queue a notification to send
            apnsBroker.QueueNotification(new ApnsNotification
            {
                DeviceToken = token,
                Payload = Payloadd
            });

            // Stop the broker, wait for it to finish   
            // This isn't done after every message, but after you're
            // done with the broker
            apnsBroker.Stop();

        }


    }
}