using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using Telecomms.src.models;

namespace Telecomms.src
{
    public class CustomButton : Button
    {
        public enum ButtonType {
            JOINED_GROUP,CREATED_GROUP, USER    
        }

        public string title { get; set; }

        public Client client { get; set; }

        public Server server { get; set; }

        public ButtonType buttonType { get; set; }

        public CustomButton(string title)
        {   
            this.title = title;
            init();
        }

        private void init()
        {
            this.Height = 50;
            this.BorderThickness = new System.Windows.Thickness(0);
            this.Margin = new System.Windows.Thickness(1);
            this.Background = new System.Windows.Media.SolidColorBrush() { Color = Color.FromRgb(250, 250, 250) };
            this.Content = title;
            this.FontSize = 20;
        }

    }
}
