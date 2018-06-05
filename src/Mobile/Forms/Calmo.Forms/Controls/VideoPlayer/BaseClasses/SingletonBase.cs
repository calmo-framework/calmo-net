namespace ResourceIT.Forms.Controls.VideoPlayer.BaseClasses
{
    using System;
    using System.Diagnostics;

    [DebuggerStepThrough]
    public abstract class SingletonBase<T> where T: new()
    {
        protected static readonly object _ConcurrencyLock;
        private static T _Current;

        static SingletonBase()
        {
            SingletonBase<T>._ConcurrencyLock = new object();
        }

        protected SingletonBase()
        {
        }

        public static T Current
        {
            get
            {
                if (SingletonBase<T>._Current == null)
                {
                    object obj2 = SingletonBase<T>._ConcurrencyLock;
                    lock (obj2)
                    {
                        if (SingletonBase<T>._Current == null)
                        {
                            SingletonBase<T>._Current = Activator.CreateInstance<T>();
                        }
                    }
                }
                return SingletonBase<T>._Current;
            }
            set
            {
                object obj2 = SingletonBase<T>._ConcurrencyLock;
                lock (obj2)
                {
                    SingletonBase<T>._Current = value;
                }
            }
        }
    }
}

