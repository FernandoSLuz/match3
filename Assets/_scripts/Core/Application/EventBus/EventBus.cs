using System;
using System.Collections.Generic;

namespace MatchTree.Core.Application.EventBus
{
    public class EventBus
    {
        private readonly Dictionary<Type, List<Action<IGameEvent>>> handlers = new Dictionary<Type, List<Action<IGameEvent>>>();

        public void Publish<T>(T gameEvent) where T : IGameEvent
        {
            var type = typeof(T);
            if (handlers.TryGetValue(type, out var list))
            {
                foreach (var h in list)
                {
                    h(gameEvent);
                }
            }
        }

        public void Subscribe<T>(Action<T> handler) where T : IGameEvent
        {
            var type = typeof(T);
            if (!handlers.TryGetValue(type, out var list))
            {
                list = new List<Action<IGameEvent>>();
                handlers[type] = list;
            }
            list.Add(e => handler((T)e));
        }
    }
}


