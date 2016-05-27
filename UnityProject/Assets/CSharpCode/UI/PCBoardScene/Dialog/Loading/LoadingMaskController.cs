using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Controller
{
    public class LoadingMaskController : TtaUIControllerMonoBehaviour
    {
        [UsedImplicitly]
        public GameBoardManager Manager;

        [UsedImplicitly]
        public void Start()
        {
            Manager.GameBoardManagerEvent += OnSubscribedGameEvents;
            Manager.Regiseter(this);
            gameObject.transform.position=new Vector3(gameObject.transform.position.x, gameObject.transform.position.y,-9f);
        }
        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            //响应Refresh（来重新创建UI Element）
            if (args.EventType == GameUIEventType.Refresh)
            {
               gameObject.SetActive(false);
            }else if (args.EventType == GameUIEventType.WaitingNetwork)
            {
                gameObject.SetActive(true);
            }
        }
    }
}
