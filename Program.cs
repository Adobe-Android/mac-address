using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Linq;
using System.Text;

namespace MacAddress
{
    class Program
    {
        static void Main()
        {
            Console.WriteLine(GetDefaultMacAddress());
        }

        /// <summary>
        /// Finds the MAC address of the NIC with the most total usage.
        /// </summary>
        /// <returns>A MAC address.</returns>
        public static string GetDefaultMacAddress()
        {
            // Check online status before calling the following MAC address code
            if (!IsOnline()) return "Are you connected to the internet?";
            long maxValue = 0;
            string macAddress = string.Empty;

            Dictionary<string, long> macAddressesDict = new Dictionary<string, long>();
            foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces()
                // An attempt to help filter out virtual network devices
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                nic.Description.StartsWith("VMware") == false && nic.Description.StartsWith("Hyper-V") == false && nic.Description.StartsWith("VirtualBox") == false))
            {
                // Gets total usage of a given NIC
                macAddressesDict[nic.GetPhysicalAddress().ToString()] = nic.GetIPStatistics().BytesSent + nic.GetIPStatistics().BytesReceived;
            }

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

        /// <summary>
        /// Checks online status via network ping.
        /// </summary>
        /// <returns>Ping reply status.</returns>
        public static bool IsOnline()
        {
            // Disconnect from your network or use "www.contoso.com" to test offline status
            //string host = "www.contoso.com";
            string host = "www.google.com";
            string data = string.Empty;
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            int timeout = 5_000;
            PingOptions options = new PingOptions(64, true);
            bool result = false;
            Ping ping = new Ping();

            try
            {
                PingReply reply = ping.Send(host, timeout, buffer, options);
                if (reply.Status == IPStatus.Success)
                    return true;
                else
                    Console.WriteLine(reply.Status);
            }
            catch (PingException)
            {
                // Be sure not to include http:// or https:// in the host URL
                throw new PingException("Invalid or poorly formed host URL.");
            }
            catch
            {
                throw;
            }

            return result;
        }
    }
}
