using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Un4seen.Bass;
using Un4seen.BassWasapi;

namespace AudioSpectrum
{

    internal class Analyzer
    {
        private bool _enable;               //enabled status
        private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fft;               //buffer for fft data
        private ProgressBar _l, _r;         //progressbars for left and right channel intensity
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private Spectrum _spectrum;         //spectrum dispay control
        private ComboBox _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index

        private int _lines = 72;            // number of spectrum lines
        private Label lb;
        //ctor
        public Analyzer(ProgressBar left, ProgressBar right, Spectrum spectrum, ComboBox devicelist, MainWindow mw)
        {
            lb = mw.mode;
            _fft = new float[1024];
            _lastlevel = 0;
            _hanctr = 0;
            _t = new DispatcherTimer();
            _t.Tick += _t_Tick;
            _t.Interval = TimeSpan.FromMilliseconds(25); //40hz refresh rate
            _t.IsEnabled = false;
            _l = left;
            _r = right;
            _l.Minimum = 0;
            _r.Minimum = 0;
            _r.Maximum = ushort.MaxValue;
            _l.Maximum = ushort.MaxValue;
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<byte>(73);
            _spectrum = spectrum;
            _devicelist = devicelist;
            _initialized = false;
            Init();
        }

        // Serial port for arduino output
        public SerialPort Serial { get; set; }

        // flag for display enable
        public bool DisplayEnable { get; set; }

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
                if (value)
                {
                    if (!_initialized)
                    {
                        var array = (_devicelist.Items[_devicelist.SelectedIndex] as string).Split(' ');
                        devindex = Convert.ToInt32(array[0]);
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                        if (!result)
                        {
                            var error = Bass.BASS_ErrorGetCode();
                            MessageBox.Show(error.ToString());
                        }
                        else
                        {
                            _initialized = true;
                            _devicelist.IsEnabled = false;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else BassWasapi.BASS_WASAPI_Stop(true);
                System.Threading.Thread.Sleep(500);
                _t.IsEnabled = value;
            }
        }
        SerialPort serialPort1 = new SerialPort("COM3", 115200);
        Thread snder ;
        // initialization
        private void Init()
        {
            //serialPort1.Open();
            //serialPort1.DataReceived += serialPort1_DataReceived;
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && device.IsLoopback)
                {
                    _devicelist.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            _devicelist.SelectedIndex = 0;
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            //snder = new Thread(Sender);
            //snder.Start();
            if (!result) throw new Exception("Init Error");
        }
        void serialPort1_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            ready = true;
            //MessageBox.Show(serialPort1.ReadByte().ToString());
        }
        string s;
        void SendData(ref SerialPort ser, byte param1, byte param2, byte param3)
        {
            s = String.Format("{0:000}{1:000}{2:000}{3:000}", param1, param2, param3, 111);
            //ready = false;
            ser.WriteLine(s);
            //data = new byte[] { command, param1, param2, param3, 111 };
        }
        private void SendData(int p1, int p2, int p3, int p4)
        {
            b1 = (byte)p1;
            b2 = (byte)p2;
            b3 = (byte)p3;
            b4 = (byte)p4;
            SendData(b1, b2, b3, b4);

        }
        bool ready = true;
        byte b1, b2, b3, b4,b5,b6;
        double dcol = 0;
        bool forward = true;
        //timer 
        byte i1, i2;
        byte r, g, b;
        byte[] rr = new byte[24];
        byte[] gg = new byte[24];
        byte[] bb = new byte[24];
        byte[] rro = new byte[24];
        byte[] ggo = new byte[24];
        byte[] bbo = new byte[24];
        List<byte> spdata=new List<byte>(72);
        byte f = 0;
        byte smax = 3;

        private byte[] SpectrI = new byte[72];
        private List<byte> inpt; 
        private int tn = 0;
        private byte mmk = 40;
        private byte mmmin =35;
        private double avrgs = 128;

