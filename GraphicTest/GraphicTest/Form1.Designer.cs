namespace GraphicTest
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.renderBtn = new System.Windows.Forms.Button();
            this.textureBtn = new System.Windows.Forms.Button();
            this.cullingBtn = new System.Windows.Forms.Button();
            this.lightBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(136, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 600);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // renderBtn
            // 
            this.renderBtn.Location = new System.Drawing.Point(30, 20);
            this.renderBtn.Name = "renderBtn";
            this.renderBtn.Size = new System.Drawing.Size(75, 23);
            this.renderBtn.TabIndex = 1;
            this.renderBtn.Text = "线框";
            this.renderBtn.UseVisualStyleBackColor = true;
            this.renderBtn.Click += new System.EventHandler(this.RenderBtn_Click);
            // 
            // textureBtn
            // 
            this.textureBtn.Location = new System.Drawing.Point(30, 85);
            this.textureBtn.Name = "textureBtn";
            this.textureBtn.Size = new System.Drawing.Size(75, 23);
            this.textureBtn.TabIndex = 2;
            this.textureBtn.Text = "显示贴图";
            this.textureBtn.UseVisualStyleBackColor = true;
            this.textureBtn.Click += new System.EventHandler(this.textureBtn_Click);
            // 
            // cullingBtn
            // 
            this.cullingBtn.Location = new System.Drawing.Point(30, 148);
            this.cullingBtn.Name = "cullingBtn";
            this.cullingBtn.Size = new System.Drawing.Size(75, 23);
            this.cullingBtn.TabIndex = 3;
            this.cullingBtn.Text = "裁剪";
            this.cullingBtn.UseVisualStyleBackColor = true;
            this.cullingBtn.Click += new System.EventHandler(this.cullingBtn_Click);
            // 
            // lightBtn
            // 
            this.lightBtn.Location = new System.Drawing.Point(29, 202);
            this.lightBtn.Name = "lightBtn";
            this.lightBtn.Size = new System.Drawing.Size(75, 23);
            this.lightBtn.TabIndex = 4;
            this.lightBtn.Text = "灯光";
            this.lightBtn.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 643);
            this.Controls.Add(this.lightBtn);
            this.Controls.Add(this.cullingBtn);
            this.Controls.Add(this.textureBtn);
            this.Controls.Add(this.renderBtn);
            this.Controls.Add(this.pictureBox1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button renderBtn;
        private System.Windows.Forms.Button textureBtn;
        private System.Windows.Forms.Button cullingBtn;
        private System.Windows.Forms.Button lightBtn;
    }
}

