using System;
using System.Collections.Generic;
using System.Linq;
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
using Telecomms.src.models;
using System.Net.Sockets;
using System.Collections;
using System.Net;
using Telecomms.src;

namespace Telecomms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string username;
        public string password;

        Client groupClient;
        Client mClient;
        Client mGroupCLient;
        Client testClient;

        ClientMessage groupClientMessage;
        ClientMessage mgroupClientMessage;
        ClientMessage mClientMessage;

        Server groupServer;
        Server mServer;
        Server testServer;

        int _randPort = 0;

        public CustomButton selectedUser { get; set; }

        private delegate void UpdateDelegate(string pMessage);

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(string username, string password)
        {
            this.username = username;
            this.password = password;
            InitializeComponent();
            username_text_area.Text = username;
            Random rand = new Random();
            _randPort = rand.Next(2001, 3000);
            mServer = new Server(this, _randPort);
            mServer.Window_Loaded();
            userIdTextView.Text = "UID: " + encodeIpAndPort(_randPort, username, true);
        }

        public enum UserRequestType {
            CREATE_SERVER, JOIN_SERVER, ADDUSER
        }

        private void logoutOnClick(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        public void initUserView(string title, CustomButton.ButtonType bt, object o)
        {
            CustomButton newUser = new CustomButton(title);
            newUser.buttonType = bt;
            switch (bt){
                case CustomButton.ButtonType.CREATED_GROUP:
                    newUser.server = (Server) o;
                        break;
                case CustomButton.ButtonType.JOINED_GROUP:
                    newUser.client = (Client) o;
                    break;
                case CustomButton.ButtonType.USER:
                    newUser.client = (Client) o;
                    break;
            }
            selectedUser = newUser;
            newUser.Click += new RoutedEventHandler((object s, RoutedEventArgs e) => {
                chatTitle.Text = title;
                chatCode.Text = newUser.server.groupCode;
                selectedUser = newUser;
                Console.WriteLine(selectedUser.Content);
                /*chatStackPanel.Children.Clear();*/
            });

            Grid grid = new Grid();
            ColumnDefinitionCollection colums = grid.ColumnDefinitions;
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            colums.Add(col1);
            colums.Add(col2);

            //BitmapImage src = new BitmapImage(new Uri("./assets/img/profile.png"));
            //ImageSource src = System.Windows.Media.ImageSource();
            //Image img = new Image() { Source = "./assets/img/profile.png" };

            TextBlock text = new TextBlock() { Text = title };

            Border userBorder = new Border();
            userBorder.Effect = new System.Windows.Media.Effects.DropShadowEffect() { BlurRadius = 8, Opacity = 0.1, ShadowDepth = 2, Direction = 270 };
            userBorder.Child = newUser;
            userBorder.Margin = new System.Windows.Thickness(3);
            userBorder.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(236, 240, 255) };
            userBorder.CornerRadius = new System.Windows.CornerRadius(4);
            usersStack.Children.Add(userBorder);
        }

        public void wrapMessage(string user, string msg)
        {
            TextBlock message = new TextBlock();
            message.Margin = new System.Windows.Thickness(5);
            message.Padding = new System.Windows.Thickness(0);
            message.FontSize = 20;
            message.TextWrapping = TextWrapping.Wrap;

            message.Text = user + ": " + msg + Environment.NewLine;

            Border messageBorder = new Border();
            messageBorder.Child = message;
            messageBorder.Margin = new System.Windows.Thickness(3);
            messageBorder.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(246, 250, 255) };
            messageBorder.CornerRadius = new System.Windows.CornerRadius(4);
            this.chatStackPanel.Children.Add(messageBorder);
        }

        private void sendOnClick(object sender, RoutedEventArgs e)
        {
            switch (selectedUser.buttonType)
            {
                case CustomButton.ButtonType.JOINED_GROUP:
                    //groupClient = new Client(2000);
                    groupClientMessage = new ClientMessage(selectedUser.client.clientSocket, username, this);
                    groupClientMessage.sendMessage(messageTextInput.Text);
                    wrapMessage(username, messageTextInput.Text);
                    break;
                case CustomButton.ButtonType.USER:
                    mClientMessage = new ClientMessage(selectedUser.client.clientSocket, username, this);
                    mClientMessage.sendMessage(messageTextInput.Text);
                    wrapMessage(username, messageTextInput.Text);
                    break;
                default:
                    mgroupClientMessage = new ClientMessage(mGroupCLient.clientSocket, username, this);
                    mgroupClientMessage.sendMessage(messageTextInput.Text);
                    testClient = new Client(2005);
                    testClient.OnLoginPressed();
                    ClientMessage cm = new ClientMessage(testClient.clientSocket, username, this);
                    cm.sendMessage(messageTextInput.Text);
                    //wrapMessage(username, messageTextInput.Text);
                    break;
            }
        }

        public void showAndSetupDialog(UserRequestType urt, int port, string title, string messageBoxTitle)
        {
            AddDialog dialog = new AddDialog(title);
            dialog.ShowDialog();
            if (dialog.result != null) {
                switch (urt) {
                    case UserRequestType.CREATE_SERVER:
                        groupServer = new Server(this, port);
                        groupServer.Window_Loaded();
                        mGroupCLient = new Client(2000);
                        mGroupCLient.OnLoginPressed();
                        
                        initUserView("Group: " + dialog.result, CustomButton.ButtonType.CREATED_GROUP, groupServer);
                        chatTitle.Text = dialog.result;
                        string code = encodeIpAndPort(2000, dialog.result, true);
                        groupServer.groupCode = code;
                        chatCode.Text = code;
                        break;
                    case UserRequestType.JOIN_SERVER:
                        groupClient = new Client(port);
                        groupClient.OnLoginPressed();

                        testServer = new Server(this, 2005);
                        testServer.Window_Loaded();

                        string[] decode = decodeIpAndPort(dialog.result);
                        if (decode != null)
                        {
                            chatTitle.Text = decode[0];
                            initUserView("Group: " + decode[0], CustomButton.ButtonType.JOINED_GROUP, groupClient);
                        }
                        break;
                    case UserRequestType.ADDUSER:
                        string[] usrDecode = decodeIpAndPort(dialog.result);
                        if (usrDecode != null)
                        {
                            mClient = new Client(Convert.ToInt32(usrDecode[2]));
                            mClient.OnLoginPressed();
                            initUserView("User: " + usrDecode[0], CustomButton.ButtonType.USER, mClient);
                        }
                        break;
                }
            }
            else {
                MessageBox.Show(messageBoxTitle, "Error");
            }

        }

        private void addGroupOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(UserRequestType.CREATE_SERVER, 2000, "Type Server Name", "Invalid Input");
        }

        private void addUserOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(UserRequestType.ADDUSER, 2001, "Type UserID", "Invalid Input");
        }

        private void joinOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(UserRequestType.JOIN_SERVER, 2000, "Type Group Code", "Invalid Input");
        }

        public string encodeIpAndPort(int port, string name, bool isGroup)
        {
            string ip = Client.GetLocalIPAddress();
            string[] sl = ip.Split('.');
            string ret = "";
            foreach (string s in sl) {
                string tmp = s;
                if (s.Equals("0"))
                { ret += tmp + "0"; }
                else
                {
                    int t = Convert.ToInt32(tmp);
                    string hex = t.ToString("X");
                    ret += hex;
                }
            }
            byte[] ba = Encoding.Default.GetBytes(name);
            var hexName = BitConverter.ToString(ba);
            hexName = hexName.Replace("-", "");
            string _port = port.ToString("X");

            Console.WriteLine("Coded: " + ret + _port + "-" + hexName);

            return (isGroup) ? ret + _port + "-" + hexName : ret + _port;
        }
        public string[] decodeIpAndPort(string code)
        {
            try
            {
                Console.WriteLine("Code: " + code);
                string[] sl = code.Split('-');
                string ip_port = sl[0];
                string hname = sl[1];
                string name = hexToString(hname);

                string ip = ip_port.Substring(0, ip_port.Length - 3);
                string port = "" + Convert.ToInt32(ip_port.Substring(ip_port.Length - 3), 16);
                Console.WriteLine("Port Decode " + port);
                Console.WriteLine("Ip Decode " + ip);
                Console.WriteLine("Decode Name " + name);
                string _ip = convertToIp(ip);
                string[] ret = { name, _ip, port };
                return ret;
            }
            catch (Exception e)
            {
                MessageBox.Show("There was a problem while adding a new user", "Unexpected Error");
                return null;
            }
        }


        public string convertToIp(string iph)
        {
            char[] arr = iph.ToCharArray();
            string ret = "";
            for (int i = 0; i < iph.Length; i++) {
                if (i % 2 != 0) {
                    string tmp2 = "" + arr[i - 1] + arr[i];
                    ret += "" + Convert.ToInt32(tmp2, 16) + ".";
                }
            }
            Console.WriteLine(ret.Substring(0, ret.Length - 1));
            return ret.Substring(0, ret.Length - 1);
        }
        public string hexToString(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);

            return Encoding.UTF8.GetString(bytes);
        }

    }

}
