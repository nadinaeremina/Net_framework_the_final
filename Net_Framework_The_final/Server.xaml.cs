using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace English_for_kids
{
    /// <summary>
    /// Логика взаимодействия для Server.xaml
    /// </summary>
    public partial class Server : Window
    {
        Socket s;

        public Server(ComboBox combo)
        {
            InitializeComponent();

            Connect();
        }

        private async void Connect()
        {
            await ConnectServer();
        }

        static async Task ConnectServer()
        {
            // тк мы прослушиваем - то указываем любой адрес
            var ipEndPoint = new IPEndPoint(IPAddress.Any, 16);

            TcpListener listener = new TcpListener(ipEndPoint);

            try
            {
                // запускаем 'listener', который будет слушать входящие соед-ия
                listener.Start();

                // это тот клиент, который подключился - под него заводим тоже 'TcpClient'
                // получаем его с помощью 'Accept' - вернем 'TcpClient'
                // (также как с помощью него можно вернуть и сокет)
                using TcpClient handler = await listener.AcceptTcpClientAsync();
                // если бы исп-ли без 'Async' - то выносили бы в отдельный поток

                // ожидаем, когда предыдущий процесс завершится - поэтому 'await'
                await using NetworkStream stream = handler.GetStream();

                // далее будем считывать сообщения от сервера
                // создаем буфер
                var buffer = new byte[1024];

                // вернется размер того, что считали
                int recLength = await stream.ReadAsync(buffer); 

                // расшифровываем сообщение
                var msg = Encoding.UTF8.GetString(buffer, 0, recLength);

                string answer = "";

                if (msg == "0")
                    answer = "Это образовательное приложение было разработано с целью изучения английского языка.";
                else if (msg == "1")
                    answer = "Приложение расчитано на детей от 3 до 7 лет";
                else if (msg == "2")
                    answer = "Приложение имеет два раздела: 'Живое' и 'Неживое', в каждом разделе по 5 вопросов.";
                else if (msg == "3")
                    answer = "Приложение имеет три режима: 'Игра на время', 'Ограниченное число ошибок' и 'Дополнительная попытка'";
                else if (msg == "4")
                    answer = "Пожалуйста, выберите интересующий Вас режим";
                else if (msg == "5")
                    answer = "Чтобы отключить звук в игре - достаточно нажать";

                //// преобразовываем в массив байт
                var byteMsg = Encoding.UTF8.GetBytes(answer);

                //// передаем это сообщение с помощью метода 'WriteAsync'
                await stream.WriteAsync(byteMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}
