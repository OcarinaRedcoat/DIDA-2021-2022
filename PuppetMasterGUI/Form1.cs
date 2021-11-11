using PuppetMasterGUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PuppetMasterGUI
{
    public partial class Form1 : Form
    {
        public PuppetMasterLogic logic;

        public bool debugFlag = false;

        private string logs = "";
        public string Logs
        {
            get { return logs; }
            set
            {
                logs = logs + value;
                outputs.Text = logs;
                outputs.SelectionStart = outputs.Text.Length;
                outputs.ScrollToCaret();
            }
        }

        public Form1()
        {
            InitializeComponent();
        }

        public void SetLogic(PuppetMasterLogic logic)
        {
            this.logic = logic;
        }

        public void AddLog(string log)
        {
            Logs = log + "\r\n";
        }

        private void createScheduler_Click(object sender, EventArgs e)
        {
            string serverId = schedulerServerId.Text;
            string url = schedulerURL.Text;
            logic.CreateScheduler(serverId, url);
            btnDebug.Enabled = false;
        }

        private void btnImportScriptFile_Click(object sender, EventArgs e)
        {
            string scriptFileName = txtScriptFileName.Text;
            logic.ImportScriptFile(scriptFileName);
            if (debugFlag)
            {
                txtDebug.Text = "Debug: True";
            }
            btnDebug.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void createWorker_Click(object sender, EventArgs e)
        {
            string serverId = workerServerId.Text;
            string url = workerURL.Text;
            int gossipDelay = Int32.Parse(workerGossipDelay.Text);
            logic.CreateWorker(serverId, url, gossipDelay);
            btnDebug.Enabled = false;
        }

        private void createStorage_Click(object sender, EventArgs e)
        {
            string serverId = storageServerId.Text;
            string url = storageURL.Text;
            int gossipDelay = Int32.Parse(storageGossipDelay.Text);
            logic.CreateStorage(serverId, url, gossipDelay);
            btnDebug.Enabled = false;
        }

        private void btnDebug_Click(object sender, EventArgs e)
        {
            logic.Debug();
            txtDebug.Text = "Debug: True";
            btnDebug.Enabled = false;
        }

        private void clientRun_Click(object sender, EventArgs e)
        {
            string input = inputClient.Text;
            string appFileName = appFile.Text;
            logic.ClientRequest(appFileName, input);
            btnDebug.Enabled = false;
        }

        private void populate_Click(object sender, EventArgs e)
        {
            string fileName = txtPopulateFile.Text;
            logic.Populate(fileName);
            btnDebug.Enabled = false;
        }

        private void status_Click(object sender, EventArgs e)
        {
            logic.Status();
            btnDebug.Enabled = false;
        }

        private void btnListServer_Click(object sender, EventArgs e)
        {
            string serverId = listServerId.Text;
            logic.ListServer(serverId);
            btnDebug.Enabled = false;
        }

        private void listGlobal_Click(object sender, EventArgs e)
        {
            logic.ListGlobal();
            btnDebug.Enabled = false;
        }

        private void btnCrash_Click(object sender, EventArgs e)
        {
            string serverId = txtCrashServerId.Text;
            logic.Crash(serverId);
            btnDebug.Enabled = false;
        }

        private void wait_Click(object sender, EventArgs e)
        {
            int waitInt = Int32.Parse(waitInterval.Text);
            logic.Wait(waitInt);
            btnDebug.Enabled = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            logic.Exit();
        }
    }
}
