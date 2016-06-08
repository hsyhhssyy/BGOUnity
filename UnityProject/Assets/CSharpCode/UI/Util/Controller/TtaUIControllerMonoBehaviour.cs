using System;
using System.Diagnostics.CodeAnalysis;
using Assets.CSharpCode.Managers;
using Assets.CSharpCode.UI.Util.Input;

namespace Assets.CSharpCode.UI.Util.Controller
{
    public abstract class TtaUIControllerMonoBehaviour: InputActionTriggerMonoBehaviour
    {
        public virtual String UIKey { get; protected set; }

        protected String Guid { get; private set; }

        private GameEventChannel _channel;
        public GameEventChannel Channel
        {
            get { return _channel;}
            set
            {
                _channel = value;
                _channel.GameEvent += DestoryNullReference;
            }
        }

        [SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
        private void DestoryNullReference(System.Object sender, GameUIEventArgs args)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (this == null)
            {
                _channel.GameEvent -= DestoryNullReference;
            }
            else
            {
                OnSubscribedGameEvents(sender, args);
            }
        }

        protected abstract void OnSubscribedGameEvents(System.Object sender, GameUIEventArgs args);
        
        protected TtaUIControllerMonoBehaviour()
        {
            Guid= System.Guid.NewGuid().ToString();
        }
        
    }
}
