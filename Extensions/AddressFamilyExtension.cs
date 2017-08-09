using System;
using System.Net;

namespace Nomad.Extensions
{
    public static class AddressFamilyExtension
    {
        public static String ConvertToFriendlyAddress(this string clientIp)
        {
            IPAddress ipAddress;
            var address = String.Empty;

            if (IPAddress.TryParse(clientIp, out ipAddress))
            {
                switch (ipAddress.AddressFamily)
                {
                    case System.Net.Sockets.AddressFamily.InterNetwork:
                        address = "http://" + clientIp + ":4646";
                        break;
                    case System.Net.Sockets.AddressFamily.InterNetworkV6:
                        address = "http://[" + clientIp + "]:4646";
                        break;
                    default:
                        throw new Exception();
                }
            }

            return address;
        }
    }
}
