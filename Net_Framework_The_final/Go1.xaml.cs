using System;
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
        SqlDataReader dr;

        List<string> questions = new List<string>();
        List<string> answers = new List<string>();

        string connectionstring = ConfigurationManager.ConnectionStrings["English"].ConnectionString;
        string str_name, str_pass, right_answer = "", procedure;
        int right = 0, wrong = 0, limit_wrong = 5, inner_wrong = 1, left = 1, count = 60, int_age, index_question = 0, id_question;
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

            InitializeComponent();

            using (conn = new SqlConnection(connectionstring))
            {
                conn.Open();

                procedure = "dbo.alive_simple"; // выбираем хранимую процедуру
                cmd = new SqlCommand(procedure, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr.Read())
                    questions.Add((string)dr[0]);

                dr.Close();

                procedure = "dbo.alive_difficult"; // выбираем хранимую процедуру
                cmd = new SqlCommand(procedure, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd.CommandType = CommandType.StoredProcedure;

                dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr.Read())
                    questions.Add((string)dr[0]);

                dr.Close();

                procedure = "dbo.know_id_question"; // выбираем хранимую процедуру
                cmd = new SqlCommand(procedure, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@question", questions[index_question]); // нужно быть уверенным, что второй пар-р нужного типа

                dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr.Read())
                    id_question = (int)dr[0];

                dr.Close();

                procedure = "dbo.show_answers"; // выбираем хранимую процедуру
                cmd = new SqlCommand(procedure, conn);
                // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@id", id_question); // нужно быть уверенным, что второй пар-р нужного типа

                dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                while (dr.Read())
                    answers.Add((string)dr[0]);

                dr.Close();
            }

            answer.Items.Add(answers[0]);
            answer.Items.Add(answers[1]);
            answer.Items.Add(answers[2]);

            txt_choose_answer.Text = questions[index_question];

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
                using (conn = new SqlConnection(connectionstring))
                {
                    conn.Open();

                    procedure = "dbo.show_right_answer"; // выбираем хранимую процедуру
                    cmd = new SqlCommand(procedure, conn);
                    // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@id", id_question); // нужно быть уверенным, что второй пар-р нужного типа

                    dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                    while (dr.Read())
                        right_answer = (string)dr[0];

                    dr.Close();
                }

                index_question++;

                if (answer.SelectedItem.ToString() == right_answer)
                {
                    System.Windows.MessageBox.Show("Правильно!");
                    flag = false;
                    txt_right.Text = (++right).ToString();
                }
                else
                {
                    System.Windows.MessageBox.Show("Ошибка!");
                    if (inner_wrong == 1 || flag)
                    {
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

                answers.Clear();

                if (index_question < 5)
                {
                    using (conn = new SqlConnection(connectionstring))
                    {
                        conn.Open();

                        procedure = "dbo.know_id_question"; // выбираем хранимую процедуру
                        cmd = new SqlCommand(procedure, conn);
                        // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@question", questions[index_question]); // нужно быть уверенным, что второй пар-р нужного типа

                        dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                        while (dr.Read())
                            id_question = (int)dr[0];

                        dr.Close();

                        procedure = "dbo.show_answers"; // выбираем хранимую процедуру
                        cmd = new SqlCommand(procedure, conn);
                        // определаем у нашей команды тип - 'StoredProcedure', дефолтно - текст (как раньше делали)
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@id", id_question); // нужно быть уверенным, что второй пар-р нужного типа

                        dr = cmd.ExecuteReader(); // достаем строку, а это не одно значение, а целый массив записей

                        while (dr.Read())
                            answers.Add((string)dr[0]);

                        dr.Close();
                    }
                }
            }
            else
                System.Windows.MessageBox.Show("Сначала выберите ответ!");

            if (inner_wrong == 1 || (inner_wrong == 2 && !flag))
            {
                if (left != 5)
                {
                    txt_choose_answer.Text = questions[index_question];

                    answer.Items.Clear();

                    answer.Items.Add(answers[0]);
                    answer.Items.Add(answers[1]);
                    answer.Items.Add(answers[2]);
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
