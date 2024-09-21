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
using System.Windows.Shapes;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for Welcome.xaml
    /// </summary>
    public partial class Welcome : Window
    {
        MediaPlayer media_pl = new MediaPlayer();
        string first_namee, last_namee;
        int agee;
        bool exist;
        public Welcome(string name, string last_name, int age, bool exicting)
        {
            string path = "C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\audio3.mp3";

            try
            {
                media_pl.Open(new Uri(path));
                if (path == " ")
                    throw new Exception("Отсутствует аудиофайл!");
                media_pl.Play();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
            InitializeComponent();
            welcome.Title = $"Добро пожаловать, {name}!";
            first_namee = name;
            last_namee = last_name;
            agee = age;
            exist = exicting;
        }

        private void choose_Clickc(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            Go1 form_go1 = new Go1(check_time.IsChecked.Value, wrongs.IsChecked.Value, onemore_try.IsChecked.Value, first_namee, last_namee, agee, exist);
            form_go1.Show();
            Close();
        }

        private void choose2_Clickc(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            Go2 form_go2 = new Go2(check_time2.IsChecked.Value, wrongs2.IsChecked.Value, onemore_try2.IsChecked.Value, first_namee, last_namee, agee, exist);
            form_go2.Show();
            Close();
        }

        private void btn_exit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Пока!");
            media_pl.Stop();
            Close();
        }
    }
}
