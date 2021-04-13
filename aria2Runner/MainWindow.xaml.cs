using System;
using System.Collections.Generic;
using System.IO;
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
using System.Diagnostics;
using System.Threading;

namespace aria2Runner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string aria2ConfPath = @"./aria2.conf";
        const string confPath = @"./aria2Runner.conf";
        Process aria2c = null;
        Thread outputThread;
        public MainWindow()
        {
            InitializeComponent();
            outputThread = new Thread(new ThreadStart(Aria2OutputToText));
            outputThread.IsBackground = true;
        }

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            StreamReader sr = new StreamReader(aria2ConfPath);
            this.confTextBox.Text = sr.ReadToEnd();
        }

        private void Aria2OutputToText()
        {
            //if(aria2c.HasExited)
            //{
                
            //}
            System.IO.StreamReader reader = aria2c.StandardOutput;
            string line = reader.ReadLine();
            while (!reader.EndOfStream && !aria2c.HasExited)
            {
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    if (string.IsNullOrEmpty(line))
                    {
                        Thread.Sleep(50);
                        return;
                    }
                    outputTextBox.Text += line + "\n";
                    outputTextBox.ScrollToEnd();
                });
                line = reader.ReadLine();
            }
            aria2c.WaitForExit();
        }

        private void RunButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (runningRadio.IsChecked == true)
            {
                if(aria2c != null)
                {
                    aria2c.Kill();
                }
            } 
            ProcessStartInfo info = new ProcessStartInfo("./aria2c.exe", "-D --conf-path=aria2.conf");
            info.RedirectStandardOutput = true;
            info.CreateNoWindow = true;
            aria2c = Process.Start(info);
            if(!outputThread.IsAlive)
                outputThread.Start();
        }

        private void StopButton_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: Stop
            throw new NotImplementedException();
        }

        private void HideButton_OnClick(object sender, RoutedEventArgs e)
        {
            // TODO: Hide
            throw new NotImplementedException();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (aria2c != null)
            {
                aria2c.Kill();
                aria2c.WaitForExit();
            }
        }
    }
}