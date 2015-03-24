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
    public class FBApiConnector : IFBApiConnector
    {
        public string GetIdInfo(string Id)
        {
            FBConnector fbConn = new FBConnector();
            return fbConn.getIDInfo(Id);
        }

        public Stream GetIdInfoRaw(string Id)
        {
            FBConnector fbConn = new FBConnector();
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(Encoding.Default.GetBytes(fbConn.getIDInfo(Id)));
        }

        public Stream GetPostsRaw(string Id, string sinceDate, string untilDate)
        {
            FBConnector fbConn = new FBConnector();
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/plain";
            return new MemoryStream(Encoding.Default.GetBytes(fbConn.getPosts(Id,sinceDate,untilDate)));
        }
    }
}
