using LMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace LMS.Core
{
    public class FAQRespondLogger : FileLogger, IFileLoggerSettings
    {
        public string LogDir
        {
            get
            {
                string sDir = string.Concat(base.AppDir, @"\FAQsLog");
                if (!Directory.Exists(sDir))
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
                return base.LogFileName;
            }

            set { }
        }
    }
}