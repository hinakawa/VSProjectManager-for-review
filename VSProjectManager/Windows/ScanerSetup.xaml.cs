using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using VSProjectManager.Windows;

namespace VSProjectManager
{
    public partial class ScanerSetup : Window
    {
        public List<string> DefaultPaths;
        public List<string> SkipPaths;
        public ScanerSetup(List<string> defaultPaths, List<string> skipPaths)
        {
            InitializeComponent();
            DefaultPaths = defaultPaths;
            SkipPaths = skipPaths;
            foreach (string path in DefaultPaths)
            {
                stackDefaultPaths.Children.Add(new PathBox(path));
            }
            foreach (string path in SkipPaths)
            {
                stackSkipPaths.Children.Add(new PathBox(path));
            }
        }

        private void _addPathLine(object sender, RoutedEventArgs e)
        {
            var pathLine = new PathBox();
            if (sender == addDefault)
            {
                stackDefaultPaths.Children.Add(pathLine);
            }
            else
            {
                stackSkipPaths.Children.Add(pathLine);
            }
        }
        private void _browseForPath(object sender, RoutedEventArgs e)
        {

        }

        private void AcceptClick(object sender, RoutedEventArgs e)
        {
            Regex path = new Regex(@"[a-zA-Z]:[\\\/](?:[a-zа-яA-ZА-Я0-9]+[\\\/])*");

            foreach (PathBox child in stackDefaultPaths.Children)
            {
                if (!path.IsMatch(child.Text))
                {
                    Message message = new Message("Invalid input in focused field!");
                    message.ShowDialog();
                    child.Path.Focus();
                    return;
                }
            }
            foreach (PathBox child in stackSkipPaths.Children)
            {
                if(!path.IsMatch(child.Text))
                {
                    Message message = new Message("Invalid input in focused field!");
                    message.ShowDialog();
                    child.Path.Focus();
                    return;
                }
            }

            DefaultPaths.Clear();
            SkipPaths.Clear();

            foreach (PathBox child in stackDefaultPaths.Children)
            {
                if (!DefaultPaths.Contains(child.Text))
                    DefaultPaths.Add(child.Text);
            }
            foreach (PathBox child in stackSkipPaths.Children)
            {
                if (DefaultPaths.Contains(child.Text))
                {
                    Message message = new Message("Similar paths coldn't be in both categories!");
                    message.ShowDialog();
                    return;
                }
                if (!SkipPaths.Contains(child.Text))
                    SkipPaths.Add(child.Text);
            }

            DialogResult = true;
            Close();
        }
    }
}
