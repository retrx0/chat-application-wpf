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

namespace Telecomms
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow: Window
    {
        public string username;
        public string password;

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

        private void addOnClick(object sender, RoutedEventArgs e)
        {
            Button newUser = new Button();
            newUser.Height = 50;
            newUser.BorderThickness = new System.Windows.Thickness(0);
            newUser.Margin = new System.Windows.Thickness(1);
            newUser.Background = new System.Windows.Media.SolidColorBrush() {Color = Color.FromRgb(250,250,250)};
            newUser.Content = "User";
            usersStack.Children.Add(newUser);
        }

        private void sendOnClick(object sender, RoutedEventArgs e)
        {
            chatTextArea.Text += textInput.Text + Environment.NewLine;
        }
    }
}
