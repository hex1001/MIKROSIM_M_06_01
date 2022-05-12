using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MIKROSIM_M_06_01
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool run = false;
        SerialPort serial;
        string com;
        string[] ports;
        
        public MainWindow() {
            InitializeComponent();
            ports = SerialPort.GetPortNames();
            foreach (string p in ports)
                serCombo.Items.Add(p);
        }

        async public void Reading() {
            String[] str = new String[2];
            float kg = 0;
            try {
                com = serCombo.SelectedValue.ToString();
                serial = new SerialPort(serCombo.SelectedValue.ToString(), 9600, Parity.None, 8, StopBits.One);
                run = true;
                while (run) {
                    serial.Open();
                    serial.Write(outCode, 0, outCode.Length);
                    string ssss = serial.ReadLine().Trim();
                    serial.Close();

                    kgLabel.Content = ssss;
                    using (StreamWriter outputFile = new StreamWriter(Path.Combine(ssss, Directory.GetCurrentDirectory() + "\\natija.txt"))) {
                        outputFile.WriteLine(ssss);
                    }
                    await Task.Delay(50);
                }
            } catch (Exception ex) {
                serial.Close();
                MessageBox.Show(ex.Message);
                await Task.Delay(5000);
                Reading();
            }

        }

        private void btnOpen_Click(object sender, RoutedEventArgs e) {
            if (serCombo.SelectedIndex == -1)
                return;

            btnOpen.IsEnabled = false;
            run = true;
            btnClose.IsEnabled = true;
            Reading();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e) {
            if (serial.IsOpen)
                serial.Close();
            btnOpen.IsEnabled = true;
            btnClose.IsEnabled = false;
            run = false;
        }
    }
}
