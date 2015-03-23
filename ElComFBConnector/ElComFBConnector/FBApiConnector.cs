using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Web;
using System.IO;
using System.Text;

namespace ElComFBConnector
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "FBApiConnector" in both code and config file together.

    /*
     
     class Program
    {
        static void Main(string[] args)
        {
            HttpWebRequest request = WebRequest.Create("https://graph.facebook.com/oauth/access_token?client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials") as HttpWebRequest;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var encoding = ASCIIEncoding.ASCII;
            string responseText = string.Empty;
            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
            {
                responseText = reader.ReadToEnd();
                Console.WriteLine(responseText);
            }
            string accessToken = string.Empty;
            if (!String.IsNullOrEmpty(responseText))
            {
                accessToken = responseText.Split('=')[1];
            }


            //HttpWebRequest request2 = WebRequest.Create("https://graph.facebook.com/v2.2/71263708835/posts?access_token=" + accessToken) as HttpWebRequest;
            string urlPosts = "https://graph.facebook.com/v2.2/71263708835/posts?access_token=" + accessToken;
            string str2 = RequestResponse(urlPosts);
            //HttpWebResponse response2 = (HttpWebResponse)request.GetResponse();
            //var encoding2 = UTF8Encoding.UTF8;

            //var json = JObject.Parse(response2.);

            //using (var reader = new System.IO.StreamReader(response2.GetResponseStream(), encoding2))
            //{
            //    responseText = reader.ReadToEnd();
            //    Console.WriteLine(responseText);
            //}

            Console.WriteLine(str2);



            Console.ReadLine();
        }

        public static  string RequestResponse(string pUrl)
        {
            HttpWebRequest webRequest = System.Net.WebRequest.Create(pUrl) as HttpWebRequest;
            webRequest.Method = "GET";
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.Timeout = 20000;

            Stream responseStream = null;
            StreamReader responseReader = null;
            string responseData = "";
            try
            {
                WebResponse webResponse = webRequest.GetResponse();
                responseStream = webResponse.GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
            }
            finally
            {
                if (responseStream != null)
                {
                    responseStream.Close();
                    responseReader.Close();
                }
            }

            return responseData;
        }
    }
     */
    public class FBApiConnector : IFBApiConnector
    {
        //http://graph.facebook.com/
        //https://graph.facebook.com/oauth/access_token?client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials
        //https://graph.facebook.com/v2.2/71263708835/posts?access_token=884427038267146|M85AmsihHrGsf7UuYF4GO2s4BeE

        public string GetPagesLikes(string pageId)
        {
            HttpRequest request = new HttpRequest("", Constantes.ApiBaseUrl, "client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials");

            TextWriter twrtr = TextWriter.Null;
            HttpResponse response = new HttpResponse(twrtr);

            return "";
        }
    }
}
