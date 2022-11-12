using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Win32;
using Microsoft.Windows.Themes;
using ModernWpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace timActionCenter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool[] buttonClicked = new bool[9];
        System.Windows.Forms.NotifyIcon notifyIcon = new System.Windows.Forms.NotifyIcon();
        public MainWindow()
        {
            InitializeComponent();
            this.Hide();
            
            this.Top = System.Windows.SystemParameters.WorkArea.Height - this.Height - 10;
            this.Left = System.Windows.SystemParameters.WorkArea.Width - this.Width - 10;

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.AppDomain.CurrentDomain.BaseDirectory + "\\settings.ico");
            notifyIcon.Text = "Action Center";
            notifyIcon.Visible = true;
            notifyIcon.Click += new System.EventHandler(notifyIcon_Click);
        }

   

        private void notifyIcon_Click(object? sender, EventArgs e)
        {
            if (this.IsVisible)
            {
                this.Hide();
            }
            else
            {
                this.Show();
                this.Focus();
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {


            if (buttonClicked[0])
            {
                buttonClicked[0] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[0] = true;
            }

            RegistryKey k = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            var wr = k.OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            wr.SetValue("ProxyEnable", buttonClicked[0], RegistryValueKind.DWord);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[1])
            {
                buttonClicked[1] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[1] = true;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[2])
            {
                buttonClicked[0] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[2] = true;
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[3])
            {
                buttonClicked[3] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[3] = true;
            }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[4])
            {
                buttonClicked[4] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[4] = true;
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[5])
            {
                buttonClicked[5] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[5] = true;
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[6])
            {
                buttonClicked[6] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[6] = true;
            }
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[7])
            {
                buttonClicked[7] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[7] = true;
            }
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[8])
            {
                buttonClicked[8] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[8] = true;
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            updateBtn1Color();
        }

        private void updateBtn1Color()
        {
            RegistryKey k = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Default);
            var wr = k.OpenSubKey(@"Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings", true);

            var state = wr.GetValue("ProxyEnable");
            if (state.ToString() == "1")
            {
                buttonClicked[0] = true;
                btn1.Style = this.FindResource("AccentButtonStyle") as Style;
            }
            else
            {
                buttonClicked[0] = false;
                btn1.Style = this.FindResource("DefaultButtonStyle") as Style;
            }
        }
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon.Dispose();
        }
    }
}
