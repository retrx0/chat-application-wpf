using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using System.Xml.Linq;
using Telecomms.components;
using Telecomms.src.models;

namespace Telecomms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {

        public static string XML_FILE_PATH = "../../assets/files/users.xml";

        public LoginWindow()
        {
            InitializeComponent();
        }

        public class User
        {
            public string username { get; set; }
            public string password {get; set;}

            public User(string usr, string pass)
            {
                this.username = usr;
                this.password = pass;
            }
        }
        

        private void loginButtonClicked(object sender, RoutedEventArgs e)
        {
            string username = username_input.Text;
            string password = password_input.Password;
            if (loginAuth(username,password))
            {
                MainWindow main = new MainWindow(username, password);
                main.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password", "Login Error");
            }
        }

        private bool loginAuth(string username, string password)
        {
            XmlReader xmlReader = XmlReader.Create(XML_FILE_PATH);

            while (xmlReader.Read())
            {
                //keep reading until we see your element
                if (xmlReader.Name.Equals("user") && (xmlReader.NodeType == XmlNodeType.Element))
                {
                    // get attribute from the Xml element here
                    string name = xmlReader.GetAttribute("username");
                    string pass = xmlReader.GetAttribute("password");
                    //Console.WriteLine(name + " - " + pass);
                    if (username.Equals(name) && password.Equals(pass)) return true;
                }
            }

            return false;
        }

        public  void CreateOnClick(object sender, RoutedEventArgs e)
        {
            SignUp signup = new SignUp();
            signup.ShowDialog();
        }
    }
}
