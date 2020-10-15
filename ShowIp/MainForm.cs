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
        public static readonly string pathToLog = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");

        public MainForm()
        {
            if (!Directory.Exists(pathToLog))
            {
                Directory.CreateDirectory(pathToLog);
            }

            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkUpdates();

            String strHostName = string.Empty;
            // Getting Ip address of local machine...
            // First get the host name of local machine.
            strHostName = Dns.GetHostName();
            Console.WriteLine("Local Machine's Host Name: " + strHostName);
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

            this.Text += ":" + Application.ProductVersion;
        }

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

        public void checkUpdates()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"settings.xml");
            string updaterFolder = doc.GetElementsByTagName("updaterFolder")[0].InnerText;

            Version FileVersion = new Version(FileVersionInfo.GetVersionInfo($"{updaterFolder}ShowIp.update").FileVersion);
            Version ProductVersion = new Version(Application.ProductVersion);

            try
            {
                if (File.Exists($"{updaterFolder}ShowIp.update"))
                    if (FileVersion > ProductVersion)
                    {
                        Logger.WriteLog("checkUpdates", 0, "Process.Start:" + $"{updaterFolder}ShowIp.update " + Process.GetCurrentProcess().ProcessName + ".exe\"");
                        Process.Start(@"Updater.exe", $"{updaterFolder}ShowIp.update " + Process.GetCurrentProcess().ProcessName + ".exe\"");
                        // Process.Start(@"Updater.exe", $"{updaterFolder}ShowIp.update \"" + Process.GetCurrentProcess().ProcessName + "\"");
                        //Process.Start(@"d:\project\c#\ShowIp\ShowIp\UpdaterFolder\Updater.exe", "ShowIp.update");
                        Process.GetCurrentProcess().CloseMainWindow();

                        Logger.WriteLog("checkUpdates", 0, "Process.GetCurrentProcess().CloseMainWindow()");
                    }
                    else
                    {
                        if (File.Exists("ShowIp.update")) { File.Delete("ShowIp.update"); }
                        Download();
                    }
            }
            catch (Exception ex)
            {
                if (File.Exists("ShowIp.update")) { File.Delete("ShowIp.update"); }
                Download();
            }
        }

        private void Download()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                //doc.Load(@"http://mysite/version.xml");
                doc.Load(@"d:\project\c#\ShowIp\ShowIp\UpdaterFolder\version.xml");

                Version remoteVersion = new Version(doc.GetElementsByTagName("version")[0].InnerText);
                Version localVersion = new Version(Application.ProductVersion);

                if (localVersion < remoteVersion)
                {
                    if (File.Exists("ShowIp.update")) { File.Delete("ShowIp.update"); }

                    WebClient client = new WebClient();
                    //client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                    //client.DownloadFileCompleted += new AsyncCompletedEventHandler(Completed);
                    //client.DownloadFileAsync(new Uri(@"http://mysite/launcher.exe"), "launcher.update");
                    client.DownloadFileAsync(new Uri(@"d:\project\c#\ShowIp\ShowIp\UpdaterFolder\ShowIp.update"), "ShowIp.update");

                    try
                    {
                        Process.Start("Updater.exe", "ShowIp.update");
                        Process.GetCurrentProcess().Kill();
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception) { }
        }
    }
}
