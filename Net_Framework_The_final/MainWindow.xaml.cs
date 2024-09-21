using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Windows.Threading;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer media_pl = new MediaPlayer();
        DispatcherTimer dt = new DispatcherTimer();
        public MainWindow()
        {
            string path = "C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\audio1.mp3";

            try
            {
                media_pl.Open(new Uri(path));
                if (path == " ")
                    throw new Exception();
                media_pl.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
            btn_start.Visibility = Visibility.Hidden;
            Loading();
        }

        private void Show_timer(object sender, EventArgs e)
        {
            if (btn_start.Visibility == Visibility.Visible)
                btn_start.Visibility = Visibility.Hidden;
            else
                btn_start.Visibility = Visibility.Visible;
        }

        private async void Loading()
        {
            p_bar.Value = 0;

            do
            {
                p_bar.Value++;
                await Task.Delay(20);
                if (Convert.ToInt32(p_bar.Value) == 100)
                {
                    btn_start.Visibility = Visibility.Visible;
                    break;
                } 
            } while (true);

            dt.Tick += new EventHandler(Show_timer);
            dt.Interval = TimeSpan.FromSeconds(0.5);
            dt.Start();
        }

        private void btn_start_Click(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            Settings f1 = new Settings();
            f1.Show();
            Close();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            Close();
        }
    }
}
