using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.CSharpCode.Entity;
using Assets.CSharpCode.UI.PCBoardScene;
using JetBrains.Annotations;

namespace Assets.CSharpCode.UI.Util
{
    [Obsolete]
    public interface TtaActionBinder
    {
        /// <summary>
        /// 除非特殊需要，否则在执行Bind之前都要考虑执行一个Unbind
        /// </summary>
        void BindAction(List<PlayerAction> actions, PCBoardBehavior boardBehavior);

        /// <summary>
        /// <para>BoardBehavior在下列情况会考虑执行最高层的Unbind</para>
        ///  <para>1、玩家切换到他人面板</para>
        /// 2、Scene要被销毁<para/>
        /// 3、玩家面板被刷新<para/>
        /// 在下列情况下不会执行最高层的Unbind<para/>
        /// 1、写入了InterAction（然后会立刻执行一个Bind但是不会先Unbind，并且不会refresh board）<para/>
        /// 具体会不会执行Unbind取决于你的父级ActionBinder有没有在他的Unbind里调用你的Unbind<para/>
        /// </summary>
        [UsedImplicitly]
        void Unbind();
    }
}
