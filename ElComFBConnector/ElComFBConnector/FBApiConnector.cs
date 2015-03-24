using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
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

        public string GetIdInfo(string Id)
        {
            FBConnector fbConn = new FBConnector();
            return fbConn.getIDInfo(Id);
        }

        public Stream GetIdInfoRaw(string Id)
        {
            FBConnector fbConn = new FBConnector();
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(Encoding.UTF8.GetBytes(fbConn.getIDInfo(Id)));
        }
    }
}
