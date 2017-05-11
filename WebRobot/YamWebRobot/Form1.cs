using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebMMengine;
using System.Net;
using System.Collections;
using System.Net.Http;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace YamWebRobot
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            System.Threading.Thread thread = new System.Threading.Thread(start);
            thread.Start();

            dataGridView2.Visible = false;
            pictureBox1.Visible = true;

            LoginUser();    //登陆
        }

        private WebMMengine.WebMMengine web;
        private string receiveName;    //接收者名字

        private void start()
        {
            web = new WebMMengine.WebMMengine();
            web.on_loadQr += on_loadQrEventHandler;  //
            web.on_scanQR += Web_on_scanQR;
            web.on_scanQR_conform += Web_on_scanQR_conform;
            web.on_GetContactList += Web_on_GetContactList;
            web.on_NewMessage += Web_on_NewMessage;


            web.on_GetContact += Web_on_GetContact; //获取联系人
            web.on_webMM_status += Web_on_webMM_status;
            web.on_webMM_status_changed += Web_on_webMM_status_changed;

            web.WebMM_Start();
        }

        //获取联系人 （没有调用到）
        private void Web_on_GetContact(Contact Contacts)
        {
            Console.WriteLine("获取联系人。。。");
        }

        //状态改变
        private void Web_on_webMM_status(WX_status state)
        {
            Console.WriteLine("statu222s...: " + state);
        }
        //状态改变
        private void Web_on_webMM_status_changed(WX_status state)
        {
            Console.WriteLine("status...: " + state);
        }

        public void on_loadQrEventHandler(Bitmap bmp)
        {
            pictureBox1.Image = bmp;
       
        }

        public System.Collections.ArrayList Contacts;
        //收到新消息
        private void Web_on_NewMessage(WebMMengine.webmm_mesg msg)
        {
            //{2134182796520768348 [@5e5bb81c165e58d008a34aeeaee7364654e5be0bfbeedfaaf1d5841e735bd0e6]->[@18e0d59192193122216582b496fd782e]:芬芬开启了朋友验证，你还不是他（她）朋友。请先发送朋友验证请求，对方验证通过后，才能聊天。&lt;a href="weixin://findfriend/verifycontact"&gt;发送朋友验证&lt;/a&gt;}
            dataGridView2.Visible = true;
            pictureBox1.Visible = false;

            if (Contacts == null) return;

            foreach (WebMMengine.Contact contact in Contacts)
            {
                if (msg.FromUserName == contact.UserName)
                {
                    dataGridView2.BeginInvoke(new MethodInvoker(() =>
                    {
                        addMsg(contact.NickName, null, msg.Content);
                    }));
                }
            }
        }

        public void AppendRow(WebMMengine.Contact contact)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView1);

            string userName = contact.UserName;  //用户名
            string type = "好友";

            if (userName.IndexOf("@@") == 0)
            {
                type = "微信群";
            }
            else if (Int32.Parse(contact.VerifyFlag) > 0 && Int32.Parse(contact.SnsFlag) < 1)
            {
                type = "公众号";
            }

            row.Cells[0].Value = type;
            row.Cells[1].Value = contact.NickName + ", " + contact.RemarkName + ", " + contact.RemarkPYQuanPin;
            row.Cells[2].Value = userName;
            //row.Cells[3].Value = web.mm_webwxgeticon(contact.HeadImgUrl); //获取图片
            dataGridView1.Rows.Add(row);
        }


        private void Web_on_GetContactList(System.Collections.ArrayList Contacts)
        {
            lblInfo.Text = web.UIN + ", " + web.uuid + ", " + web.ChatSet + ", " + web.User_Agent + ", " + web.logFilename + ", " + web.StatusNotifyUserName+ ","
                +web.StatusNotifyUserNameContent + ", " + web.pgv_pvi + ", " + web.pgv_si;

            this.Contacts = Contacts;
            foreach (WebMMengine.Contact x in Contacts)
            {
                dataGridView1.BeginInvoke(new MethodInvoker(() =>
                {
                    AppendRow(x);
                }));
            }

            UploadContacts();   //上传联系人
        }

        private void UploadContacts()
        {
            //创建任务
            Task task = new Task(() =>
            {
                Console.WriteLine("current.Thread: {0}", Thread.CurrentThread.Name);
                SendRequest();
            });
            task.Start();
            /*
   5:              Task task = new Task(() => {
   6:                  Console.WriteLine("使用System.Threading.Tasks.Task执行异步操作.");
   7:                  for (int i = 0; i < 10; i++)
   8:                  {
   9:                      Console.WriteLine(i);
  10:                  }
  11:              });
  12:              //启动任务,并安排到当前任务队列线程中执行任务(System.Threading.Tasks.TaskScheduler)
  13:              task.Start();
            */
        }

        private void LoginUser()
        {
            HttpWebRequest request = null;
            string url = "http://172.168.30.220:28006/user/login/in";
            //string url = "http://192.168.75.128:80/You/GetPost/";
            
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
            request.Headers.Add("Authorization", "z9oMTs0510J2IXYM1Z");
            request.Headers.Add("Accept-Encoding", "gzip");

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            //发送POST数据  
            string jsonString = "{\"userName\":\"2000001\", \"password\":\"123456\"}";

            byte[] data = Encoding.UTF8.GetBytes(jsonString);
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }


            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Stream stream2 = response.GetResponseStream();   //获取响应的字符串流
            StreamReader sr = new StreamReader(stream2); //创建一个stream读取流
            string html = sr.ReadToEnd();   //从头读到尾，放到字符串html李米
            sr.Close();
            stream2.Close();

            /*
            //需要引用附件dll
            TextReader reader = File.OpenText("json.txt");
            JsonReader readerJson = new JsonTextReader(reader);
            Dictionary<object, object> dict = new Dictionary<object, object>();
            object temp = new object();
            while (readerJson.Read())
            {
                if (readerJson.Value != null)
                {
                    switch (readerJson.TokenType)
                    {
                        case JsonToken.PropertyName:
                            dict.Add(readerJson.Value, new object());
                            temp = readerJson.Value;
                            break;
                        default:
                            dict[temp] = readerJson.Value;
                            break;
                    }
                    Console.WriteLine(readerJson.TokenType + "\t" + readerJson.Value);
                }
            }
             * */

            JToken token = JToken.Parse(html);
            Dictionary<string, object> dict = JsonConvert.DeserializeObject<Dictionary<string, object>>(html);

            string code = dict["code"] as string;

            if (code.Equals("000000"))
            {
                object obj = dict["data"];

                Dictionary<string, object> dataDict = dict["data"] as Dictionary<string, object>;

                string orgName = dataDict["orgName"] as string;
                string token2 = dataDict["token"] as string;

                Console.Write("{0}, {1}", orgName, token2);
            }

            Console.WriteLine(".....");


            Dictionary<String, Object> map = new Dictionary<string, object>();

            Type t = dict.GetType();

            PropertyInfo[] pi = t.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo p in pi)
            {
                MethodInfo mi = p.GetGetMethod();

                if (mi != null && mi.IsPublic)
                {
                    map.Add(p.Name, mi.Invoke(dict, new Object[] { }));
                }
            }

             
        }


        private void SendRequest()
        {
            HttpWebRequest request = null;
            string url = "http://172.168.30.220:28006/ics/event/newfriend/";
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
            request.ContentType = "application/json;charset=UTF-8";

            //设置代理UserAgent和超时
            //request.UserAgent = userAgent;
            //request.Timeout = timeout; 

            //发送POST数据  
            StringBuilder buffer = new StringBuilder();
            ArrayList array = new ArrayList();

            for (int i = 0; i < web.MemberList.Count; i++)
            {
                Contact contact = web.MemberList[i] as Contact;

                Dictionary<string, string> dict = new Dictionary<string, string>();
                dict.Add("area", contact.City);
                dict.Add("bWxId", web.UIN);
                dict.Add("friendsCount", web.MemberList.Count.ToString());
                dict.Add("gender", contact.Sex);
                dict.Add("headUrl", contact.HeadImgUrl);
                dict.Add("thumHeadUrl", contact.HeadImgUrl);
                dict.Add("type", "1");
                dict.Add("wechatId", contact.UserName);
                dict.Add("wxName", contact.NickName);
                dict.Add("wxNo", contact.UserName);

                array.Add(dict);
            }
            
            byte[] data = Encoding.ASCII.GetBytes(array.ToString());
            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            string[] values = request.Headers.GetValues("Content-Type");
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            Console.WriteLine("server: {0}, content:{1}", response.Server, response.ContentEncoding);
        }

        private void Web_on_scanQR()
        {
            Console.WriteLine("scan qr");
        }

        private void Web_on_scanQR_conform()
        {
            pictureBox1.BeginInvoke(new MethodInvoker(() =>
            {
                pictureBox1.Hide();
                dataGridView2.Show();
            }));
        }

        private string chatId;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[index];
            String value = (String)row.Cells[1].Value;
            receiveName = value;
            chatId = (String)row.Cells[2].Value;
        }

        //发送文字
        private void button1_Click(object sender, EventArgs e)
        {
            if (chatId == null || chatId.Length < 1)
            {
                MessageBox.Show("还没选择聊天对象呢");
                return;
            }

            string err = "";
            web.mm_webwxsendmsg(WebMMengine.sendMsgType.文字, chatId, contentTxt.Text, ref err);
            addMsg(null, receiveName, contentTxt.Text);
            contentTxt.Text = "";
        }

        public void addMsg(String fromUserName, String toUserName, String content)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridView2);
            row.Cells[0].Value = fromUserName;
            row.Cells[1].Value = toUserName;
            row.Cells[2].Value = null;
            row.Cells[3].Value = content;
            dataGridView2.Rows.Add(row);
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                button1_Click(null, null);
            }
        }


        private string sendImgPath; //发送的图片路径
        //选择图片
        private void selectPicBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "选择文件";
            openFileDialog.Filter = "png|*.png|jpg|*.jpg|jpeg|*.jpeg";
            openFileDialog.FileName = string.Empty;
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;
            openFileDialog.DefaultExt = "jpg";
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.Cancel)
            {
                openFileDialog.Dispose();
                return;
            }
            string fileName = openFileDialog.FileName;
            this.selectPicBtn.Text = fileName;
            sendImgPath = fileName;

            openFileDialog.Dispose();
        }

        //发送图片
        private void button3_Click(object sender, EventArgs e)
        {
            if (sendImgPath == null || sendImgPath.Length < 1)
            {
                MessageBox.Show("请选择图片");
                return;
            }

            if (chatId == null || chatId.Length < 1)
            {
                MessageBox.Show("还没选择聊天对象呢");
                return;
            }

            string err = "";
            if (web.mm_webwxsendmsg(WebMMengine.sendMsgType.图片, chatId, sendImgPath, ref err))
            {
                this.selectPicBtn.Text = "选择图片";
            }
        }
    }
}
