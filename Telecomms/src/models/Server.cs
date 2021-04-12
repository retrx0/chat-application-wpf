using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Telecomms.src.models
{
    public class Server : MainWindow
    {
        public struct ClientInfo
        {
            public Socket socket;   //Socket of the client
            public string strName;  //Name by which the user logged into the chat room
        }

        public struct ClientServerInfo
        {
            public int serverSocket;
            public string username;  
        }

        public ArrayList clientList { get; set; }
        public ArrayList clientServerList { get; set; }

        MainWindow mainWindowInstance;
        int portNumber = 2000;

        public string groupCode { get; set; }

        public Socket serverSocket { get; set; }

        public Client broadcastClient { get; set; }

        byte[] byteData = new byte[20 * 1024];

        public Server(MainWindow mainw, int port)
        {
            clientList = new ArrayList();
            clientServerList = new ArrayList();
            this.mainWindowInstance = mainw;
            portNumber = port;
            broadcastClient = new Client(port);
            broadcastClient.ConnectWithNoCommand();
        }

        private delegate void UpdateDelegate(string pMessage);

        public void Window_Loaded()
        {
            try
            {
                //We are using TCP sockets
                //Control.CheckForIllegalCrossThreadCalls = false;
                serverSocket = new Socket(AddressFamily.InterNetwork,
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

                //Assign the any IP of the machine and listen on port number 1000
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, portNumber);

                //Bind and listen on the given address
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen(4);

                //Accept the incoming clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);
                //serverSocket.Accept();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Creating Server");
            }
        }
        private void OnAccept(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = serverSocket.EndAccept(ar);

                //Start listening for more clients
                serverSocket.BeginAccept(new AsyncCallback(OnAccept), null);

                //Once the client connects then start receiving the commands from her
                //clientSocket.Receive(byteData, totalBytesRead, byteData.Length - totalBytesRead, SocketFlags.None);
                clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);

                //Application.Current.Dispatcher.BeginInvoke(new Action(() =>{
                //    mainWindowInstance.initUserView("User: Unknown", CustomButton.ButtonType.USER, new Client(3000));
                //    }
                //));

                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Connecting to Server");
            }
        }

        bool flag1 = true;
        int flag2 = 0;

        private void OnReceive(IAsyncResult ar)
        {
            try
            {
                Socket clientSocket = (Socket)ar.AsyncState;
                clientSocket.EndReceive(ar);

                //Transform the array of bytes received from the user into an
                //intelligent form of object Data

                Data msgReceived = new Data(byteData);
                //We will send this object in response the users request
                Data msgToSend = new Data();

                byte[] message;

                //If the message is to login, logout, or simple text message
                //then when send to others the type of the message remains the same
                msgToSend.cmdCommand = msgReceived.cmdCommand;
                msgToSend.strName = msgReceived.strName;
                Client cl = null;
                if (flag1)
                {
                    if (msgReceived.strMessage != null)
                    {
                        Console.WriteLine("--->>>"+msgReceived.strMessage);
                        int r = 9000;
                        if (int.TryParse(msgReceived.strMessage,out r))
                        {
                            cl = new Client(int.Parse(msgReceived.strMessage), msgReceived.strName);
                            cl.clientType = Client.ClientType.NEW_USER;
                            cl.OnLoginPressed();
                        }
                    }
                }


                switch (msgReceived.cmdCommand)
                {
                    case Command.Login:

                        ClientInfo clientInfo = new ClientInfo();
                        clientInfo.socket = clientSocket;
                        clientInfo.strName = msgReceived.strMessage;
                        clientList.Add(clientInfo);

                        if (flag1)
                        {
                            //When a user logs in to the server then we add her to our
                            //list of clients
                            msgToSend.cmdCommand = Command.Accept;
                            //message = msgToSend.ToByte();
                            //clientSocket.Send(message);

                            //Set the text of the message that we will broadcast to all users
                            msgToSend.strMessage = "<<<" + msgReceived.strName + " has joined the room>>>";
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                                string someinfo = "<< " + msgReceived.strName + " added >>";
                                mainWindowInstance.appendSomeInfo(someinfo);
                                if (msgReceived.strMessage != null)
                                    {
                                        if (cl.clientType == Client.ClientType.NEW_USER)
                                        {
                                            mainWindowInstance.initUserView("User: "+msgReceived.strName, CustomButton.ButtonType.USER, cl, null);
                                            mainWindowInstance.selectedUser.Messages.Add(someinfo);
                                        }
                                    }
                                }));
                            flag1 = false;
                            flag2++;
                        }
                        break;
                    case Command.Logout:

                        //When a user wants to log out of the server then we search for her 
                        //in the list of clients and close the corresponding connection

                        int nIndex = 0;
                        foreach (ClientInfo client in clientList)
                        {
                            if (client.socket == clientSocket)
                            {
                                clientList.RemoveAt(nIndex);
                                break;
                            }
                            ++nIndex;
                        }

                        clientSocket.Close();

                        msgToSend.strMessage = "<<<" + msgReceived.strName + " has left the room>>>";
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            mainWindowInstance.appendSomeInfo("<<<" + msgReceived.strName + " has left the room>>>");
                        }));
                        break;

                    case Command.Message:

                        //Set the text of the message that we will broadcast to all users
                        msgToSend.strMessage = msgReceived.strName + ": " + msgReceived.strMessage;
                        Console.WriteLine(msgReceived.strMessage);
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            mainWindowInstance.wrapMessage(msgReceived.strName, msgReceived.strMessage);
                            mainWindowInstance.selectedUser.Messages.Add(msgReceived.strName+": "+msgReceived.strMessage);
                        }));
                        break;

                    case Command.List:

                        //Send the names of all users in the chat room to the new user
                        msgToSend.cmdCommand = Command.List;
                        msgToSend.strName = null;
                        msgToSend.strMessage = null;

                        //Collect the names of the user in the chat room
                        foreach (ClientInfo client in clientList)
                        {
                            //To keep things simple we use asterisk as the marker to separate the user names
                            msgToSend.strMessage += client.strName + "*";
                            message = msgToSend.ToByte();

                            //Send the name of the users in the chat room
                            clientSocket.BeginSend(message, 0, message.Length, SocketFlags.None,
                                    new AsyncCallback(OnSend), clientSocket);
                            Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                                Console.WriteLine("LIST: " + msgToSend.strMessage);
                                Console.WriteLine("Clients " + client.socket);
                                //mainWindowInstance.wrapMessage(msgReceived.strName, msgReceived.strMessage);
                            }));
                        }
                        break;

                    case Command.File:
                        Console.WriteLine(msgReceived.strMessage);
                        string[] fn = msgReceived.strName.Split('\\');
                        string filename = fn[fn.Length - 1];
                        Console.WriteLine(filename);
                        Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                            mainWindowInstance.onDownloadFile(filename,msgReceived.strMessage);
                        }));
                        break;

                    case Command.Broadcast:
                        Console.WriteLine("Broadcast " + msgReceived.strName + " : " + msgReceived.strMessage);
                        ClientServerInfo ci = new ClientServerInfo();
                        ci.serverSocket = int.Parse(msgReceived.strMessage);
                        ci.username = msgReceived.strName;
                        clientServerList.Add(ci);
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            string info = "<< " + msgReceived.strName + " joined the server >>";
                            mainWindowInstance.appendSomeInfo(info);
                            mainWindowInstance.selectedUser.Messages.Add(info);
                        }));
                        break;

                    case Command.BroadcastMessage:
                        message = msgToSend.ToByte();
                        foreach (ClientServerInfo ci2 in clientServerList)
                        {
                            Data broadCastMsg = new Data();
                            broadCastMsg.cmdCommand = Command.Message;
                            broadCastMsg.strMessage = msgToSend.strMessage;
                            broadCastMsg.strName = msgToSend.strName;
                            byte[] b = broadCastMsg.ToByte();

                            //Client _cl1 = new Client(ci2.serverSocket);
                            //_cl1.OnLoginPressed();
                            //ClientMessage _cm1 = new ClientMessage(_cl1.clientSocket, msgToSend.strName, null);
                            //_cm1.sendMessage(msgToSend.strMessage);
                            //_cl1.clientSocket.Send(b,0,b.Length, SocketFlags.None);
                            //Console.WriteLine(msgToSend.strMessage);
                            //Console.WriteLine("Broadcast Message in Server " + ci2.username + " : " + ci2.serverSocket);
                            //Send the message to all users
                            //ci2.socket.BeginSend(b, 0, b.Length, SocketFlags.None, new AsyncCallback(OnSend), ci2.socket);
                            //ci2.socket.Send(message, 0, message.Length, SocketFlags.None);
                        }
                        break;
                }

                if (msgToSend.cmdCommand != Command.List)   //List messages are not broadcasted
                {
                    message = msgToSend.ToByte();

                    foreach (ClientInfo cInfo in clientList)
                    {
                        if (cInfo.socket != clientSocket ||
                            msgToSend.cmdCommand != Command.Login)
                        {
                            //Send the message to all users
                            //clientInfo.socket.BeginSend(message, 0, message.Length, SocketFlags.None,
                            //new AsyncCallback(OnSend), clientInfo.socket);
                            //cInfo.socket.Send(message, 0, message.Length, SocketFlags.None);
                            //Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            //{
                            //    mainWindowInstance.wrapMessage(msgReceived.strName, msgReceived.strMessage);
                            //    mainWindowInstance.selectedUser.Messages.Add(msgReceived.strName + ": " + msgReceived.strMessage);
                            //}));
                        }
                    }
                    //textBox1.Text += msgToSend.strMessage;
                    //UpdateDelegate update = new UpdateDelegate(wrapMessage);

                }

                //If the user is logging out then we need not listen from her
                if (msgReceived.cmdCommand != Command.Logout)
                {
                    //Start listening to the message send by the user
                    clientSocket.BeginReceive(byteData, 0, byteData.Length, SocketFlags.None, new AsyncCallback(OnReceive), clientSocket);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
                throw;
                MessageBox.Show(ex.Message, "Receiving: Server");
            }
        }

        public void OnSend(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                client.EndSend(ar);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sending: Server");
            }
        }

    }
}
