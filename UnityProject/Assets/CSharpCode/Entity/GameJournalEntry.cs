using System;

namespace Assets.CSharpCode.Entity
{
    #region 基类
    public class GameJournalEntry
    {
        public String EntryTime;
        public String PlayerName;
        public String Age;
        public String Turn;

        public String Title;
        public String EntryText;

        public GameJournalEntryType Type;
    }

    public enum GameJournalEntryType
    {
        Unknown,
    }
    #endregion

}