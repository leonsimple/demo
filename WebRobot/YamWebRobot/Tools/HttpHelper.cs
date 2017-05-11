using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.IO.Compression;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace YamWebRobot
{
    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpHelper
    {
        const string rootUrl = "http://172.168.30.220:28006/"; //根url
        
        /// <summary>
        /// post请求
        /// </summary>
        /// <param name="url">接口</param>
        /// <param name="paramString">参数字符串</param>
        /// <param name="result">服务器返回结果字符串</param>
        /// <returns></returns>
        public static bool HttpPostRequest(string url, string paramString, ref string result)
        {
            HttpWebRequest request = null;
            url = rootUrl + url;
           
            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                //ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                //request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/json;charset=utf-8";
            request.Headers.Add("DeviceID", "99000774935779");
            request.Headers.Add("Authorization", User.currentUser.token);
            request.Headers.Add("Accept-Encoding", "gzip");

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            //发送POST数据  
            byte[] data = Encoding.UTF8.GetBytes(paramString);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            HttpWebResponse response;

            try
            {
                 response = request.GetResponse() as HttpWebResponse;
            }
            catch (Exception ex)
            {
                result = "error: " + ex.Message;
                return false;
            }
            
            Stream stream2 = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream2); //创建一个stream读取流
            string jsonStr = sr.ReadToEnd();   //从头读到尾，读取json字符串
            sr.Close();
            stream2.Close();

            JObject obj = JObject.Parse(jsonStr);
            string code = obj["code"].ToString();

            if (obj["data"] != null)
            {
                result = obj["data"].ToString();
            }
            else if (obj["msg"] != null)
            {
                result = obj["msg"].ToString();
            }
            
            if (code.Equals("000000"))
            {
                return true;
            }

            return false;
        }
    }
   
}