using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.IO;
using NLog;

using canvasApiLib.Base;

namespace canvasApiLib.API
{
	public class clsFileUpload : clsHttpMethods
	{
		////NLog
		private static Logger _logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// This method was created to show how to execute a file upload to a URL
		/// Follow documentation found here:  
		///     https://canvas.instructure.com/doc/api/file.file_uploads.html
		///     specifically the section "Uploading via POST"
		/// To get the "folderId" parameter, reference the following API document:
		///     https://canvas.instructure.com/doc/api/files.html#method.folders.api_index
		///     This is the API call to list folders, either for a course or a user or a group
		/// </summary>
		/// <param name="accessToken"></param>
		/// <param name="baseUrl"></param>
		/// <param name="folderId"></param>
		/// <param name="fi"></param>
		/// <returns></returns>
		public static async Task<string> uploadFileToFolder(string accessToken, string baseUrl, int folderId, FileInfo fi)
		{
			string rval = string.Empty;
			string urlCommand = "/api/v1/folders/:folder_id/files";
			dynamic phase1Result = null;
			string httpResultString = string.Empty;

			#region PHASE 1
			//PHASE 1: tell canvas you want to upload a file, and get required paramters back in the response
			urlCommand = urlCommand.Replace(":folder_id", folderId.ToString());
			Dictionary<string, string> vars = new Dictionary<string, string>();
			vars.Add("name", fi.Name);
			vars.Add("size", fi.Length.ToString());
			vars.Add("content_type", MimeMapping.GetMimeMapping(fi.Name));
			var p1Content = new FormUrlEncodedContent(vars.ToArray());

			using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, p1Content))
			{
				httpResultString = await response.Content.ReadAsStringAsync();
				_logger.Debug("[uploadFileToFolder] API Response: [" + httpResultString + "]");

				if (response.IsSuccessStatusCode)
				{
					phase1Result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(httpResultString);
				}
				else
				{
					string msg = "[Phase1][uploadFileToFolder]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
					_logger.Error(msg + "\n" + httpResultString);
					throw new Exception(httpResultString);
				}
			}
			#endregion

			#region Phase 2
			//PHASE 2: upload file to upload_url and get back the location needed for PHASE 3
			string uploadUrl = phase1Result.upload_url.ToString();

			string finalLocation = string.Empty;
			HttpClientHandler handler = new HttpClientHandler();
			handler.AllowAutoRedirect = false;
			using (HttpClient client = new HttpClient(handler))
			{
				MultipartFormDataContent p2Content = new MultipartFormDataContent();
				//add upload_params to form data in same order we received it
				foreach (dynamic key in phase1Result.upload_params)
				{
					p2Content.Add(new StringContent(key.Value.ToString()), key.Name.ToString());
				}

				//read file to upload into byte array
				FileStream fs = fi.OpenRead();
				byte[] fileBytes = new byte[fi.Length];
				int numBytes = fs.Read(fileBytes, 0, fileBytes.Length);
				fs.Close();

				//add byte array as last element of the form data, per Canvas document
				p2Content.Add(new ByteArrayContent(fileBytes), "file");

				//post for data to the upload_url defined by Canvas
				HttpResponseMessage p2Response = await client.PostAsync(uploadUrl, p2Content);
				httpResultString = await p2Response.Content.ReadAsStringAsync();

				//We are looking for any variation of status 3XX - "RedirectMethod" represents all variations of status 300, 301, 302, 303, etc.
				if (p2Response.StatusCode == System.Net.HttpStatusCode.RedirectMethod)
				{
					IEnumerable<string> location;
					if (p2Response.Headers.TryGetValues("Location", out location))
					{
						finalLocation = location.First();
					}
				}
				else
				{
					string msg = "[Phase2][uploadFileToFolder]:[" + uploadUrl + "] returned status[" + p2Response.StatusCode + "]: " + p2Response.ReasonPhrase;
					_logger.Error(msg + "\n" + p2Response);
					throw new Exception(httpResultString);
				}
			}
			#endregion

			#region Phase 3
			//phase 3: final post back to Canvas with the Location value
			if (finalLocation != string.Empty)
			{
				using (HttpClient client = new HttpClient())
				{
					client.DefaultRequestHeaders.Accept.Clear();
					client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
					HttpResponseMessage p3Response = await client.PostAsync(finalLocation, null);
					httpResultString = await p3Response.Content.ReadAsStringAsync();

					if(!p3Response.IsSuccessStatusCode)
					{
						rval = finalLocation;
					}
				}
			}
			else
			{
				string errMsg = "[Phase2:uploadFileToFolder] did not return expected [finalLocation], not able to verify upload.";
				_logger.Error(errMsg);
				_logger.Error(httpResultString);
				throw new Exception(errMsg);
			}
			#endregion

