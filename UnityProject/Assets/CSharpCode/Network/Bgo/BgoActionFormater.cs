using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;

namespace Assets.CSharpCode.Network.Bgo
{
    public static class BgoActionFormater
    {
        private static TtaCivilopedia civilopedia = TtaCivilopedia.GetCivilopedia("2.0");

        public static void Format(BgoGame game, String html)
        {
            //Unknown的话
            //Data[0]是opt的显示内容，就是文本
            //Data[1]是optValue（要传给后端的）

            foreach (
                var action in
                    game.PossibleActions.Where(action => action.ActionType == PlayerActionType.Unknown).ToList())
            {
                String bgoStr = action.Data[0].ToString();
                String optValue = action.Data[1].ToString();

                if (bgoStr.StartsWith("Increase"))
                {
                    int foodCost = Convert.ToInt32(bgoStr.CutBetween("(", "F)"));
                    action.ActionType = PlayerActionType.IncreasePopulation;
                    action.Data[0] = bgoStr;
                    action.Data[1] = foodCost;
                    action.Data[2] = optValue;
                }
                else if (bgoStr.StartsWith("Build"))
                {
                    //Build关键字的出现可能有多种情况。包括
                    //Build Iron (5R)
                    //Build free temple/Build free warrior.
                    //Build 1 stage of Library of Alexandria (1R)
                    //Build 4 stages of Library of Alexandria (1R)
                    if (bgoStr.IndexOf("stage", StringComparison.Ordinal) == 8 ||
                        bgoStr.IndexOf("stages", StringComparison.Ordinal) == 8)
                    {
                        //Wonder
                    }
                    else if (bgoStr.Contains("("))
                    {
                        //Building
                        String cardName = bgoStr.CutBetween("Build ", " (");
                        int resCost = Convert.ToInt32(bgoStr.CutBetween("(", "R)"));
                        action.ActionType = PlayerActionType.BuildBuilding;
                        action.Data[0] = civilopedia.GetCardInfoByName(cardName);
                        action.Data[1] = resCost;
                        action.Data[2] = optValue;
                    }
                    else
                    {
                        //以Build开头的其他无意义选项
                    }
                }
            }
        }
    }
}