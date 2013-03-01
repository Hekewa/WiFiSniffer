using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// Namespace for WlanApi.
using NativeWifi;

namespace ConsoleApplication2
{
    class Program
    {
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        static void Main(string[] args)
        {
            try
            {
                WlanClient client = new WlanClient();
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt"))

                    while (true)
                    {
                        foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                        {
                            Console.WriteLine(wlanIface.InterfaceDescription);
                            Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                            foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                            {
                                Console.WriteLine(GetStringForSSID(bssEntry.dot11Ssid));
                                //Console.WriteLine(Encoding.ASCII.GetString(bssEntry.dot11Bssid, 0, 6));
                                byte[] macAddr = bssEntry.dot11Bssid;
                                var macAddrLen = (uint)macAddr.Length;
                                var str = new string[(int)macAddrLen];
                                for (int i = 0; i < macAddrLen; i++)
                                {
                                    str[i] = macAddr[i].ToString("x2");
                                }
                                string mac = string.Join("", str);
                                Console.WriteLine(mac);
                                outputfile.WriteLine(mac);
                            }
                            Console.WriteLine("Press enter to scan again");
                            Console.ReadLine();

                        }
                    }
            }

            catch
            {
                Console.WriteLine("Error: Wlan not working properly");
                while (true) { }
            }
        }
    }
}
            
      
    


// Wlan.WlanBssEntry[] entry = wlanIface.GetNetworkBssList();


/*
foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
{
    Console.WriteLine(wlanIface.InterfaceDescription);
    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
    foreach (Wlan.WlanAvailableNetwork network in networks)
    {
        Console.WriteLine(GetStringForSSID(network.dot11Ssid)
            + "\t RSSI strength: {0}", network.wlanSignalQuality);
    }
}
*/