			return rval;
		}

		public static async Task<string> uploadSubmissionCommentFile(string accessToken, string baseUrl, int courseId, int assignmentId, int userId, FileInfo fi)
		{
			string rval = string.Empty;
			string urlCommand = "/api/v1/courses/:course_id/assignments/:assignment_id/submissions/:user_id/comments/files";
			dynamic phase1Result = null;
			string httpResultString = string.Empty;

			#region PHASE 1
			//PHASE 1: tell canvas you want to upload a file, and get required paramters back in the response
			urlCommand = urlCommand.Replace(":course_id", courseId.ToString());
			urlCommand = urlCommand.Replace(":assignment_id", assignmentId.ToString());
			urlCommand = urlCommand.Replace(":user_id", userId.ToString());

			Dictionary<string, string> vars = new Dictionary<string, string>();
			vars.Add("name", fi.Name);
			vars.Add("size", fi.Length.ToString());
			vars.Add("content_type", MimeMapping.GetMimeMapping(fi.Name));
			var p1Content = new FormUrlEncodedContent(vars.ToArray());

			try
			{
				using (HttpResponseMessage response = await httpPOST(baseUrl, urlCommand, accessToken, p1Content))
				{
					httpResultString = await response.Content.ReadAsStringAsync();
					_logger.Debug("[uploadFileToFolder] API Response: [" + httpResultString + "]");

					if (response.IsSuccessStatusCode)
					{
						phase1Result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(httpResultString);
					}
					else
					{
						string msg = "[Phase1][uploadFileToFolder]:[" + urlCommand + "] returned status[" + response.StatusCode + "]: " + response.ReasonPhrase;
						_logger.Error(msg + "\n" + httpResultString);
						throw new Exception(httpResultString);
					}
				}
			}
			catch (Exception p1err)
			{
				_logger.Error(p1err, "[PHASE1] file upload failed");
				throw p1err;
			}
			#endregion

			#region Phase 2
			//PHASE 2: upload file to upload_url and get back the location needed for PHASE 3
			string uploadUrl = phase1Result.upload_url.ToString();

			string finalLocation = string.Empty;
			try
			{
				HttpClientHandler handler = new HttpClientHandler();
				handler.AllowAutoRedirect = false;
				using (HttpClient client = new HttpClient(handler))
				{
					MultipartFormDataContent p2Content = new MultipartFormDataContent();
					//add upload_params to form data in same order we received it
					foreach (dynamic key in phase1Result.upload_params)
					{
						p2Content.Add(new StringContent(key.Value.ToString()), key.Name.ToString());
					}

					//read file to upload into byte array
					FileStream fs = fi.OpenRead();
					byte[] fileBytes = new byte[fi.Length];
					int numBytes = fs.Read(fileBytes, 0, fileBytes.Length);
					fs.Close();

					//add byte array as last element of the form data, per Canvas document
					p2Content.Add(new ByteArrayContent(fileBytes), "file");

					//post for data to the upload_url defined by Canvas
					HttpResponseMessage p2Response = await client.PostAsync(uploadUrl, p2Content);
					httpResultString = await p2Response.Content.ReadAsStringAsync();

					//We are looking for any variation of status 3XX - "RedirectMethod" represents all variations of status 300, 301, 302, 303, etc.
					if (p2Response.StatusCode == System.Net.HttpStatusCode.RedirectMethod)
					{
						IEnumerable<string> location;
						if (p2Response.Headers.TryGetValues("Location", out location))
						{
							finalLocation = location.First();
						}
					}
					else
					{
						string msg = "[Phase2][uploadFileToFolder]:[" + uploadUrl + "] returned status[" + p2Response.StatusCode + "]: " + p2Response.ReasonPhrase;
						_logger.Error(msg + "\n" + p2Response);
						throw new Exception(httpResultString);
					}
				}
			}
			catch(Exception p2err)
			{
				_logger.Error(p2err, "[PHASE2] file upload failed");
				throw p2err;
			}
			#endregion

			#region Phase 3
			//phase 3: final post back to Canvas with the Location value
			if (finalLocation != string.Empty)
			{
				try
				{
					using (HttpClient client = new HttpClient())
					{
						client.DefaultRequestHeaders.Accept.Clear();
						client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
						HttpResponseMessage p3Response = await client.PostAsync(finalLocation, null);
						httpResultString = await p3Response.Content.ReadAsStringAsync();
						_logger.Debug("[Phase3] Result: " + httpResultString);
						if (p3Response.IsSuccessStatusCode)
						{
							rval = finalLocation;
						}
					}
				}
				catch(Exception p3err)
				{
					_logger.Error(p3err, "[PHASE3] file upload failed");
					throw p3err;
				}
			}
			else
			{
				string errMsg = "[Phase2:uploadFileToFolder] did not return expected [finalLocation], not able to verify upload.";
				_logger.Error(errMsg);
				_logger.Error(httpResultString);
				throw new Exception(errMsg);
			}
			#endregion

			//make comment and attach the uploaded file

			return rval;
		}

	}
}
