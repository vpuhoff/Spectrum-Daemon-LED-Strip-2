using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SendBytes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            serialPort1.Open();
            serialPort1.DataReceived += serialPort1_DataReceived;
        }


        void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            ready = true;
            //MessageBox.Show(serialPort1.ReadByte().ToString());
        }

        //protected override void WndProc(ref Message m)
        //{
        //    //if (m.Msg == WM_POWERBROADCAST)
        //    //{
        //    //    if (m.WParam.ToInt32() == PBT_APMSUSPEND)
        //    //    {
        //    //        timer1.Enabled = false;
        //    //        for (int i = 0; i < 255 - 1; i += 1)
        //    //        {
        //    //            while (!ready)
        //    //            {
        //    //                Application.DoEvents();
        //    //            }
        //    //            SendData(200, i, 0, 0);
        //    //            Thread.Sleep(10);
        //    //        }
        //    //        for (int i = 0; i < 255 - 1; i += 1)
        //    //        {
        //    //            while (!ready)
        //    //            {
        //    //                Application.DoEvents();
        //    //            }
        //    //            SendData(200, 255 - i, 0, 0);
        //    //            Thread.Sleep(10);
        //    //        }
        //    //        serialPort1.Close();
        //    //        // Код, который нужно выполнить при засыпании
        //    //    }
        //    //    else if (m.WParam.ToInt32() == PBT_APMRESUMESUSPEND)
        //    //    {
        //    //        serialPort1.Open();
        //    //        for (int i = 0; i < 255 - 1; i += 1)
        //    //        {
        //    //            while (!ready)
        //    //            {
        //    //                Application.DoEvents();
        //    //            }
        //    //            SendData(200, 0, i, 0);
        //    //            Thread.Sleep(10);
        //    //        }
        //    //        for (int i = 0; i < 255 - 1; i += 1)
        //    //        {
        //    //            while (!ready)
        //    //            {
        //    //                Application.DoEvents();
        //    //            }
        //    //            SendData(200, 0, 255 - i, 0);
        //    //            Thread.Sleep(10);
        //    //        }
        //    //        timer1.Enabled = true;
        //    //        // Код, который нужно выполнить при пробуждении
        //    //    }
        //    //}

        //    //base.WndProc(ref m);
        //}
        //byte[] data;
        string s;
        const int WM_POWERBROADCAST = 0x218;
        const int PBT_APMSUSPEND = 0x0004;
        const int PBT_APMRESUMESUSPEND = 0x0007;
        void SendData(byte command, byte param1, byte param2, byte param3)
        {
            if (ready )
            {
                s = String.Format("{0:000}{1:000}{2:000}{3:000}", param1, param2, param3, command);
                ready = false;
                serialPort1.WriteLine(s);
            }
            //data = new byte[] { command, param1, param2, param3, 111 };
        }

        bool ready = true;
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            SendData( 250,trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }

        byte b1, b2, b3, b4;
        private void SendData(int p1, int p2, int p3, int p4)
        {
            b1 = (byte)p1;
            b2 = (byte)p2;
            b3 = (byte)p3;
            b4 = (byte)p4;
            SendData(b1, b2, b3, b4);
            
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            SendData(250, trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            SendData(200, trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SendData(200, trackBar1.Value, trackBar2.Value, trackBar3.Value);
        }

        int[] proc = new int[50];
        int jp = 0;
        private void timer1_Tick(object sender, EventArgs e)
        {
            //proc[jp] = (int) Math.Ceiling(performanceCounter1.NextValue());
            //jp++;
            //if (jp>proc.Length-1 )
            //{
            //    jp = 0;
                
            //}
            //SendData(100, (int)(Math.Ceiling(proc.Average()) * 2), trackBar2.Value, trackBar3.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SendData(200, 0, 0, 0);
        }
    }
}
