using System;
using System.Collections.Generic;
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
            var byteMsg = Encoding.UTF8.GetBytes(combo_questions.SelectedIndex.ToString());

            // передаем это сообщение с помощью метода 'WriteAsync'
            await stream.WriteAsync(byteMsg);

            //// вернется размер того, что считали
            int recLength = await stream.ReadAsync(buffer);

            //// расшифровываем сообщение
            var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

            TextBlock txt_answer = new TextBlock();

            txt_answer.Text = msg;
            txt_answer.TextWrapping = TextWrapping.Wrap;

            Dispatcher.BeginInvoke(new add_del(Add_text), txt_answer);

            if (msg == "Пожалуйста, выберите интересующий Вас режим")
            {
                ComboBox combo = new ComboBox();

                combo.Items.Add("Игра на время");
                combo.Items.Add("Ограниченное число ошибок");
                combo.Items.Add("Дополнительная попытка");

                Dispatcher.BeginInvoke(new add_del(Add_text), combo);
            }
        }

        private void combo_questions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ConnectToServer();
        }
    }
}
