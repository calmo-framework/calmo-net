using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ResourceIT.Forms.Controls.VideoPlayer.Diagnostics;
using ResourceIT.Forms.Controls.VideoPlayer.ExtensionMethods;
using Xamarin.Forms;

namespace ResourceIT.Forms.Controls.VideoPlayer
{
    [TypeConverter(typeof(VideoSourceConverter))]
    public abstract class VideoSource : Element, IEquatable<VideoSource>
    {
        private CancellationTokenSource _cancellationTokenSource;
        private TaskCompletionSource<bool> _completionSource;
        private readonly object _syncHandle;
        internal EventHandler SourceChanged;

        internal VideoSource()
        {
            this._syncHandle = new object();
        }

        public virtual Task<bool> Cancel()
        {
            if (!this.IsLoading)
                return Task.FromResult<bool>(false);

            TaskCompletionSource<bool> source = new TaskCompletionSource<bool>();
            TaskCompletionSource<bool> source2 = Interlocked.CompareExchange<TaskCompletionSource<bool>>(ref this._completionSource, source, null);
            if (source2 == null)
            {
                this._cancellationTokenSource.Cancel();
            }
            else
            {
                source = source2;
            }
            return source.Task;
        }


        public abstract bool Equals(VideoSource other);

        public static VideoSource FromFile(string file) => new FileVideoSource { File = file };

        public static VideoSource FromResource(string resource, Assembly assembly = null)
        {
            if (!string.IsNullOrEmpty(resource))
            {
                if (!Path.HasExtension(resource))
                {
                    object[] objArray1 = new object[] { resource };
                    throw new Exception($"The specified resource '{(object[])objArray1}' must contain a valid file extension.");
                }
                bool flag = false;
                string format = Path.GetExtension(resource).Replace((Path.GetExtension(resource) == null) ? null : ".", string.Empty);
                Assembly assembly2 = null;
                Assembly assembly3 = null;
                Assembly assembly4 = null;
                if (assembly != null)
                {
                    assembly2 = assembly;
                    flag = assembly.ContainsManifestResource(resource);
                }
                if (!flag)
                {
                    assembly = (Assembly)IntrospectionExtensions.GetTypeInfo((Type)typeof(Assembly)).GetDeclaredMethod("GetCallingAssembly").Invoke(null, new object[0]);
                    assembly3 = assembly;
                    flag = assembly.ContainsManifestResource(resource);
                }
                if (!flag)
                {
                    assembly = (Assembly)IntrospectionExtensions.GetTypeInfo((Type)typeof(Assembly)).GetDeclaredMethod("GetEntryAssembly").Invoke(null, new object[0]);
                    assembly4 = assembly;
                    flag = assembly.ContainsManifestResource(resource);
                }
                if (flag)
                {
                    return FromStream(delegate {
                        return assembly.GetEmbeddedResourceStream(resource);
                    }, format);
                }
                List<string> list = new List<string>();
                if (assembly2 != null)
                {
                    list.AddRange(assembly2.GetManifestResourceNames());
                }
                if (assembly3 != null)
                {
                    list.AddRange(assembly3.GetManifestResourceNames());
                }
                if (assembly4 != null)
                {
                    list.AddRange(assembly4.GetManifestResourceNames());
                }
                object[] objArray2 = new object[] { resource };
                object[] objArray3 = new object[] { Environment.NewLine, string.Join(Environment.NewLine, (IEnumerable<string>)list) };
                Log.Error($"Unable to locate the embedded resource '{((object[])objArray2)}'. " + string.Format("Possible candidates are: {0}{1}", (object[])objArray3));
            }
            return null;
        }


        public static VideoSource FromStream(Func<Stream> stream, string format) =>
            new StreamVideoSource
            {
                Format = format,
                Stream = delegate(CancellationToken token) { return Task.Run<Stream>(stream, token); }
            };


        public static VideoSource FromUri(string uri) => FromUri(new Uri(uri));


        public static VideoSource FromUri(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("uri is relative");
            }
            return new UriVideoSource { Uri = uri };
        }


        protected void OnLoadingCompleted(bool cancelled)
        {
            throw new NotImplementedException();
        }

        protected void OnLoadingStarted()
        {
            object obj2 = this._syncHandle;
            lock (obj2)
            {
                this.CancellationTokenSource = new CancellationTokenSource();
            }
        }


        protected void OnSourceChanged()
        {
            this.SourceChanged.RaiseEvent(this, null);
        }


        public static implicit operator VideoSource(string source)
        {
            Uri uri;
            if (Uri.TryCreate(source, (UriKind)UriKind.Absolute, out uri) && (uri.Scheme != "file"))
            {
                return FromUri(uri);
            }
            return FromFile(source);
        }

        public static implicit operator VideoSource(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                throw new ArgumentException("uri is relative");
            }
            return FromUri(uri);
        }

        protected CancellationTokenSource CancellationTokenSource
        {
            get { return this._cancellationTokenSource; }
            private set
            {
                if (this._cancellationTokenSource != value)
                {
                    if (this._cancellationTokenSource != null)
                    {
                        this._cancellationTokenSource.Cancel();
                    }
                    else
                    {
                        CancellationTokenSource expressionStack_13_0 = this._cancellationTokenSource;
                    }
                    this._cancellationTokenSource = value;
                }
            }
        }
        private bool IsLoading => (this._cancellationTokenSource != null);
    }
}