        private void mergeData()
        {
            for (int i = 0; i < inpt.Count; i++)
            {
                tn = SpectrI[i]*mmk;
                tn += inpt[i];
                tn = tn/(mmk + 1);
                SpectrI[i] = (byte) tn;
            }
            double avrg = SpectrI.Average(x => x);
            if (avrg<64)
            {
                avrg = 64;
            }
            avrgs = avrgs*4 + avrg;
            avrgs = avrgs/5;
            avrg = avrgs;
            stindext = 0;
            //for (int i = 0; i < SpectrI.Count(); i++)
            //{
            //    if (SpectrI[i] > avrg/mmmin)
            //    {
            //        stindext = i;
            //    }
            //    else
            //    {
            //        if (stindext>0)
            //        {
            //            endindext = i;
            //        }
            //        break;
            //    }
            //}
            //for (int i = SpectrI.Count() - 1; i > 0; i--)
            //{
            //    if (SpectrI[i] > avrg/mmmin)
            //    {
            //        endindex = i;
            //        break;
            //    }
            //    else
            //    {
            //        endindex = 71;
            //    }
            //}
            stindext = 0;
            endindext = 72;
            for (int i = SpectrI.Count() - 1; i > 0; i--)
            {
                if (SpectrI[i] > avrg / mmmin)
                {
                    endindext = i;
                    if (stindext>0)
                    {
                        break;
                    }
                }
                else
                {
                    if (endindext<70)
                    {
                        stindext = i;
                    }
                }
            }
            stindex = stindext;
            endindex = endindext;
            diap = endindex - stindex;
            if (diap>35)
            {
                mmmin--;
            }
            else
            {
                mmmin++;
            }
            if (diap<20)
            {
                diap = 72;
                stindex = 0;
                endindex = 72;
            }
        }

        private int stindex = 0;
        private int endindex = 71;
        private int stindext = 0;
        private int endindext = 71;
        private int diap = 71;
        private double diad = 0;
        private int stick = 0;
        int GetI(int n)
        {
            diad = (double)n/72;
            diad = diad*diap;
            diad = diad + stindex;
            diad = Math.Ceiling(diad);
            return (int) diad;
        }

