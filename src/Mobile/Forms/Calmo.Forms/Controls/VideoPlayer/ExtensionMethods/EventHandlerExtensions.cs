namespace ResourceIT.Forms.Controls.VideoPlayer.ExtensionMethods
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    internal static class EventHandlerExtensions
    {
        public static void RaiseEvent(this EventHandler eventHandler, object sender, EventArgs eventArgs = null)
        {
            if (eventHandler != null)
            {
                eventHandler(sender, eventArgs ?? EventArgs.Empty);
            }
        }

        public static void RaiseEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs eventArgs = null) where TEventArgs: EventArgs
        {
            if (eventHandler != null)
            {
                if (eventArgs == null)
                {
                }
                eventHandler(sender, EventArgs.Empty as TEventArgs);
            }
        }
    }
}

