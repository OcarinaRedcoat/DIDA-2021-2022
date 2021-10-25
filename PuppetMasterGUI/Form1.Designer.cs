
namespace PuppetMasterGUI
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.outputs = new System.Windows.Forms.TextBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.schedulerServerId = new System.Windows.Forms.TextBox();
            this.schedulerURL = new System.Windows.Forms.TextBox();
            this.workerGossipDelay = new System.Windows.Forms.TextBox();
            this.createScheduler = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.createWorker = new System.Windows.Forms.Button();
            this.workerURL = new System.Windows.Forms.TextBox();
            this.workerServerId = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.createStorage = new System.Windows.Forms.Button();
            this.storageURL = new System.Windows.Forms.TextBox();
            this.storageServerId = new System.Windows.Forms.TextBox();
            this.storageGossipDelay = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.appFile = new System.Windows.Forms.TextBox();
            this.inputClient = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.clientRun = new System.Windows.Forms.Button();
            this.populate = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.status = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.listServerId = new System.Windows.Forms.TextBox();
            this.listGlobal = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.waitInterval = new System.Windows.Forms.TextBox();
            this.wait = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(240, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(132, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Script File Name";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 13);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(209, 23);
            this.textBox1.TabIndex = 1;
            // 
            // outputs
            // 
            this.outputs.Location = new System.Drawing.Point(790, 13);
            this.outputs.Multiline = true;
            this.outputs.Name = "outputs";
            this.outputs.Size = new System.Drawing.Size(419, 453);
            this.outputs.TabIndex = 2;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(455, 17);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(61, 19);
            this.checkBox2.TabIndex = 4;
            this.checkBox2.Text = "Debug";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // schedulerServerId
            // 
            this.schedulerServerId.Location = new System.Drawing.Point(70, 77);
            this.schedulerServerId.Name = "schedulerServerId";
            this.schedulerServerId.Size = new System.Drawing.Size(100, 23);
            this.schedulerServerId.TabIndex = 5;
            this.schedulerServerId.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // schedulerURL
            // 
            this.schedulerURL.Location = new System.Drawing.Point(227, 76);
            this.schedulerURL.Name = "schedulerURL";
            this.schedulerURL.Size = new System.Drawing.Size(135, 23);
            this.schedulerURL.TabIndex = 6;
            // 
            // workerGossipDelay
            // 
            this.workerGossipDelay.Location = new System.Drawing.Point(488, 122);
            this.workerGossipDelay.Name = "workerGossipDelay";
            this.workerGossipDelay.Size = new System.Drawing.Size(100, 23);
            this.workerGossipDelay.TabIndex = 7;
            // 
            // createScheduler
            // 
            this.createScheduler.Location = new System.Drawing.Point(396, 77);
            this.createScheduler.Name = "createScheduler";
            this.createScheduler.Size = new System.Drawing.Size(132, 23);
            this.createScheduler.TabIndex = 8;
            this.createScheduler.Text = "Create Scheduler";
            this.createScheduler.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(52, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Server Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(193, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 15);
            this.label2.TabIndex = 10;
            this.label2.Text = "URL";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(193, 125);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(28, 15);
            this.label3.TabIndex = 15;
            this.label3.Text = "URL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 125);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(52, 15);
            this.label4.TabIndex = 14;
            this.label4.Text = "Server Id";
            // 
            // createWorker
            // 
            this.createWorker.Location = new System.Drawing.Point(629, 122);
            this.createWorker.Name = "createWorker";
            this.createWorker.Size = new System.Drawing.Size(132, 23);
            this.createWorker.TabIndex = 13;
            this.createWorker.Text = "Create Worker";
            this.createWorker.UseVisualStyleBackColor = true;
            // 
            // workerURL
            // 
            this.workerURL.Location = new System.Drawing.Point(227, 121);
            this.workerURL.Name = "workerURL";
            this.workerURL.Size = new System.Drawing.Size(135, 23);
            this.workerURL.TabIndex = 12;
            // 
            // workerServerId
            // 
            this.workerServerId.Location = new System.Drawing.Point(70, 122);
            this.workerServerId.Name = "workerServerId";
            this.workerServerId.Size = new System.Drawing.Size(100, 23);
            this.workerServerId.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(394, 125);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 15);
            this.label5.TabIndex = 16;
            this.label5.Text = "Gossip Delay";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(394, 172);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 15);
            this.label6.TabIndex = 23;
            this.label6.Text = "Gossip Delay";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(193, 172);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(28, 15);
            this.label7.TabIndex = 22;
            this.label7.Text = "URL";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 172);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 15);
            this.label8.TabIndex = 21;
            this.label8.Text = "Server Id";
            // 
            // createStorage
            // 
            this.createStorage.Location = new System.Drawing.Point(629, 169);
            this.createStorage.Name = "createStorage";
            this.createStorage.Size = new System.Drawing.Size(132, 23);
            this.createStorage.TabIndex = 20;
            this.createStorage.Text = "Create Storage";
            this.createStorage.UseVisualStyleBackColor = true;
            // 
            // storageURL
            // 
            this.storageURL.Location = new System.Drawing.Point(227, 168);
            this.storageURL.Name = "storageURL";
            this.storageURL.Size = new System.Drawing.Size(135, 23);
            this.storageURL.TabIndex = 19;
            // 
            // storageServerId
            // 
            this.storageServerId.Location = new System.Drawing.Point(70, 169);
            this.storageServerId.Name = "storageServerId";
            this.storageServerId.Size = new System.Drawing.Size(100, 23);
            this.storageServerId.TabIndex = 18;
            // 
            // storageGossipDelay
            // 
            this.storageGossipDelay.Location = new System.Drawing.Point(488, 169);
            this.storageGossipDelay.Name = "storageGossipDelay";
            this.storageGossipDelay.Size = new System.Drawing.Size(100, 23);
            this.storageGossipDelay.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(12, 235);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(69, 15);
            this.label9.TabIndex = 24;
            this.label9.Text = "Client Input";
            // 
            // appFile
            // 
            this.appFile.Location = new System.Drawing.Point(278, 232);
            this.appFile.Name = "appFile";
            this.appFile.Size = new System.Drawing.Size(250, 23);
            this.appFile.TabIndex = 25;
            // 
            // inputClient
            // 
            this.inputClient.Location = new System.Drawing.Point(87, 232);
            this.inputClient.Name = "inputClient";
            this.inputClient.Size = new System.Drawing.Size(100, 23);
            this.inputClient.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(209, 235);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(50, 15);
            this.label10.TabIndex = 27;
            this.label10.Text = "App File";
            // 
            // clientRun
            // 
            this.clientRun.Location = new System.Drawing.Point(629, 231);
            this.clientRun.Name = "clientRun";
            this.clientRun.Size = new System.Drawing.Size(132, 23);
            this.clientRun.TabIndex = 28;
            this.clientRun.Text = "Client";
            this.clientRun.UseVisualStyleBackColor = true;
            // 
            // populate
            // 
            this.populate.Location = new System.Drawing.Point(629, 287);
            this.populate.Name = "populate";
            this.populate.Size = new System.Drawing.Size(132, 23);
            this.populate.TabIndex = 33;
            this.populate.Text = "Populate";
            this.populate.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(111, 287);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(491, 23);
            this.textBox2.TabIndex = 31;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 291);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(93, 15);
            this.label12.TabIndex = 29;
            this.label12.Text = "Populate File Dir";
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(12, 353);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(132, 23);
            this.status.TabIndex = 34;
            this.status.Text = "Status";
            this.status.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(384, 352);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(132, 23);
            this.button2.TabIndex = 35;
            this.button2.Text = "List Server";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(208, 355);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(52, 15);
            this.label11.TabIndex = 37;
            this.label11.Text = "Server Id";
            // 
            // listServerId
            // 
            this.listServerId.Location = new System.Drawing.Point(266, 352);
            this.listServerId.Name = "listServerId";
            this.listServerId.Size = new System.Drawing.Size(100, 23);
            this.listServerId.TabIndex = 36;
            // 
            // listGlobal
            // 
            this.listGlobal.Location = new System.Drawing.Point(629, 352);
            this.listGlobal.Name = "listGlobal";
            this.listGlobal.Size = new System.Drawing.Size(132, 23);
            this.listGlobal.TabIndex = 38;
            this.listGlobal.Text = "List Global";
            this.listGlobal.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(75, 443);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(100, 23);
            this.textBox3.TabIndex = 40;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(193, 443);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(132, 23);
            this.button3.TabIndex = 39;
            this.button3.Text = "Crash";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(422, 447);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(73, 15);
            this.label14.TabIndex = 44;
            this.label14.Text = "Wait Interval";
            // 
            // waitInterval
            // 
            this.waitInterval.Location = new System.Drawing.Point(511, 443);
            this.waitInterval.Name = "waitInterval";
            this.waitInterval.Size = new System.Drawing.Size(100, 23);
            this.waitInterval.TabIndex = 43;
            // 
            // wait
            // 
            this.wait.Location = new System.Drawing.Point(629, 442);
            this.wait.Name = "wait";
            this.wait.Size = new System.Drawing.Size(132, 23);
            this.wait.TabIndex = 42;
            this.wait.Text = "Wait";
            this.wait.UseVisualStyleBackColor = true;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(17, 446);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(52, 15);
            this.label13.TabIndex = 41;
            this.label13.Text = "Server Id";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1236, 483);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.waitInterval);
            this.Controls.Add(this.wait);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.listGlobal);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.listServerId);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.status);
            this.Controls.Add(this.populate);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.clientRun);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.inputClient);
            this.Controls.Add(this.appFile);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.createStorage);
            this.Controls.Add(this.storageURL);
            this.Controls.Add(this.storageServerId);
            this.Controls.Add(this.storageGossipDelay);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.createWorker);
            this.Controls.Add(this.workerURL);
            this.Controls.Add(this.workerServerId);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.createScheduler);
            this.Controls.Add(this.workerGossipDelay);
            this.Controls.Add(this.schedulerURL);
            this.Controls.Add(this.schedulerServerId);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.outputs);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "PuppetMaster";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox outputs;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.TextBox schedulerServerId;
        private System.Windows.Forms.TextBox schedulerURL;
        private System.Windows.Forms.TextBox workerGossipDelay;
        private System.Windows.Forms.Button createScheduler;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button createWorker;
        private System.Windows.Forms.TextBox workerURL;
        private System.Windows.Forms.TextBox workerServerId;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button createStorage;
        private System.Windows.Forms.TextBox storageURL;
        private System.Windows.Forms.TextBox storageServerId;
        private System.Windows.Forms.TextBox storageGossipDelay;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox appFile;
        private System.Windows.Forms.TextBox inputClient;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button clientRun;
        private System.Windows.Forms.Button populate;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button status;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox listServerId;
        private System.Windows.Forms.Button listGlobal;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox waitInterval;
        private System.Windows.Forms.Button wait;
        private System.Windows.Forms.Label label13;
    }
}

