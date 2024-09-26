using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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
using System.Windows.Media.Media3D;
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
        // передаем в конструктор 'XmlSerializer' тип класса ''Player''
        XmlSerializer xmlser = new XmlSerializer(typeof(List<Player>));
        MediaPlayer media_pl = new MediaPlayer();
        List<Player> players = new List<Player>();
        Player player;

        bool exicting = false, playing = false;

        public Settings()
        {
            string path = "C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\audio2.mp3";
            try
            {
                media_pl.Open(new Uri(path));

                if (path == " ")
                    throw new Exception("Отсутствует аудиофайл!");

                media_pl.Play();
                playing = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            InitializeComponent();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!playing)
                playing = true;

            int chislo, age = 0;
            if (txt_name.Text.Length > 0)
            {
                if (txt_password.Text.Length > 0)
                {
                    if ((txt_age.Text.Length > 0 && !exicting) || exicting)
                    {
                        if (!exicting && int.TryParse(txt_age.Text, out chislo) == true || exicting)
                        {
                            try
                            {
                                // десериализуем объект
                                using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                                {
                                    players = (List<Player>)xmlser.Deserialize(fs);
                                }

                                bool flag_name = false, flag_password = false; 

                                if (!exicting)
                                {
                                    foreach (Player pl in players)
                                    {
                                        if (pl.Nickname == txt_name.Text)
                                        {
                                            MessageBox.Show("Такой никнейм уже занят!\nПридумайте новый!");
                                            txt_password.Clear();
                                            txt_name.Clear();
                                            txt_age.Clear();
                                            flag_name = true;
                                            break;
                                        }
                                    }

                                    if (!flag_name)
                                    {
                                        MessageBox.Show("Данные сохранены!");
                                        media_pl.Stop();
                                        Welcome f2 = new Welcome(txt_name.Text, txt_password.Text, Convert.ToInt32(txt_age.Text), exicting);
                                        f2.Show();
                                        Close();
                                    }
                                }
                                else
                                {
                                    foreach (Player pl in players)
                                    {
                                        if (pl.Nickname == txt_name.Text)
                                        {
                                            flag_name = true;
                                            if (pl.Password == txt_password.Text)
                                            {
                                                MessageBox.Show("Пароль верный!");
                                                flag_password = true;
                                                break;
                                            } 
                                        }
                                    }
                                        
                                    if (flag_name)
                                    {
                                        if (!flag_password)
                                        {
                                            MessageBox.Show("Пароль не верный!");
                                            txt_password.Clear();
                                        }
                                        else
                                        {
                                            media_pl.Stop();
                                            Welcome f2 = new Welcome(txt_name.Text, txt_password.Text, age, exicting);
                                            f2.Show();
                                            Close();
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Такого игрока не существует!");
                                        txt_password.Clear();
                                        txt_name.Clear();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                if (!exicting)
                                {
                                    MessageBox.Show("Данные сохранены!");
                                    media_pl.Stop();
                                    Welcome f2 = new Welcome(txt_name.Text, txt_password.Text, Convert.ToInt32(txt_age.Text), exicting);
                                    f2.Show();
                                    Close();
                                }
                                else
                                    MessageBox.Show("Нет ни одного зарегистрированного игрока!\nПожалуйста, зарегистрируйтесь!");
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
                    MessageBox.Show("Введите пароль!");

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
            try
            {
                // десериализуем объект
                using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                {
                    players = (List<Player>)xmlser.Deserialize(fs);
                }

                new_or_exist.Text = "Авторизация существующего игрока:";
                exicting = true;
                st_panel_auto.Visibility = Visibility.Visible;
                enter_age.Visibility = Visibility.Hidden;
                txt_age.Visibility = Visibility.Hidden;
            }
            catch (Exception)
            {
                MessageBox.Show("Нет ни одного зарегистрированного игрока! Пожалуйста, зарегистрируйтесь!");
                new_or_exist.Text = "Регистрация нового игрока";
                exicting = false;
                st_panel_auto.Visibility = Visibility.Visible;
                enter_age.Visibility = Visibility.Visible;
                txt_age.Visibility = Visibility.Visible;
            }
        }
        private void rating_Click(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            playing = false;

            try
            {
                // десериализуем объект
                using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                {
                    players = (List<Player>)xmlser.Deserialize(fs);
                }

                Rating rating_form = new Rating(players);
                rating_form.ShowDialog();
            }
            catch (Exception)
            {
                MessageBox.Show("Рейтинг игроков пока пуст!!");
            }
        }

        private void info_Click(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            playing = false;
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

        private void txt_password_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void txt_password_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if ((!char.IsDigit(e.Text, 0) && !char.IsLetter(e.Text, 0)))
                e.Handled = true; // тогда не обрабатывать введенный символ(и, следовательно, не выводить его в TextBox)
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (!playing)
            {
                media_pl.Play();
                playing = true;
            }
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            if (!playing)
            {
                media_pl.Play();
                playing = true;
            }
        }
    }
}
