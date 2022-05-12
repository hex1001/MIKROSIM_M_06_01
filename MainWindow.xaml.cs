using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using EasyModbus;

namespace MIKROSIM_M_06_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int COUNT = 100000;
        string com;
        volatile int count = 0;
        SerialPort serial;
        string[] myPort;
        ModbusClient mb;
        bool start = false;

        public MainWindow() {
            InitializeComponent();
            myPort = SerialPort.GetPortNames();
            foreach (string ss in myPort)                
                serCombo.Items.Add(ss);
        }

        async public void Reading() {
            String[] str = new String[2];
            try {
                com = serCombo.SelectedValue.ToString();
                serial = new SerialPort(com);
                mb = new ModbusClient(com);
                mb.UnitIdentifier = 1;
                mb.Baudrate = 9600;
                mb.Parity = System.IO.Ports.Parity.None;
                mb.StopBits = System.IO.Ports.StopBits.One;
                mb.ConnectionTimeout = 500;
                mb.Connect();
                start = true;

                //MessageBox.Show(com);

            } catch (Exception ex) {
                MessageBox.Show(ex.Message);
            }
            await Task.Delay(100);
            while (start) {
                if (count > COUNT) {
                    mb.Disconnect();
                    serial = new SerialPort(com);
                    mb = new ModbusClient(com);
                    mb.UnitIdentifier = 1;
                    mb.Baudrate = 9600;
                    mb.Parity = Parity.None;
                    mb.StopBits = StopBits.One;
                    mb.ConnectionTimeout = 500;
                    mb.Connect();
                    count = 0;
                }

                //string ssss = (mb.ReadHoldingRegisters(1, 4)[0] / 100.0).ToString();
                string ssss = (mb.ReadHoldingRegisters(2,1)[0] / 100.0).ToString();
                //MessageBox.Show(ssss);
                kgLabel.Content = ssss;

                await Task.Delay(100);
                count++;
            }
            count = 0;
            mb.Disconnect();
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e) {
            if (serCombo.SelectedIndex == -1)
                return;

            btnOpen.IsEnabled = false;
            start = true;
            btnClose.IsEnabled = true;
            Reading();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            if (serial.IsOpen)
                serial.Close();
            btnOpen.IsEnabled = true;
            btnClose.IsEnabled = false;
            start = false;
        }
    }
}
