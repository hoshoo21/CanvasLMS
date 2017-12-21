using LMS.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;

namespace LMS.Core
{
    public class FileLogger
    {
        private const string LOG_FILE_DIR = "FileLogger";
        private ReaderWriterLockSlim _fileLock = null;
        protected ReaderWriterLockSlim fileLock
        {
            get
            {
                if(_fileLock == null)
                {
                    _fileLock = new ReaderWriterLockSlim();
                }
                return _fileLock;
            }
        }

        protected string AppDir
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        protected string Year
        {
            get
            {
                string sLogDir = LOG_FILE_DIR;
                if(this is IFileLoggerSettings)
                {
                    sLogDir = ((IFileLoggerSettings)this).LogDir;
                }
                string sYearPath = string.Concat(sLogDir, @"\", DateTime.Now.Year.ToString());
                if (!Directory.Exists(sYearPath))
                {
                    Directory.CreateDirectory(sYearPath);
                }
                return sYearPath;
            }
        }
        protected string YearMonth
        {
            get
            {
                string sYearMonth = string.Concat(Year, @"\", DateTime.Now.ToString("yyyyMM"));
                if (!Directory.Exists(sYearMonth))
                {
                    Directory.CreateDirectory(sYearMonth);
                }
                return sYearMonth;
            }
        }

        public virtual string LogFileName
        {
            get
            {
                string sFullPath = string.Format(@"{0}\{1}.txt", YearMonth, DateTime.Now.ToString("yyyyMMdd"));
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

        public void Log(string sMessage)
        {
            Log(sMessage, true);
        }
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Log(string sMessage, bool AddNewLine)
        {
            if (!string.IsNullOrEmpty(sMessage))
            {
                try
                {
                    lock (fileLock)
                    {
                        using (var tw = new StreamWriter(LogFileName, true))
                        {
                            string sLineToWrite = string.Format("[{1}] : {0}", sMessage, DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss"));
                            switch (AddNewLine)
                            {
                                case true:
                                    tw.WriteLine(sLineToWrite);
                                    break;

                                case false:
                                    tw.Write(sLineToWrite);
                                    break;
                            }

#if DEBUG
                            Debug.WriteLine(sLineToWrite);
#endif
                            tw.Close();
                        }
                    }
                }
                catch(Exception ex)
                {

                }
            }
        }

        public string RemoveHtmlTags(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}