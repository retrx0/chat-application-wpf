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

namespace Telecomms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public string username;
        public string password;
        Client client;
        ClientMessage clientMessage;
        Server server;
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
        }

        private void logoutOnClick(object sender, RoutedEventArgs e)
        {
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }

        public void initUserView(string title, bool isGroup)
        {
            Button newUser = new Button();
            newUser.Height = 50;
            newUser.BorderThickness = new System.Windows.Thickness(0);
            newUser.Margin = new System.Windows.Thickness(1);
            newUser.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(250, 250, 250) };
            newUser.Content = (isGroup) ? "Group: "+ title : "User: "+ title;
            newUser.FontSize = 20;

            newUser.Click += new RoutedEventHandler((s,e) => { /*chatStackPanel.Children.Clear();*/ });

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
            //setUpClient(2001);
            wrapMessage(username, messageTextInput.Text);
            Client client = new Client(2001);
            clientMessage = new ClientMessage(client.clientSocket, username, this);
            clientMessage.sendMessage(messageTextInput.Text);
            //Socket soc;
            //ClientMessage clientMessage = new ClientMessage(soc, username);
        }

        public void showAndSetupDialog(bool isServer,bool isGroup, int port, string title, string messageBoxTitle)
        {
            AddDialog dialog = new AddDialog(title);
            dialog.ShowDialog();
            if (dialog.result != null){
                if (isServer) {
                   Server server = new Server(this, port);
                    server.Window_Loaded();
                    initUserView(dialog.result, isGroup);
                }
                else{
                   Client client = new Client(port);
                    client.OnLoginPressed();
                    initUserView(dialog.result, isGroup);
                }
            }
            else{
                MessageBox.Show(messageBoxTitle);
            }

        }

        private void addGroupOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(true, true, 2000, "Type Server Name", "Invalid Input");
        }

        private void addUserOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(false, false, 2001, "Type UserName", "Invalid Input");
        }

        private void joinOnClick(object sender, RoutedEventArgs e)
        {
            showAndSetupDialog(false, true, 2000, "Type Group ID", "Invalid Input");
        }

        public void encodeIpAndPort(int port)
        {
            string ip = Client.GetLocalIPAddress();
           
        }
        public void decodeIpAndPort(string code)
        {

        }
    }

}
