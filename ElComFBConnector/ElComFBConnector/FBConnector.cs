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
            StringBuilder sb = new StringBuilder();
            string strPageInfoRequest = string.Empty;
            try
            {
                string accessToken = generarAccessToken();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                    string cabecera = generarCabeceraData(new string[] { "id", "name", "category", "link", "birthday", "gender", "likes", "talking_about_count", "first_name", "last_name", "locale", "username", "location_city", "location_country" }, delimiter);
                    sb.AppendLine(cabecera);

                    //Info de la página
                    strPageInfoRequest = Constantes.ApiBaseUrl + pageId + "?access_token=" + accessToken;
                    string strPageInfoResponse = RequestResponse(strPageInfoRequest);
                    var jsonPageInfo = JObject.Parse(strPageInfoResponse);

                    //Info de la página - sin seguridad
                    string strPageInfoRequestUnsec = Constantes.ApiBaseUrlUnversioned + pageId;
                    string strPageInfoResponseUnsec = RequestResponse(strPageInfoRequestUnsec);
                    var jsonPageInfoUnsec = JObject.Parse(strPageInfoResponseUnsec);

                    string data = (jsonPageInfo["id"] != null ? jsonPageInfo["id"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["name"] != null ? jsonPageInfo["name"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["category"] != null ? jsonPageInfo["category"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["link"] != null ? jsonPageInfo["link"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["birthday"] != null ? jsonPageInfo["birthday"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["gender"] != null ? jsonPageInfo["gender"].ToString() : (jsonPageInfoUnsec["gender"] != null ? jsonPageInfoUnsec["gender"].ToString() : string.Empty)) +
                                  delimiter + (jsonPageInfo["likes"] != null ? jsonPageInfo["likes"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfo["talking_about_count"] != null ? jsonPageInfo["talking_about_count"].ToString() : string.Empty) +

                                  delimiter + (jsonPageInfoUnsec["first_name"] != null ? jsonPageInfoUnsec["first_name"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfoUnsec["last_name"] != null ? jsonPageInfoUnsec["last_name"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfoUnsec["locale"] != null ? jsonPageInfoUnsec["locale"].ToString() : string.Empty) +
                                  delimiter + (jsonPageInfoUnsec["username"] != null ? jsonPageInfoUnsec["username"].ToString() : string.Empty) +

                                  delimiter + ((jsonPageInfo["location"] != null && (jsonPageInfo["location"])["city"] != null) ? (jsonPageInfo["location"])["city"].ToString() : string.Empty) +
                                  delimiter + ((jsonPageInfo["location"] != null && (jsonPageInfo["location"])["country"] != null) ? (jsonPageInfo["location"])["country"].ToString() : string.Empty);
                    sb.AppendLine(data);

                }
                else
                {
                    sb.Clear();
                    sb.AppendLine("ERROR");
                    sb.AppendLine("Ocurrió un error al generar el token de acceso. Token de acceso: " + accessToken);
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("Ocurrió un error al invocar el servicio.");
                sb.AppendLine("Link llamada API FB");
                sb.AppendLine(strPageInfoRequest);
                sb.AppendLine("Error");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(ex.Message);
            }
            return sb.ToString();
        }

        public string getPosts(string pageId, string sinceDate, string untilDate)
        {
            StringBuilder sb = new StringBuilder();
            string strPagePostsRequest = string.Empty;
            try
            {
                string accessToken = generarAccessToken();

                if (!string.IsNullOrEmpty(accessToken))
                {
                    string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                    string cabecera = generarCabeceraData(new string[] { "id", "story", "link", "type", "object_id", "created_time", "updated_time", "shares", "like_count", "comment_count", "message" }, delimiter);
                    sb.AppendLine(cabecera);

                    //Posts públicos de la página
                    strPagePostsRequest = Constantes.ApiBaseUrl + pageId + "/posts/?";
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
                            data += delimiter + getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Likes);
                            data += delimiter + getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Comments);
                            data += delimiter + (jsonPostInfo["message"] != null ? jsonPostInfo["message"].ToString() : string.Empty);


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
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("Ocurrió un error al invocar el servicio.");
                sb.AppendLine("Link llamada API FB");
                sb.AppendLine(strPagePostsRequest);
                sb.AppendLine("Error");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(ex.Message);
            }
            return sb.ToString();
        }

        private string getCantidadSubColeccion(string objectID, string accessToken, string subcoleccion)
        {
            string cantidadSubCol = "-1";

            string strPageSubColSummaryRequest = Constantes.ApiBaseUrl + objectID + "/"+subcoleccion+"/?summary=true"+"&access_token=" + accessToken;
            string strPageSubColSummaryResponse = RequestResponse(strPageSubColSummaryRequest);
            var jsonPageSubColSummaryInfo = JObject.Parse(strPageSubColSummaryResponse);

            cantidadSubCol = ((jsonPageSubColSummaryInfo["summary"] != null && (jsonPageSubColSummaryInfo["summary"])["total_count"] != null) ? (jsonPageSubColSummaryInfo["summary"])["total_count"].ToString() : "0");

            return cantidadSubCol;
        }

        public string getComments(string objectId)
        {
            StringBuilder sb = new StringBuilder();
            string strPostCommentsRequest = string.Empty;

            try
            {
                string accessToken = generarAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                    string cabecera = generarCabeceraData(new string[] { "id", "story", "link", "type", "object_id", "created_time", "updated_time", "shares", "like_count", "comment_count", "message" }, delimiter);
                    sb.AppendLine(cabecera);

                    //Comments de algún objeto
                    strPostCommentsRequest = Constantes.ApiBaseUrl + objectId + "/comments/?access_token=" + accessToken;
                    string strPostCommentsResponse = RequestResponse(strPostCommentsRequest);
                    var jsonPostComments = JObject.Parse(strPostCommentsResponse);
                }
                else
                {
                    sb.Clear();
                    sb.AppendLine("ERROR");
                    sb.AppendLine("Ocurrió un error al generar el token de acceso. Token de acceso: " + accessToken);
                }
            }
            catch (Exception ex)
            {
                sb.Clear();
                sb.AppendLine("ERROR");
                sb.AppendLine("Ocurrió un error al invocar el servicio.");
                sb.AppendLine("Link llamada API FB");
                sb.AppendLine(strPostCommentsRequest);
                sb.AppendLine("Error");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }
        
        private string getCantidadLikesX(string objectID,string accessToken, JObject likeCollection)
        {
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

        private string getCantidadCommentsX(string objectID, string accessToken, JObject commentCollection)
        {
            Int32 cantidadComments = -1;

            cantidadComments += commentCollection["data"].ToArray().Length;

            string after = string.Empty;
            after = ((commentCollection["paging"])["cursors"])["after"].ToString();

            string strPagePostCommentsRequest = Constantes.ApiBaseUrl + objectID + "/comments/?after=" + after + "&access_token=" + accessToken;
            string strPagePostCommentsResponse = RequestResponse(strPagePostCommentsRequest);
            var jsonPagePostCommentsInfo = JObject.Parse(strPagePostCommentsResponse);

            while (after != null && !string.IsNullOrEmpty(after))
            {
                if (jsonPagePostCommentsInfo["data"] != null)
                {
                    cantidadComments += jsonPagePostCommentsInfo["data"].ToArray().Length;
                    if (jsonPagePostCommentsInfo["paging"] != null &&
                        (jsonPagePostCommentsInfo["paging"])["cursors"] != null &&
                            ((jsonPagePostCommentsInfo["paging"])["cursors"])["after"] != null)
                    {
                        after = ((jsonPagePostCommentsInfo["paging"])["cursors"])["after"].ToString();
                        strPagePostCommentsRequest = Constantes.ApiBaseUrl + objectID + "/comments/?after=" + after + "&access_token=" + accessToken;
                        strPagePostCommentsResponse = RequestResponse(strPagePostCommentsRequest);
                        jsonPagePostCommentsInfo = JObject.Parse(strPagePostCommentsResponse);
                    }
                    else
                    {
                        after = string.Empty;
                    }
                }
            }

            string before = string.Empty;
            before = ((commentCollection["paging"])["cursors"])["before"].ToString(); ;

            strPagePostCommentsRequest = Constantes.ApiBaseUrl + objectID + "/comments/?before=" + before + "&access_token=" + accessToken;
            strPagePostCommentsResponse = RequestResponse(strPagePostCommentsRequest);
            jsonPagePostCommentsInfo = JObject.Parse(strPagePostCommentsResponse);

            while (before != null && !string.IsNullOrEmpty(before))
            {
                if (jsonPagePostCommentsInfo["data"] != null)
                {
                    cantidadComments += jsonPagePostCommentsInfo["data"].ToArray().Length;
                    if (jsonPagePostCommentsInfo["paging"] != null &&
                        (jsonPagePostCommentsInfo["paging"])["cursors"] != null &&
                            ((jsonPagePostCommentsInfo["paging"])["cursors"])["before"] != null)
                    {
                        before = ((jsonPagePostCommentsInfo["paging"])["cursors"])["before"].ToString();
                        strPagePostCommentsRequest = Constantes.ApiBaseUrl + objectID + "/comments/?before=" + before + "&access_token=" + accessToken;
                        strPagePostCommentsResponse = RequestResponse(strPagePostCommentsRequest);
                        jsonPagePostCommentsInfo = JObject.Parse(strPagePostCommentsResponse);
                    }
                    else
                    {
                        before = string.Empty;
                    }
                }
            }

            return cantidadComments.ToString();
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
