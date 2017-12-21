using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LMS.Core
{
    public interface ICanvasManager
    {
        string CanvasAccessToken { get; }
        string CanvasUrl { get; }

        Task<object> GetCourseDetail(int iCourseId);

    }
}
