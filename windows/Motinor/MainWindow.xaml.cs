using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using System.IO;

namespace Motinor
{
    public partial class MainWindow : Window
    {
        private UdpClient udpClient;
        private IPEndPoint serverEndPoint;
        private Thread receiveThread;
        private System.Timers.Timer heartbeatTimer;

        public MainWindow()
        {
            InitializeComponent();
            serverEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.9"), 8888); // 替换为服务器的IP和端口
            udpClient = new UdpClient();
            SendInitialMessage();
            StartHeartbeat();
            receiveThread = new Thread(ReceiveImages);
            receiveThread.Start();
        }

        private void SendInitialMessage()
        {
            byte[] message = Encoding.UTF8.GetBytes("Hello, server");
            udpClient.Send(message, message.Length, serverEndPoint);
        }

        private void StartHeartbeat()
        {
            heartbeatTimer = new System.Timers.Timer(4000); // 每4秒发送一次心跳消息
            heartbeatTimer.Elapsed += (sender, e) => SendHeartbeat();
            heartbeatTimer.AutoReset = true;
            heartbeatTimer.Enabled = true;
        }

        private void SendHeartbeat()
        {
            try
            {
                byte[] message = Encoding.UTF8.GetBytes("Heartbeat");
                udpClient.Send(message, message.Length, serverEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending heartbeat: {ex.Message}");
            }
        }

        private void ReceiveImages()
        {
            while (true)
            {
                try
                {
                    byte[] receivedData = udpClient.Receive(ref serverEndPoint);
                    Application.Current.Dispatcher.Invoke(() => UpdateImage(receivedData));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                }
            }
        }

        private void UpdateImage(byte[] imageData)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream(imageData))
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.StreamSource = ms;
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    ImageControl.Source = bitmap;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating image: {ex.Message}");
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            heartbeatTimer.Stop();
            receiveThread.Abort();
            udpClient.Close();
        }
    }
}