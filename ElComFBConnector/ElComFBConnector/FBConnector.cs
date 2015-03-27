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
            string strGenerateToken = Constantes.ApiBaseUrlUnversioned + "oauth/access_token?" + string.Format("client_id={0}&client_secret={1}&grant_type=client_credentials", AppId, AppSecret);
            string strResponseToken = RequestResponse(strGenerateToken);
            string accessToken = string.Empty;
            string[] arrResponseToken = strResponseToken.Split('=');
            if (!String.IsNullOrEmpty(strResponseToken) && arrResponseToken.Length > 1)
            {
                accessToken = arrResponseToken[1];
            }
            return accessToken;
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

                    string data = ((jsonPageInfo["id"] != null ? jsonPageInfo["id"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["name"] != null ? jsonPageInfo["name"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["category"] != null ? jsonPageInfo["category"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["link"] != null ? jsonPageInfo["link"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["birthday"] != null ? jsonPageInfo["birthday"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["gender"] != null ? jsonPageInfo["gender"].ToString() : (jsonPageInfoUnsec["gender"] != null ? jsonPageInfoUnsec["gender"].ToString() : string.Empty))).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["likes"] != null ? jsonPageInfo["likes"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfo["talking_about_count"] != null ? jsonPageInfo["talking_about_count"].ToString() : string.Empty)).Replace(delimiter, "") +

                                  delimiter + ((jsonPageInfoUnsec["first_name"] != null ? jsonPageInfoUnsec["first_name"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfoUnsec["last_name"] != null ? jsonPageInfoUnsec["last_name"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfoUnsec["locale"] != null ? jsonPageInfoUnsec["locale"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + ((jsonPageInfoUnsec["username"] != null ? jsonPageInfoUnsec["username"].ToString() : string.Empty)).Replace(delimiter, "") +

                                  delimiter + (((jsonPageInfo["location"] != null && (jsonPageInfo["location"])["city"] != null) ? (jsonPageInfo["location"])["city"].ToString() : string.Empty)).Replace(delimiter, "") +
                                  delimiter + (((jsonPageInfo["location"] != null && (jsonPageInfo["location"])["country"] != null) ? (jsonPageInfo["location"])["country"].ToString() : string.Empty)).Replace(delimiter, "");
                    sb.AppendLine(data.Replace('\n', ' '));

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
                    strPagePostsRequest = Constantes.ApiBaseUrl + pageId + "/" + Constantes.Posts + "/?";
                    bool blnValidacion = false;

                    if (!string.IsNullOrEmpty(sinceDate))
                    {
                        DateTime desdeD;
                        if (DateTime.TryParse(sinceDate,out desdeD))
                        {
                            strPagePostsRequest = strPagePostsRequest + "since=" + dateTimeToUnixTimestamp(desdeD).ToString();
                            blnValidacion = true;
                        }
                        else
                        {
                            sb.Clear();
                            sb.AppendLine("ERROR");
                            sb.AppendLine("Ocurrió un error al convertir la fecha sinceDate. Valor: " + sinceDate);
                            return sb.ToString();
                        }
                    }

                    if (!string.IsNullOrEmpty(untilDate))
                    {
                        DateTime hastaD;
                        if (DateTime.TryParse(untilDate, out hastaD))
                        {
                            strPagePostsRequest = strPagePostsRequest + ((!blnValidacion) ? "until=" + dateTimeToUnixTimestamp(hastaD).ToString() : "&until=" + dateTimeToUnixTimestamp(hastaD).ToString());
                            blnValidacion = true;
                        }
                        else
                        {
                            sb.Clear();
                            sb.AppendLine("ERROR");
                            sb.AppendLine("Ocurrió un error al convertir la fecha untilDate. Valor: " + untilDate);
                            return sb.ToString();
                        }
                    }

                    strPagePostsRequest = strPagePostsRequest + ((!blnValidacion) ? "access_token=" + accessToken : "&access_token=" + accessToken);
                    strPagePostsRequest = strPagePostsRequest + "&limit=25";
                    string strPagePostsResponse = RequestResponse(strPagePostsRequest);
                    var jsonPostsInfo = JObject.Parse(strPagePostsResponse);

                    string next = string.Empty;
                    next = (jsonPostsInfo["paging"])["next"].ToString();

                    string data = string.Empty;

                    while (next != null && !string.IsNullOrEmpty(next))
                    {
                        if (jsonPostsInfo["data"] != null)
                        {
                            foreach (JObject jsonPostInfo in jsonPostsInfo["data"].ToArray())
                            {
                                data = string.Empty;

                                data += ((jsonPostInfo["id"] != null ? jsonPostInfo["id"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["story"] != null ? jsonPostInfo["story"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["link"] != null ? jsonPostInfo["link"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["type"] != null ? jsonPostInfo["type"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["object_id"] != null ? jsonPostInfo["object_id"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["created_time"] != null ? jsonPostInfo["created_time"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["updated_time"] != null ? jsonPostInfo["updated_time"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + (((jsonPostInfo["shares"] != null && (jsonPostInfo["shares"])["count"] != null) ? (jsonPostInfo["shares"])["count"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + (getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Likes)).Replace(delimiter, "");
                                data += delimiter + (getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Comments)).Replace(delimiter, "");
                                data += delimiter + ((jsonPostInfo["message"] != null ? jsonPostInfo["message"].ToString() : string.Empty)).Replace(delimiter, "");

                                sb.AppendLine(data.Replace('\n', ' ').Replace("\"", "").Replace("'", ""));
                            }
                            if (jsonPostsInfo["paging"] != null &&
                                (jsonPostsInfo["paging"])["next"] != null)
                            {
                                next = (jsonPostsInfo["paging"])["next"].ToString();
                                strPagePostsRequest = next;
                                strPagePostsResponse = RequestResponse(strPagePostsRequest);
                                jsonPostsInfo = JObject.Parse(strPagePostsResponse);
                            }
                            else
                            {
                                next = string.Empty;
                            }
                        }
                        else
                        {
                            next = string.Empty;
                        }
                    }
                    //if (jsonPostsInfo["data"] != null)
                    //{
                    //    foreach (JObject jsonPostInfo in jsonPostsInfo["data"].ToArray())
                    //    {
                    //        data = string.Empty;

                    //        data += ((jsonPostInfo["id"] != null ? jsonPostInfo["id"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["story"] != null ? jsonPostInfo["story"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["link"] != null ? jsonPostInfo["link"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["type"] != null ? jsonPostInfo["type"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["object_id"] != null ? jsonPostInfo["object_id"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["created_time"] != null ? jsonPostInfo["created_time"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["updated_time"] != null ? jsonPostInfo["updated_time"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + (((jsonPostInfo["shares"] != null && (jsonPostInfo["shares"])["count"] != null) ? (jsonPostInfo["shares"])["count"].ToString() : string.Empty)).Replace(delimiter, "");
                    //        data += delimiter + (getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Likes)).Replace(delimiter, "");
                    //        data += delimiter + (getCantidadSubColeccion(jsonPostInfo["id"].ToString(), accessToken, Constantes.Comments)).Replace(delimiter, "");
                    //        data += delimiter + ((jsonPostInfo["message"] != null ? jsonPostInfo["message"].ToString() : string.Empty)).Replace(delimiter, "");

                    //        sb.AppendLine(data.Replace('\n', ' ').Replace("\"", "").Replace("'", ""));
                    //    }
                    //}
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
                    string cabecera = generarCabeceraData(new string[] { "id", "id_from", "name_from", "message", "created_time", "like_count" }, delimiter);
                    sb.AppendLine(cabecera);

                    //Comments de algún objeto
                    strPostCommentsRequest = Constantes.ApiBaseUrl + objectId + "/" + Constantes.Comments + "/?limit=25&access_token=" + accessToken;
                    string strPostCommentsResponse = RequestResponse(strPostCommentsRequest);
                    var jsonPostComments = JObject.Parse(strPostCommentsResponse);

                    string after = string.Empty;
                    if (jsonPostComments["paging"] != null &&
                            (jsonPostComments["paging"])["cursors"] != null &&
                                ((jsonPostComments["paging"])["cursors"])["after"] != null)
                    {
                        after = ((jsonPostComments["paging"])["cursors"])["after"].ToString();
                    }

                    string data = string.Empty;

                    while ((after != null && !string.IsNullOrEmpty(after)) || (jsonPostComments["data"] != null && jsonPostComments["data"].ToArray().Length > 0))
                    {
                        if (jsonPostComments["data"] != null)
                        {
                            foreach (JObject jsonCommentInfo in jsonPostComments["data"].ToArray())
                            {
                                data = string.Empty;

                                data += (jsonCommentInfo["id"] != null ? jsonCommentInfo["id"].ToString() : string.Empty);
                                data += delimiter + (((jsonCommentInfo["from"] != null && (jsonCommentInfo["from"])["id"] != null) ? (jsonCommentInfo["from"])["id"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + (((jsonCommentInfo["from"] != null && (jsonCommentInfo["from"])["name"] != null) ? (jsonCommentInfo["from"])["name"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonCommentInfo["message"] != null ? jsonCommentInfo["message"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonCommentInfo["created_time"] != null ? jsonCommentInfo["created_time"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonCommentInfo["like_count"] != null ? jsonCommentInfo["like_count"].ToString() : string.Empty)).Replace(delimiter, "");

                                sb.AppendLine(data.Replace('\n', ' ').Replace("\"","").Replace("'",""));
                            }
                            if (jsonPostComments["paging"] != null &&
                                (jsonPostComments["paging"])["cursors"] != null &&
                                    ((jsonPostComments["paging"])["cursors"])["after"] != null)
                            {
                                after = ((jsonPostComments["paging"])["cursors"])["after"].ToString();
                                strPostCommentsRequest = Constantes.ApiBaseUrl + objectId + "/" + Constantes.Comments + "/?after=" + after + "&limit=25&access_token=" + accessToken;
                                strPostCommentsResponse = RequestResponse(strPostCommentsRequest);
                                jsonPostComments = JObject.Parse(strPostCommentsResponse);
                            }
                            else
                            {
                                after = string.Empty;
                            }
                        }
                        else
                        {
                            after = string.Empty;
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
                sb.AppendLine(strPostCommentsRequest);
                sb.AppendLine("Error");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
        }

        public string getLikes(string objectId)
        {
            StringBuilder sb = new StringBuilder();
            string strPostLikesRequest = string.Empty;

            try
            {
                string accessToken = generarAccessToken();
                if (!string.IsNullOrEmpty(accessToken))
                {
                    string delimiter = ConfigurationManager.AppSettings["delimiter"].ToString();
                    string cabecera = generarCabeceraData(new string[] { "id", "name" }, delimiter);
                    sb.AppendLine(cabecera);

                    //Likes de algún objeto
                    strPostLikesRequest = Constantes.ApiBaseUrl + objectId + "/" + Constantes.Likes + "/?limit=25&access_token=" + accessToken;
                    string strPostLikesResponse = RequestResponse(strPostLikesRequest);
                    var jsonPostLikes = JObject.Parse(strPostLikesResponse);

                    string after = string.Empty;
                    if (jsonPostLikes["paging"] != null &&
                            (jsonPostLikes["paging"])["cursors"] != null &&
                                ((jsonPostLikes["paging"])["cursors"])["after"] != null)
                    {
                        after = ((jsonPostLikes["paging"])["cursors"])["after"].ToString();
                    }

                    string data = string.Empty;

                    while ((after != null && !string.IsNullOrEmpty(after)) || (jsonPostLikes["data"] != null && jsonPostLikes["data"].ToArray().Length > 0))
                    {
                        if (jsonPostLikes["data"] != null)
                        {
                            foreach (JObject jsonLikeInfo in jsonPostLikes["data"].ToArray())
                            {
                                data = string.Empty;

                                data += ((jsonLikeInfo["id"] != null ? jsonLikeInfo["id"].ToString() : string.Empty)).Replace(delimiter, "");
                                data += delimiter + ((jsonLikeInfo["name"] != null ? jsonLikeInfo["name"].ToString() : string.Empty)).Replace(delimiter, "");

                                sb.AppendLine(data.Replace('\n', ' ').Replace("\"", "").Replace("'", ""));
                            }
                            if (jsonPostLikes["paging"] != null &&
                                (jsonPostLikes["paging"])["cursors"] != null &&
                                    ((jsonPostLikes["paging"])["cursors"])["after"] != null)
                            {
                                after = ((jsonPostLikes["paging"])["cursors"])["after"].ToString();
                                strPostLikesRequest = Constantes.ApiBaseUrl + objectId + "/" + Constantes.Likes + "/?after=" + after + "&limit=25&access_token=" + accessToken;
                                strPostLikesResponse = RequestResponse(strPostLikesRequest);
                                jsonPostLikes = JObject.Parse(strPostLikesResponse);
                            }
                            else
                            {
                                after = string.Empty;
                            }
                        }
                        else
                        {
                            after = string.Empty;
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
                sb.AppendLine(strPostLikesRequest);
                sb.AppendLine("Error");
                sb.AppendLine("----------------------------------------");
                sb.AppendLine(ex.Message);
            }

            return sb.ToString();
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

        private string getCantidadSubColeccion(string objectID, string accessToken, string subcoleccion)
        {
            string cantidadSubCol = "-1";

            string strPageSubColSummaryRequest = Constantes.ApiBaseUrl + objectID + "/" + subcoleccion + "/?summary=true" + "&access_token=" + accessToken;
            string strPageSubColSummaryResponse = RequestResponse(strPageSubColSummaryRequest);
            var jsonPageSubColSummaryInfo = JObject.Parse(strPageSubColSummaryResponse);

            cantidadSubCol = ((jsonPageSubColSummaryInfo["summary"] != null && (jsonPageSubColSummaryInfo["summary"])["total_count"] != null) ? (jsonPageSubColSummaryInfo["summary"])["total_count"].ToString() : "0");

            return cantidadSubCol;
        }

        private Int32 dateTimeToUnixTimestamp(DateTime value)
        {
            TimeSpan span = (value - new DateTime(1970, 1, 1, 0, 0, 0, 0).ToLocalTime());
            return (Int32)span.TotalSeconds;
        }
    }
}
