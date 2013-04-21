using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NativeWifi;



namespace Wifi_Sniffer
{
    public partial class Form1 : Form
    {

        //For scanm to use
        WlanClient client = new WlanClient();



        public Form1()
        {
            InitializeComponent();
            scan();
        }

        /// <summary> 
        /// This will return SSID
        /// </summary>
        /// </returns> SSID as ASCII
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }


        //int[] channelsFR = new int[14] {2412, 2417, 2422, 2427, 2432, 2437, 2442, 2447, 2452, 2457, 2462, 2467, 2472, 2484};
        /// <summary> 
        /// This will return channel number as string
        /// </summary>
        /// </returns> string
        static string returnChannelStr(uint frequency)
        {

            string value = "0";
            uint[] channelsFR = { 2412000, 2417000, 2422000, 2427000, 2432000, 2437000, 2442000, 2447000, 2452000, 2457000, 2462000, 2467000, 2472000, 2484000 };
            /*foreach (int i in channelsFR)
            {
                if (frequency == channelsFR[i])
                {
                    return (i+1).ToString();
                   // break;
                }
            }*/
            
            //Input the channel frequenzy
            for(int i=0; i<channelsFR.Length; i++){
                if (frequency == channelsFR[i])
                {
                    return (i + 1).ToString();
                    // break;
                }
            }
            return value; 
        }


        //Initialazion of variables
        int wirelessIndex = -1;
        string[,] foundWireless = new string[50, 5];
        string[,] wirelessOnDisplay = new string[50, 5];
    


       
        /// <summary> 
        /// Starts the programm by checking wlan-adapter. Done only once in the beginning
        /// </summary> 
        private void scan()
        {
            try
            {

                wirelessIndex = -1;




                // Output file to write collected AP data....
                //using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt", true))

                //This mayy broke whole HELL if there are more adapters//////////////////////////////////////
                //WlanClient.WlanInterface[] wlan = client.WlanInterfaces();

                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {

                    wlanIface.Scan();

                    // Print date and time of scan.
                    DateTime dateTime = DateTime.Now;
                    File.AppendAllText(@"C:\Users\Public\WifiSniffer.txt", Environment.NewLine + dateTime.ToString() + Environment.NewLine);



                    adapterName.Text = wlanIface.InterfaceDescription;
                    Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                    foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                    {
                        //Found new 
                        wirelessIndex++;
                        //create indekxed listview for i= columns
                        //found bit is not shown in listview as it is used 
                        //Name | mac | RSSi | channel  | found bit
                        for (int i = 0; i < 5; i++)
                        {
                            foundWireless[wirelessIndex, i] = " ";
                        }


                        //Store the name of  the found wlan
                        string wlanName = (GetStringForSSID(bssEntry.dot11Ssid));
                        foundWireless[wirelessIndex, 0] = wlanName;


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

                        foundWireless[wirelessIndex, 1] = mac;


                        int RSSI = bssEntry.rssi;
                        foundWireless[wirelessIndex, 2] = RSSI.ToString();

                        uint channel = bssEntry.chCenterFrequency;
                        foundWireless[wirelessIndex, 3] = returnChannelStr(channel);

                        File.AppendAllText(@"C:\Users\Public\WifiSniffer.txt", mac.PadRight(20) + wlanName.PadRight(20)
                       + foundWireless[wirelessIndex, 2].PadRight(20) + Environment.NewLine);

                    }


                    /*
                    //Change every wireless RSSi to zero if not found
                    for (int i = 0; i < listView1.Items.Count; i++)
                    {
                        listView1.Items[i].SubItems[2].Text = "0";
                    }
                    */

                    //Go through all found items and add them to the view
                    for (int i = 0; i < wirelessIndex; i++)
                    {


                        //Try to find old element by mac-address
                        ListViewItem SearchItem = new ListViewItem();
                        SearchItem = listView1.FindItemWithText(foundWireless[i, 1]);
                        if (SearchItem == null)
                        {
                            //Add new element
                            listView1.Items.Add(foundWireless[i, 0]); //Name
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 1]); //mac
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 2]); //Rssi
                            listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 3]); //channel
                            //listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 4]); //optionalinfo

                        }
                        else
                        {
                            //old element
                            listView1.Items[SearchItem.Index].SubItems[0].Text = foundWireless[i, 0]; //name
                            listView1.Items[SearchItem.Index].SubItems[2].Text = foundWireless[i, 2]; //RSSi
                        }

                    }
                    //Clear the found wireless
                    Array.Clear(foundWireless, 0, 50);
                }
            }
            catch (SystemException ex)
            {

                errorTextBox.Text = ex.ToString();
                //this.Close();
            }


        }
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////  


        /// <summary> 
        /// Close the Program
        /// </summary> 
        private void closeButton_Click(object sender, EventArgs e)
        {
            // Close the form. 
            this.Close();
        }



        /// <summary> 
        /// This is main function for the programm. Called by timer
        /// </summary> 
        private void showWlanData()
        {
            //1 Go through wlan-adapters data
            //2. Store data to database and infoblocks
            //3. If new wlan found, call createWlanData


        }

        /// <summary> 
        /// Done once in second(controlled by timer). Calls scan-function
        /// </summary> 
        private void timer1_Tick(object sender, EventArgs e)
        {
            scan();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }


    }
}
