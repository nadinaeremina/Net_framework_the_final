using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Serialization;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        MediaPlayer media_pl = new MediaPlayer();
        bool exicting = false;
        public Settings()
        {
            string path = "C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\audio2.mp3";
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
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int chislo, age = 0;
            if (txt_name.Text.Length > 0)
            {
                if (txt_lastname.Text.Length > 0)
                {
                    if ((txt_age.Text.Length > 0 && !exicting) || exicting)
                    {
                        if ((!exicting && int.TryParse(txt_age.Text, out chislo) == true && Convert.ToInt32(txt_age.Text) >= 3 && Convert.ToInt32(txt_age.Text) < 8) || exicting)
                        {
                            if (!exicting)
                            {
                                MessageBox.Show("Данные сохранены!");
                                media_pl.Stop();
                                Welcome f2 = new Welcome(txt_name.Text, txt_lastname.Text, Convert.ToInt32(txt_age.Text), exicting);
                                f2.Show();
                                Close();
                            }
                            else
                            {
                                bool flag = false;
                                StreamReader reader = new StreamReader(@"C:\\Users\\Nadya\\Desktop\\Players.txt", true);
                                string str = reader.ReadToEnd();
                                string[] mas = str.Split('/');
                                List<string> list = new List<string>();
                                list.AddRange(mas);
                                list.RemoveAt(list.Count - 1);

                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (list[i].Contains(txt_name.Text) && list[i].Contains(txt_lastname.Text))
                                    {
                                        flag = !flag;
                                        string[] mas2 = list[i].Split(' ');
                                        age = Convert.ToInt32(mas2[2]);
                                        break;
                                    }
                                }
                                reader.Close();
                                if (flag)
                                {
                                    MessageBox.Show("Такой игрок существует!");
                                    media_pl.Stop();
                                    Welcome f2 = new Welcome(txt_name.Text, txt_lastname.Text, age, exicting);
                                    f2.Show();
                                    Close();
                                }
                                else
                                {
                                    MessageBox.Show("Такого игрока не существует!");
                                    txt_lastname.Clear();
                                    txt_name.Clear();
                                }   
                            }
                        }
                        else
                        {
                            MessageBox.Show("Некорректный возраст!");
                            txt_age.Clear();
                        }
                    }
                    else
                        MessageBox.Show("Введите возраст!");
                }
                else
                    MessageBox.Show("Введите фамилию!");
            }
            else
                MessageBox.Show("Введите имя!");
        }

        private void new_player_Click(object sender, RoutedEventArgs e)
        {
            new_or_exist.Text = "Регистрация нового игрока:";
            exicting = false;
            st_panel_auto.Visibility = Visibility.Visible;
            enter_age.Visibility = Visibility.Visible;
            txt_age.Visibility = Visibility.Visible;
        }
        private void ex_player_Click(object sender, RoutedEventArgs e)
        {
            if (!File.Exists("C:\\Users\\Nadya\\Desktop\\Players.txt"))
            {
                MessageBox.Show("Нет ни одного зарегистрированного игрока! Пожалуйста, зарегистрируйтесь!");
                new_or_exist.Text = "Регистрация нового игрока";
                exicting = false;
                st_panel_auto.Visibility = Visibility.Visible;
                enter_age.Visibility = Visibility.Visible;
                txt_age.Visibility = Visibility.Visible;
            }
            else
            {
                new_or_exist.Text = "Авторизация существующего игрока:";
                exicting = true;
                st_panel_auto.Visibility = Visibility.Visible;
                enter_age.Visibility = Visibility.Hidden;
                txt_age.Visibility = Visibility.Hidden;
            }
        }
        private void rating_Click(object sender, RoutedEventArgs e)
        {
            using (StreamReader reader = new StreamReader(@"C:\\Users\\Nadya\\Desktop\\Players.txt", true))
            {
                string str = reader.ReadToEnd();
                reader.Close();
                List<Player> players = new List<Player>();
                string[] mas = str.Split('/');
                string[] my_str;
                for (int i = 0; i < mas.Length; i++)
                {
                    my_str = mas[i].Split(' ');
                    if (my_str.Length == 4)
                        players.Add(new Player { First_name = my_str[0], Last_name = my_str[1], Age = Convert.ToInt32(my_str[2]), Rating = my_str[3] });
                }
                Rating rating_form = new Rating(players);
                rating_form.ShowDialog();
            }  
        }
        private void info_Click(object sender, RoutedEventArgs e)
        {
            Info info_form = new Info();
            info_form.ShowDialog();
        }
        private void exit_player_Click(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            MessageBox.Show("До новых встреч!");
            Close();
        }

        private void txt_name_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_name_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_lastname_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsLetter(e.Text, 0))
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_lastname_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_age_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, 0))
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_age_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }
    }
}
