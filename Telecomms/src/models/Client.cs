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

        public delegate string getNameDelegate();
        public delegate void UjFormDelegate();

        public Client(int port) {
            this.portNumber = port;

            string myIpAddress = GetLocalIPAddress();
            IPAddress ipAddress = IPAddress.Parse(myIpAddress);
            ipEndPoint = new IPEndPoint(ipAddress, portNumber);
            //clientSocket.Connect(ipEndPoint);
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
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
                //string l_ip;
                
                //getNameDelegate IP = new getNameDelegate(getIP);
                //l_ip = (string)this.Dispatcher.Invoke(IP, null);

                //Server is listening on port 1000

                //Connect to the server
                //clientSocket.Connect(ipEndPoint);
                //clientSocket.BeginConnect(ipEndPoint, new AsyncCallback(OnConnect), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
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
                msgToSend.strMessage = null;

                byte[] b = msgToSend.ToByte();

                //Send the message to the server
                clientSocket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), null);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }



        private void OnSend(IAsyncResult ar)
        {
            try
            {

                clientSocket.EndSend(ar);
                byte[] byteData = new byte[1024];

                //Várunk a válaszra
                clientSocket.Receive(byteData, 0, 1024, SocketFlags.None);

                Data msg = new Data(byteData);

                //UjFormDelegate pForm = new UjFormDelegate(UjForm);
                //this.Dispatcher.Invoke(pForm, null);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "SGSclient");
            }
        }

        //private void UjForm()
        //{
        //    CliensMessage uj_form;
        //    uj_form = new CliensMessage(clientSocket, textBox1.Text);
        //    uj_form.Show();
        //    Close();
        //}

    }
}
