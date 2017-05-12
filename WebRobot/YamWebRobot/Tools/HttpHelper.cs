﻿using System;
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
using System.Collections;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using System.Drawing;

namespace YamWebRobot
{
    /// <summary>
    /// Http连接操作帮助类
    /// </summary>
    public class HttpHelper
    {
        static string serverRootUrl = "http://172.168.30.220:28006/"; //服务器根地址
        static string imHostAddress = "172.168.30.214";
        static int imPort = 9999;

        //阿里云OSS相关配置信息
        static string bucketName = "kotdev";
        static string endpoint = "https://oss-cn-shenzhen.aliyuncs.com";
        static string accessKeyId = "LTAITgVHmUU5cDg5";
        static string accessKeySecret = "xNxiRNwfpCqZhd8XZBs6Ibfihu5YRV";
      
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
            url = serverRootUrl + url;
           
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
            request.Headers.Add("DeviceID", "99000774935780z");
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


        //阿里云访问对象
        static OssClient client = new OssClient(endpoint, accessKeyId, accessKeySecret);

        public static void CreateBucket()
        {
            try
            {
                client.CreateBucket(bucketName);
                Console.WriteLine("Created bucket name:{0} succeeded ", bucketName);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error info: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                  ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="dirName">本地文件夹名字</param>
        /// <param name="imgName">图片名字</param>
        /// /// <returns>返回oss相对路径</returns>
        public static string OSSUploadImage(string imgName)
        {
            DateTime today = DateTime.Now;

            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0:0000}{1:00}/{2:00}/images/{3}", today.Year, today.Month, today.Day, imgName);
            string fileToUpload = LocalImageFolder() + "\\" + imgName;

            try
            {
                client.PutObject(bucketName, sb.ToString(), fileToUpload);

                if (File.Exists(fileToUpload))
                {
                    File.Delete(fileToUpload);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 下载oss的图片
        /// </summary>
        /// <param name="imgName"></param>
        /// <returns>返回本地图片路径</returns>
        public static string OSSDownloadImage(string imgName)
        {
            string localImgPath = LocalImageFolder() + "\\" + Guid.NewGuid().ToString() + ".jpg";
            try
            {
                OssObject obj = client.GetObject(bucketName, imgName);

                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[1024];
                    var fs = File.Open(localImgPath, FileMode.OpenOrCreate);
                    var len = 0;
                    while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                return "";
            }
            return localImgPath;
        }

        /// <summary>
        /// 获取本地图片路径
        /// </summary>
        /// <returns></returns>
        public static string LocalImageFolder()
        {
            string dirPath = Directory.GetCurrentDirectory() + "\\tempImages";

            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            return dirPath;
        }

        /// <summary>
        /// 保存图片到本地
        /// </summary>
        /// <param name="bmp"></param>
        /// <returns>返回图片名字</returns>
        public static string SaveImageByBitmap(Bitmap bmp)
        {
            if (bmp == null) return "";

            string imgName = Guid.NewGuid().ToString() + ".jpg";
            string localImgPath = LocalImageFolder() + "\\" + imgName;

            using (bmp)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    bmp.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
                    byte[] bytes = stream.ToArray();

                    FileStream fs = new FileStream(localImgPath, FileMode.Create);
                    BinaryWriter bw = new BinaryWriter(fs);
                    bw.Write(bytes);
                    bw.Close();
                    fs.Close();
                }
            }

            return imgName;
        }

        /// <summary>
        /// 保存图片字节数组到本地
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string SaveImageByBytes(byte[] bytes)
        {
            string imgName = Guid.NewGuid().ToString() + ".jpg";
            string localImgPath = LocalImageFolder() + "\\" + imgName;

            FileStream fs = new FileStream(localImgPath, FileMode.Create);
            BinaryWriter bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
            return imgName;
        }
    }
}