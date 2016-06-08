using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.CSharpCode.Managers
{
    public class GameEventChannel
    {
        public event EventHandler<GameUIEventArgs> GameEvent;

        private Queue<GameUIEventArgs> EventQueue=new Queue<GameUIEventArgs>();

        public void Broadcast(GameUIEventArgs args)
        {
            EventQueue.Enqueue(args);
        }

        public void Flush()
        {
            while (EventQueue.Count > 0)
            {
                var args = EventQueue.Dequeue();
                if (GameEvent != null)
                {
                    GameEvent(this, args);
                }
            }
        }
    }
}
