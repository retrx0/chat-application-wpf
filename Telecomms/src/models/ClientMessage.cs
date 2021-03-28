using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Telecomms.src.models
{
    class ClientMessage
    {
        public Socket ClientSocket;
        public string Username;
        byte[] byteData = new byte[1024];

        private delegate void UpdateDelegate(string pMessage);

        private void UpdateMessage(string pMessage)
        {
            //this.textBox1.Text += pMessage;
        }

        public ClientMessage(Socket pSocket, string pName)
        {
            ClientSocket = pSocket;
            Username = pName;

            ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), ClientSocket);
        }

        private void OnReceive(IAsyncResult ar)
        {

            Socket clientSocket = (Socket)ar.AsyncState;
            clientSocket.EndReceive(ar);


            //Transform the array of bytes received from the user into an
            //intelligent form of object Data
            Data msgReceived = new Data(byteData);

            ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None,
                                    new AsyncCallback(OnReceive), ClientSocket);

            UpdateDelegate update = new UpdateDelegate(UpdateMessage);
            ///this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, update,
              //  msgReceived.strMessage + "\r\n");
        }

        public void sendMessage(string message)
        {
            Data msgToSend = new Data();
            msgToSend.cmdCommand = Command.Message;

            msgToSend.strName = Username;
            msgToSend.strMessage = message;

            byte[] b = msgToSend.ToByte();
            ClientSocket.Send(b);
        }

    }
}
