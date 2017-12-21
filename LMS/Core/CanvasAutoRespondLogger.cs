using LMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Web;

namespace LMS.Core
{
    public class CanvasAutoRespondLogger : FileLogger, IFileLoggerSettings
    {        
        
        public string LogDir
        {
            get
            {
                string sDir = string.Concat(base.AppDir, @"\CanvasLog");
                if(!Directory.Exists(sDir))
                {
                    Directory.CreateDirectory(sDir);
                }
                return sDir;
            }
            set { }
        }

        public override string LogFileName
        {
            get
            {
                string sFullPath = string.Format(@"{0}\{1}.txt", base.YearMonth, DateTime.Now.ToString("yyyyMMdd"));
                if (!File.Exists(sFullPath))
                {
                    //File.Create(sFullPath);
                    var myFile = File.Create(sFullPath);
                    myFile.Close();
                }
                return sFullPath;
            }
            set { }
        }

        public void CanvasResponderInitiated()
        {
            Log("Canvas scheduler started.");
        }

    }
}