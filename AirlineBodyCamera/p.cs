using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Edward;

namespace AirlineBodyCamera
{
    class p
    {

        public static string AppFolder = Environment.CurrentDirectory + @"\执勤记录仪";
        public static string IniPath = AppFolder + @"\SysConfig";
        //
        public static string SetCopy = "0";
        public static string CopyDestFolder = "";




        /// <summary>
        /// 
        /// </summary>
        public static void CreateFolder()
        {
            if (!Directory.Exists(AppFolder))
                Directory.CreateDirectory(AppFolder);
        }


        /// <summary>
        /// 
        /// </summary>
        public static void CreateIni()
        {
            IniFile.IniFilePath = IniPath;
            if (!File.Exists(IniPath))
            {
                IniFile.CreateIniFile(IniPath);
                IniFile.IniWriteValue("SysConfig", "SetCopy", SetCopy);
                IniFile.IniWriteValue("SysConfig", "CopyDestFolder", CopyDestFolder);
            }
        }

        public static void ReadIni()
        {
            IniFile.IniFilePath = IniPath;
            if (File.Exists(IniPath))
            {
                SetCopy = IniFile.IniReadValue("SysConfig", "SetCopy");
                CopyDestFolder = IniFile.IniReadValue("SysConfig", "CopyDestFolder");
            }
        }

    }
}
