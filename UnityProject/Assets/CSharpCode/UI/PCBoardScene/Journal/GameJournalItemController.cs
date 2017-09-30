using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.UI.PCBoardScene.Controller;
using UnityEngine;

namespace Assets.CSharpCode.UI.PCBoardScene.Journal
{
    public class GameJournalItemController:DisplayOnlyUIController
    {
        public GameJournalEntry Entry;
        public TextMesh JournalText;

        protected override void Refresh()
        {
            if (Entry == null)
            {
                JournalText.text = "日志出错";
                return;
            }
            JournalText.text = (/*Entry.EntryTime + " 玩家" +Entry.PlayerName
                + " " +  */Entry.Title).WordWrap(12);

        }
    }
}