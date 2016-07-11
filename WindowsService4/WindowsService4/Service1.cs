using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System.IO;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace WindowsService4
{
    public partial class Service1 : ServiceBase
    {
        static string defaultpath = "D:\\wfa";
        static Socket serverSocket;
        private static byte[] result = new byte[1024];
        string path = File.ReadAllText(defaultpath + "\\" + "pathfile.txt");
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            IPAddress ip = IPAddress.Any;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            serverSocket.Bind(new IPEndPoint(ip, 8885));  //绑定IP地址：端口  
            serverSocket.Listen(50);
            Thread th = new Thread(server);
            th.Start(serverSocket);
        }
        private void server(Object sk)
        {
            while (true)
            {
                Socket s = (Socket)sk;
                Socket clientSocket = s.Accept();
                Thread reciveTh = new Thread(oneConnection);
                reciveTh.Start(clientSocket);
            }
        }

        private void oneConnection(Object clientSocket)
        {
            Socket myClientSocket = (Socket)clientSocket;
            string conselected = "";
            while (true)
            {
                int receivePasswordNumber;
                try
                {
                    receivePasswordNumber = myClientSocket.Receive(result);
                }
                catch { return; }
                string password = Encoding.ASCII.GetString(result, 0, receivePasswordNumber);
                if (string.Compare(password, File.ReadAllText(defaultpath + "\\" + "password.txt")) != 0)
                {
                    myClientSocket.Send(Encoding.ASCII.GetBytes("wrongp"));
                    continue;
                }
                else { break; }
            }
            myClientSocket.Send(Encoding.ASCII.GetBytes("rightp"));
            Thread sendth = new Thread(send);
            sendth.Start(myClientSocket);
            List<Thread> sending = new List<Thread>();
            while (true)
            {
                int receiveNumber;
                try { receiveNumber = myClientSocket.Receive(result); }
                catch
                {
                    sendth.Abort();
                    foreach (Thread t in sending)
                        t.Abort();
                    return;
                }
                if (receiveNumber == 0)
                {
                    sendth.Abort();
                    foreach (Thread t in sending)
                        t.Abort();
                    return;
                }
                string s = Encoding.ASCII.GetString(result, 0, receiveNumber);
                switch (s.Substring(0, 3))
                {
                    case "his":
                        try
                        {
                            string b = s.Substring(3, s.Length - 3);
                            int a = b.IndexOf("a"),
                                y = b.IndexOf("y"),
                                m = b.IndexOf("m"),
                                d = b.IndexOf("d");
                            try
                            {
                                string his = File.ReadAllText(path + "\\" + "类型 " + b.Substring(1, 1) + " 地址 " + b.Substring(a + 1, y - 1 - a) + "." + b.Substring(y + 1, m - 1 - y) + "年" + b.Substring(m + 1, d - 1 - m) + "月" + b.Substring(d + 1, b.Length - d - 1) + "日.txt");
                                myClientSocket.Send(Encoding.ASCII.GetBytes("hisdata" + his));
                            }
                            catch
                            {
                                myClientSocket.Send(Encoding.ASCII.GetBytes("hisdatanull"));
                            }
                            break;
                        }
                        catch { break; }
                    case "con":
                        foreach (Thread t in sending)
                            t.Abort();
                        try
                        {
                            conselected = s.Substring(3, s.Length - 3);
                            sendcondata scd = new sendcondata(myClientSocket, conselected);
                            Thread th = new Thread(new ThreadStart(scd.senddata));
                            th.Start();
                            sending.Add(th);
                            break;
                        }
                        catch { break; }
                }
            }
        }
        private  void send(Object sk)
        {
            while (true)
            {
                Socket send = (Socket)sk;             
                StringBuilder histdt = new StringBuilder();
                DirectoryInfo TheFolder = new DirectoryInfo(path);
                FileInfo[] tas = TheFolder.GetFiles();
                try
                {
                    for (int j = 0; j < tas.Length; j++)
                    {
                        if (tas[j].Name.Length > 20)
                        {
                            bool a = true;
                            for (int i = 0; i < j; i++)
                            {
                                if (tas[j].Name.Substring(3, 1) + tas[j].Name.Substring(8, 1) == tas[i].Name.Substring(3, 1) + tas[i].Name.Substring(8, 1))
                                { a = false; }
                            }
                            if (a == true)
                                histdt.Append("type " + tas[j].Name.Substring(3, 1) + " add " + tas[j].Name.Substring(8, 1) + ",");
                        }
                    }
                    send.Send(Encoding.ASCII.GetBytes("histdt" + histdt.ToString()));
                }
                catch { }
                string connecting = File.ReadAllText(defaultpath + "\\" + "connecting" + ".txt");
                string[] con = connecting.Split('\n');
                StringBuilder contdt = new StringBuilder();
                for (int i = 0; i < con.Length; i++)
                {
                    try
                    {
                        contdt.Append("type " + con[i].Substring(3, 1) + " add " + con[i].Substring(8, 1) + ",");
                    }
                    catch { }
                }
                send.Send(Encoding.ASCII.GetBytes("contdt" + contdt.ToString()));
                Thread.Sleep(1000);
            }
        }
        public class sendcondata
        {
            private string s;
            Socket sendconsocket;
            public sendcondata(Socket a, string b)
            {
                sendconsocket = a;
                s = b;
            }
            public void senddata()
            {
                string lastbuffer = "";
                while (true)
                {
                    try
                    {
                        string read = File.ReadAllText(defaultpath + "\\" + "类型 " + s.Substring(1, 1) + " 地址 " + s.Substring(3, 1) + "buffer" + ".txt");
                        if (read != lastbuffer)
                        {
                            sendconsocket.Send(Encoding.ASCII.GetBytes("condata" + read));
                            lastbuffer = read;
                        }
                    }
                    catch
                    {
                        sendconsocket.Send(Encoding.ASCII.GetBytes("disconnecting"));
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}

