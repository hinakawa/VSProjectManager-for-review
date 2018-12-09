using System.Windows;
using System.Windows.Controls;

namespace VSProjectManager
{
    /// <summary>
    /// Логика взаимодействия для PathBox.xaml
    /// </summary>
    public partial class PathBox : UserControl
    {
        public string Text
        {
            get
            {
                return Path.Text;
            }
        }
        public bool Correct;
        public PathBox()
        {
            InitializeComponent();
            Margin = new Thickness(2.5, 5, 2.5, 0);
        }
        public PathBox(string path) : this()
        {
            Path.Text = path;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            (Parent as StackPanel).Children.Remove(this);
        }
    }
}
