using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace webwx
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            System.Threading.Thread thread = new System.Threading.Thread(start);
            thread.Start();




            string s = "git ";

        }

        public void on_loadQrEventHandler(Bitmap bmp)
        {
            pictureBox1.BackgroundImage = bmp;
            pictureBox1.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private WebMMengine.WebMMengine web;
        private void start()
        {
            web = new WebMMengine.WebMMengine();
            web.on_loadQr += on_loadQrEventHandler;
            web.on_scanQR += Web_on_scanQR;
            web.on_scanQR_conform += Web_on_scanQR_conform;
            web.on_GetContactList += Web_on_GetContactList;
            web.on_NewMessage += Web_on_NewMessage;
            web.WebMM_Start();
        }

        public System.Collections.ArrayList Contacts ;

        private void Web_on_NewMessage(WebMMengine.webmm_mesg msg)
        {
            foreach(WebMMengine.Contact contact in Contacts)
            {
                if(msg.FromUserName == contact.UserName)
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
            row.Cells[0].Value = contact.NickName;
            row.Cells[1].Value = contact.UserName;
            dataGridView1.Rows.Add(row);
        }


        private void Web_on_GetContactList(System.Collections.ArrayList Contacts)
        {
            this.Contacts = Contacts;
            foreach (WebMMengine.Contact x in Contacts)
            {
                dataGridView1.BeginInvoke(new MethodInvoker(()=>
                {
                    AppendRow(x);


                }));
            }

        }

        private void Web_on_scanQR()
        {
            Console.WriteLine("scan qr");
        }

        private void Web_on_scanQR_conform()
        {
            pictureBox1.BeginInvoke(new MethodInvoker(()=> {
                pictureBox1.Hide();
                dataGridView2.Show();
            }));
        }

        private string chatId;

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            int index = e.RowIndex;
            DataGridViewRow row = dataGridView1.Rows[index];
            String value = (String) row.Cells[0].Value;
            label1.Text = value;
            chatId = (String)row.Cells[1].Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string err = "";
            web.mm_webwxsendmsg(WebMMengine.sendMsgType.文字, chatId, textBox1.Text,ref err);
            addMsg(null, label1.Text, textBox1.Text);
            textBox1.Text = "";

        }

        public void addMsg(String fromUserName,String toUserName,String content)
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
            if(e.KeyValue == 13)
            {
                button1_Click(null, null);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }



}
