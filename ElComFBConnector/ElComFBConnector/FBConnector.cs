using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Configuration;

namespace ElComFBConnector
{
    /*
     class Program
    {
        static void Main(string[] args)
        {
            string strGenerateToken = Constantes.ApiBaseUrl + "oauth/access_token?" + string.Format("client_id={0}&client_secret={1}&grant_type=client_credentials", Constantes.AppID, Constantes.AppSecret);
            string strResponseToken = RequestResponse(strGenerateToken);
            string accessToken = string.Empty;
            if (!String.IsNullOrEmpty(strResponseToken))
            {
                accessToken = strResponseToken.Split('=')[1];
            }
            Console.WriteLine(accessToken);

            //Info de la página
            string strPageInfoRequest = Constantes.ApiBaseUrl + Constantes.comercioFBId + "?access_token=" + accessToken;
            string strPageInfoResponse = RequestResponse(strPageInfoRequest);
            var jsonPageInfo = JObject.Parse(strPageInfoResponse);

            if (jsonPageInfo.Property("id") != null)
            {
                Console.WriteLine(jsonPageInfo["id"].ToString());
            }
            if (jsonPageInfo.Property("name") != null)
            {
                Console.WriteLine(jsonPageInfo["name"].ToString());
            }
            if (jsonPageInfo.Property("category") != null)
            {
                Console.WriteLine(jsonPageInfo["category"].ToString());
            }
            if (jsonPageInfo.Property("link") != null)
            {
                Console.WriteLine(jsonPageInfo["link"].ToString());
            }
            if (jsonPageInfo.Property("birthday") != null)
            {
                Console.WriteLine(jsonPageInfo["birthday"].ToString());
            }
            if (jsonPageInfo.Property("gender") != null)
            {
                Console.WriteLine(jsonPageInfo["gender"].ToString());
            }
            if (jsonPageInfo.Property("likes") != null)
            {
                Console.WriteLine(jsonPageInfo["likes"].ToString());
            }
            if (jsonPageInfo.Property("talking_about_count") != null)
            {
                Console.WriteLine(jsonPageInfo["talking_about_count"].ToString());
            }


            //Posts públicos de la página
            string strPagePostsRequest = Constantes.ApiBaseUrl + Constantes.comercioFBId + "/posts/?since=2015-01-01&until=2015-01-15" + "&access_token=" + accessToken;
            string strPagePostsResponse = RequestResponse(strPagePostsRequest);
            var jsonPostsInfo = JObject.Parse(strPagePostsResponse);
            if (jsonPostsInfo["data"] != null)
            {
                foreach (JObject jsonPostInfo in jsonPostsInfo["data"].ToArray())
                {
                    if (jsonPostInfo["id"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["id"].ToString());
                    }
                    if (jsonPostInfo["story"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["story"].ToString());
                    }
                    if (jsonPostInfo["link"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["link"].ToString());
                    }
                    if (jsonPostInfo["type"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["type"].ToString());
                    }
                    if (jsonPostInfo["object_id"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["object_id"].ToString());
                    }
                    if (jsonPostInfo["created_time"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["created_time"].ToString());
                    }
                    if (jsonPostInfo["updated_time"] != null)
                    {
                        Console.WriteLine(jsonPostInfo["updated_time"].ToString());
                    }
                    if (jsonPostInfo["shares"] != null && (jsonPostInfo["shares"])["count"] != null)
                    {
                        Console.WriteLine((jsonPostInfo["shares"])["count"].ToString());
                    }
                    if (jsonPostInfo["shares"] != null && (jsonPostInfo["shares"])["count"] != null)
                    {
                        Console.WriteLine((jsonPostInfo["shares"])["count"].ToString());
                    }
                    if (jsonPostInfo["likes"] != null && (jsonPostInfo["likes"])["data"] != null)
                    {
                        Int32 cantidadLikes = 0;
                        cantidadLikes += (jsonPostInfo["likes"])["data"].ToArray().Length;

                        string after = string.Empty;
                        after = (((jsonPostInfo["likes"])["paging"])["cursors"])["after"].ToString();

                        string strPagePostLikesRequest = Constantes.ApiBaseUrl + jsonPostInfo["id"].ToString() + "/likes/?after=" + after + "&access_token=" + accessToken;
                        string strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                        var jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);

                        while (after != null && !string.IsNullOrEmpty(after))
                        {
                            if (jsonPagePostLikesInfo["data"] != null)
                            {
                                cantidadLikes += jsonPagePostLikesInfo["data"].ToArray().Length;
                                if (jsonPagePostLikesInfo["paging"] != null && 
                                    (jsonPagePostLikesInfo["paging"])["cursors"] != null &&
                                        ((jsonPagePostLikesInfo["paging"])["cursors"])["after"] != null)
                                {
                                    after = ((jsonPagePostLikesInfo["paging"])["cursors"])["after"].ToString();
                                    strPagePostLikesRequest = Constantes.ApiBaseUrl + jsonPostInfo["id"].ToString() + "/likes/?after=" + after + "&access_token=" + accessToken;
                                    strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                                    jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);
                                }
                                else
                                {
                                    after = string.Empty;
                                }
                            }
                        }

                        string before = string.Empty;
                        before = (((jsonPostInfo["likes"])["paging"])["cursors"])["before"].ToString();

                        strPagePostLikesRequest = Constantes.ApiBaseUrl + jsonPostInfo["id"].ToString() + "/likes/?before=" + before + "&access_token=" + accessToken;
                        strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                        jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);

                        while (before != null && !string.IsNullOrEmpty(before))
                        {
                            if (jsonPagePostLikesInfo["data"] != null)
                            {
                                cantidadLikes += jsonPagePostLikesInfo["data"].ToArray().Length;
                                if (jsonPagePostLikesInfo["paging"] != null &&
                                    (jsonPagePostLikesInfo["paging"])["cursors"] != null &&
                                        ((jsonPagePostLikesInfo["paging"])["cursors"])["before"] != null)
                                {
                                    before = ((jsonPagePostLikesInfo["paging"])["cursors"])["before"].ToString();
                                    strPagePostLikesRequest = Constantes.ApiBaseUrl + jsonPostInfo["id"].ToString() + "/likes/?before=" + after + "&access_token=" + accessToken;
                                    strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                                    jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);
                                }
                                else
                                {
                                    before = string.Empty;
                                }
                            }
                        }


                        //paging;
                        Console.WriteLine(cantidadLikes.ToString());
                    }
                }
            }




            ////HttpWebRequest request2 = WebRequest.Create("https://graph.facebook.com/v2.2/71263708835/posts?access_token=" + accessToken) as HttpWebRequest;
            //string urlPosts = "https://graph.facebook.com/v2.2/71263708835/posts?access_token=" + accessToken;
            //string str2 = RequestResponse(urlPosts);
            ////HttpWebResponse response2 = (HttpWebResponse)request.GetResponse();
            ////var encoding2 = UTF8Encoding.UTF8;

            //var json = JObject.Parse(str2);

            //foreach (JObject minij in json["data"].ToArray())
            //{

            //}

            //Console.WriteLine(json.);

            //using (var reader = new System.IO.StreamReader(response2.GetResponseStream(), encoding2))
            //{
            //    responseText = reader.ReadToEnd();
            //    Console.WriteLine(responseText);
            //}



            Console.ReadLine();
        }

        
    }
     */
    public class FBConnector
    {
        private string generarAccessToken()
        {
            string AppId = ConfigurationManager.AppSettings["AppId"].ToString();
            string AppSecret = ConfigurationManager.AppSettings["AppSecret"].ToString();
            string strGenerateToken = Constantes.ApiBaseUrl + "oauth/access_token?" + string.Format("client_id={0}&client_secret={1}&grant_type=client_credentials", AppId, AppSecret);
            string strResponseToken = RequestResponse(strGenerateToken);
            string accessToken = string.Empty;
            string[] arrResponseToken = strResponseToken.Split('=');
            if (!String.IsNullOrEmpty(strResponseToken) && arrResponseToken.Length > 1)
            {
                accessToken = arrResponseToken[1];
            }
            return accessToken;
        }

