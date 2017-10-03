//using System;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.Android;
//using System.ComponentModel;
//using Android.Graphics;
//using Android.Graphics.Drawables;
//using Android.Renderscripts;
//using Android.Support.V4.View;
//using Android.Support.V7.Widget;
//using Android.Views;
//using Android.Widget;
//using Calmo.Forms.Droid.Controls;
//using Button = Xamarin.Forms.Button;

//[assembly: ExportRenderer(typeof(MaterialButton), typeof(MaterialButtonRenderer))]
//namespace Calmo.Forms.Droid.Controls
//{
//    public class MaterialButtonRenderer : Xamarin.Forms.Platform.Android.AppCompat.ButtonRenderer
//    {
//        private static float _density = float.MinValue;

//        private MaterialButton Button
//        {
//            get { return (MaterialButton)Element; }
//        }

//        protected override async void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Button> e)
//        {
//            base.OnElementChanged(e);
//            if (e.NewElement == null)
//                return;

//            var materialButton = (MaterialButton)Element;


//            // we need to reset the StateListAnimator to override the setting of Elevation on touch down and release.
//            Control.StateListAnimator = new Android.Animation.StateListAnimator();

//            // set the elevation manually
//            ViewCompat.SetElevation(this, materialButton.Elevation);
//            ViewCompat.SetElevation(Control, materialButton.Elevation);

//            _density = Resources.DisplayMetrics.Density;

//            var targetButton = this.Control;
//            if (targetButton != null) targetButton.SetOnTouchListener(TouchListener.Instance.Value);

//            if (Element != null)
//                await SetImageSourceAsync(targetButton, this.Button).ConfigureAwait(false);
//        }

//        public override void Draw(Canvas canvas)
//        {
//            var materialButton = (MaterialButton)Element;
//            Control.Elevation = materialButton.Elevation;
//            base.Draw(canvas);
//        }

//        /// <summary>
//        /// Update the elevation when updated from Xamarin.Forms
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
//        {
//            base.OnElementPropertyChanged(sender, e);
//            if (e.PropertyName == MaterialButton.ElevationProperty.PropertyName)
//            {
//                var materialButton = (MaterialButton)Element;
//                ViewCompat.SetElevation(this, materialButton.Elevation);
//                ViewCompat.SetElevation(Control, materialButton.Elevation);
//                UpdateLayout();
//            }
//            else if (e.PropertyName == Xamarin.Forms.Button.ImageProperty.PropertyName)
//                await SetImageSourceAsync(Control, this.Button).ConfigureAwait(false);
//        }

//        protected override void Dispose(bool disposing)
//        {
//            base.Dispose(disposing);
//            if (disposing && Control != null)
//            {
//                Control.Dispose();
//            }
//        }

//        private async Task SetImageSourceAsync(Android.Widget.Button targetButton, MaterialButton model)
//        {
//            if (targetButton == null || targetButton.Handle == IntPtr.Zero || model == null) return;

//            using (var bitmap = await GetBitmapAsync(model.Image).ConfigureAwait(false))
//            {
//                if (bitmap == null)
//                    targetButton.SetCompoundDrawables(null, null, null, null);
//                else
//                {
//                    var drawable = new BitmapDrawable(bitmap);

//                    using (var scaledDrawable = GetScaleDrawable(drawable, GetWidth(model.ImageWidthRequest), GetHeight(model.ImageHeightRequest)))
//                    {
//                        Drawable left = null;
//                        Drawable right = null;
//                        Drawable top = null;
//                        Drawable bottom = null;

//                        var paddingLeft = model.Padding.Left;
//                        var paddingTop = model.Padding.Top;
//                        var paddingRight = model.Padding.Right;
//                        var paddingBottom = model.Padding.Bottom;

//                        if (paddingLeft == paddingTop && paddingLeft == paddingRight && paddingLeft == paddingBottom)
//                        {
//                            int padding = 10; //model.Padding
//                            targetButton.CompoundDrawablePadding = RequestToPixels(padding);
//                        }
                        
