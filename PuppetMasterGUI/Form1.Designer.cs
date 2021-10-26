
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
            this.btnImportScriptFile = new System.Windows.Forms.Button();
            this.txtScriptFileName = new System.Windows.Forms.TextBox();
            this.outputs = new System.Windows.Forms.TextBox();
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
            this.txtPopulateFile = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.status = new System.Windows.Forms.Button();
            this.btnListServer = new System.Windows.Forms.Button();
            this.label11 = new System.Windows.Forms.Label();
            this.listServerId = new System.Windows.Forms.TextBox();
            this.listGlobal = new System.Windows.Forms.Button();
            this.txtCrashServerId = new System.Windows.Forms.TextBox();
            this.btnCrash = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.waitInterval = new System.Windows.Forms.TextBox();
            this.wait = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.btnDebug = new System.Windows.Forms.Button();
            this.txtDebug = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnImportScriptFile
            // 
            this.btnImportScriptFile.Location = new System.Drawing.Point(274, 16);
            this.btnImportScriptFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnImportScriptFile.Name = "btnImportScriptFile";
            this.btnImportScriptFile.Size = new System.Drawing.Size(151, 31);
            this.btnImportScriptFile.TabIndex = 0;
            this.btnImportScriptFile.Text = "Script File Name";
            this.btnImportScriptFile.UseVisualStyleBackColor = true;
            this.btnImportScriptFile.Click += new System.EventHandler(this.btnImportScriptFile_Click);
            // 
            // txtScriptFileName
            // 
            this.txtScriptFileName.Location = new System.Drawing.Point(14, 17);
            this.txtScriptFileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtScriptFileName.Name = "txtScriptFileName";
            this.txtScriptFileName.Size = new System.Drawing.Size(238, 27);
            this.txtScriptFileName.TabIndex = 1;
            this.txtScriptFileName.Text = "sample_script";
            // 
            // outputs
            // 
            this.outputs.Location = new System.Drawing.Point(903, 17);
            this.outputs.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.outputs.Multiline = true;
            this.outputs.Name = "outputs";
            this.outputs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputs.Size = new System.Drawing.Size(478, 603);
            this.outputs.TabIndex = 2;
            // 
            // schedulerServerId
            // 
            this.schedulerServerId.Location = new System.Drawing.Point(80, 103);
            this.schedulerServerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.schedulerServerId.Name = "schedulerServerId";
            this.schedulerServerId.Size = new System.Drawing.Size(114, 27);
            this.schedulerServerId.TabIndex = 5;
            // 
            // schedulerURL
            // 
            this.schedulerURL.Location = new System.Drawing.Point(259, 101);
            this.schedulerURL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.schedulerURL.Name = "schedulerURL";
            this.schedulerURL.Size = new System.Drawing.Size(154, 27);
            this.schedulerURL.TabIndex = 6;
            // 
            // workerGossipDelay
            // 
            this.workerGossipDelay.Location = new System.Drawing.Point(558, 163);
            this.workerGossipDelay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.workerGossipDelay.Name = "workerGossipDelay";
            this.workerGossipDelay.Size = new System.Drawing.Size(114, 27);
            this.workerGossipDelay.TabIndex = 7;
            // 
            // createScheduler
            // 
            this.createScheduler.Location = new System.Drawing.Point(453, 103);
            this.createScheduler.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.createScheduler.Name = "createScheduler";
            this.createScheduler.Size = new System.Drawing.Size(151, 31);
            this.createScheduler.TabIndex = 8;
            this.createScheduler.Text = "Create Scheduler";
            this.createScheduler.UseVisualStyleBackColor = true;
            this.createScheduler.Click += new System.EventHandler(this.createScheduler_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 107);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 20);
            this.label1.TabIndex = 9;
            this.label1.Text = "Server Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(221, 107);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 20);
            this.label2.TabIndex = 10;
            this.label2.Text = "URL";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(221, 167);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 20);
            this.label3.TabIndex = 15;
            this.label3.Text = "URL";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 167);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 20);
            this.label4.TabIndex = 14;
            this.label4.Text = "Server Id";
            // 
            // createWorker
            // 
            this.createWorker.Location = new System.Drawing.Point(719, 163);
            this.createWorker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.createWorker.Name = "createWorker";
            this.createWorker.Size = new System.Drawing.Size(151, 31);
            this.createWorker.TabIndex = 13;
            this.createWorker.Text = "Create Worker";
            this.createWorker.UseVisualStyleBackColor = true;
            this.createWorker.Click += new System.EventHandler(this.createWorker_Click);
            // 
            // workerURL
            // 
            this.workerURL.Location = new System.Drawing.Point(259, 161);
            this.workerURL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.workerURL.Name = "workerURL";
            this.workerURL.Size = new System.Drawing.Size(154, 27);
            this.workerURL.TabIndex = 12;
            // 
            // workerServerId
            // 
            this.workerServerId.Location = new System.Drawing.Point(80, 163);
            this.workerServerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.workerServerId.Name = "workerServerId";
            this.workerServerId.Size = new System.Drawing.Size(114, 27);
            this.workerServerId.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(450, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 20);
            this.label5.TabIndex = 16;
            this.label5.Text = "Gossip Delay";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(450, 229);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(95, 20);
            this.label6.TabIndex = 23;
            this.label6.Text = "Gossip Delay";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(221, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(35, 20);
            this.label7.TabIndex = 22;
            this.label7.Text = "URL";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(14, 229);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 20);
            this.label8.TabIndex = 21;
            this.label8.Text = "Server Id";
            // 
            // createStorage
            // 
            this.createStorage.Location = new System.Drawing.Point(719, 225);
            this.createStorage.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.createStorage.Name = "createStorage";
            this.createStorage.Size = new System.Drawing.Size(151, 31);
            this.createStorage.TabIndex = 20;
            this.createStorage.Text = "Create Storage";
            this.createStorage.UseVisualStyleBackColor = true;
            this.createStorage.Click += new System.EventHandler(this.createStorage_Click);
            // 
            // storageURL
            // 
            this.storageURL.Location = new System.Drawing.Point(259, 224);
            this.storageURL.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.storageURL.Name = "storageURL";
            this.storageURL.Size = new System.Drawing.Size(154, 27);
            this.storageURL.TabIndex = 19;
            // 
            // storageServerId
            // 
            this.storageServerId.Location = new System.Drawing.Point(80, 225);
            this.storageServerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.storageServerId.Name = "storageServerId";
            this.storageServerId.Size = new System.Drawing.Size(114, 27);
            this.storageServerId.TabIndex = 18;
            // 
            // storageGossipDelay
            // 
            this.storageGossipDelay.Location = new System.Drawing.Point(558, 225);
            this.storageGossipDelay.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.storageGossipDelay.Name = "storageGossipDelay";
            this.storageGossipDelay.Size = new System.Drawing.Size(114, 27);
            this.storageGossipDelay.TabIndex = 17;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(14, 313);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 20);
            this.label9.TabIndex = 24;
            this.label9.Text = "Client Input";
            // 
            // appFile
            // 
            this.appFile.Location = new System.Drawing.Point(318, 309);
            this.appFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.appFile.Name = "appFile";
            this.appFile.Size = new System.Drawing.Size(285, 27);
            this.appFile.TabIndex = 25;
            // 
            // inputClient
            // 
            this.inputClient.Location = new System.Drawing.Point(99, 309);
            this.inputClient.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.inputClient.Name = "inputClient";
            this.inputClient.Size = new System.Drawing.Size(114, 27);
            this.inputClient.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(239, 313);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 20);
            this.label10.TabIndex = 27;
            this.label10.Text = "App File";
            // 
            // clientRun
            // 
            this.clientRun.Location = new System.Drawing.Point(719, 308);
            this.clientRun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.clientRun.Name = "clientRun";
            this.clientRun.Size = new System.Drawing.Size(151, 31);
            this.clientRun.TabIndex = 28;
            this.clientRun.Text = "Client";
            this.clientRun.UseVisualStyleBackColor = true;
            this.clientRun.Click += new System.EventHandler(this.clientRun_Click);
            // 
            // populate
            // 
            this.populate.Location = new System.Drawing.Point(719, 383);
            this.populate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.populate.Name = "populate";
            this.populate.Size = new System.Drawing.Size(151, 31);
            this.populate.TabIndex = 33;
            this.populate.Text = "Populate";
            this.populate.UseVisualStyleBackColor = true;
            this.populate.Click += new System.EventHandler(this.populate_Click);
            // 
            // txtPopulateFile
            // 
            this.txtPopulateFile.Location = new System.Drawing.Point(130, 383);
            this.txtPopulateFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPopulateFile.Name = "txtPopulateFile";
            this.txtPopulateFile.Size = new System.Drawing.Size(558, 27);
            this.txtPopulateFile.TabIndex = 31;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(14, 388);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(119, 20);
            this.label12.TabIndex = 29;
            this.label12.Text = "Populate File Dir";
            // 
            // status
            // 
            this.status.Location = new System.Drawing.Point(14, 471);
            this.status.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.status.Name = "status";
            this.status.Size = new System.Drawing.Size(151, 31);
            this.status.TabIndex = 34;
            this.status.Text = "Status";
            this.status.UseVisualStyleBackColor = true;
            this.status.Click += new System.EventHandler(this.status_Click);
            // 
            // btnListServer
            // 
            this.btnListServer.Location = new System.Drawing.Point(439, 469);
            this.btnListServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnListServer.Name = "btnListServer";
            this.btnListServer.Size = new System.Drawing.Size(151, 31);
            this.btnListServer.TabIndex = 35;
            this.btnListServer.Text = "List Server";
            this.btnListServer.UseVisualStyleBackColor = true;
            this.btnListServer.Click += new System.EventHandler(this.btnListServer_Click);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(238, 473);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(67, 20);
            this.label11.TabIndex = 37;
            this.label11.Text = "Server Id";
            // 
            // listServerId
            // 
            this.listServerId.Location = new System.Drawing.Point(304, 469);
            this.listServerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listServerId.Name = "listServerId";
            this.listServerId.Size = new System.Drawing.Size(114, 27);
            this.listServerId.TabIndex = 36;
            // 
            // listGlobal
            // 
            this.listGlobal.Location = new System.Drawing.Point(719, 469);
            this.listGlobal.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.listGlobal.Name = "listGlobal";
            this.listGlobal.Size = new System.Drawing.Size(151, 31);
            this.listGlobal.TabIndex = 38;
            this.listGlobal.Text = "List Global";
            this.listGlobal.UseVisualStyleBackColor = true;
            this.listGlobal.Click += new System.EventHandler(this.listGlobal_Click);
            // 
            // txtCrashServerId
            // 
            this.txtCrashServerId.Location = new System.Drawing.Point(86, 591);
            this.txtCrashServerId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCrashServerId.Name = "txtCrashServerId";
            this.txtCrashServerId.Size = new System.Drawing.Size(114, 27);
            this.txtCrashServerId.TabIndex = 40;
            // 
            // btnCrash
            // 
            this.btnCrash.Location = new System.Drawing.Point(221, 591);
            this.btnCrash.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCrash.Name = "btnCrash";
            this.btnCrash.Size = new System.Drawing.Size(151, 31);
            this.btnCrash.TabIndex = 39;
            this.btnCrash.Text = "Crash";
            this.btnCrash.UseVisualStyleBackColor = true;
            this.btnCrash.Click += new System.EventHandler(this.btnCrash_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(482, 596);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(93, 20);
            this.label14.TabIndex = 44;
            this.label14.Text = "Wait Interval";
            // 
            // waitInterval
            // 
            this.waitInterval.Location = new System.Drawing.Point(584, 591);
            this.waitInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.waitInterval.Name = "waitInterval";
            this.waitInterval.Size = new System.Drawing.Size(114, 27);
            this.waitInterval.TabIndex = 43;
            // 
            // wait
            // 
            this.wait.Location = new System.Drawing.Point(719, 589);
            this.wait.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.wait.Name = "wait";
            this.wait.Size = new System.Drawing.Size(151, 31);
            this.wait.TabIndex = 42;
            this.wait.Text = "Wait";
            this.wait.UseVisualStyleBackColor = true;
            this.wait.Click += new System.EventHandler(this.wait_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(19, 595);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(67, 20);
            this.label13.TabIndex = 41;
            this.label13.Text = "Server Id";
            // 
            // btnDebug
            // 
            this.btnDebug.Location = new System.Drawing.Point(508, 17);
            this.btnDebug.Name = "btnDebug";
            this.btnDebug.Size = new System.Drawing.Size(94, 29);
            this.btnDebug.TabIndex = 45;
            this.btnDebug.Text = "Debug";
            this.btnDebug.UseVisualStyleBackColor = true;
            this.btnDebug.Click += new System.EventHandler(this.btnDebug_Click);
            // 
            // txtDebug
            // 
            this.txtDebug.Enabled = false;
            this.txtDebug.Location = new System.Drawing.Point(620, 18);
            this.txtDebug.Name = "txtDebug";
            this.txtDebug.Size = new System.Drawing.Size(104, 27);
            this.txtDebug.TabIndex = 46;
            this.txtDebug.Text = "Debug: False";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1413, 644);
            this.Controls.Add(this.txtDebug);
            this.Controls.Add(this.btnDebug);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.waitInterval);
            this.Controls.Add(this.wait);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txtCrashServerId);
            this.Controls.Add(this.btnCrash);
            this.Controls.Add(this.listGlobal);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.listServerId);
            this.Controls.Add(this.btnListServer);
            this.Controls.Add(this.status);
            this.Controls.Add(this.populate);
            this.Controls.Add(this.txtPopulateFile);
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
            this.Controls.Add(this.outputs);
            this.Controls.Add(this.txtScriptFileName);
            this.Controls.Add(this.btnImportScriptFile);
            this.Name = "Form1";
            this.Text = "PuppetMaster";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnImportScriptFile;
        private System.Windows.Forms.TextBox txtScriptFileName;
        private System.Windows.Forms.TextBox outputs;
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
        private System.Windows.Forms.TextBox txtPopulateFile;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button status;
        private System.Windows.Forms.Button btnListServer;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox listServerId;
        private System.Windows.Forms.Button listGlobal;
        private System.Windows.Forms.TextBox txtCrashServerId;
        private System.Windows.Forms.Button btnCrash;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox waitInterval;
        private System.Windows.Forms.Button wait;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnDebug;
        private System.Windows.Forms.TextBox txtDebug;
    }
}

