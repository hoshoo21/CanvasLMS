using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NLog;

namespace canvasApiLib.Base
{
	public class clsRegEx
	{
		////NLog
		private static Logger _logger = LogManager.GetLogger(typeof(clsRegEx).FullName);

		/// <summary>
		/// Verify that the string is not empty, and contains alphanumeric characters
		/// There is no requirement associated with special characters
		/// </summary>
		/// <param name="text">string to verify</param>
		/// <returns></returns>
		public static bool confirmAlphaNumeric(string text)
		{
			bool rval = false;
			string pattern = "^[a-zA-Z0-9]+$";
			try
			{
				rval = Regex.IsMatch(text, pattern);
			}
			catch(Exception err)
			{
				_logger.Error(err, "[confirmAlphaNumeric]");
			}
			return rval;
		}
	}
}
