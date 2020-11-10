using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCF
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServiceChat" in both code and config file together.
    [DataContract]
    public class AllData
    {
        [DataMember(Name = "Username", Order = 1)]
        public string Username { get; set; }
        [DataMember(Name = "Message", Order = 2)]
        public string Message { get; set; }
        [DataMember(Name = "Time", Order = 3)]
        public string Time { get; set; }
    }
    [ServiceContract(CallbackContract = typeof (IServiceChatCallBack))]
    public interface IServiceChat
    {
        [OperationContract]
        int Connect(string name);

        [OperationContract]
        void Disconnect(int id);

        [OperationContract(IsOneWay = true)]
        void SendMsg(string msg, int id);

        [OperationContract]
        AllData[] GetData();

        [OperationContract(IsOneWay = true)]
        void NewMsg(int id_u, string msg, string time);
    }

    public interface IServiceChatCallBack
    {
        [OperationContract(IsOneWay = true)]
        void MsgCallBack(string msg);
    }

}