        private List<List<Color>> lines = new List<List<Color>>();
        private Thread t ;
        private void _t_Tick(object sender, EventArgs e)
        {
            try
            {
                int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
                if (ret < -1) return;
                int x, y;
                int b0 = 0;
                i1 = 0;
                i2 = 1;
                _spectrumdata.Clear();
                //computes the spectrum data, the code is taken from a bass_wasapi sample.
                switch (_spectrum.mode)
                {
                    case 1:
                        {
                            if (Serial != null)
                            {
                                for (x = 0; x < 72; x++)
                                {
                                    float peak = 0;
                                    int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                                    if (b1 > 1023) b1 = 1023;
                                    if (b1 <= b0) b1 = b0 + 1;
                                    for (; b0 < b1; b0++)
                                    {
                                        if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                                    }
                                    y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                                    if (y > 255) y = 255;
                                    if (y < 0) y = 0;

                                    switch (i2)
                                    {
                                        case 1:
                                            {
                                                r = (byte)y;
                                                i2++;
                                                break;
                                            }
                                        case 2:
                                            {
                                                g = (byte)y;
                                                i2++;
                                                break;
                                            }
                                        case 3:
                                            {
                                                b = (byte)y;
                                                i2 = 1;
                                                SendDatav2(Serial, r, g, b, 222, i1);
                                                i1++;
                                                break;
                                            }
                                    }
                                    //Console.Write("{0, 3} ", y);
                                }
                            }
                            SendDatav2(Serial, 0, 0, 0, 250, 0);
                            break;
                        }

                    case 2:
                        {
                            if (Serial != null)
                            {
                                for (x = 0; x < 72; x++)
                                {
                                    float peak = 0;
                                    int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                                    if (b1 > 1023) b1 = 1023;
                                    if (b1 <= b0) b1 = b0 + 1;
                                    for (; b0 < b1; b0++)
                                    {
                                        if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                                    }
                                    y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                                    if (y > 255) y = 255;
                                    if (y < 0) y = 0;
                                    _spectrumdata.Add((byte)y);
                                }
                                spdata.Clear();

                                stick++;
                                if (stick > 10)
                                {
                                    stick = 0;
                                    inpt = _spectrumdata;
                                    if (t == null)
                                    {
                                        t = new Thread(mergeData);
                                        t.Start();
                                    }
                                    else
                                    {
                                        if (t.ThreadState == ThreadState.Stopped)
                                        {
                                            t = new Thread(mergeData);
                                            t.Start();
                                        }
                                    }
                                }
                                foreach (byte item in _spectrumdata)
                                {
                                    spdata.Add(item);
                                }

                                spdata.Sort();
                                for (x = 0; x < 72; x++)
                                {
                                    f = _spectrumdata[GetI(x)];
                                    f = GetValue(f, GetIndex(ref spdata, f));
                                    switch (i2)
                                    {
                                        case 1:
                                            {
                                                if (i1 >= _spectrum.k1)
                                                {
                                                    i2++;

                                                }
                                                if (i1 >= 24)
                                                {
                                                    i1 = 0;
                                                }
                                                if (_spectrum.kd1 < 1)
                                                {
                                                    rr[i1] = (byte)(Math.Ceiling((double)f * _spectrum.kd1));
                                                }
                                                else
                                                {
                                                    rr[i1] = f;
                                                }


                                                i1++;

                                                break;
                                            }
                                        case 2:
                                            {
                                                if (i1 >= _spectrum.k2)
                                                {
                                                    i2++;

                                                }
                                                if (i1 >= 24)
                                                {
                                                    i1 = 0;
                                                }

                                                if (_spectrum.kd2 < 1)
                                                {
                                                    bb[i1] = (byte)(Math.Ceiling((double)f * _spectrum.kd2));
                                                }
                                                else
                                                {
                                                    bb[i1] = f;
                                                }
                                                i1++;

                                                break;
                                            }
                                        case 3:
                                            {
                                                if (i1 >= _spectrum.k3)
                                                {
                                                    i2 = 1;

                                                }
                                                if (i1 >= 24)
                                                {
                                                    i1 = 0;
                                                }

                                                if (_spectrum.kd3 < 1)
                                                {
                                                    gg[i1] = (byte)(Math.Ceiling((double)f * _spectrum.kd3));
                                                }
                                                else
                                                {
                                                    gg[i1] = f;
                                                }
                                                i1++;
                                                i1++;

                                                break;
                                            }
                                    }
                                    //Console.Write("{0, 3} ", y);
                                }
                            }

                            if (sd.Count > 10)
                            {
                                smax++;
                            }
                            else
                            {
                                if (smax - 1 > 2)
                                {
                                    smax--;
                                }
                            }
                            string ssc = "";
                            //Color ccc;
                            //List<Color> clrs = new List<Color>();
                            for (int l = 0; l < 24; l++)
                            {
                                //ccc = Color.FromRgb(rr[l], gg[l], bb[l]);
                                //clrs.Add(ccc);
                                ssc += GetSendDataText(rr[l], gg[l], bb[l], 222, l) + "\n";
                            }
                            //lines.Add(clrs);
                            ssc += GetSendDataText(0, 0, 0, 250, 0) + "\n";
                            Serial.Write(ssc);
                            //rro = (byte[])rr.Clone();
                            //bbo = (byte[])bb.Clone();
                            //ggo = (byte[])gg.Clone();
                            break;
                        }
                }
                if (DisplayEnable) _spectrum.Set(_spectrumdata);
                int level = BassWasapi.BASS_WASAPI_GetLevel();
                if (level == _lastlevel && level != 0) _hanctr++;
                _lastlevel = level;
                if (_hanctr > 3)
                {
                    _hanctr = 0;
                    _l.Value = 0;
                    _r.Value = 0;
                    Free();
                    Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                    _initialized = false;
                    Enable = true;
                }
            }
            catch (Exception)
            {
                         
            }      
        }


        byte GetIndex(ref List<byte> data, byte value)
        {
           return (byte) data.BinarySearch(value);
        }

        double d = 0;
        byte GetValue(byte oldvalue, byte index)
        {
            d = getfunc(index);

            d = (double)oldvalue / d;
            d = Math.Ceiling(d);
            if (d > 255)
            {
                return 255;
            }
            else
            {
                return (byte)d;
            }

        }

