using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Xml;

namespace ShowIp
{
    public partial class MainForm : Form
    {
        //pathToLog
        public static readonly string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

        public MainForm()
        {
            if (!DotNetUtils.IsCompatible())
                return;

            Logger.WriteLog("ShowIp", 0, "try MainForm()");
            if (!Directory.Exists(pathToLog))
            {
                Logger.WriteLog("ShowIp", 0, "try Directory.CreateDirectory(pathToLog)");
                Directory.CreateDirectory(pathToLog);
            }

            InitializeComponent();
        }

        /// <summary>
        /// MainForm_Load
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            checkUpdates();

            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            //Console.WriteLine("Local Machine's Host Name: " + strHostName);
            // Then using host name, get the IP address list..
            //IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
            //IPAddress[] addr = ipEntry.AddressList;

            //for (int i = 0; i < addr.Length; i++)
            //{
            //    Console.WriteLine("IP Address {0}: {1} ", i, addr[i].ToString());

            label1.Text += "\t" + strHostName;
            //label2.Text += "\t" + addr[1];
            label2.Text += "\t" + GetLocalIPAddress();
            //}

            this.Text +=  $" V:{Application.ProductVersion}";
        }

        /// <summary>
        /// GetLocalIPAddress
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "No network adapters with an IPv4!";
            //throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        /// <summary>
        /// checkUpdates
        /// </summary>
        public void checkUpdates()
        {
            Logger.WriteLog("checkUpdates", 0, "try checkUpdates()");

            XmlDocument doc = new XmlDocument();

            //if (!File.Exists("settings.xml"))
            //    throw new Exception("File not exists");

            doc.Load(@"settings.xml");
            string updaterFolder = doc.GetElementsByTagName("updaterFolder")[0].InnerText;

            try
            {
                if (File.Exists($"{updaterFolder}ShowIp.update"))
                {
                    Version FileVersion = new Version(FileVersionInfo.GetVersionInfo($"{updaterFolder}ShowIp.update").FileVersion);
                    Version ProductVersion = new Version(Application.ProductVersion);
                    if (FileVersion > ProductVersion)
                    {
                        Logger.WriteLog("checkUpdates", 0, "try Process.Start:" + $"{updaterFolder}ShowIp.update " + Process.GetCurrentProcess().ProcessName + ".exe\"");
                        Process.Start(@"Updater.exe", $"{updaterFolder}ShowIp.update " + Process.GetCurrentProcess().ProcessName + ".exe\"");
                        Logger.WriteLog("checkUpdates", 0, "try Process.GetCurrentProcess().CloseMainWindow()");
                        Process.GetCurrentProcess().CloseMainWindow();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog("checkUpdatesException", 0, ex.Message);
            }
        }
    }
}
