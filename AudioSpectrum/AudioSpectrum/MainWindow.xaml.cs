using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO.Ports;

namespace AudioSpectrum
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Analyzer _analyzer;
        private SerialPort _port;

        public MainWindow()
        {
            InitializeComponent();
            _analyzer = new Analyzer(PbL, PbR, Spectrum, DeviceBox,this );
            Comports.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports) Comports.Items.Add(port);
            Comports.SelectedIndex = Comports.Items.Count-1;
            CkbSerial.IsChecked = true;
            try
            {
                if (CkbSerial.IsChecked == true)
                {
                    Comports.IsEnabled = false;
                    _port = new SerialPort((Comports.Items[Comports.SelectedIndex] as string));
                    _port.BaudRate = 115200;
                    _port.StopBits = StopBits.One;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.DtrEnable = true;
                    _port.Open();
                    _analyzer.Serial = _port;
                }
                else
                {
                    Comports.IsEnabled = true;
                    _analyzer.Serial = null;
                    if (_port != null)
                    {
                        _port.Close();
                        _port.Dispose();
                        _port = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            BtnEnable.IsChecked = true;

            BtnEnable.Content = "Disable";
            _analyzer.Enable = true;

        }

        private void BtnEnable_Click(object sender, RoutedEventArgs e)
        {
            if (BtnEnable.IsChecked == true)
            {
                BtnEnable.Content = "Disable";
                _analyzer.Enable = true;
            }
            else
            {
                _analyzer.Enable = false;
                BtnEnable.Content = "Enable";
            }
        }

        private void Comports_DropDownOpened(object sender, EventArgs e)
        {
            Comports.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports) Comports.Items.Add(port);
        }

        private void CkbSerial_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CkbSerial.IsChecked == true)
                {
                    Comports.IsEnabled = false;
                    _port = new SerialPort((Comports.Items[Comports.SelectedIndex] as string));
                    _port.BaudRate = 115200;
                    _port.StopBits = StopBits.One;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.DtrEnable = true;
                    _port.Open();
                    _analyzer.Serial = _port;
                }
                else
                {
                    Comports.IsEnabled = true;
                    _analyzer.Serial = null;
                    if (_port != null)
                    {
                        _port.Close();
                        _port.Dispose();
                        _port = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
           
        }

        private void CkbDisplay_Click(object sender, RoutedEventArgs e)
        {
            _analyzer.DisplayEnable = (bool)CkbDisplay.IsChecked;
        }

        private void CkbSerial_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEnable_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void mode2_Checked(object sender, RoutedEventArgs e)
        {
            Spectrum.mode = 2;
        }

        private void mode1_Checked(object sender, RoutedEventArgs e)
        {
            Spectrum.mode = 1;
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.k1 =(byte) e.NewValue;
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.k2 = (byte)e.NewValue;
        }

        private void Slider_ValueChanged_2(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.k3 = (byte)e.NewValue;
        }

        private void Slider_ValueChanged_3(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.kd1 = e.NewValue;
        }

        private void Slider_ValueChanged_4(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.kd2 = e.NewValue;
        }

        private void Slider_ValueChanged_5(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Spectrum.kd3 = e.NewValue;
        }

        private void main_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Spectrum.exiting = true;
        }
    }
}
