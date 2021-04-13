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
using System.Xml.Linq;

namespace Telecomms.components
{
    /// <summary>
    /// Interaction logic for SignUp.xaml
    /// </summary>
    public partial class SignUp : Window
    {
        public SignUp()
        {
            InitializeComponent();
        }

        private void CancelOnClick(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void SignUpOnClick(object sender, RoutedEventArgs e)
        {
            if(!username_input.Text.Equals("") || !password_input.Text.Equals(""))
            {
                XDocument doc = XDocument.Load(LoginWindow.XML_FILE_PATH);
                XElement root = new XElement("user");
                root.Add(new XAttribute("username", username_input.Text));
                root.Add(new XAttribute("password", password_input.Text));
                doc.Root.Add(root);
                doc.Save(LoginWindow.XML_FILE_PATH);
                this.DialogResult = true;
            }
        }
    }
}
