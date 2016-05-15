using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                }else if (bgoStr.StartsWith("Upgrade"))
                {
                    //Upgrade Agriculture -> Irrigation (2R)
                    var card1 = bgoStr.CutBetween("Upgrade ", " ->");
                    var card2 = bgoStr.CutBetween("> ", " (");
                    var resCost= bgoStr.CutBetween("(", "R)");

                    action.ActionType = PlayerActionType.UpgradeBuilding;
                    action.Data[0] = civilopedia.GetCardInfoByName(card1);
                    action.Data[1] = civilopedia.GetCardInfoByName(card2);
                    action.Data[2] = resCost;
                    action.Data[3] = optValue;
                }
            }

            //---删除多余 / 一拆多
            var disband =
                game.PossibleActions.FirstOrDefault(
                    action =>
                        action.ActionType == PlayerActionType.Unknown &&
                        action.Data[0].ToString() == "Disband / Destroy");
            if (disband != null)
            {
                //为每个建筑物建立自己的disband/destory
                game.PossibleActions.Remove(disband);
                var subDropdown = BgoRegexpCollections.ExtractSubDropDown("unite").Match(html);
                if (subDropdown.Success == true)
                {
                    foreach (Match match in BgoRegexpCollections.ExtractActions.Matches(subDropdown.Groups[1].Value))
                    {
                        //8;9 A/ Agriculture
                        var optValue = match.Groups[1].Value;
                        var cardName = match.Groups[2].Value;
                        var cardAge = cardName.CutBefore("/");
                        cardName = cardName.CutAfter("/ ");

                        var card = civilopedia.GetCardInfoByName((Age)Enum.Parse(typeof (Age), cardAge), cardName);

                        if (card.CardType == CardType.MilitaryTechAirForce ||
                            card.CardType == CardType.MilitaryTechArtillery ||
                            card.CardType == CardType.MilitaryTechCavalry ||
                            card.CardType == CardType.MilitaryTechInfantry)
                        {
                            game.PossibleActions.Add(new BgoPlayerAction()
                            {
                                ActionType = PlayerActionType.Disband,
                                Data = new Dictionary<int, object>
                                {
                                    {0,card },
                                    {1,disband.Data[1]},
                                    {2,optValue }

                                }
                            });
                        }
                        else
                        {
                            game.PossibleActions.Add(new BgoPlayerAction()
                            {
                                ActionType = PlayerActionType.Destory,
                                Data = new Dictionary<int, object>
                                {
                                    {0,card },
                                    {1,disband.Data[1]},
                                    {2,optValue }

                                }
                            });
                        }
                    }
                }
            }
        }
    }
}