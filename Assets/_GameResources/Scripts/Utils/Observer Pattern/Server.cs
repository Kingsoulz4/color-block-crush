using System;
using System.Collections.Generic;

namespace Yoolax.Framework
{
    public static class Server
    {
        private static Dictionary<Type, IBaseEvent> eventHubs = new Dictionary<Type, IBaseEvent>();

        public static T Get<T>() where T : IBaseEvent
        {
            Type type = typeof(T);
            IBaseEvent e = null;
            eventHubs.TryGetValue(type, out e);

            if (e == null)
            {
                e = (IBaseEvent)Activator.CreateInstance(type);
                eventHubs.Add(type, e);
            }
            return (T)e;
        }
        public static bool HadListener<T>() where T : IBaseEvent
        {
            Type type = typeof(T);
            IBaseEvent e = null;
            eventHubs.TryGetValue(type, out e);

            if (e == null)
            {
                return false;
            }
            return true;
        }
    }
}