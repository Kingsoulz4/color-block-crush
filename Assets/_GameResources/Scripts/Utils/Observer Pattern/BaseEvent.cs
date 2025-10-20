
using System;

namespace Yoolax.Framework
{
    public class BaseEvent : IBaseEvent
    {
        public Action _callback;

        public void AddListener(Action handler)
        {
            _callback += handler;
        }
        public void RemoveListener(Action handler)
        {
            _callback -= handler;
        }
        public void Dispatch()
        {
            if (_callback != null)
            {
                _callback();
            }
        }
    }
    public class BaseEvent<T> : IBaseEvent
    {
        public Action<T> _callback;

        public void AddListener(Action<T> handler)
        {
            _callback += handler;
        }
        public void RemoveListener(Action<T> handler)
        {
            _callback -= handler;
        }
        public void Dispatch(T t)
        {
            if (_callback != null)
            {
                _callback(t);
            }
        }
    }
    public class BaseEvent<T, U> : IBaseEvent
    {
        public Action<T, U> _callback;

        public void AddListener(Action<T, U> handler)
        {
            _callback += handler;
        }
        public void RemoveListener(Action<T, U> handler)
        {
            _callback -= handler;
        }
        public void Dispatch(T t, U u)
        {
            if (_callback != null)
            {
                _callback(t, u);
            }
        }
    }
    public class BaseEvent<T, U, V> : IBaseEvent
    {
        public Action<T, U, V> _callback;

        public void AddListener(Action<T, U, V> handler)
        {
            _callback += handler;
        }
        public void RemoveListener(Action<T, U, V> handler)
        {
            _callback -= handler;
        }
        public void Dispatch(T t, U u, V v)
        {
            if (_callback != null)
            {
                _callback(t, u, v);
            }
        }
    }
    public class BaseEvent<T, U, V, X> : IBaseEvent
    {
        public Action<T, U, V, X> _callback;

        public void AddListener(Action<T, U, V, X> handler)
        {
            _callback += handler;
        }
        public void RemoveListener(Action<T, U, V, X> handler)
        {
            _callback -= handler;
        }
        public void Dispatch(T t, U u, V v, X x)
        {
            if (_callback != null)
            {
                _callback(t, u, v, x);
            }
        }
    }
}
