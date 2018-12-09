using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace VSProjectManager
{
    /// <summary>
    /// Логика взаимодействия для SolutionPage.xaml
    /// </summary>
    public partial class SolutionPage : Page, IPresenterPage
    {
        private Solution solution;
        public SolutionPage(Solution solution) : base()
        {
            this.solution = solution;
        }
        public void Initialize()
        {
            InitializeComponent();
            SolutionName.Content = solution.Name;
            IDE.Content = solution.IDE;
            List<Property> parameters = new List<Property>
            {
                new Property("GUID", solution.GuID),
                new Property("Path", solution.Path),
                new Property("IDE Version", solution.IDEVersion),
                new Property("IDE Minimum Version", solution.IDEMinimumVersion),
                new Property("File Format Version", solution.FileFormatVersion)
            };
            foreach (string config in solution.Config)
            {
                parameters.Add(new Property("Configuration", config));
            }
            List<Property> projects = new List<Property>();
            foreach (Project project in solution.Includes)
            {
                projects.Add(new Property(project.Name, project.Type));
            }

            ParametersGrid.ItemsSource = parameters;
            ProjectsGrid.ItemsSource = projects;
        }
        private void buttonOpenClick(object sender, RoutedEventArgs e)
        {
            Process.Start(solution.Path);
        }

        private void buttonRefreshClick(object sender, RoutedEventArgs e)
        {
            AppController.Instance.RefreshElement(solution.Path);
        }
    }
}
