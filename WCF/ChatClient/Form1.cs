using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using ChatClient.ServiceReference;
using System.ServiceModel;
using System.Linq;
using System.Collections.Generic;

namespace ChatClient
{
    public partial class Form1 : Form, IServiceChatCallback
    {
        bool isConnected = false;
        ServiceChatClient client;
        int ID;

        public Form1()
        {
            InitializeComponent();
        }

        void ConnectUser()
        {
            if (!isConnected)
            {
                client = new ServiceChatClient(new System.ServiceModel.InstanceContext(this));
                ID = client.Connect(textBox1.Text);
                isConnected = true;
                button1.Text = "Отключиться";
                textBox1.Enabled = false;

                AllData [] field = client.GetData();
                for (int i = 0; i < field.Length; i++)
                    listBox1.Items.Add(Convert.ToDateTime(field[i].Time).ToString("HH:mm:ss") + " " + field[i].Username + " " + field[i].Message);
            }
        }
        void DisconnectUser()
        {
            if (isConnected)
            {
                client.Disconnect(ID);
                client = null;
                isConnected = false;
                button1.Text = "Подключиться";
                textBox1.Enabled = true;

                listBox1.Items.Clear();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if(isConnected)
            {
                DisconnectUser();
            }
            else
            {
                ConnectUser();
            }
        }

        public void MsgCallBack(string msg)
        {
            listBox1.Items.Add(msg);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DisconnectUser();
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                if (client != null)
                {
                    client.SendMsg(textBox2.Text, ID);
                    client.NewMsg(ID, textBox2.Text, DateTime.Now.ToString());
                    textBox2.Text = ""; 
                }
            }
        }
    }
}
