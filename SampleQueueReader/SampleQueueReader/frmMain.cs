/// \file SampleQueueReader
///
/// \mainpage Advanced SQL (PROG3070) Milestone 01 - Sample Queue Reader
///
/// \section intro Program Introduction
/// - This class was used as a temporary UI for building the DAL
///   
/// \reference
/// - Advanced SQL Modules and Demos
///   Norbert Mika: Demo code
///
/// Major <b>frmMain.cs</b>
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Messaging;
using System.Diagnostics;
using DALlib;

namespace SampleQueueReader
{
    public partial class frmMain : Form
    {
        MessageQueue msmq = new MessageQueue();
        
        Boolean bRead = false;
        String queueName = "\\private$\\yoyo";
        DAL dataBase;
        Boolean connectionStatus;
        int queueSize;
        Boolean dataRemains;
        int currentTransactSize;
        int singleTransaction;

        int messageQueueTotal;

        int totalMessagesRead;

        public frmMain()
        {
            InitializeComponent();
            msmq.Formatter = new ActiveXMessageFormatter();
            msmq.MessageReadPropertyFilter.LookupId = true;
            msmq.SynchronizingObject = this;
            msmq.ReceiveCompleted += new ReceiveCompletedEventHandler(msmq_ReceiveCompleted);
            dataBase = new DAL();
            connectionStatus = false;
            queueSize = 0;
            dataRemains = false;
            currentTransactSize = 10;
            singleTransaction = 1;
            messageQueueTotal = 0;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstQueueData.Items.Clear();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            txtQueueServer.Text = System.Windows.Forms.SystemInformation.ComputerName;
            IsRunning(false);
        }

        /// <summary>
        /// Set the configuration
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            if ((txtQueueServer.Text == "") || (btnConnectDB.BackColor == Color.Red))
            {
                MessageBox.Show("Message Queue Server and Database Connection required!");
            }
            else
            {
                msmq.Path = "Formatname:Direct=os:" + txtQueueServer.Text + queueName;
                bRead = true;
                msmq.BeginReceive();
                IsRunning(true);
            }

        }

        /// <summary>
        /// when message is recieved, put into the queue and do transaction
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void msmq_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                queueSize = dataBase.AddManufacturingData(e.Message.Body.ToString());

                if (queueSize > currentTransactSize)
                {
                    currentTransactSize = dataBase.AddDataToDatabase(currentTransactSize);
                    
                    messageQueueTotal = GetMessageCount(msmq);

                    currentTransactSize = TransactionSize(messageQueueTotal);
                }

                msmq.EndReceive(e.AsyncResult);
                if (chkCount.Checked)
                {
                    txtRemaining.Text = GetMessageCount(msmq).ToString();
                    Application.DoEvents();
                }

                //System.Threading.Thread.Sleep(10);

                if (bRead)
                {
                    msmq.BeginReceive();
                }
            }
            catch
            {
                MessageBox.Show("Unhandled Exception");
            }
        }

        /// <summary>
        /// Control transaction size
        /// </summary>
        /// <param name="queueSize"></param>
        /// <returns></returns>
        public int TransactionSize(int queueSize)
        {
            if (queueSize == 1) { currentTransactSize = 1; }
            else if ((queueSize > 10) && (queueSize <= 50)) { currentTransactSize = 10; }
            else if ((queueSize > 50) && (queueSize <= 100)) { currentTransactSize = 50; }
            else if ((queueSize > 100) && (queueSize <= 250)) { currentTransactSize = 100; }
            else if ((queueSize > 250) && (queueSize <= 500)) { currentTransactSize = 250; }
            else if ((queueSize > 500) && (queueSize <= 1000)) { currentTransactSize = 500; }
            else if ((queueSize > 1000) && (queueSize <= 5000)) { currentTransactSize = 1000; }
            else if ((queueSize > 5000) && (queueSize <= 10000)) { currentTransactSize = 5000; }
            else if (queueSize > 10000) { currentTransactSize = 10000; }

            return currentTransactSize;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bRead = false;
            IsRunning(false);
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            lstQueueData.Items.Clear();
        }

        /// <summary>
        /// check how many messages left
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private int GetMessageCount(MessageQueue m)
        {
            Int32 count = 0;
            MessageEnumerator msgEnum = m.GetMessageEnumerator2();
            while (msgEnum.MoveNext(new TimeSpan(0, 0, 0)))
            {
                count++;
            }
            return count;
        }

        /// <summary>
        /// check the status 
        /// </summary>
        /// <param name="state"></param>
        private void IsRunning(Boolean state)
        {
            if (state == true)
            {
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                btnSingleRead.Enabled = false;
            }
            else
            {
                btnStart.Enabled = true;
                btnStop.Enabled = false ;
                btnSingleRead.Enabled = true;
            }
        }

        /// <summary>
        /// Read the messages on by one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSingleRead_Click(object sender, EventArgs e)
        {
            if ((txtQueueServer.Text == "") || (btnConnectDB.BackColor == Color.Red))
            {
                MessageBox.Show("Message Queue Server and Database Connection required");
            }
            else
            {
                msmq.Path = "Formatname:Direct=os:" + txtQueueServer.Text + queueName;
                try
                {
                    System.Messaging.Message msg = msmq.Receive(new TimeSpan(0));
                    if (msg != null)
                    {
                        queueSize = dataBase.AddManufacturingData(msg.Body.ToString());
                        singleTransaction = dataBase.AddDataToDatabase(singleTransaction);
                    }
                }
                catch (Exception ex)
                {
                    
                    MessageBox.Show("Insert into database Error: " + ex.ToString());
                }
            }

        }

        /// <summary>
        /// Clean message queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPurgeQ_Click(object sender, EventArgs e)
        {
            msmq.Purge();
        }

        /// <summary>
        /// Connect with Database
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConnectDB_Click(object sender, EventArgs e)
        {
            if ((connectionStatus == false) && (dataRemains == false))
            {
                connectionStatus = dataBase.OpenConnection("sa", "Conestoga1");
                if (connectionStatus) 
                { 
                    lstQueueData.Items.Insert(0, "Connection to yoyo Database was made!");
                    btnConnectDB.BackColor = Color.Green;
                    connectionStatus = true;
                }
                else 
                { 
                    lstQueueData.Items.Insert(0, DAL.sqlException); 
                }
                
            }
            else if (connectionStatus == true)
            {
                btnConnectDB.BackColor = Color.Orange;
                lstQueueData.Items.Insert(0, "Saving current queue backload...");
                btnStop_Click(sender, e);
                connectionStatus = dataBase.CloseConnection();
                if (!connectionStatus) 
                { 
                    lstQueueData.Items.Insert(0, "Connection to yoyo Database was successfully closed!");
                  
                    btnConnectDB.BackColor = Color.Red;
                    dataRemains = false;
                }
                else 
                { 
                    lstQueueData.Items.Insert(0, "Cannot close connection either due to data still remaining in queue or and exception occured!\n\n" + DAL.sqlException);
                    dataRemains = true;
                }
            }
        }

        /// <summary>
        /// Check the messages still in the queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if ((dataRemains == true) || (connectionStatus = true))
            {
                lstQueueData.Items.Insert(0, "\n\nDatabase connection is still open and/or data still remains in the queue.\n\n\n" +
                                                "Attempting to close DB connection first...");
                bRead = false;
                
                btnConnectDB_Click(sender, e);
            }
        }

    }
}
