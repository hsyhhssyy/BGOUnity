using UnityEngine;
using Assets.CSharpCode.Civilopedia;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.Helper;
using Assets.CSharpCode.Network.Bgo;
using Assets.CSharpCode.Translation;
using System.Collections.Generic;
using Assets.CSharpCode.UI;
using Assets.CSharpCode.UI.PCBoardScene;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;
using System.IO;

public class TestSceneUIBehaviour : MonoBehaviour
{
    public bool isDebug = false;

    public GameObject Canvas;
    public GameObject CardSmall;
    public GameObject CardNormal;

    private List<CardInfo> TestCards;

    [UsedImplicitly]
    private void Start()
    {
       Assets.CSharpCode.UI.Util.LogRecorder.Log("Started");

        var informationMesh = GameObject.Find("Information").GetComponent<TextMesh>();

        informationMesh.text = SceneTransporter.LastError.WordWrap(60);

        if (File.Exists(Application.persistentDataPath + "/UsernamePassword"))
        {

            var up = File.ReadAllText(Application.persistentDataPath + "/UsernamePassword").Split("|".ToCharArray());
            GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text = up[0];
            GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text = up[1];
        }

        LoadTestCards();
    }



    #region 几个直接打开特定页面的按钮

    [UsedImplicitly]
    public void 登录并打开尝鲜8_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server=new BgoServer();

       Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame {GameId = "7279176", Nat = "1", Name = "2.5尝鲜 8"};

            SceneTransporter.CurrentGame = g;
            
