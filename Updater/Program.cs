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
            Logger.WriteLog("Updater", 0, "Start Updater");

            if (args.Length > 1)
            {
                Logger.WriteLog("Updater", 0, "args.Length > 1");
                Version FileVersion;
                Version ProductVersion;

                XmlDocument doc = new XmlDocument();
                doc.Load(@"settings.xml");
                string updaterFolder = doc.GetElementsByTagName("updaterFolder")[0].InnerText;

                GetVersion(args, out FileVersion, out ProductVersion, updaterFolder);
                if (FileVersion > ProductVersion)
                    try
                    {
                        Logger.WriteLog("Updater", 0, "FileVersion > ProductVersion");

                        string process = args[1].Replace(".exe", "");

                        if (CheckStatusProcess(process))
                        {
                            try
                            {
                                Process[] processToKill = Process.GetProcessesByName(process);
                                processToKill[0].Kill();
                                Logger.WriteLog("Updater", 0, "processToKill[0].Kill()");
                                Console.WriteLine("Terminate process!");
                                Thread.Sleep(1000);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                Logger.WriteLog("UpdaterException[processToKill]", 0, ex.Message);
                            }
                        }

                        if (File.Exists(args[0]))
                        {
                            if (!CheckStatusProcess(process))
                            {
                                Logger.WriteLog("Updater", 0, "File.Delete(args[1]);");
                                File.Delete(args[1]);
                            }
                            else
                            {
                                Logger.WriteLog("UpdaterDontDelete", 0, "!CheckStatusProcess(process))");
                            }
                        }
                        else
                        {
                            Logger.WriteLog("UpdaterDontDelete", 0, "!(File.Exists(args[0])");
                        }


                        //File.Move(args[0], process);
                        File.Copy(args[0], args[1], true);


                        GetVersion(args, out FileVersion, out ProductVersion, updaterFolder);
                        if ((FileVersion <= ProductVersion) && !CheckStatusProcess(process))
                        {
                            Console.WriteLine("Starting " + args[1]);
                            Logger.WriteLog("Updater Starting", 0, "FileVersion <= ProductVersion");
                            Logger.WriteLog("Updater Starting", 0, "Starting " + args[1]);
                            Process.Start(args[1]);
                        }
                        else
                        {
                            Logger.WriteLog("UpdaterStartingDontStart", 0, "FileVersion > ProductVersion && CheckStatusProcess(process)");
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog("UpdaterException[if (FileVersion > ProductVersion)]", 0, e.Message);
                    }
                else if (FileVersion == ProductVersion)
                {
                    Logger.WriteLog("Updater Starting", 0, "FileVersion == ProductVersion");
                    Logger.WriteLog("Updater Starting", 0, "Starting " + args[1]);
                    Process.Start(args[1]);
                }
                else
                {
                    Logger.WriteLog("UpdaterException", 0, "FileVersion <= ProductVersion");
                    Logger.WriteLog("Updater Starting", 0, "Starting " + args[1]);
                    Process.Start(args[1]);
                }
            }
            else 
            {
                Logger.WriteLog("Updater", 0, "args.Length <= 1");
            }
        }

        /// <summary>
        /// GetVersion
        /// </summary>
        /// <param name="args"></param>
        /// <param name="FileVersion"></param>
        /// <param name="ProductVersion"></param>
        /// <param name="updaterFolder"></param>
        private static void GetVersion(string[] args, out Version FileVersion, out Version ProductVersion, string updaterFolder)
        {
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
        }

        /// <summary>
        /// CheckStatusProcess
        /// </summary>
        /// <param name="nameProcess"></param>
        /// <returns></returns>
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