        int curfunc = 1;
        int ccc = 0;
        double getfunc(byte index)
        {
            ccc++;
            if (ccc >= 20000)
            {
                ccc = 0;
                curfunc++;
                lb.Content = curfunc.ToString();
                if (curfunc > 6)
                {
                    curfunc = 1;
                }
                if (curfunc > 4)
                {
                    RandomizeKd();
                }
                else
                {
                    _spectrum.kd1 = 1;
                    _spectrum.kd2 = 1;
                    _spectrum.kd3 = 1;
                }

            }
            switch (curfunc)
            {
                case 1:
                    {

                        return 35 / Math.Log( index + 1) / Math.Sqrt( index + 1);
                    }
                case 2: return Math.Sqrt(73 - index + 1 ) / index * 30; ;
                case 3: { MoveKd(); return Math.Log10(73 - index + 1) / index*30; }
                case 4: { MoveKd(); return Math.Atan(73 - index + 1) / index * 25; ; }
                case 5: { MoveKd(); return 35 / Math.Log(index + 1) / Math.Sqrt(index + 1) / index * 30; ; }
                case 6: { MoveKd(); return Math.Log10(73 - index + 1) / index * 35; ; }
            }
            return index / 72;
        }

        Random rnd = new Random();
        double nkd1 = 1;
        double nkd2 = 1;
        double nkd3 = 1;
        double mk = 0;
        void RandomizeKd()
        {
            nkd1 = rnd.NextDouble();
            nkd2 = rnd.NextDouble();
            nkd3 = rnd.NextDouble();
        }

        void MoveKd()
        {
            if (nkd1 != _spectrum.kd1)
            {
                if (nkd1 > _spectrum.kd1)
                {
                    mk = 0.0001;
                }
                else
                {
                    mk = -0.0001;
                }
                _spectrum.kd1 += mk;
            }
            if (nkd2 != _spectrum.kd2)
            {
                if (nkd2 > _spectrum.kd2)
                {
                    mk = 0.0001;
                }
                else
                {
                    mk = -0.0001;
                }
                _spectrum.kd2 += mk;
            }
            if (nkd3 != _spectrum.kd3)
            {
                if (nkd3 > _spectrum.kd3)
                {
                    mk = 0.0001;
                }
                else
                {
                    mk = -0.0001;
                }
                _spectrum.kd3 += mk;
            }
        }

        
        
        private void SendDatav2(SerialPort Serial, int p1, int p2, int p3)
        {
            s = String.Format("{0:000}{1:000}{2:000}{3:000}", p1, p2, p3, 111);
            //ready = false;
            Serial.WriteLine(s);
        }
        void SendDatav2(SerialPort ser, byte param1, byte param2, byte param3, int command, int num)
        {
            s = String.Format("{0:000}{1:000}{2:000}{3:000}{4:000}", param1, param2, param3, command, num);
            //ready = false;
            //SendAsy(ser, s);
            Serial.WriteLine(s);
            //data = new byte[] { command, param1, param2, param3, 111 };
        }

        string GetSendDataText( byte param1, byte param2, byte param3, int command, int num)
        {
            return  String.Format("{0:000}{1:000}{2:000}{3:000}{4:000}", param1, param2, param3, command, num);
        }

        Queue sd = new Queue();
        long le = 1000;
        string curstr = "";
        void Sender()
        {
            do
            {
                if (sd.Count > 0)
                {
                    curstr = (string)sd.Dequeue();
                    Serial.WriteLine(curstr);
                    le -= 100;
                    
                }
                else
                {
                    if (_spectrum.exiting )
                    {
                        return;
                    }
                    le += 1;
                    if (le<=1)
                    {
                        le = 1;
                    }
                    Thread.Sleep(new TimeSpan(le + le/20));
                }

            } while (true);
        }

        long de = 10000;
        void SendAsy(SerialPort ser, string command)
        {
            sd.Enqueue(command);
            if (sd.Count>10)
            {
                de += 1;
            }
            else
            {
                de -= 10;
            }
            if (de<0)
            {
                de = 0;
            }
            else
            {
                Thread.Sleep(new TimeSpan(de));
            }
            
            //ready = false;
            
            //data = new byte[] { command, param1, param2, param3, 111 };
        }
        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
            
        }
    }
}