        private string RequestResponse(string pUrl)
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

        public string getIDInfo(string pageId)
        {
            string accessToken = generarAccessToken();
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(accessToken))
            {
                string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                string cabecera = generarCabeceraData(new string[] { "id", "name", "category", "link", "birthday", "gender", "likes", "talking_about_count" }, delimiter);
                sb.AppendLine(cabecera);

                //Info de la página
                string strPageInfoRequest = Constantes.ApiBaseUrl + pageId + "?access_token=" + accessToken;
                string strPageInfoResponse = RequestResponse(strPageInfoRequest);
                var jsonPageInfo = JObject.Parse(strPageInfoResponse);

                string data = (jsonPageInfo["id"] != null ? jsonPageInfo["id"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["name"] != null ? jsonPageInfo["name"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["category"] != null ? jsonPageInfo["category"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["link"] != null ? jsonPageInfo["link"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["birthday"] != null ? jsonPageInfo["birthday"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["gender"] != null ? jsonPageInfo["gender"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["likes"] != null ? jsonPageInfo["likes"].ToString() : string.Empty) +
                              delimiter + (jsonPageInfo["talking_about_count"] != null ? jsonPageInfo["talking_about_count"].ToString() : string.Empty);
                sb.AppendLine(data);

            }
            else
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("Ocurrió un error al generar el token de acceso. Token de acceso: " + accessToken);
            }

            return sb.ToString();
        }

        public string getPosts(string pageId, string sinceDate, string untilDate)
        {
            string accessToken = generarAccessToken();
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(accessToken))
            {
                string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                string cabecera = generarCabeceraData(new string[] { "id", "story", "link", "type", "object_id", "created_time", "updated_time", "shares", "like_count", "comment_count" }, delimiter);
                sb.AppendLine(cabecera);

                //Posts públicos de la página
                string strPagePostsRequest = Constantes.ApiBaseUrl + pageId + "/posts/?";
                bool blnValidacion = false;

                if (!string.IsNullOrEmpty(sinceDate))
                {
                    strPagePostsRequest = strPagePostsRequest + "since=" + sinceDate;
                    blnValidacion = true;
                }

                if (!string.IsNullOrEmpty(untilDate))
                {
                    strPagePostsRequest = strPagePostsRequest + ((!blnValidacion) ? "until=" + untilDate : "&until=" + untilDate);
                    blnValidacion = true;
                }

                strPagePostsRequest = strPagePostsRequest + ((!blnValidacion) ? "access_token=" + accessToken : "&access_token=" + accessToken);
                string strPagePostsResponse = RequestResponse(strPagePostsRequest);
                var jsonPostsInfo = JObject.Parse(strPagePostsResponse);

                string data = string.Empty;
                if (jsonPostsInfo["data"] != null)
                {
                    foreach (JObject jsonPostInfo in jsonPostsInfo["data"].ToArray())
                    {
                        data = string.Empty;

                        data += (jsonPostInfo["id"] != null ? jsonPostInfo["id"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["story"] != null ? jsonPostInfo["story"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["link"] != null ? jsonPostInfo["link"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["type"] != null ? jsonPostInfo["type"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["object_id"] != null ? jsonPostInfo["object_id"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["created_time"] != null ? jsonPostInfo["created_time"].ToString() : string.Empty);
                        data += delimiter + (jsonPostInfo["updated_time"] != null ? jsonPostInfo["updated_time"].ToString() : string.Empty);
                        data += delimiter + ((jsonPostInfo["shares"] != null && (jsonPostInfo["shares"])["count"] != null) ? (jsonPostInfo["shares"])["count"].ToString() : string.Empty);
                        data += delimiter + ((jsonPostInfo["likes"] != null && (jsonPostInfo["likes"])["data"] != null) ? (getCantidadLikes(jsonPostInfo["id"].ToString(), accessToken, (JObject)jsonPostInfo["likes"])) : string.Empty);
                        data += delimiter + string.Empty;

                        //(jsonPostInfo["likes"])["data"]
                        //jsonPostInfo["likes"] != null && (jsonPostInfo["likes"])["data"] != null

                        sb.AppendLine(data);
                    }
                }
            }
            else
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("Ocurrió un error al generar el token de acceso. Token de acceso: " + accessToken);
            }

            return sb.ToString();
        }

        private string getCantidadLikes(string objectID,string accessToken, JObject likeCollection)
        {
            string resultado = string.Empty;
            Int32 cantidadLikes = -1;

            cantidadLikes += likeCollection["data"].ToArray().Length;

            string after = string.Empty;
            after = ((likeCollection["paging"])["cursors"])["after"].ToString();

            string strPagePostLikesRequest = Constantes.ApiBaseUrl + objectID + "/likes/?after=" + after + "&access_token=" + accessToken;
            string strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
            var jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);

            while (after != null && !string.IsNullOrEmpty(after))
            {
                if (jsonPagePostLikesInfo["data"] != null)
                {
                    cantidadLikes += jsonPagePostLikesInfo["data"].ToArray().Length;
                    if (jsonPagePostLikesInfo["paging"] != null &&
                        (jsonPagePostLikesInfo["paging"])["cursors"] != null &&
                            ((jsonPagePostLikesInfo["paging"])["cursors"])["after"] != null)
                    {
                        after = ((jsonPagePostLikesInfo["paging"])["cursors"])["after"].ToString();
                        strPagePostLikesRequest = Constantes.ApiBaseUrl + objectID + "/likes/?after=" + after + "&access_token=" + accessToken;
                        strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                        jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);
                    }
                    else
                    {
                        after = string.Empty;
                    }
                }
            }

            string before = string.Empty;
            before = ((likeCollection["paging"])["cursors"])["before"].ToString(); ;

            strPagePostLikesRequest = Constantes.ApiBaseUrl + objectID + "/likes/?before=" + before + "&access_token=" + accessToken;
            strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
            jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);

            while (before != null && !string.IsNullOrEmpty(before))
            {
                if (jsonPagePostLikesInfo["data"] != null)
                {
                    cantidadLikes += jsonPagePostLikesInfo["data"].ToArray().Length;
                    if (jsonPagePostLikesInfo["paging"] != null &&
                        (jsonPagePostLikesInfo["paging"])["cursors"] != null &&
                            ((jsonPagePostLikesInfo["paging"])["cursors"])["before"] != null)
                    {
                        before = ((jsonPagePostLikesInfo["paging"])["cursors"])["before"].ToString();
                        strPagePostLikesRequest = Constantes.ApiBaseUrl + objectID + "/likes/?before=" + before + "&access_token=" + accessToken;
                        strPagePostLikesResponse = RequestResponse(strPagePostLikesRequest);
                        jsonPagePostLikesInfo = JObject.Parse(strPagePostLikesResponse);
                    }
                    else
                    {
                        before = string.Empty;
                    }
                }
            }

            return cantidadLikes.ToString();
        }

        private string generarCabeceraData(string[] cabecera, string delimiter)
        {
            string cabeceraFinal = string.Empty;

            if (cabecera != null && cabecera.Length > 1)
            {
                foreach (string item in cabecera)
                {
                    cabeceraFinal = (string.IsNullOrEmpty(cabeceraFinal)) ? item : cabeceraFinal + delimiter + item;
                }
            }

            return cabeceraFinal;
        }
    }
}
