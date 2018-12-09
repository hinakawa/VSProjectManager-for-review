using System.Windows;

namespace VSProjectManager.Windows
{
    /// <summary>
    /// Логика взаимодействия для Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        public Message(string text)
        {
            InitializeComponent();
            MessageText.Content = text;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
