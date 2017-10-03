using System;
using System.Collections.Generic;

namespace Calmo.WindowsServices
{
    /// <summary>
    /// 
    /// </summary>
    public class AsyncThreadCaller
    {
        public delegate void AsyncDelegate(Dictionary<string, object> parameters);
        public delegate void AsyncDelegateCallBack();

        private event AsyncDelegate methodCall;
        private event AsyncDelegateCallBack methodCallback;

        public event AsyncDelegate MethodCall
        {
            add { methodCall += value; }
            remove { methodCall -= value; }
        }

        public event AsyncDelegateCallBack MethodCallback
        {
            add { methodCallback += value; }
            remove { methodCallback -= value; }
        }

        public void Start(Dictionary<string, object> parameters)
        {
            AsyncDelegate asyncDelegate = new AsyncDelegate(this.OnMethodCall);

            IAsyncResult asyncResult = asyncDelegate.BeginInvoke(parameters, new AsyncCallback(this.OnMethodCallback), asyncDelegate);
        }

        internal void OnMethodCall(Dictionary<string, object> parameters)
        {
            if (methodCall != null)
                methodCall(parameters);
            else
                throw new MissingMethodException("AsyncThreadCaller", "MethodCall");
        }

        internal void OnMethodCallback(IAsyncResult asyncResult)
        {
            AsyncDelegate asyncDelegate = (AsyncDelegate)asyncResult.AsyncState;

            asyncDelegate.EndInvoke(asyncResult);

            if (methodCallback != null)
                methodCallback();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TThreadReturn"></typeparam>
    public class AsyncThreadCaller<TThreadReturn>
    {
        public delegate TThreadReturn AsyncDelegate(Dictionary<string, object> parameters);
        public delegate void AsyncDelegateCallBack(TThreadReturn returnValue);

        private event AsyncDelegate methodCall;
        private event AsyncDelegateCallBack methodCallback;

        public event AsyncDelegate MethodCall
        {
            add { methodCall += value; }
            remove { methodCall -= value; }
        }

        public event AsyncDelegateCallBack MethodCallback
        {
            add { methodCallback += value; }
            remove { methodCallback -= value; }
        }

        public void Start(Dictionary<string, object> parameters)
        {
            AsyncDelegate asyncDelegate = new AsyncDelegate(this.OnMethodCall);

            IAsyncResult asyncResult = asyncDelegate.BeginInvoke(parameters, new AsyncCallback(this.OnMethodCallback), asyncDelegate);
        }

        internal TThreadReturn OnMethodCall(Dictionary<string, object> parameters)
        {
            if (methodCall != null)
                return methodCall(parameters);
            else
                throw new MissingMethodException("AsyncThreadCaller", "MethodCall");
        }

        internal void OnMethodCallback(IAsyncResult asyncResult)
        {
            AsyncDelegate asyncDelegate = (AsyncDelegate)asyncResult.AsyncState;

            TThreadReturn returnValue = asyncDelegate.EndInvoke(asyncResult);

            if (methodCallback != null)
                methodCallback(returnValue);
        }
    }
}
