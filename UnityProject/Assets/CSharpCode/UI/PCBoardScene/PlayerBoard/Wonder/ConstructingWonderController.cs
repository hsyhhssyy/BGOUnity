using System.Collections.Generic;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Controller;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.PCBoardScene.PlayerBoard.Wonder
{
    /// <summary>
    /// 正在建造的奇迹的那个框框的Controller
    /// </summary>
    [UsedImplicitly]
    public class ConstructingWonderController:SimpleClickUIController
    {
        public WonderMenuController Menu;

        protected override void Refresh()
        {
            
        }


        protected override void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args)
        {
            base.OnSubscribedGameEvents(sender, args);

            if (!args.UIKey.Contains(Guid))
            {
                return;
            }

            if (args.EventType == GameUIEventType.PopupMenu)
            {
                Menu.Popup(args.AttachedData["Actions"] as List<PlayerAction>);
            }
        }

        protected override string GetUIKey()
        {
            return "PCBoard.ConstructingWonderBox." + Guid;
        }
    }
}
