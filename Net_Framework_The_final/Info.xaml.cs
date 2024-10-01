using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Xml.Serialization;

namespace English_for_kids
{
    /// <summary>
    /// Interaction logic for Info.xaml
    /// </summary>
    public partial class Info : Window
    {
        Socket sock;
        IPAddress ip;
        IPEndPoint ipep;
        Server server;
        delegate void add_del(Object obj);
        delegate void con_del();
        ComboBox my_combo;
        string control_str = "m"; // основной вопрос

        public Info()
        {
            InitializeComponent();

            CreateServer();
        }

        public void Add_text(Object obj)
        {
            answer_panel.Children.Add((UIElement)obj);
        }

        private void CreateServer()
        {
            Server server = new Server(combo_questions);
        }

        private async void ConnectToServer()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16);

            using TcpClient client = new();

            // подсоединяемся к серверу
            await client.ConnectAsync(ipEndPoint);

            // создаем наш поток - получая от клиента поток
            await using NetworkStream stream = client.GetStream();

            // далее будем считывать сообщения от сервера
            // создаем буфер
            var buffer = new byte[1024];

            // преобразовываем в массив байт
            var byteMsg = Encoding.UTF8.GetBytes(combo_questions.SelectedIndex.ToString()+control_str);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream.WriteAsync(byteMsg);

            //// вернется размер того, что считали
            int recLength = await stream.ReadAsync(buffer);

            //// расшифровываем сообщение
            var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

            TextBlock txt_answer = new TextBlock();

            txt_answer.Text = msg;
            txt_answer.Margin = new Thickness(0, 0, 10, 0);
            txt_answer.TextWrapping = TextWrapping.Wrap;
            txt_answer.Width = 500;
            txt_answer.FontSize = 14;
            txt_answer.FontStyle = FontStyles.Italic;
            txt_answer.FontFamily = new FontFamily("Times New Roman");

            Dispatcher.BeginInvoke(new add_del(Add_text), txt_answer);

            if (msg == "Пожалуйста, выберите интересующий Вас режим")
            {
                ComboBox combo = new ComboBox();
                combo.Margin = new Thickness(0, 0, 20, 0);

                combo.Items.Add("Игра на время");
                combo.Items.Add("Ограниченное число ошибок");
                combo.Items.Add("Дополнительная попытка");

                my_combo = combo;

                Dispatcher.BeginInvoke(new add_del(Add_text), combo);

                my_combo.SelectionChanged += ComboBox_SelectedChanged;
            }
        }

        private async void ConnectToServer_Add()
        {
            var ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 16);

            using TcpClient client = new();

            // подсоединяемся к серверу
            await client.ConnectAsync(ipEndPoint);

            // создаем наш поток - получая от клиента поток
            await using NetworkStream stream = client.GetStream();

            // далее будем считывать сообщения от сервера
            // создаем буфер
            var buffer = new byte[1024];

            control_str = "a"; // add - доп режим

            // преобразовываем в массив байт
            var byteMsg = Encoding.UTF8.GetBytes(my_combo.SelectedIndex.ToString()+control_str);

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream.WriteAsync(byteMsg);

            //// вернется размер того, что считали
            int recLength = await stream.ReadAsync(buffer);

            //// расшифровываем сообщение
            var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

            TextBlock txt_answer = new TextBlock();

            txt_answer.Text = msg;
            txt_answer.Margin = new Thickness(0, 0, 10, 0);
            txt_answer.TextWrapping = TextWrapping.Wrap;
            txt_answer.Width = 500;
            txt_answer.FontSize = 14;
            txt_answer.FontStyle = FontStyles.Italic;
            txt_answer.FontFamily = new FontFamily("Times New Roman");

            Dispatcher.BeginInvoke(new add_del(Add_text), txt_answer);

            control_str = "m"; // main - главная
        }

        private void ComboBox_SelectedChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < answer_panel.Children.Count; i++)
            {
                if (answer_panel.Children[i].GetType() == typeof(TextBlock))
                    answer_panel.Children.Remove(answer_panel.Children[i]);
            }
            ConnectToServer_Add();
        }

        private void combo_questions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            answer_panel.Children.Clear();
            ConnectToServer();
        }
    }
}
