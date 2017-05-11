namespace YamWebRobot
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.dataGridView2 = new System.Windows.Forms.DataGridView();
            this.sender = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.receiver = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.contentTxt = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.selectPicBtn = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.lblInfo = new System.Windows.Forms.Label();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wxid = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.headUrl = new System.Windows.Forms.DataGridViewImageColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.type,
            this.name,
            this.wxid,
            this.headUrl});
            this.dataGridView1.Location = new System.Drawing.Point(-1, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(402, 442);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellClick);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(565, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(169, 151);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // dataGridView2
            // 
            this.dataGridView2.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView2.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.sender,
            this.receiver,
            this.time,
            this.content});
            this.dataGridView2.Location = new System.Drawing.Point(407, 46);
            this.dataGridView2.MultiSelect = false;
            this.dataGridView2.Name = "dataGridView2";
            this.dataGridView2.RowTemplate.Height = 23;
            this.dataGridView2.Size = new System.Drawing.Size(536, 314);
            this.dataGridView2.TabIndex = 2;
            // 
            // sender
            // 
            this.sender.HeaderText = "发送者";
            this.sender.Name = "sender";
            this.sender.Width = 65;
            // 
            // receiver
            // 
            this.receiver.HeaderText = "接收者";
            this.receiver.Name = "receiver";
            this.receiver.Width = 65;
            // 
            // time
            // 
            this.time.HeaderText = "时间";
            this.time.Name = "time";
            this.time.Width = 80;
            // 
            // content
            // 
            this.content.HeaderText = "内容";
            this.content.Name = "content";
            this.content.Width = 350;
            // 
            // contentTxt
            // 
            this.contentTxt.Location = new System.Drawing.Point(426, 372);
            this.contentTxt.Multiline = true;
            this.contentTxt.Name = "contentTxt";
            this.contentTxt.Size = new System.Drawing.Size(366, 45);
            this.contentTxt.TabIndex = 3;
            this.contentTxt.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(810, 375);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(133, 42);
            this.button1.TabIndex = 4;
            this.button1.Text = "发送文字";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // selectPicBtn
            // 
            this.selectPicBtn.Location = new System.Drawing.Point(426, 433);
            this.selectPicBtn.Name = "selectPicBtn";
            this.selectPicBtn.Size = new System.Drawing.Size(366, 30);
            this.selectPicBtn.TabIndex = 5;
            this.selectPicBtn.Text = "选择图片";
            this.selectPicBtn.UseVisualStyleBackColor = true;
            this.selectPicBtn.Click += new System.EventHandler(this.selectPicBtn_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(810, 429);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(133, 38);
            this.button3.TabIndex = 6;
            this.button3.Text = "发送图片";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(426, 14);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(41, 12);
            this.lblInfo.TabIndex = 7;
            this.lblInfo.Text = "label1";
            // 
            // type
            // 
            this.type.HeaderText = "类型";
            this.type.Name = "type";
            this.type.Width = 60;
            // 
            // name
            // 
            this.name.HeaderText = "名字";
            this.name.Name = "name";
            this.name.Width = 120;
            // 
            // wxid
            // 
            this.wxid.HeaderText = "wxid";
            this.wxid.Name = "wxid";
            this.wxid.Width = 120;
            // 
            // headUrl
            // 
            this.headUrl.HeaderText = "头像";
            this.headUrl.Name = "headUrl";
            this.headUrl.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.headUrl.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.headUrl.Width = 50;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(955, 471);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.selectPicBtn);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.contentTxt);
            this.Controls.Add(this.dataGridView2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.DataGridView dataGridView2;
        private System.Windows.Forms.TextBox contentTxt;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button selectPicBtn;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.DataGridViewTextBoxColumn sender;
        private System.Windows.Forms.DataGridViewTextBoxColumn receiver;
        private System.Windows.Forms.DataGridViewTextBoxColumn time;
        private System.Windows.Forms.DataGridViewTextBoxColumn content;
        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn wxid;
        private System.Windows.Forms.DataGridViewImageColumn headUrl;
    }
}

