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
<<<<<<< HEAD
        // Returns a string for an SSID.
=======
        // Returns a string for an SSID...
>>>>>>> cbe4a83e31c7f5315135687d8b4cfbc4a37fc021
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        // Main function.
        static void Main(string[] args)
        {
            try
            {
                WlanClient client = new WlanClient();
<<<<<<< HEAD
                // Output file to write collected AP data. True means that we append to a existing file.
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt", true))
=======
                // Output file to write collected AP data....
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt"))
>>>>>>> cbe4a83e31c7f5315135687d8b4cfbc4a37fc021

                    for (int j = 0; j < 5; j++ )
                    {
                        Console.Clear();
                        foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                        {
                            // Network adapter vendor and model
                            Console.WriteLine(wlanIface.InterfaceDescription);
                            Console.WriteLine();
                            Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                            foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                            {
                                // SSID
                                Console.Write((GetStringForSSID(bssEntry.dot11Ssid)).PadRight(20));
                                Console.Write("   ");
                                //Console.WriteLine(Encoding.ASCII.GetString(bssEntry.dot11Bssid, 0, 6));
                                // MAC address
                                byte[] macAddr = bssEntry.dot11Bssid;
                                var macAddrLen = (uint)macAddr.Length;
                                var str = new string[(int)macAddrLen];
                                for (int i = 0; i < macAddrLen; i++)
                                {
                                    str[i] = macAddr[i].ToString("x2");
                                }
                                string mac = string.Join("", str);
                                Console.Write(mac.PadRight(20));
                                Console.Write("   ");
                                outputfile.WriteLine(mac);
                                Console.WriteLine(bssEntry.rssi);
                            }
<<<<<<< HEAD
                            //Console.WriteLine("Press enter to scan again");
                            //Console.ReadLine();
                            System.Threading.Thread.Sleep(1000);

=======
                            Console.WriteLine("Press enter to scan again");
                            Console.ReadLine();
                            Console.Clear();
>>>>>>> cbe4a83e31c7f5315135687d8b4cfbc4a37fc021
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
