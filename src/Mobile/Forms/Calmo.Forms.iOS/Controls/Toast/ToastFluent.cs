using CoreGraphics;
using Foundation;
using System;
using UIKit;

namespace Xamarin.Forms
{
    internal class ToastFluent
    {
        private const long CURRENT_TOAST_TAG = 0x6a93e6L;
        private readonly nfloat TopAndBottomPadding = 14;
        private readonly nfloat LeftAndRightPadding = 24;
        public const int LENGTH_LONG = 3500;
        public const int LENGTH_SHORT = 2000;
        public static ToastSetting settings;
        private string text;
        public UIView view;

        public ToastFluent()
        {

        }

        public ToastFluent(string itext)
        {
            this.text = itext;
        }

        private CGRect FrameForImage(ToastType type, CGRect toastFrame)
        {
            var image = this.theSettings().images[type];
            if (image == null)
                return CGRect.Empty;

            //var empty = CGRect.Empty;
            /*var imageLocation = this.theSettings().imageLocation;
            if (imageLocation != ToastImageLocation.Left)
            {
                if (imageLocation != ToastImageLocation.Top)
                    return empty;
            }
            else*/
                return new CGRect(this.LeftAndRightPadding, (toastFrame.Size.Height - image.Size.Height) / 2, image.Size.Width, image.Size.Height);
            
            //return new CGRect((toastFrame.Size.Width - image.Size.Width) / 2, this.LeftAndRightPadding, image.Size.Width, image.Size.Height);
        }

        private void HideToast()
        {
            UIView.BeginAnimations(null, IntPtr.Zero);
            this.view.Alpha = 0;
            UIView.CommitAnimations();
            var timer = NSTimer.CreateTimer(TimeSpan.FromSeconds(0.5), t => this.HideToast());
            NSRunLoop.Main.AddTimer(timer, 0);
        }

        private void HideToastEventHandler(object sender, EventArgs e)
        {
            this.HideToast();
        }

        public void MakeNewSetting()
        {
            ToastSetting.MakeNewSetting();
        }

        public static ToastFluent MakeText(string text)
        {
            var toast = new ToastFluent(text);
            toast.MakeNewSetting();
            return toast;
        }

        public static ToastFluent MakeText(string text, nint duration) => 
            MakeText(text).SetDuration(duration);

        private void RemoveToast()
        {
            this.view.RemoveFromSuperview();
        }

        public ToastFluent SetBgAlpha(nfloat bgAlpha)
        {
            this.theSettings().bgAlpha = bgAlpha;
            return this;
        }

        public ToastFluent SetBgBlue(nfloat bgBlue)
        {
            this.theSettings().bgBlue = bgBlue;
            return this;
        }

        public ToastFluent SetBgGreen(nfloat bgGreen)
        {
            this.theSettings().bgGreen = bgGreen;
            return this;
        }

        public ToastFluent SetBgRed(nfloat bgRed)
        {
            this.theSettings().bgRed = bgRed;
            return this;
        }

        public ToastFluent SetCornerRadius(nfloat cornerRadius)
        {
            this.theSettings().cornerRadius = cornerRadius;
            return this;
        }

        public ToastFluent SetDuration(nint duration)
        {
            this.theSettings().duration = duration;
            return this;
        }

        public ToastFluent SetFontSize(nfloat fontSize)
        {
            this.theSettings().fontSize = fontSize;
            return this;
        }

        public ToastFluent SetGravity(ToastGravity gravity)
        {
            this.theSettings().gravity = gravity;
            return this;
        }

        public ToastFluent SetGravity(ToastGravity gravity, nint left, nint top)
        {
            this.theSettings().gravity = gravity;
            this.theSettings().offsetLeft = left;
            this.theSettings().offsetTop = top;
            return this;
        }

        public ToastFluent SetPosition(CGPoint position)
        {
            this.theSettings().position = position;
            return this;
        }

        public ToastFluent SetType(ToastType type)
        {
            this.theSettings().toastType = type;
            return this;
        }

        public ToastFluent SetUseShadow(bool useShadow)
        {
            this.theSettings().useShadow = useShadow;
            return this;
        }

        public void Show()
        {
            this.Show(ToastType.Default);
        }

