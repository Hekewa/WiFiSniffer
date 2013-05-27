using System;
using System.Collections.Generic;
using System.ComponentModel;
using MySql.Data.MySqlClient;
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

        //ListViewItem.ListViewSubItem indicators = new ListViewItem.ListViewSubItem();
        
        //Initialazion of variables
        int wirelessIndex = -1;
        //Array where wireless information is stored when found
        //Name | mac | RSSi | channel  | RSSiindex for image
        string[,] foundWireless = new string[50, 5];
        //Displayed wireless store also here and to gui
        // mac | update bit | how many rounds it have not been found
        //string[,] wirelessOnDisplay = new string[50, 3];
        //This is rounds after it will be deleted from listview
        int deleteAfterRounds = 4;

        public class wirelessData
        {
            public string mac_;// = " ";
            public bool updatebit;// = false;
            public int notFound;// = 0;
        };

        List<wirelessData> wirelessOnDisplay = new List<wirelessData>();
        wirelessData instance = new wirelessData();
        List<int> removeInstances = new List<int>();
       
        //imageListSmall.Images.Add(Bitmap.
        //RSSi values
        //lower than -95: red
        //          -75-(-60) yellow
        //             -60-0  green
        //ImageList imageListforIndicator = new ImageList();
        //imageListforIndicator 
        //0-green, 2-yellow, 1-red
        //-45--60, -60--75, -75---

        /// <summary> 
        /// Starts the programm by checking wlan-adapter. Done only once in the beginning
        /// </summary> 
        private void scan()
        {
            try
            {
                //Create Connection object to connect to the mySQL database on local server (we were using EasyPHP)
                string ConStr = "server = localhost; user = root; database = sniffer; port = 3306";
                MySqlConnection conn = new MySqlConnection(ConStr);
                

                wirelessIndex = -1;


                // Output file to write collected AP data....
                //using (System.IO.StreamWriter outputfile = new System.IO.StreamWriter(@"C:\Users\Public\WifiSniffer.txt", true))

                //This mayy broke whole HELL if there are more adapters//////////////////////////////////////
                //WlanClient.WlanInterface[] wlan = client.WlanInterfaces();

                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {

                    wlanIface.Scan();

                    // Print date and time of scan.
                    DateTime dateTime = new DateTime(2011, 05, 08, 01, 05, 00);
                    dateTime = DateTime.Now;
                    string timeDate = dateTime.Year.ToString() + "-" + dateTime.Month.ToString() + "-" + dateTime.Day.ToString()
                        +" " + dateTime.TimeOfDay.ToString();
                    File.AppendAllText(@"C:\Users\Public\WifiSniffer.txt", Environment.NewLine + timeDate + Environment.NewLine);



                    adapterName.Text = wlanIface.InterfaceDescription;
                    Wlan.WlanBssEntry[] bssEntries = wlanIface.GetNetworkBssList();
                    foreach (Wlan.WlanBssEntry bssEntry in bssEntries)
                    {
                        //Found new 
                        wirelessIndex++;
                        //create indekxed listview for i= columns
                        //found bit is not shown in listview as it is used 
                        //Name | mac | RSSi | channel  | found bit | RSSI-index
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
                        string mac = string.Join(":", str);

                        foundWireless[wirelessIndex, 1] = mac;

                        //RSSI and the index image
                        //0-green, 2-yellow, 1-red
                        //-45--60, -60--75, -75---
                        int RSSI = bssEntry.rssi;
                        foundWireless[wirelessIndex, 2] = RSSI.ToString();
                        if(RSSI >= -60)
                        {
                            foundWireless[wirelessIndex, 4] = "0";
                        }
                        else if(RSSI >= -75)
                        {
                            foundWireless[wirelessIndex, 4] = "2";
                        }
                        else
                        {
                            foundWireless[wirelessIndex, 4] = "1";
                        }

                        uint channel = bssEntry.chCenterFrequency;
                        foundWireless[wirelessIndex, 3] = returnChannelStr(channel);

                        File.AppendAllText(@"C:\Users\Public\WifiSniffer.txt", mac.PadRight(20) + wlanName.PadRight(20)
                       + foundWireless[wirelessIndex, 2].PadRight(20) + Environment.NewLine);
                        
                        conn.Open();
                        string sql = "INSERT INTO sniffing (date, mac, ssid, rssi) values ('"
                            + timeDate + "','" + mac + "','" + wlanName + "'," + foundWireless[wirelessIndex, 2] + ")";
                        MySqlCommand cmd = new MySqlCommand(sql, conn);
                        MySqlDataReader rdr = cmd.ExecuteReader();
                        conn.Close();

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

                            if (foundWireless[i, 4] == "0")
                            {
                                listView1.Items[listView1.Items.Count - 1].ImageIndex = 0;
                            }
                            else if (foundWireless[i, 4] == "1")
                            {
                                listView1.Items[listView1.Items.Count - 1].ImageIndex = 1;
                            }
                            else if (foundWireless[i, 4] == "2")
                            {
                                listView1.Items[listView1.Items.Count - 1].ImageIndex = 2;
                            }
                            //wirelessData
                            //wirelessOnDisplay[listView1.Items.Count - 1, 0] = foundWireless[i, 1];
                            //wirelessOnDisplay[listView1.Items.Count - 1, 1] = "1"; //Updated
                            //wirelessOnDisplay[listView1.Items.Count - 1, 2] = "0"; //Not Found == 0

                            //ListViewItem.ListViewSubItem indicators = new ImageList.ImageCollection();

                            
                            
                            //instance.mac_ = foundWireless[i, 1];
                            //instance.updatebit = true;
                            //instance.notFound = 0;
                            //wirelessOnDisplay.Insert(listView1.Items.Count - 1, instance);
                            wirelessData tmp = new wirelessData();
                            wirelessOnDisplay.Add(tmp);
                            wirelessOnDisplay[listView1.Items.Count - 1].updatebit = true;
                            wirelessOnDisplay[listView1.Items.Count - 1].mac_ = foundWireless[i, 1];
                            wirelessOnDisplay[listView1.Items.Count - 1].notFound = 0;
                        }
                        else
                        {
                            //old element
                            listView1.Items[SearchItem.Index].SubItems[0].Text = foundWireless[i, 0]; //name
                            listView1.Items[SearchItem.Index].SubItems[2].Text = foundWireless[i, 2]; //RSSi

                            if (foundWireless[i, 4] == "0")
                            {
                                listView1.Items[SearchItem.Index].ImageIndex = 0;
                            }
                            else if (foundWireless[i, 4] == "1")
                            {
                                listView1.Items[SearchItem.Index].ImageIndex = 1;
                            }
                            else if (foundWireless[i, 4] == "2")
                            {
                                listView1.Items[SearchItem.Index].ImageIndex = 2;
                            }
                            //Something
                            //instance = wirelessOnDisplay[SearchItem.Index];
                            //instance.updatebit = true;
                            //instance.notFound = 2;
                            wirelessOnDisplay[SearchItem.Index].updatebit = true; //Updated
                            wirelessOnDisplay[SearchItem.Index].notFound = 0; //Not Found == 0
                            //errorTextBox.Text = wirelessOnDisplay[SearchItem.Index].notFound.ToString();
                            //wirelessOnDisplay[SearchItem.Index].updatebit = true;

                        }

                    }

                    //Go through wirelessOnDisplay to see which ones are updated and not
                    for (int i = 0; i < wirelessOnDisplay.Count; i++)
                    {
                        //errorTextBox.Text = "Tuhoa1";                       
                        if (wirelessOnDisplay[i].updatebit == false)
                        {

                            wirelessOnDisplay[i].notFound += 1;

                            if (wirelessOnDisplay[i].notFound >= deleteAfterRounds)
                            {
                                removeInstances.Add(i);
                                //instance.updatebit = false;
                                //errorTextBox.Text = "Tuhoa";
                            }
                        }                     
                        else
                        {
                            wirelessOnDisplay[i].updatebit = false;
                            wirelessOnDisplay[i].notFound = 0;
                            //errorTextBox.Text = wirelessOnDisplay[i].mac_.ToString();
                        }

                    }
                    
                    //REmove the instances from Listview1 and wirelessOnDisplay (List)
                    for (int i = removeInstances.Count-1; i >= 0 ; i--)
                    {
                        //removeInstances[i].
                        wirelessOnDisplay.RemoveAt(removeInstances[i]);
                        listView1.Items.RemoveAt(removeInstances[i]);
                        //errorTextBox.Text = removeInstances[i].ToString();
                    }
                    /*
                    for (int i = wirelessOnDisplay.Count - 1; i >= 0; i--)
                    {
                        //removeInstances[i].
                        
                           wirelessOnDisplay.RemoveAt(removeInstances[i]);
                           listView1.Items.RemoveAt(i);
                           errorTextBox.Text = removeInstances[i].ToString();
                        
                    }*/

                    
                    
                    removeInstances.Clear();
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

        private void restart_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }


    }
}
