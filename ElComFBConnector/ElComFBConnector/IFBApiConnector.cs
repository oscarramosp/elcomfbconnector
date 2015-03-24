using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;

namespace ElComFBConnector
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IFBApiConnector" in both code and config file together.
    [ServiceContract]
    public interface IFBApiConnector
    {
        [OperationContract]
        [WebGet]
        string GetIdInfo(string Id);

        [OperationContract]
        [WebGet]
        Stream GetIdInfoRaw(string Id);

        [OperationContract]
        [WebGet]
        Stream GetPostsRaw(string Id, string sinceDate, string untilDate);
    }
}
