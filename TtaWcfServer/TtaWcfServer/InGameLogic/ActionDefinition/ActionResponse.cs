using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using TtaWcfServer.InGameLogic.GameJournals;

namespace TtaWcfServer.InGameLogic.ActionDefinition
{
    [DataContract]
    public class ActionResponse
    {
        [DataMember]
        public ActionResponseType Type;
        [DataMember]
        public List<GameMove> Changes = new List<GameMove>();
    }

    public enum ActionResponseType
    {
        /// <summary>
        /// 表示无条件接受本Action的ClientMove，但是请客户端额外执行下列内容
        /// </summary>
        Accepted,
        /// <summary>
        /// 表示接受本Action，但是需要客户端比对如下内容
        /// </summary>
        ChangeList,
        /// <summary>
        /// 表示不接受Action，并且该Action不是当前合法的action
        /// </summary>
        InvalidAction,
        /// <summary>
        /// 表示不接受Action，并且当前客户端状态可能与服务器已经不一致，需要进行刷新
        /// </summary>
        ForceRefresh
    }
}