//                        switch (model.Orientation)
//                        {
//                            case ImageOrientation.ImageToLeft:
//                                targetButton.LayoutDirection = LayoutDirection.Ltr;
//                                targetButton.Gravity = GravityFlags.CenterVertical;
//                                left = scaledDrawable;
//                                targetButton.CompoundDrawablePadding = RequestToPixels(paddingLeft);
//                                break;
//                            case ImageOrientation.ImageToRight:
//                                targetButton.Gravity = GravityFlags.Right | GravityFlags.CenterVertical;
//                                right = scaledDrawable;

//                                targetButton.CompoundDrawablePadding = RequestToPixels(paddingRight);
//                                break;
//                            case ImageOrientation.ImageOnTop:
//                                targetButton.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;
//                                top = scaledDrawable;
//                                targetButton.CompoundDrawablePadding = RequestToPixels(paddingTop);
//                                break;
//                            case ImageOrientation.ImageOnBottom:
//                                targetButton.Gravity = GravityFlags.Bottom | GravityFlags.CenterHorizontal;
//                                bottom = scaledDrawable;
//                                targetButton.CompoundDrawablePadding = RequestToPixels(paddingBottom);
//                                break;
//                            case ImageOrientation.ImageCentered:
//                                targetButton.Gravity = GravityFlags.Center; // | GravityFlags.Fill;
//                                top = scaledDrawable;
//                                break;
//                        }

//                        targetButton.SetCompoundDrawables(left, top, right, bottom);
//                    }
//                }
//            }
//        }

//        private async Task<Bitmap> GetBitmapAsync(ImageSource source)
//        {
//            var handler = GetHandler(source);
//            var returnValue = (Bitmap)null;

//            if (handler != null)
//                returnValue = await handler.LoadImageAsync(source, Context).ConfigureAwait(false);

//            return returnValue;
//        }

//        private static IImageSourceHandler GetHandler(ImageSource source)
//        {
//            IImageSourceHandler returnValue = null;
//            if (source is UriImageSource)
//                returnValue = new ImageLoaderSourceHandler();
//            else if (source is FileImageSource)
//                returnValue = new FileImageSourceHandler();
//            else if (source is StreamImageSource)
//                returnValue = new StreamImagesourceHandler();

//            return returnValue;
//        }

//        private int GetWidth(int requestedWidth)
//        {
//            const int DefaultWidth = 50;
//            return requestedWidth <= 0 ? DefaultWidth : requestedWidth;
//        }

//        private int GetHeight(int requestedHeight)
//        {
//            const int DefaultHeight = 50;
//            return requestedHeight <= 0 ? DefaultHeight : requestedHeight;
//        }

//        private Drawable GetScaleDrawable(Drawable drawable, int width, int height)
//        {
//            var returnValue = new ScaleDrawable(drawable, 0, 100, 100).Drawable;

//            returnValue.SetBounds(0, 0, RequestToPixels(width), RequestToPixels(height));

//            return returnValue;
//        }

//        public int RequestToPixels(double sizeRequest)
//        {
//            if (_density == float.MinValue)
//            {
//                if (Resources.Handle == IntPtr.Zero || Resources.DisplayMetrics.Handle == IntPtr.Zero)
//                    _density = 1.0f;
//                else
//                    _density = Resources.DisplayMetrics.Density;
//            }

//            return (int)(sizeRequest * _density);
//        }
//    }

//    class TouchListener : Java.Lang.Object, Android.Views.View.IOnTouchListener
//    {
//        public static readonly Lazy<TouchListener> Instance = new Lazy<TouchListener>(() => new TouchListener());
        
//        private TouchListener()
//        { }

//        public bool OnTouch(Android.Views.View v, MotionEvent e)
//        {
//            var buttonRenderer = v.Tag as ButtonRenderer;
//            if (buttonRenderer != null && e.Action == MotionEventActions.Down) buttonRenderer.Control.Text = buttonRenderer.Element.Text;

//            return false;
//        }
//    }
//}