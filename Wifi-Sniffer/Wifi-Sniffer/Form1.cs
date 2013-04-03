using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NativeWifi;



namespace Wifi_Sniffer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            startProgram();
        }

        /// <summary> 
        /// This will return SSID
        /// </summary>
        /// </returns> SSID as ASCII
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        
        /// <summary> 
        /// Starts the programm by checking wlan-adapter. Done only once in the beginning
        /// </summary> 
        private void startProgram()
        {
            try
            {

                int wirelessIndex = -1;
                string[,] foundWireless = new string[50, 5];


                
                
                    WlanClient client = new WlanClient();
                    //adapterName.Text = "notfound";
                

                // Output file to write collected AP data....
                using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt"))

                    //This mayy broke whole HELL if there are more adapters//////////////////////////////////////
                    //NativeWifi.WlanClient.WlanInterface wlan = client.WlanInterfaces[0];
                    foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                    {

                        wlanIface.Scan();

                        adapterName.Text = wlanIface.InterfaceDescription;
                        Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                        foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                        {
                            //Found new 
                            wirelessIndex++;
                            //create indekxed listview for i= columns
                            //Name | mac | RSSi | draw | optional
                            for (int i = 0; i < 3; i++)
                            {
                                foundWireless[wirelessIndex, i] = " ";
                            }

                            try
                            {
                                //Store the name of  the found wlan
                                string wlanName = (GetStringForSSID(bssEntry.dot11Ssid));
                                foundWireless[wirelessIndex, 0] = wlanName;
                            }
                            catch {
                                errorTextBox.Text = "Error ïn wlanName";
                            }

                            try
                            {
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

                                //Writeoutput-file
                                outputfile.WriteLine(mac);
                            }
                            catch
                            {
                                errorTextBox.Text = "Error in mac";
                            }

                            try
                            {
                                int RSSI = bssEntry.rssi;
                                foundWireless[wirelessIndex, 2] = RSSI.ToString();
                            }
                            catch
                            {

                                errorTextBox.Text = "Error in rssi";
                            }
                        }
                        //Console.WriteLine("Press enter to scan again");
                        //Console.ReadLine();
                        //Console.Clear();

                        //Scan
                        //wlanIface.Scan();


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
                                //listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 3]); //Draw
                                //listView1.Items[listView1.Items.Count - 1].SubItems.Add(foundWireless[i, 4]); //optionalinfo

                            }
                            else
                            {
                                //old element
                                listView1.Items[SearchItem.Index].SubItems[0].Text = foundWireless[i, 0]; //name
                                listView1.Items[SearchItem.Index].SubItems[2].Text = foundWireless[i, 2]; //RSSi
                            }

                        }
                        
                    }
            }
            catch
            {
                errorTextBox.Text = "Error detected";
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
        /// Done once in second(controlled by timer). Calls createWlanData
        /// </summary> 
        private void timer1_Tick(object sender, EventArgs e)
        {
            //Here we call scan method every second to refresh data
            //becausedefault scan time is every 60seconds
            //wlanIface.Scan();
            //showWlanData();
            startProgram();
        }


    /*
        /// <summary> 
        /// This creates new data block for new wlan. 
        /// </summary> 
        private void createWlanData(string wlanName, string mac, int RSSi)
        {
            
            //Container
            FlowLayoutPanel panel = new FlowLayoutPanel();
            TextBox wlanName1 = new TextBox();
            TextBox wlanMac1 = new TextBox();
            TextBox wlanRSSI1 = new TextBox();

            panel.Controls.Add(wlanName1);
            panel.Controls.Add(wlanMac1);
            panel.Controls.Add(wlanRSSI1);

            wlanName1.Text = wlanName;
            wlanMac1.Text = mac;
            wlanRSSI1.Text = RSSi.ToString();

            panel.Name = mac;

            tableLayoutPanel1.Controls.Add(panel, 2, 1);
            

        }

        */
      
      
       

       
    }
}



/*
           //WlanClient client = new WlanClient();
               // Output file to write collected AP data....

           //using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt"))
              
            
                       foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                      {
                           // Network adapter vendor and model
                           //Console.WriteLine(wlanIface.InterfaceDescription);
                           //Console.WriteLine();
                           //textBox1.Text = wlanIface.InterfaceDescription;
                           label1.Text = wlanIface.InterfaceDescription;
                           Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                          foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                          {
                               // SSID
                               //Console.Write((GetStringForSSID(bssEntry.dot11Ssid)).PadRight(20));

                               //Store the name of  the found wlan
                               string wlanName = (GetStringForSSID(bssEntry.dot11Ssid));
                                
                                
                               //Console.WriteLine(Encoding.ASCII.GetString(bssEntry.dot11Bssid, 0, 6));
                               // MAC address
           //                    byte[] macAddr = bssEntry.dot11Bssid;
           //                    var macAddrLen = (uint)macAddr.Length;
           //                    var str = new string[(int)macAddrLen];
           //                    for (int i = 0; i < macAddrLen; i++)
           //77                    {
           //                        str[i] = macAddr[i].ToString("x2");
           //                    }
           //                    string mac = string.Join("", str);
                               //Console.Write(mac.PadRight(20));
                               //Console.Write("   ");
         //                      outputfile.WriteLine(mac);
                               //Console.WriteLine(bssEntry.rssi);
          //                     int RSSI = bssEntry.rssi;
                               //
                               //createWlanData(wlanName, mac, RSSI);
          //                 }
                           //Console.WriteLine("Press enter to scan again");
                           //Console.ReadLine();
                           //Console.Clear();

                           //Scan
                           //wlanIface.Scan();
          //             }
                  // }
             

           /*
           }
            
           catch
           {
               //Console.WriteLine("Error: Wlan not working properly");

               //Close Programm if wlan-adapter is not found.
               this.Close();
           }
           */