        public void Show(ToastType type)
        {
            var image = this.theSettings().images[type];
            var font = UIFont.SystemFontOfSize(this.theSettings().fontSize);
            var str = new NSAttributedString(this.text, font);
            var textSize = str.GetBoundingRect(new CGSize(260, 50), NSStringDrawingOptions.UsesLineFragmentOrigin, null).Size;
            textSize.Width = textSize.Width + 5;

            var label =
                new UILabel(new CGRect(0, 0, textSize.Width + this.LeftAndRightPadding,
                    textSize.Height + this.TopAndBottomPadding))
                {
                    BackgroundColor = UIColor.Clear,
                    TextColor = UIColor.White,
                    TextAlignment = UITextAlignment.Left,
                    Text = this.text,
                    Lines = 0,
                    Font = UIFont.SystemFontOfSize(this.theSettings().fontSize)
                };
            if (this.theSettings().useShadow)
            {
                label.ShadowColor = UIColor.DarkGray;
                label.ShadowOffset = new CGSize(1, 1);
            }

            var button = new UIButton(UIButtonType.Custom);
            if (image != null)
            {
                button.Frame = this.ToastFrameForImageSize(image.Size, textSize);
                /*switch (this.theSettings().imageLocation)
                {
                    case ToastImageLocation.Left:*/
                        label.TextAlignment = UITextAlignment.Left;
                        label.Center = new CGPoint(image.Size.Width + (this.LeftAndRightPadding * 2) + (((button.Frame.Size.Width - image.Size.Width) - (this.LeftAndRightPadding * 2)) / 2), button.Frame.Size.Height / 2);
                 /*       break;

                    case ToastImageLocation.Top:
                        label.TextAlignment = UITextAlignment.Center;
                        label.Center = new CGPoint(button.Frame.Size.Width / 2, (image.Size.Height + (this.TopAndBottomPadding * 2)) + (((button.Frame.Size.Height - image.Size.Height) - (this.TopAndBottomPadding * 2)) / 2));
                        break;
                }*/
            }
            else
            {
                button.Frame = new CGRect(0, 0, textSize.Width + (this.LeftAndRightPadding * 2), textSize.Height + (this.TopAndBottomPadding * 2));
                label.Center = new CGPoint(button.Frame.Size.Width / 2, button.Frame.Size.Height / 2);
            }

            var rect8 = label.Frame;
            rect8.X = (nfloat) Math.Ceiling((double) rect8.X);
            rect8.Y = (nfloat) Math.Ceiling((double) rect8.Y);
            label.Frame = rect8;
            button.AddSubview(label);

            if (image != null)
            {
                var view = new UIImageView(image);
                view.Frame = this.FrameForImage(type, button.Frame);
                button.AddSubview(view);
            }

            button.BackgroundColor = UIColor.FromRGBA(this.theSettings().bgRed, this.theSettings().bgGreen, this.theSettings().bgBlue, this.theSettings().bgAlpha);
            button.Layer.CornerRadius = this.theSettings().cornerRadius;

            var window = UIApplication.SharedApplication.Windows[0];
            var empty = CGPoint.Empty;
            var orientation = UIApplication.SharedApplication.StatusBarOrientation;
            switch (this.theSettings().gravity)
            {
                case ToastGravity.Top:
                    empty = new CGPoint(window.Frame.Size.Width / 2, 0x2d);
                    break;

                case ToastGravity.Bottom:
                    empty = new CGPoint(window.Frame.Size.Width / 2, window.Frame.Size.Height - 0x2d);
                    break;

                case ToastGravity.Center:
                    empty = new CGPoint(window.Frame.Size.Width / 2, window.Frame.Size.Height / 2);
                    break;
            }

            empty = new CGPoint(empty.X + this.theSettings().offsetLeft, empty.Y + this.theSettings().offsetTop);
            button.Center = empty;
            button.Frame = RectangleFExtensions.Integral(button.Frame);

            var timer = NSTimer.CreateTimer(TimeSpan.FromSeconds(this.theSettings().duration / 1000), t => this.HideToast());
            NSRunLoop.Main.AddTimer(timer, 0);
            button.Tag = (nint)CURRENT_TOAST_TAG;

            var view2 = window.ViewWithTag((nint)CURRENT_TOAST_TAG);
            if (view2 != null)
                view2.RemoveFromSuperview();
            
            button.Alpha = 0;
            window.AddSubview(button);
            UIView.BeginAnimations(null, IntPtr.Zero);
            button.Alpha = 1;
            UIView.CommitAnimations();
            this.view = button;
            button.AddTarget(new EventHandler(this.HideToastEventHandler), UIControlEvent.TouchDown);
            ToastSetting.SharedSettings = null;
        }

        public ToastSetting theSettings() =>  ToastSetting.GetSharedSettings();

        private CGRect ToastFrameForImageSize(CGSize imageSize, CGSize textSize)
        {
            /*
            var empty = CGRect.Empty;
            if (location != ToastImageLocation.Left)
            {
                if (location != ToastImageLocation.Top)
                    return empty;
            }
            else*/
                return new CGRect(0, 0, (imageSize.Width + textSize.Width) + (this.LeftAndRightPadding * 3), ((nfloat) Math.Max((double) textSize.Height, (double) imageSize.Height)) + (this.TopAndBottomPadding * 2));
            
            //return new CGRect(0, 0, ((nfloat) Math.Max((double) textSize.Width, (double) imageSize.Width)) + (this.LeftAndRightPadding * 2), (imageSize.Height + textSize.Height) + (this.TopAndBottomPadding * 3));
        }
    }
}

