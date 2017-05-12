using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace YamWebRobot.Im
{
    public class Im
    {
        const int CMD_HEAHT = 100;
        const int CMD_VERVIFY = 200;
        const int CMD_SEND = 201;
        const int CMD_RECEIVE = 102;

        public const int MSG_TYPE_TEXT = 1;
        public const int MSG_TYPE_PIC = 2;
        public const int MSG_TYPE_VIDEO = 3;
        public const int MSG_TYPE_AUDIO = 4;

        public  string ip = "172.168.30.214";
        public  int port = 9999;
        public  string UIN;

        NetworkStream stream;
        TcpClient client;
        private static Im instance;

        public event Im.IM_on_receive on_receive = new IM_on_receive(onReceive);

        public delegate void IM_on_receive(webwx.ChatMsg msg);

        public static Im getInstance()
        {
            if (instance == null)
            {
                instance = new Im();
            }
            return instance;
        }

        public void connect(String uin)
        {
            if (uin == null)
            {
                return;
            }

           UIN = uin;
           ThreadPool.QueueUserWorkItem(imConnect);
        }

        private void imConnect(object o)
        {
            try
            {
                if (!checkConneted())
                {
                    client = new TcpClient();
                    client.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    Debug.WriteLine("socket 连接成功 ...");
                  
                    stream = client.GetStream();

                    verifyIdentity();//验证

                    System.Timers.Timer t = new System.Timers.Timer(180000); // 心跳
                    t.Elapsed += new System.Timers.ElapsedEventHandler(heartBeat);
                    t.AutoReset = true;
                    t.Enabled = true;

                    Thread receive = new Thread(imReceive);
                    receive.Start();

                }

            }
            catch (Exception error)
            {
               Debug.WriteLine("socket ERROR :" + error.ToString());
            }
        }

        private void imReceive()
        {
            if (!checkConneted())
            {
                ThreadPool.QueueUserWorkItem(imConnect);
            }
            else
            {
                while (true)
                {
                    try
                    {
                        if (stream.DataAvailable)
                        {
                            webwx.ServiceRobotResp resp = webwx.ServiceRobotResp.Parser.ParseDelimitedFrom(stream);
                            if (resp.Cmd == CMD_RECEIVE)
                                on_receive(resp.ChatMsg);
                        }
                    }
                    catch (Exception error)
                    {
                        Debug.WriteLine("imReceive ERROR :" + error.ToString());
                    }
                }
            }

        }

        private Boolean checkConneted()
        {
            return client != null && client.Connected && stream != null;
        }

        private static void onReceive(webwx.ChatMsg chatMsg)
        {
            
            //TODO 调用微信发送方法
            switch (chatMsg.MsgType)
            {
                case MSG_TYPE_TEXT: 
                    Debug.WriteLine("收到一条文本消息 username: {0} --content: {1}", chatMsg.Talker, chatMsg.Content);
                    break;
                case MSG_TYPE_PIC: 
                    Debug.WriteLine("收到一条图片消息 username: {0} --content: {1}", chatMsg.Talker, chatMsg.Content);
                    break;
                case MSG_TYPE_VIDEO: 
                    break;
                case MSG_TYPE_AUDIO: 
                    break;

            }
        }

        

        public void send(string userName, int msgType , string content)
        {
            webwx.RobotReq req = new webwx.RobotReq{
                Cmd = CMD_SEND,
                Identity = identity(),
            };

            webwx.ChatMsg msg = new webwx.ChatMsg
            {
                Talker = userName,
                MsgType = msgType,
                Content = content,
                CreateTime = DateTime.Now.ToBinary(),
            };
            req.ChatMsgList.Add(msg);

            send(req);
        }

        public void verifyIdentity()
        {
            webwx.RobotReq req = new webwx.RobotReq();
            req.Cmd = CMD_VERVIFY;
            req.Identity = identity();
            send(req);
        }

        private void send(webwx.RobotReq req)
        {
            if (!checkConneted())
            {
                ThreadPool.QueueUserWorkItem(imConnect);
            }
            else
            {
                Google.Protobuf.MessageExtensions.WriteDelimitedTo(req, stream);
            }
        }

        public void heartBeat(object o, System.Timers.ElapsedEventArgs e)
        {
            webwx.RobotReq req = new webwx.RobotReq();
            req.Cmd = CMD_HEAHT;
            req.Identity = identity();
            send(req);
        }

        private webwx.Identity identity()
        {
            return new webwx.Identity
            {
                ClientImei = "123456789123456",
                WxID = UIN,
                ShopId = 1001
            };
        }




    }
}
