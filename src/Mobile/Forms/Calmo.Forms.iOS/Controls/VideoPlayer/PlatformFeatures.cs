using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using Foundation;

namespace ResourceIT.Forms.iOS.Controls.VideoPlayer
{
    internal class PlatformFeatures : ResourceIT.Forms.Controls.VideoPlayer.Interfaces.IPlatformFeatures
    {
        public void Exit()
        {
            NSThread.MainThread.Cancel();
        }

        public string HashSha1(string value)
        {
            byte[] buffer = new SHA1Managed().ComputeHash(Encoding.UTF8.GetBytes(value));
            return string.Join(string.Empty, (from b in buffer select b.ToString("x2")).ToArray<string>());
        }

        public string PackageName => NSBundle.MainBundle.BundleIdentifier;
    }
}