            ExtensionMethods.LoadScene("Scene/BoardScene-PC",1);
        }));
    }
    [UsedImplicitly]
    public void 登录并打开尝鲜9_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
            Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279177", Nat = "1", Name = "2.5尝鲜 9", Version = "2.0" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene-PC");
        }));
    }
    [UsedImplicitly]
    public void 登录并打开尝鲜10_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoServer();

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("hsyhhssyy", "hsy12345", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "2.5尝鲜 10",Version="2.0" };

            SceneTransporter.CurrentGame = g;

            SceneManager.LoadScene("Scene/BoardScene-PC");
        }));
    }

    [UsedImplicitly]
    public void 根据测试文本展示页面_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoTestServer();
        (SceneTransporter.Server as BgoTestServer).File = "Test/TestPage1";

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "Text based Game" };

            SceneTransporter.CurrentGame = g;

            ExtensionMethods.LoadScene("Scene/BoardScene-PC", 1);
            
        }));
    }

    [UsedImplicitly]
    public void 根据测试文本展示页面2_Clicked()
    {
        //登录
        TtaTranslation.LoadDictionary();
        SceneTransporter.Server = new BgoTestServer();
        (SceneTransporter.Server as BgoTestServer).File = "Test/TestPage2";
        

        Assets.CSharpCode.UI.Util.LogRecorder.Log("Clicked");

        StartCoroutine(SceneTransporter.Server.LogIn("username", "password", () =>
        {
            Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            BgoGame g = new BgoGame { GameId = "7279178", Nat = "2", Name = "Text based Game" };

            SceneTransporter.CurrentGame = g;

            ExtensionMethods.LoadScene("Scene/BoardScene-PC", 1);

        }));
    }

    #endregion

    #region 登录BGO的相关代码

    [UsedImplicitly]
    public void LoginButton_Clicked()
    {
       Assets.CSharpCode.UI.Util.LogRecorder.Log("LoginButton!");
        //LoginButton

        GameObject.Find("Canvas/Login/Login").GetComponent<Button>().interactable = false;

        SceneTransporter.Server = new BgoServer();

        StartCoroutine(SceneTransporter.Server.LogIn(
            GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text,
            GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text, () =>
        {
           Assets.CSharpCode.UI.Util.LogRecorder.Log("Logged in!");

            
                var up = GameObject.Find("Canvas/Login/Username").GetComponent<InputField>().text + "|" +
                         GameObject.Find("Canvas/Login/Password").GetComponent<InputField>().text;
                File.WriteAllText(Application.persistentDataPath + "/UsernamePassword", up);
            

            StartCoroutine(SceneTransporter.Server.ListGames(
            gamesReturn =>
            {
               Assets.CSharpCode.UI.Util.LogRecorder.Log("List Get!");

                SceneTransporter.LastListedGames = gamesReturn;

                var dropDown = GameObject.Find("Canvas/Login/MyGamesList").GetComponent<Dropdown>();
                dropDown.options = new List<Dropdown.OptionData>();
                foreach (var pa in gamesReturn)
                {
                    UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
                    op.text = pa.Name;
                    dropDown.options.Add(op);
                }

                if (dropDown.options.Count > 0)
                {
                    dropDown.value = 0; GameObject.Find("Canvas/Login/GoButton").GetComponent<Button>().interactable = true;
                }
            }));
        }));
    }

    [UsedImplicitly]
    public void GoButton_Clicked()
    {
        var dropDown = GameObject.Find("Canvas/Login/MyGamesList").GetComponent<Dropdown>();
        SceneTransporter.CurrentGame = SceneTransporter.LastListedGames[dropDown.value];
        SceneManager.LoadScene("Scene/BoardScene-PC");
    }

    #endregion

    #region 测试CardNormal和CardSmall绘制的相关代码

    private void LoadTestCards()
    {

        TestCards = new List<CardInfo>
        {
            new CardInfo
            {
                CardName =
                    "高效升级",
                InternalId = "Internal",
                Description = "升级一座农场、矿山或者城市建筑物，少花费3[R]。",
                CardType = CardType.Action,
                CardAge = Age.II
            },
            new CardInfo
            {
                CardName =
                    "宗教图书馆",
                InternalId = "Internal",
                CardType = CardType.UrbanTechTheater,
                CardAge = Age.III,
                ResearchCost = {5},
                BuildCost = {6},
                SustainedEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.CultureIncrement,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.ScienceIncrement,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.HappyFace,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    }
                }
            },

            new CardInfo
            {
                CardName =
                    "图书管理员专权",
                InternalId = "Internal",
                CardType = CardType.Government,
                CardAge = Age.III,
                ResearchCost = {25,3},

                SustainedEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.WhiteMarker,7},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.RedMarkerMax,5},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.UrbanLimit,5},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.CultureIncrement,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.ScienceIncrement,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.HappyFace,-2},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    }
                }
            },
            new CardInfo
            {
                CardName =
                    "灌溉生铁",
                InternalId = "Internal",
                CardType = CardType.ResourceTechFarm,
                CardAge = Age.I,
                ResearchCost = {8},
                BuildCost = {10},
                SustainedEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        Data=new List<int>  {(int)ResourceType.ResourceIncrement,2},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int>  {(int)ResourceType.FoodIncrement,2},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                }
            },
            new CardInfo
            {
                CardName =
                    "马克西米连·罗伯斯庇尔",
                InternalId = "Internal",
                CardType = CardType.Leader,
                CardAge = Age.III,
                Description = "你的每座实验室根据等级生产[R]，领袖离开游戏或者游戏结束时，你的每个实验室的每个等级计1[C]。",
            },
            new CardInfo
            {
                CardName =
                    "终极狂野金字塔",
                InternalId = "Internal",
                CardType = CardType.Wonder,
                CardAge = Age.A,
                BuildCost = {1,2,3},
                Description = "你的红点和白点可以互相替换使用。但是每次替换使用时扣1[C]。",
                SustainedEffects = new List<CardEffect>
                {
                     new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.WhiteMarker,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.RedMarkerMax,3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                        Data=new List<int> {(int)ResourceType.MilitaryForce,10},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    }
                }
            },
            new CardInfo
            {
                CardName =
                    "五九改",
                InternalId = "Internal",
                CardType = CardType.MilitaryTechCavalry,
                CardAge = Age.III,
                ResearchCost = {15},
                BuildCost = {10},
                SustainedEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                       Data=new List<int> {(int)ResourceType.MilitaryForce,10},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    }
                },
                NormalImage = "SpriteTile/Dev/59下山了",
            },new CardInfo
            {
                CardName =
                    "工程学",
                InternalId = "Internal",
                CardType = CardType.SpecialTechEngineering,
                CardAge = Age.III,
                ResearchCost = {15},
                Description = @"你可以用一次行动建造奇迹的4个阶段。并且你的城市建筑物：
　等级I少消耗1[R]
　等级II少消耗2[R]
　等级III少消耗3[R]",
            },
            new CardInfo
            {
                CardName =
                    "现代化",
                InternalId = "Internal",
                CardType = CardType.Tactic,
                CardAge = Age.III,
                TacticValue = {10,5},
                TacticComposition = {1,1,2,3}
            },
            new CardInfo
            {
                CardName =
                    "弱者的掀桌",
                InternalId = "Internal",
                CardType = CardType.Event,
                CardAge = Age.III,
                Description = "若你是文化分最弱的文明，你可以选择掀翻任意一名玩家的面板然后与该玩家共同输掉比赛。",
            },
            new CardInfo
            {
                CardName =
                    "受诅咒的土地",
                InternalId = "Internal",
                CardType = CardType.Colony,
                CardAge = Age.I,
                ImmediateEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                       Data=new List<int> {(int)ResourceType.Resource, -5},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                },
                SustainedEffects = new List<CardEffect>
                {
                    new CardEffect
                    {
                       Data=new List<int> {(int)ResourceType.FoodIncrement,-2},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                       Data=new List<int> {(int)ResourceType.CultureIncrement,10},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    },
                    new CardEffect
                    {
                       Data=new List<int> {(int)ResourceType.YellowMarker,-3},
                        FunctionId =  CardEffectType.ResourceOfTypeXChangedY
                    }
                },
            },
            new CardInfo
            {
                CardName =
                    "抱大腿",
                InternalId = "Internal",
                CardType = CardType.Pact,
                CardAge = Age.A,
                Description = "在接下来的游戏中，A玩家每回合产出的分数全部交给B玩家。若B玩家获胜，则A玩家一同获胜",
            },
            new CardInfo
            {
                CardName =
                    "人品战争",
                InternalId = "Internal",
                CardType = CardType.War,
                CardAge = Age.III,
                RedMarkerCost = {1},
                Description = "胜利方每超过失败方8点军力，便可立即从内政牌堆或军事牌堆中将一张指定牌加入自己的手牌。",

            },
            new CardInfo
            {
                CardName =
                    "垃圾倾倒",
                InternalId = "Internal",
                CardType = CardType.Aggression,
                CardAge = Age.III,
                RedMarkerCost = {2},
                Description = "获得对方的粮食，数值相当于你每回合消费的4倍。",
            },

            new CardInfo
            {
                CardName =
                    "防御殖民卡",
                InternalId = "Internal",
                CardType = CardType.Defend,
                CardAge = Age.III,
                Description = "获得对方的粮食，数值相当于你每回合消费的4倍。",
            },
        };

        var dropDown = GameObject.Find("Canvas/CardSelect").GetComponent<Dropdown>();
        dropDown.options = new List<Dropdown.OptionData>();
        foreach (var ca in TestCards)
        {
            UnityEngine.UI.Dropdown.OptionData op = new Dropdown.OptionData();
            op.text = ca.CardName;
            dropDown.options.Add(op);
        }


        CardSmall.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[0]);
        CardNormal.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[0]);
    }

    [UsedImplicitly]
    public void CardSelect_ValueChanged()
    {
        var dropDown = GameObject.Find("Canvas/CardSelect").GetComponent<Dropdown>();
        CardSmall.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[dropDown.value]);
        CardNormal.GetComponent<PCBoardCardDisplayBehaviour>().Bind(TestCards[dropDown.value]);
    }

    #endregion
}
