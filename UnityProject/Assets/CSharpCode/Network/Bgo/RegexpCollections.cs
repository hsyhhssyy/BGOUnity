namespace Assets.CSharpCode.Network.Bgo
{
    class RegexpCollections
    {
        //<td rowspan="1" class="tabPartiesTexteC tabPartiesFond\d">(\d*?)</td><td rowspan="1" class="tabPartiesTexteFinG tabPartiesFond\d">([\s\S]*?)<[\s\S]*?nat=(\d)">([\s\S]*?)<
        //这个可以在My games页面匹配游戏列表，1是id，2是版本，3是nat，4是名字

        //(<form method="post" id="piocherCarte\d*?" action="([\S]*?)">)?<input type="hidden" name="idNote" value="([\S]*?)"[\s\S]{0,300}?<a class="[\s\S]*?" onClick="[\s\S]*?">[\s\S]*?<p class="ageCarte ageCarte1x">(\S*?)</p>[\s\S]*?<p class="nomCarte">([\s\S]*?)(<br />)?</p>
        //这个可以匹配卡牌列 Group 2 是 post url, 3 是 idNote ，4是 age， 5 是card name

        //<a onClick="\S*?"><img class="imageLeader" \S*?  alt="([\s\S]*?)"
        //Group 1是当前领袖

        //<div id="Plateau(\d)"( style="\S*?")? class="plateau plateau\d"( style="\S*?")?>
        //寻找玩家面板位置，前面的是玩家面板，后面两个是Journal和Chat

        //>([^>]+?)<ul id="indJoueur">((<li class="ind\S*? ind">[\s\S]*?){7})</ul>
        //1是两个玩家的名字，和他们的面板顺序一致 2是当前资源，用下面的regex来找

        //<li class="ind\S*? ind">[\s\S]*?(</li>)?
        //下面的regex先把这7个切出来，然后再具体分析

        //<div id="statusBar" class="statusActive">([\s\S]*?)</div>
        //这个先把statusbar切出来，然后再分析
    }
}
