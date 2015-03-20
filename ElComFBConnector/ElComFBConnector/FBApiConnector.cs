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
    public class FBApiConnector : IFBApiConnector
    {
        //http://graph.facebook.com/
        //https://graph.facebook.com/oauth/access_token?client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials
        public string GetPagesLikes(string pageId)
        {
            HttpRequest request = new HttpRequest("", Constantes.ApiBaseUrl, "client_id=884427038267146&client_secret=0a60d73f092c689208699724f8b0933d&grant_type=client_credentials");

            TextWriter twrtr = TextWriter.Null;
            HttpResponse response = new HttpResponse(twrtr);

            return "";
        }
    }
}
