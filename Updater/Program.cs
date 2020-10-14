using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Updater
{
    class Program
    {
        static void Main(string[] args)
        {
            Version FileVersion;
            Version ProductVersion;

            XmlDocument doc = new XmlDocument();
            doc.Load(@"settings.xml");
            string updaterFolder = doc.GetElementsByTagName("updaterFolder")[0].InnerText;

            if (File.Exists($"{updaterFolder}ShowIp.update"))
            {
                 FileVersion = new Version(FileVersionInfo.GetVersionInfo($"{updaterFolder}ShowIp.update").FileVersion);
            }
            else
            {
                 FileVersion = new Version();
            }

            if (File.Exists(args[1]))
            {
                 ProductVersion = new Version(FileVersionInfo.GetVersionInfo(args[1]).FileVersion);
            }
            else
            {
                ProductVersion = new Version();
            }

            if (FileVersion > ProductVersion)
                try
                {
                    Logger.WriteLog("UpdaterException", 0, "FileVersion > ProductVersion");

                    //string process = args[1].Replace("", ".exe");
                    string process = args[1].Replace(".exe", "");
                    //process = process.Replace(@"\\\\", @"\");

                    if (CheckStatusProcess(process))
                    {
                        try
                        {

                            Process[] processToKill = Process.GetProcessesByName(args[1].Replace(".exe", ""));
                            processToKill[0].Kill();

                            Logger.WriteLog("Updater", 0, "processToKill[0].Kill()");
                            //foreach (Process proc in Process.GetProcessesByName(process))
                            //{
                            //    proc.Kill();
                            //}

                            Console.WriteLine("Terminate process!");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Logger.WriteLog("UpdaterException", 0, ex.Message);
                        }
                    }
                    //Console.WriteLine($"Terminate process: {process}!");

                    //Process[] processToKill = Process.GetProcessesByName(process);
                    //processToKill[0].Kill();

                    //while (Process.GetProcessesByName(process).Length > 0)
                    //{
                    //    Process[] myProcesses2 = Process.GetProcessesByName(process);
                    //    for (int i = 1; i < myProcesses2.Length; i++) { myProcesses2[i].Kill(); }

                    //    Thread.Sleep(100);
                    //}

                    // if (File.Exists(args[0])) { File.Delete(args[1]); }

                    if (File.Exists(args[0])) 
                    { 
                        File.Delete(args[1]);
                        Logger.WriteLog("Updater", 0, "File.Delete(args[1]);");
                    }

                    //File.Move(process, args[0]);
                    //File.Move(args[0], process);
                    //File.Copy(args[0], args[1]);

                    File.Copy(args[0], args[1], true);

                    //Console.WriteLine("Starting " + args[1]);
                    Console.WriteLine("Starting " + args[1]);
                    Logger.WriteLog("Updater", 0, "Starting " + args[1]);
                    //Process.Start(args[1]);
                    Process.Start(args[1]);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message); //todo log
                    Logger.WriteLog("UpdaterException", 0, e.Message);
                }
            else
            {
                Logger.WriteLog("UpdaterException", 0, "FileVersion <= ProductVersion");
            }
        }

        public static bool CheckStatusProcess(string nameProcess)
        {
            Process[] pname = Process.GetProcessesByName(nameProcess);
            if (pname.Length == 0)
                return false;
            else
                return true;
        }
    }
}
