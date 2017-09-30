using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Assets.CSharpCode.Entity;

namespace Assets.CSharpCode.Network.Bgo
{
    class BgoJournalFormater
    {
        public static void Format(BgoGame game, string html)
        {
            var journalMatch=BgoRegexpCollections.ExtractGameJournal.Matches(html);

            List<GameJournalEntry> journalToAppend=new List<GameJournalEntry>();
            foreach (Match match in journalMatch)
            {
                //直接替换
                journalToAppend.Add(CreateGameJournalEntry(match));
            }

            game.Journal = journalToAppend;
        }

        private static GameJournalEntry CreateGameJournalEntry(Match match)
        {var journal=new GameJournalEntry();
            journal.EntryTime = match.Groups[1].Value.Replace("&nbsp;", " ").Trim();
            journal.PlayerName = match.Groups[2].Value.Replace("&nbsp;", " ").Trim();
            journal.Age = match.Groups[3].Value.Replace("&nbsp;", " ").Trim();
            journal.Turn = match.Groups[4].Value.Replace("&nbsp;", " ").Trim();
            journal.Title = match.Groups[5].Value.Replace("&nbsp;", " ").Trim();
            journal.EntryText = match.Groups[6].Value.Replace("&nbsp;", " ").Trim();

            return journal;
        }
    }
}
