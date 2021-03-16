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
    public partial class AddDialog : Window
    {
        public string message;
        public string result;

        public AddDialog()
        {
            InitializeComponent();
        }

        public AddDialog(string message)
        {
            InitializeComponent();
            this.message = message;
            Title.Text = message;
        }

        public Boolean isUserNameValid(string input)
        {
            return true;
        }

        private void addOnClick(object sender, RoutedEventArgs e)
        {
            if(input.Text != "")
            {
                if (isUserNameValid(input.Text))
                {
                    result = input.Text;
                }
            }
            this.Close();
        }

        private void cancelOnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
