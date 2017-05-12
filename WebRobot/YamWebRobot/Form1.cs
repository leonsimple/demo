using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebMMengine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Collections;

namespace YamWebRobot
{
    public partial class Form1 : Form
    {
        private WebMMengine.WebMMengine web;
        private string receiveName;    //接收者名字
        private Im.Im im =  Im.Im.getInstance();
        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;

            LoginUser();    //登陆

            dataGridView2.Visible = false;
            pictureBox1.Visible = true;
        }


        private void im_on_receive(webwx.ChatMsg msg)
        {
            if (web.MemberList == null) return;
            //string user= msg.Talker;
            
            string user = "谭小明";
                

            foreach (WebMMengine.Contact contact in web.MemberList){
                if(user.Equals(contact.RemarkName)){
                    user = contact.UserName;
                }
            }
            string err = "";
            web.mm_webwxsendmsg(WebMMengine.sendMsgType.文字, user, msg.Content, ref err);
        }

        private void start()
        {
            web = new WebMMengine.WebMMengine();
            web.on_loadQr += on_loadQrEventHandler;  // 获取到微信二维码图片
            web.on_scanQR_conform += Web_on_scanQR_conform;  //扫描微信二维码图片登陆确认 
            web.on_GetContactList += Web_on_GetContactList; //获取联系人列表
            web.on_NewMessage += Web_on_NewMessage;         //监听接收到新消息
            web.WebMM_Start();
            
        }

        public void on_loadQrEventHandler(Bitmap bmp)
        {
            pictureBox1.Image = bmp;
        }

        private void Web_on_scanQR_conform()
        {
            pictureBox1.BeginInvoke(new MethodInvoker(() =>
            {
                pictureBox1.Hide();
                dataGridView2.Show();

            }));
        }

        //收到新消息
        private void Web_on_NewMessage(WebMMengine.webmm_mesg msg)
        {
            //{2134182796520768348 [@5e5bb81c165e58d008a34aeeaee7364654e5be0bfbeedfaaf1d5841e735bd0e6]->[@18e0d59192193122216582b496fd782e]:芬芬开启了朋友验证，你还不是他（她）朋友。请先发送朋友验证请求，对方验证通过后，才能聊天。&lt;a href="weixin://findfriend/verifycontact"&gt;发送朋友验证&lt;/a&gt;}
            dataGridView2.Visible = true;
            pictureBox1.Visible = false;

            if (web.MemberList == null) return;

            foreach (WebMMengine.Contact contact in web.MemberList)
            {
                if (msg.FromUserName == contact.UserName)
                {
                    if (contact.RemarkName.Equals("谭小明") || contact.NickName.Equals("Leon康为"))
                        im.send("leon8165", 1, msg.Content);

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

            im.connect(web.UIN);
            im.on_receive += im_on_receive;

            if (Contacts.Count < 1) return;

            foreach (WebMMengine.Contact x in Contacts)
            {
                dataGridView1.BeginInvoke(new MethodInvoker(() =>
                {
                    AppendRow(x);
                }));
            }

            UploadWXFriendList();   //上报微信好友列表
        }

        //登陆
        private void LoginUser()
        {
            string result = "";
            string paramsStr = "{\"userName\":\"1111500001\", \"password\":\"123456\"}";

            if (HttpHelper.HttpPostRequest("user/login/in", paramsStr, ref result))
            {
                User.currentUser = (User)JsonConvert.DeserializeObject(result, typeof(User));

                //登陆成功，初始化微信
                System.Threading.Thread thread = new System.Threading.Thread(start);
                thread.Start();

               // UploadWXFriendList();
            }
            else
            {
                Console.WriteLine("请求失败：" + result);
            }
        }

        //上报微信好友列表
        private void UploadWXFriendList()
        {
            Task task = new Task(() =>
            {
                //绑定微信关系
                string result = "";
                StringBuilder sb = new StringBuilder("{");
                sb.Append("\"friendsCount\":\"" + 20 + "\",");
                sb.Append("\"wechatId\":\"" + web.UIN + "\",");
                sb.Append("\"imei\":\"99000774935780z\"");
                sb.Append("}");
                //绑定工作机的关系
                HttpHelper.HttpPostRequest("orgwx/bind", sb.ToString(), ref result);

                //上传好友列表
                ArrayList arr = web.MemberList;
                sb.Remove(0, sb.Length);
                sb.Append("[");

                System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
               
                for (int i = 0; i < 20; i++)
                {
                    Contact contact = arr[i] as Contact;

                    if (contact.UserName.Length > 40) continue;

                    if (i > 0) sb.Append(",");

                    

                    Bitmap img = web.mm_webwxgeticon(contact.HeadImgUrl); //获取图片

                    //图片保存到本地
                    string imgName = HttpHelper.SaveImage(img);

                    //图片上传到阿里云oss
                    string ossImgPath = HttpHelper.OSSUploadImage(imgName);

                    sb.Append("{");
                    sb.Append("\"area\":\"" + contact.City + "\",");
                    sb.Append("\"bWxId\":\"" + web.UIN + "\",");
                    sb.Append("\"friendsCount\":\"" + 20 + "\",");
                    sb.Append("\"gender\":\"" + contact.Sex + "\",");
                    sb.Append("\"headUrl\":\"" + ossImgPath + "\",");
                    sb.Append("\"thumHeadUrl\":\"" + ossImgPath + "\",");
                    sb.Append("\"type\":\"1\",");
                    sb.Append("\"wechatId\":\"" + contact.UserName + "\",");

                    if (contact.NickName.Contains("\""))
                    {
                        contact.NickName = contact.NickName.Replace("\"", "'");
                    }
                    sb.Append("\"wxName\":\"" + contact.NickName + "\"");
                    sb.Append("}");
                }
                sb.Append("]");

                string sss = sb.ToString();

                if (HttpHelper.HttpPostRequest("ics/event/newfriend", sb.ToString(), ref result))
                {
                    Console.WriteLine("result: " + result);
                }
                else
                {
                    Console.WriteLine("请求失败：" + result);
                }
            });

            task.Start();
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
