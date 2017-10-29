using System.Collections.Generic;
using System.Runtime.Serialization;
using Assets.CSharpCode.GameLogic.GameEvents;

namespace Assets.CSharpCode.GameLogic.Actions
{
    //TODO 移动进Entity命名空间（和GameMove一起）
    [DataContract]
    public class ActionResponse
    {
        [DataMember]
        public ActionResponseType Type;
        [DataMember]
        public List<GameMove> Changes=new List<GameMove>(); 
    }

    public enum ActionResponseType
    {
        /// <summary>
        /// 表示无条件接受本Action
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
        ForceRefresh,
        /// <summary>
        /// 表示该用户，其他用户或者服务器取消了本Action，没有事情发生
        /// </summary>
        Canceled,
    }
}