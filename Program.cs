using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;

namespace MacAddress
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(GetDefaultMacAddress());
        }

        /// <summary>
        /// Finds the MAC address of the NIC with the most total usage.
        /// </summary>
        /// <returns>A MAC address.</returns>
        public static string GetDefaultMacAddress()
        {
            Dictionary<string, long> macAddressesDict = new Dictionary<string, long>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()
                // An attempt to help filter out virtual network devices
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback && nic.Description.StartsWith("VMware") == false && nic.Description.StartsWith("Hyper-V") == false && nic.Description.StartsWith("VirtualBox") == false))
            {
                // Gets total usage of a given NIC
                macAddressesDict[nic.GetPhysicalAddress().ToString()] = nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
            }

            long maxValue = 0;
            string macAddress = string.Empty;
            foreach (KeyValuePair<string, long> pair in macAddressesDict)
            {
                if (pair.Value > maxValue)
                {
                    macAddress = pair.Key;
                    maxValue = pair.Value;
                }
            }

            return macAddress;
        }
    }
}
