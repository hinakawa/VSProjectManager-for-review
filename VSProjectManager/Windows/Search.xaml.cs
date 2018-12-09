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

namespace VSProjectManager
{
    /// <summary>
    /// Логика взаимодействия для Search.xaml
    /// </summary>
    public partial class Search : Window
    {
        public List<Property> properties;
        public Search()
        {
            InitializeComponent();
            properties = new List<Property>();
        }

        public Search(List<Property> properties) : this()
        {
            this.properties = new List<Property>(properties);
            foreach (var property in this.properties)
            {
                stackParameters.Children.Add(new OptionBox2(property));
            }
            this.properties.Clear();
        }

        private void addProperty_Click(object sender, RoutedEventArgs e)
        {
            var property = new OptionBox2();
            stackParameters.Children.Add(property);
        }

        private void ButtonAccept_Click(object sender, RoutedEventArgs e)
        {
            foreach (OptionBox2 ob in stackParameters.Children)
            {
                properties.Add(ob.GetProperty);
            }
            if (properties.Count != 0)
            {
                DialogResult = true;
            }
            else
            {
                DialogResult = false;
            }
            Close();
        }
    }
}
