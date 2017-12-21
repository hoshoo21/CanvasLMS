using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core.Interfaces
{
    public interface IFileLoggerSettings
    {
        string LogDir { get; set; }
        string LogFileName { get; set; }
    }
}
