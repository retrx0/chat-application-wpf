﻿using System;
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
        public static string Username;
        MainWindow mainWindow;
        byte[] byteData = new byte[8192];

        private delegate void UpdateDelegate(string pMessage);

        private void UpdateMessage(string pMessage)
        {
            this.mainWindow.wrapMessage(Username, pMessage);
        }

        public ClientMessage(Socket pSocket, string pName, MainWindow mainw)
        {
            ClientSocket = pSocket;
            Username = pName;
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
                MessageBox.Show(e.Message, "Problem Sending Message");
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
                MessageBox.Show(r.Message, "Problem sending file");
            }
        }

        public static void broadCastMessage(string message, Server server)
        {
            Data broadCastMsg = new Data();
            broadCastMsg.cmdCommand = Command.Broadcast;
            broadCastMsg.strMessage = message;
            broadCastMsg.strName = Username;
            byte[] b = broadCastMsg.ToByte();

            foreach (Server.ClientServerInfo csi in server.clientServerList)
            {
                //Client cl = new Client(csi.serverSocket);
                //cl.OnLoginPressed();
                //cl.clientSocket.Send(b);
                Console.WriteLine(csi.username);
                //server.serverSocket.Send(b);
            }

        }

    }
}
