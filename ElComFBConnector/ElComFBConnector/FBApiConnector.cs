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
     
     
     */
    public class FBApiConnector : IFBApiConnector
    {
        //http://graph.facebook.com/
        //https://graph.facebook.com/oauth/access_token?client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials
        //https://graph.facebook.com/v2.2/71263708835/posts?access_token=884427038267146|M85AmsihHrGsf7UuYF4GO2s4BeE

        public string GetPagesInfo(string pageId)
        {
            HttpRequest request = new HttpRequest("", Constantes.ApiBaseUrl + "oauth/access_token", string.Format("client_id={0}&client_secret={1}&grant_type=client_credentials",Constantes.AppID,Constantes.AppSecret));

            TextWriter twrtr = TextWriter.Null;
            HttpResponse response = new HttpResponse(twrtr);

            return "";
        }
    }
}
