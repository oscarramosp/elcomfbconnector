using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ElComFBConnector
{
    public class Constantes
    {
        //public const string ApiBaseUrl = "https://graph.facebook.com/v2.2/";
        //public const string ApiBaseUrlUnversioned = "https://graph.facebook.com/";

        public static string ApiBaseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"].ToString();
        public static string ApiBaseUrlUnversioned = ConfigurationManager.AppSettings["ApiBaseUrlUnversioned"].ToString();

        public static string Likes = ConfigurationManager.AppSettings["likes"].ToString();
        public static string Comments = ConfigurationManager.AppSettings["comments"].ToString();
        public static string Posts = ConfigurationManager.AppSettings["posts"].ToString();
    }
}
