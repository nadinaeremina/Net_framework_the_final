﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
//using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Data.SqlClient;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for Go1.xaml
    /// </summary>
    public partial class Go1 : Window
    {
        // передаем в конструктор 'XmlSerializer' тип класса ''Player''
        XmlSerializer xmlser = new XmlSerializer(typeof(List<Player>));
        List<Player> players = new List<Player>();
        Player player;

        SqlConnection conn = null;
        SqlCommand cmd = new SqlCommand();
        DataSet ds_s = new DataSet();
        DataTable dt_events = new DataTable();
        SqlDataAdapter events_adapter = new SqlDataAdapter();
        SqlCommandBuilder cmd_events = new SqlCommandBuilder();
        string cs = "";

        List<string> questions_simple = new List<string>();
        List<string> questions_difficult = new List<string>();

        string str_name, str_pass, my_str, new_str = "";
        string[] right_answers = { "lion", "dog", "cat", "bird", "fish" };
        int ind1 = 0, ind2 = 1, ind3 = 2, right = 0, wrong = 0, limit_wrong = 5, inner_wrong = 1, left = 1, ind_right = 0, count = 60, int_age;

        bool sound = true;

        private void btn_sound_Click(object sender, RoutedEventArgs e)
        {
            if (sound)
            {
                btn_sound.Content = "Включить звук";
                media_pl.Stop();
            }
            else
            {
                btn_sound.Content = "Выключить звук";
                media_pl.Play();
            }
            sound = !sound;
        }

        bool flag = false, existing;

        List<string> words = new List<string>();
        DispatcherTimer dt = new DispatcherTimer();
        MediaPlayer media_pl = new MediaPlayer();

        public Go1(bool check, bool check2, bool check3, string name, string password, int age, bool exist)
        {
            string path = "C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\audio4.mp3";

            try
            {
                media_pl.Open(new Uri(path));
                if (path == " ")
                    throw new Exception("Отсутствует аудиофайл!");
                media_pl.Play();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            str_name = name;
            int_age = age;
            str_pass = password;
            existing = exist;

            words.Add("lion");
            words.Add("cat");
            words.Add("volf");

            words.Add("cow");
            words.Add("dog");
            words.Add("rabbit");

            words.Add("cat");
            words.Add("duck");
            words.Add("hamster");

            words.Add("sheep");
            words.Add("mouse");
            words.Add("bird");

            words.Add("horse");
            words.Add("fish");
            words.Add("pig");

            InitializeComponent();

            answer.Items.Add(words[ind1]);
            answer.Items.Add(words[ind2]);
            answer.Items.Add(words[ind3]);

            string connectionstring = ConfigurationManager.ConnectionStrings["English"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connectionstring))
            {
                conn.Open();

                string proc1 = "dbo.alive_simple"; // выбираем хранимую процедуру
                SqlCommand cmd = new SqlCommand(proc1, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr.Read())
                {
                    var f0 = dr[0];

                    questions_simple.Add((string)f0);
                }

                dr.Close(); // желательно закрывать

                string proc2 = "dbo.alive_difficult"; // выбираем хранимую процедуру
                SqlCommand cmd2 = new SqlCommand(proc2, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd2.CommandType = CommandType.StoredProcedure;

                SqlDataReader dr2 = cmd2.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr2.Read())
                {
                    var f0 = dr2[0];

                    questions_difficult.Add((string)f0);
                }

                dr2.Close(); // желательно закрывать
            }

            txt_choose_answer.Text = questions_simple[0];

            now.Text = left.ToString();
            txt_right.Text = "0";
            txt_wrong.Text = "0";

            if (check == true)
            {
                txt_timer.Visibility = Visibility.Visible;
                txt_timer2.Visibility = Visibility.Visible;
                it_image.Visibility = Visibility.Visible;
                dt.Tick += new EventHandler(Show_timer);
                dt.Interval = TimeSpan.FromSeconds(1);
                dt.Start();
            }
            if (check == false)
                it_image2.Visibility = Visibility.Visible;
            if (check2 == true)
                limit_wrong = 3;
            if (check3 == true)
                inner_wrong = 2;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (answer.SelectedItem != null)
            {
                if (answer.SelectedItem.ToString() == right_answers[ind_right])
                {
                    System.Windows.MessageBox.Show("Правильно!");
                    flag = false;
                    txt_right.Text = (++right).ToString();
                    ind_right++;
                }
                else
                {
                    System.Windows.MessageBox.Show("Ошибка!");
                    if (inner_wrong == 1 || flag)
                    {
                        ind_right++;
                        txt_wrong.Text = (++wrong).ToString();
                        flag = false;
                        if (wrong == limit_wrong)
                        {
                            right = 0;
                            media_pl.Stop();
                            System.Windows.MessageBox.Show($"Вы допустили максимальное количество ошбок! Игра закончилась! Вы набрали {right} очков!");
                            Settings set = new Settings();
                            set.Show();
                            Close();
                            return;
                        }
                    }
                    else if (inner_wrong == 2 && !flag)
                    {
                        flag = true;
                        System.Windows.MessageBox.Show("У вас есть еще 1 попытка!");
                    }
                }
            }
            else
                System.Windows.MessageBox.Show("Сначала выберите ответ!");

            if (inner_wrong == 1 || (inner_wrong == 2 && !flag))
            {
                ind1 += 3;
                ind2 += 3;
                ind3 += 3;

                if (left != 5)
                {
                    answer.Items.Clear();

                    answer.Items.Add(words[ind1]);
                    answer.Items.Add(words[ind2]);
                    answer.Items.Add(words[ind3]);
                }

                if (left == 1)
                    my_image.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\lion.jpg");
                else if (left == 2)
                    my_image.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\bird.jpg");
                else if (left == 3)
                    my_image.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\dog.jpg");
                else if (left == 4)
                    my_image.Source = (ImageSource)new ImageSourceConverter().ConvertFromString("C:\\Users\\Nadya\\source\\repos\\Net_Framework_The_final\\Net_Framework_The_final\\fish.jpg");
                else if (left == 5)
                {
                    media_pl.Stop();
                    System.Windows.MessageBox.Show($"Игра закончилась! Вы набрали {right} очков");
                    dt.Stop();

                    player = new Player { Nickname = str_name, Age = int_age, Rating = right, Password = str_pass };


                    if (!existing)
                    {
                        try
                        {
                            // десериализуем объект
                            using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                            {
                                players = (List<Player>)xmlser.Deserialize(fs);
                            }
                        }
                        catch (Exception)
                        {

                        }
                        finally
                        {
                            players.Add(player);

                            // получаем поток, куда будем записывать сериализованный объект
                            using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                            {
                                xmlser.Serialize(fs, players); // принимает поток и обьект
                            }
                        }
                    }
                    else
                    {
                        // десериализуем объект
                        using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                        {
                            players = (List<Player>)xmlser.Deserialize(fs);

                            foreach (Player pl in players)
                            {
                                if (pl.Nickname == player.Nickname)
                                    pl.Rating += right;
                            }
                        }

                        // получаем поток, куда будем записывать сериализованный объект
                        using (FileStream fs = new FileStream("Players.txt", FileMode.OpenOrCreate))
                        {
                            xmlser.Serialize(fs, players);
                        }
                    }

                    media_pl.Stop();
                    Settings set = new Settings();
                    set.Show();
                    Close();
                    return;
                }
                now.Text = (++left).ToString();
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            media_pl.Stop();
            System.Windows.MessageBox.Show("До новых встреч!");
            Settings set = new Settings();
            set.Show();
            Close();
        }

        private void Show_timer(object sender, EventArgs e)
        {
            if (txt_timer2.Text != "0")
                txt_timer2.Text = (--count).ToString();
            else
            {
                media_pl.Stop();
                dt.Stop();
                right = 0;
                System.Windows.MessageBox.Show($"К сожалению, Вам не хватило времени! Вы набрали {right} очков"); media_pl.Stop();
                Settings set = new Settings();
                set.Show();
                Close();
            }
        }
    }
}
