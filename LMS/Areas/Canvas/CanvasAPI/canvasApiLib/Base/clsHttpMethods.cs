using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using NLog;

namespace LMS.Areas.Canvas.CanvasAPI.canvasApiLib.Base
{
    public class clsHttpMethods : clsRegEx
    {
        ////NLog
        private static Logger _logger = LogManager.GetLogger(typeof(clsHttpMethods).FullName);

        protected const int _maxPageCount = 100;

        /// <summary>
        /// Generic logic to execute an HTTP GET call
        /// </summary>
        /// <param name="baseUrl">url of the API to call, i.e. beta or test</param>
        /// <param name="urlCommand">api command, including parameters</param>
        /// <param name="accessToken">your access token to authenticate to your Canvas instance</param>
        /// <returns>http response from api endpoint</returns>
        protected static async Task<HttpResponseMessage> httpGET(string baseUrl, string urlCommand, string accessToken)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            HttpResponseMessage response = null;

            _logger.Debug("Call API[" + urlCommand + "]");

            try
            {
                //we do not want to allow auto-redirect, this will cause issues if we are uploading files using the Canvas API
                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                using (HttpClient client = new HttpClient(handler, true))
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    response = await client.GetAsync(urlCommand);
                }
            }
            catch (Exception err)
            {
                _logger.Error(err, "[EXCEPTION] Failed to execute HTTP GET command: [" + baseUrl + urlCommand + "]");
                throw err;
            }

            return response;
        }


        /// <summary>
        /// Execute an HTTP POST command, throws an exception
        /// </summary>
        /// <param name="baseUrl">url of the API to call, i.e. beta or test</param>
        /// <param name="urlCommand">api command, including parameters</param>
        /// <param name="accessToken">your access token to authenticate to your Canvas instance</param>
        /// <param name="content">can be used to pass parameters in the body</param>
        /// <returns>http response from api endpoint</returns>
        protected static async Task<HttpResponseMessage> httpPOST(string baseUrl, string urlCommand, string accessToken, HttpContent content)
        {
            HttpResponseMessage response = null;

            try
            {
                _logger.Debug("Call API[" + urlCommand + "]");

                //we do not want to allow auto-redirect, this will cause issues if we are uploading files using the Canvas API
                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                using (HttpClient client = new HttpClient(handler, true))
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    response = await client.PostAsync(urlCommand, content);
                }

            }
            catch (Exception err)
            {
                _logger.Error(err, "[EXCEPTION] Failed to execute POST command:[" + baseUrl + urlCommand + "]");
                throw err;
            }

            return response;
        }

        /// <summary>
        /// Execute an HTTP PUT command, throws an exception
        /// </summary>
        /// <param name="baseUrl">url of the API to call, i.e. beta or test</param>
        /// <param name="urlCommand">api command, including parameters</param>
        /// <param name="accessToken">your access token to authenticate to your Canvas instance</param>
        /// <param name="content">can be used to pass parameters in the body</param>
        /// <returns>http response from api endpoint</returns>
        protected static async Task<HttpResponseMessage> httpPUT(string baseUrl, string urlCommand, string accessToken, HttpContent content)
        {
            HttpResponseMessage response = null;

            try
            {
                _logger.Debug("Call API[" + urlCommand + "]");

                //we do not want to allow auto-redirect, this will cause issues if we are uploading files using the Canvas API
                HttpClientHandler handler = new HttpClientHandler();
                handler.AllowAutoRedirect = false;
                using (HttpClient client = new HttpClient(handler, true))
                {
                    client.BaseAddress = new Uri(baseUrl);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                    response = await client.PutAsync(urlCommand, content);
                }
            }
            catch (Exception err)
            {
                _logger.Error(err, "[EXCEPTOIN] Failed to execute HTTP PUT command: [" + baseUrl + urlCommand + "]");
                throw err;
            }
            return response;
        }

        /// <summary>
        /// Method to contatenate parameters with the API command
        /// </summary>
        /// <param name="urlCommand"></param>
        /// <param name="vars"></param>
        /// <returns></returns>
        protected static string concatenateHttpVars(string urlCommand, Dictionary<string, string> vars)
        {
            KeyValuePair<string, string>[] pairs = vars.ToArray();
            for (int i = 0; i < vars.Count; i++)
            {
                urlCommand += (i == 0) ? "?" : "&";
                urlCommand += pairs[i].Key + "=" + pairs[i].Value;
            }
            return urlCommand;
        }
    }
}