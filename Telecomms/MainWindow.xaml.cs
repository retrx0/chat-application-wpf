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

        public void initUserView(string title)
        {
            Button newUser = new Button();
            newUser.Height = 50;
            newUser.BorderThickness = new System.Windows.Thickness(0);
            newUser.Margin = new System.Windows.Thickness(1);
            newUser.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(250, 250, 250) };
            newUser.Content = title;

            Grid grid = new Grid();
            ColumnDefinitionCollection colums = grid.ColumnDefinitions;
            ColumnDefinition col1 = new ColumnDefinition();
            ColumnDefinition col2 = new ColumnDefinition();
            colums.Add(col1);
            colums.Add(col2);
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

        private void addOnClick(object sender, RoutedEventArgs e)
        {
            AddDialog dialog = new AddDialog("Type Username");
            dialog.ShowDialog();
            initUserView(dialog.result);
        }

        private void sendOnClick(object sender, RoutedEventArgs e)
        {
            TextBlock message = new TextBlock();
            message.Margin = new System.Windows.Thickness(5);
            message.Padding = new System.Windows.Thickness(0);
            message.FontSize = 20;
            message.TextWrapping = TextWrapping.Wrap;
            message.Text = username + ": " + messageTextInput.Text + Environment.NewLine;
            Border messageBorder = new Border();
            messageBorder.Child = message;
            messageBorder.Margin = new System.Windows.Thickness(3);
            messageBorder.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(246, 250, 255) };
            messageBorder.CornerRadius = new System.Windows.CornerRadius(4);
            chatStackPanel.Children.Add(messageBorder);
            messageTextInput.Text = "Type";
            messageTextInput.Foreground = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(150, 150, 150) };
        }

        private void messageTextInputGotFocus(object sender, RoutedEventArgs e)
        {
            messageTextInput.Text = "";
            messageTextInput.Foreground = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(0, 0, 0) };
        }

        private void addGroupOnClick(object sender, RoutedEventArgs e)
        {
            AddDialog dialog = new AddDialog("Type Group ID");
            dialog.ShowDialog();
            initUserView(dialog.result);
        }
    }
}
