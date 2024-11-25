using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace BL.Services.Common
{
    public class NetworkHelper
    {
        public static string GetServerMacAddress()
        {
            try
            {
                // Get all network interfaces
                var networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();

                foreach (var netInterface in networkInterfaces)
                {
                    // Filter out interfaces that are not operational
                    if (netInterface.OperationalStatus == OperationalStatus.Up &&
                        (netInterface.NetworkInterfaceType == NetworkInterfaceType.Ethernet ||
                         netInterface.NetworkInterfaceType == NetworkInterfaceType.Wireless80211))
                    {
                        var macAddress = netInterface.GetPhysicalAddress();
                        return macAddress.ToString(); // Return MAC address as a string
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }

            return "";
        }

    }
}
