using System;
using Xamarin.Forms;

namespace Calmo.Forms
{
    public class MenuItem
    {
        public string Text { get; set; }
        public string Icon { get; set; }
        public FontAwesomeIcons FontAwesomeIcon { get; set; }
        public Type Page { get; set; }

        public event EventHandler Tap;
        public virtual void OnTap()
        {
            this.Tap?.Invoke(this, EventArgs.Empty);
        }
    }
}
