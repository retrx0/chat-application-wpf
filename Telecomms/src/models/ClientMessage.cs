using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Telecomms.src.models
{
    public class ClientMessage
    {
        public static Socket ClientSocket;
        public string Username { get; set; }
        MainWindow mainWindow;
        byte[] byteData = new byte[8192];

        private delegate void UpdateDelegate(string pMessage);

        private void UpdateMessage(string pMessage)
        {
            this.mainWindow.wrapMessage(Username, pMessage);
        }

        public ClientMessage(MainWindow main) { this.mainWindow = main; }

        public ClientMessage(Socket pSocket, string usernmae, MainWindow mainw)
        {
            ClientSocket = pSocket;
            Username = usernmae;
            this.mainWindow = mainw;
            ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), ClientSocket);
        }

        private void OnReceive(IAsyncResult ar)
        {

            Socket clientSocket = (Socket)ar.AsyncState;
            clientSocket.EndReceive(ar);

            //Transform the array of bytes received from the user into an
            //intelligent form of object Data
            Data msgReceived = new Data(byteData);

            ClientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), ClientSocket);

            UpdateDelegate update = new UpdateDelegate(UpdateMessage);
            ///this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, update,
              //  msgReceived.strMessage + "\r\n");
        }

        public void sendMessage(string message)
        {
            try
            {
                Data msgToSend = new Data();
                msgToSend.cmdCommand = Command.Message;

                msgToSend.strName = Username;
                msgToSend.strMessage = message;

                byte[] b = msgToSend.ToByte();
                ClientSocket.Send(b);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + " Problem sending message");
                //MessageBox.Show(e.Message, "Problem Sending Message");
            }
        }

        public void sendFile(string filename,string content,byte[] b)
        {
            try
            {
                Data filrToSend = new Data();
                filrToSend.cmdCommand = Command.File;
                filrToSend.strMessage = content;
                filrToSend.strName = filename;
                byte[] fileBytes = File.ReadAllBytes(filename);
                byte[] be = filrToSend.ToByte();
                ClientSocket.Send(be);
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message + " Problem sending file");
                //MessageBox.Show(r.Message, "Problem sending file");
            }
        }
        
        public void broadcastFile(string filename,string content,Server server)
        {
            try
            {
                Data filrToSend = new Data();
                filrToSend.cmdCommand = Command.File;
                filrToSend.strMessage = content;
                filrToSend.strName = filename;
                byte[] fileBytes = File.ReadAllBytes(filename);
                byte[] be = filrToSend.ToByte();
                ClientSocket.Send(be);

                foreach (Server.ClientServerInfo csi in server.clientServerList)
                {
                    Client _cl1 = new Client(csi.serverSocket);
                    _cl1.OnLoginPressed();
                    ClientMessage _cm1 = new ClientMessage(_cl1.clientSocket, Username, null);
                    _cm1.sendFile(filename, content, be);
                    _cl1.clientSocket.Send(be, 0, be.Length, SocketFlags.None);
                    Console.WriteLine("Broadcast File in CM " + csi.username + " : " + csi.serverSocket);
                }
            }
            catch (Exception r)
            {
                Console.WriteLine(r.Message + " Problem bradcasting file");
                //MessageBox.Show(r.Message, "Problem sending file");
            }
        }

        public void broadCastMessage(string message, Server server)
        {
            Data broadCastMsg = new Data();
            broadCastMsg.cmdCommand = Command.Message;
            broadCastMsg.strMessage = message;
            broadCastMsg.strName = Username;
            byte[] b = broadCastMsg.ToByte();
            //mainWindow.mServer.broadcastClient.clientSocket.Send(b, 0, b.Length, SocketFlags.None);

            foreach (Server.ClientServerInfo csi in server.clientServerList)
            {
                Client _cl1 = new Client(csi.serverSocket);
                _cl1.OnLoginPressed();
                ClientMessage _cm1 = new ClientMessage(_cl1.clientSocket, Username, null);
                _cm1.sendMessage(message);
                _cl1.clientSocket.Send(b, 0, b.Length, SocketFlags.None);
                Console.WriteLine(message);
                Console.WriteLine("Broadcast Message in CM " + csi.username + " : " + csi.serverSocket);
            }

        }

    }
}
