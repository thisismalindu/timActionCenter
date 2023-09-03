using Microsoft.Win32;
using Microsoft.Windows.Themes;
using ModernWpf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;

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
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\timActionCenter\\Themes\\");

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

            if (IsVisible)
            {
            }
            else
            {
                this.Show();
                this.Activate();
            }
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            updateBtn1Color();
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            notifyIcon.Dispose();
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
            getWallpaper();
            if (buttonClicked[1])
            {
                buttonClicked[1] = false;
                ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
                setDarkTheme();
            }
            else
            {

                ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

                buttonClicked[1] = true;
                setLightTheme();
            }
            setWallpaper();
        }

        private string stream;
        private void setDarkTheme()
        {
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\ThemeSwitcher.exe", "dark.theme");

            lblTheme.Text = "Theme (Dark)";

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.AppDomain.CurrentDomain.BaseDirectory + "\\settings.ico");

        }
        private void setLightTheme()
        {
            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\ThemeSwitcher.exe", "aero.theme");

            lblTheme.Text = "Theme (Light)";

            notifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(System.AppDomain.CurrentDomain.BaseDirectory + "\\settings-light.ico");

        }

        private void getWallpaper()
        {
            File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Themes\\TranscodedWallpaper", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\timActionCenter\\Themes\\TranscodedWallpaper", true);
        }
        private void setWallpaper()
        {
            wait(5000);

            Process.Start(System.AppDomain.CurrentDomain.BaseDirectory + "\\RefreshWallpaper.exe", Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\timActionCenter\\Themes\\TranscodedWallpaper");
        }



        //Work in Progress
        private void createProperThemeFile(string darkOrLight, string wallpaperPath)
        {
            StreamReader sr = new StreamReader(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + @"\Microsoft\Windows\Themes\Custom.theme");

            string content = sr.ReadToEnd();

            int index = content.IndexOf("Wallpaper=");

            string firstPart;

            firstPart = content.Substring(0, index);

            string wallpaper = $"Wallpaper={wallpaperPath}";



            string filePath = System.AppDomain.CurrentDomain.BaseDirectory + $"{darkOrLight}.theme";
            File.Create(filePath);
            StreamWriter sw = new StreamWriter(filePath);
            firstPart = @"; Copyright � Microsoft Corp.

[Theme]
; Windows - IDS_THEME_DISPLAYNAME_THEMEA
DisplayName=Custom Theme
ThemeId={981A87EA-96D6-43D4-A4F1-B643CB94E424}

; Computer - SHIDI_SERVER
[CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\DefaultIcon]
DefaultValue=%SystemRoot%\System32\imageres.dll,-109

; UsersFiles - SHIDI_USERFILES
[CLSID\{59031A47-3F72-44A7-89C5-5595FE6B30EE}\DefaultIcon]
DefaultValue=%SystemRoot%\System32\imageres.dll,-123

; Network - SHIDI_MYNETWORK
[CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\DefaultIcon]
DefaultValue=%SystemRoot%\System32\imageres.dll,-25

; Recycle Bin - SHIDI_RECYCLERFULL SHIDI_RECYCLER
[CLSID\{645FF040-5081-101B-9F08-00AA002F954E}\DefaultIcon]
Full=%SystemRoot%\System32\imageres.dll,-54
Empty=%SystemRoot%\System32\imageres.dll,-55

[Control Panel\Cursors]
AppStarting=%SystemRoot%\cursors\aero_working.ani
Arrow=%SystemRoot%\cursors\aero_arrow.cur
Crosshair=
Hand=%SystemRoot%\cursors\aero_link.cur
Help=%SystemRoot%\cursors\aero_helpsel.cur
IBeam=
No=%SystemRoot%\cursors\aero_unavail.cur
NWPen=%SystemRoot%\cursors\aero_pen.cur
SizeAll=%SystemRoot%\cursors\aero_move.cur
SizeNESW=%SystemRoot%\cursors\aero_nesw.cur
SizeNS=%SystemRoot%\cursors\aero_ns.cur
SizeNWSE=%SystemRoot%\cursors\aero_nwse.cur
SizeWE=%SystemRoot%\cursors\aero_ew.cur
UpArrow=%SystemRoot%\cursors\aero_up.cur
Wait=%SystemRoot%\cursors\aero_busy.ani
DefaultValue=Windows Default

[Control Panel\Desktop]
";
            sw.Write(firstPart);
            sw.Write(wallpaper);

        }



        public void wait(int milliseconds)
        {
            var timer1 = new System.Windows.Forms.Timer();
            if (milliseconds == 0 || milliseconds < 0) return;

            timer1.Interval = milliseconds;
            timer1.Enabled = true;
            timer1.Start();

            timer1.Tick += (s, e) =>
            {
                timer1.Enabled = false;
                timer1.Stop();
            };

            while (timer1.Enabled)
            {
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (buttonClicked[2])
            {
                buttonClicked[2] = false;
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
            Process.Start("C:\\Users\\thisismalindu\\AppData\\Roaming\\thisismalindu\\Symbol Picker\\SymbolPicker.exe");
            //if (buttonClicked[3])
            //{
            //    buttonClicked[3] = false;
            //    ((System.Windows.Controls.Button)sender).Style = this.FindResource("DefaultButtonStyle") as Style;
            //}
            //else
            //{

            //    ((System.Windows.Controls.Button)sender).Style = this.FindResource("AccentButtonStyle") as Style;

            //    buttonClicked[3] = true;
            //}
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

    }
}
