using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace Telecomms.src.models
{
    public class Client
    {

        public Socket clientSocket;
        public string strName;
        public string username { get; set; }
        public int portNumber { get; set; } = 2000;
        public IPEndPoint ipEndPoint { get; set; }

        public MainWindow mainWindowInstance { get; set; }

        public ClientType clientType { get; set; }

        public bool addToView { get; set; }

        public delegate string getNameDelegate();
        public delegate void UjFormDelegate();

        public enum ClientType
        {
            NEW_USER, JOIN_GROUP, NONE
        }

        public Client(int port) {
            this.portNumber = port;
            this.clientType = ClientType.NONE;

            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Client(int port, string username)
        {
            this.portNumber = port;
            this.clientType = ClientType.NONE;
            this.username = username;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public Client(int port, string username, MainWindow main)
        {
            this.portNumber = port;
            this.clientType = ClientType.NONE;
            this.username = username;
            this.mainWindowInstance = main;
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public void OnLoginPressed()
        {
            try
            {
                //Connect to the server
                //clientSocket.Connect(ipEndPoint);
                string myIpAddress = GetLocalIPAddress();
                IPAddress ipAddress = IPAddress.Parse(myIpAddress);
                ipEndPoint = new IPEndPoint(ipAddress, portNumber);
                clientSocket.BeginConnect(ipAddress, portNumber, new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to user");
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Login;

                //l_fhName = this.textBox1.Text;
                //getNameDelegate fhName = new getNameDelegate(getLoginName);
                //l_fhName = (string)this.textBox1.Dispatcher.Invoke(fhName, null);

                msgToSend.strName = username;
                if (mainWindowInstance != null) msgToSend.strMessage = "" + mainWindowInstance._randPort;
                else msgToSend.strMessage = null;
                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }

        public void ConnectToGroupServer()
        {
            try
            {
                string myIpAddress = GetLocalIPAddress();
                IPAddress ipAddress = IPAddress.Parse(myIpAddress);
                ipEndPoint = new IPEndPoint(ipAddress, portNumber);
                clientSocket.BeginConnect(ipAddress, portNumber, new AsyncCallback(OnGroupConnect), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connect to group");
            }
        }

        private void OnGroupConnect(IAsyncResult ar)
        {
            try
            {
                clientSocket.EndConnect(ar);

                //We are connected so we login into the server
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Broadcast;

                msgToSend.strName = username;
                msgToSend.strMessage = "" + mainWindowInstance._randPort;
                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connecting to group");
            }
        }

        //public void ConnectWithNoCommand()
        //{
        //    try
        //    {
        //        string myIpAddress = GetLocalIPAddress();
        //        IPAddress ipAddress = IPAddress.Parse(myIpAddress);
        //        ipEndPoint = new IPEndPoint(ipAddress, portNumber);
        //        clientSocket.BeginConnect(ipAddress, portNumber, new AsyncCallback(NoCommandConnect), null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Connect to group");
        //    }
        //}
        //private void NoCommandConnect(IAsyncResult ar)
        //{
        //    try
        //    {
        //        clientSocket.EndConnect(ar);

        //        //We are connected so we login into the server
        //        Data msgToSend = new Data();
        //        msgToSend.cmdCommand = Command.Null;

        //        msgToSend.strName = username;
        //        msgToSend.strMessage = "" + mainWindowInstance._randPort;
        //        byte[] b = msgToSend.ToByte();

        //        //Send the message to the server
        //        clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "Connecting with no command");
        //    }
        //}

        private void OnSend(IAsyncResult ar)
        {
            try
            {

                clientSocket.EndSend(ar);
                byte[] byteData = new byte[8192];

                //Várunk a válaszra
                clientSocket.Receive(byteData, 0, 8192, SocketFlags.None);

                Data msg = new Data(byteData);

                //UjFormDelegate pForm = new UjFormDelegate(UjForm);
                //this.Dispatcher.Invoke(pForm, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sending: Client");
            }
        }

    }
}
