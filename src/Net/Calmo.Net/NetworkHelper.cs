using System;
using System.Net.NetworkInformation;

namespace Calmo.Net
{
    public class NetworkHelper
    {
        public static string GetMACAddress()
        {
            var nics = NetworkInterface.GetAllNetworkInterfaces();
            var macAddress = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (macAddress == String.Empty)// only return MAC Address from first card  
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    macAddress = adapter.GetPhysicalAddress().ToString();
                }
            } 
            
            return macAddress;
        }
    }
}
