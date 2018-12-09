using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace VSProjectManager
{
    /// <summary>
    /// Логика взаимодействия для ProjectPage.xaml
    /// </summary>
    public partial class ProjectPage : Page, IPresenterPage
    {
        private Project Project;
        public ProjectPage(Project Project) : base()
        {
            this.Project = Project;
        }
        public void Initialize()
        {
            InitializeComponent();
            ProjectName.Content = Project.Name;
            IDE.Content = Project.IDE;
            List<Property> configuration = new List<Property>();
            List<Property> parameters = new List<Property>();
            List<Property> files = new List<Property>();
            List<Property> references = new List<Property>();
           
            foreach (var config in Project.Configuration)
            {
                List<Property> target;
                foreach (var property in config.Properties)
                {
                    if (config.Name == "ItemGroup")
                    {
                        if (property.Name == "Reference")
                        {
                            target = references;
                        }
                        else
                        {
                            if (property.Name != "ProjectConfiguration" && property.Name != "ProjectReference")
                                target = files;
                            else
                                target = parameters;
                        }
                    }
                    else if (config.Attributes != null)
                    {
                        target = configuration;
                    }
                    else
                    {
                        target = parameters;
                    }
                    target.Add(new Property(property.Name, property.Value, property.Attributes ?? config.Attributes));
                }
            }
            ConfigurationGrid.ItemsSource = configuration;
            ParametersGrid.ItemsSource = parameters;
            References.ItemsSource = references;
            Files.ItemsSource = files;
            SortDataGrid(ConfigurationGrid);
            SortDataGrid(ParametersGrid);
            SortDataGrid(Files);
            SortDataGrid(References);
        }
        public static void SortDataGrid(DataGrid dataGrid, int columnIndex = 0, ListSortDirection sortDirection = ListSortDirection.Ascending)
        {
            var column = dataGrid.Columns[columnIndex];

            // Clear current sort descriptions
            dataGrid.Items.SortDescriptions.Clear();

            // Add the new sort description
            dataGrid.Items.SortDescriptions.Add(new SortDescription(column.SortMemberPath, sortDirection));

            // Apply sort
            foreach (var col in dataGrid.Columns)
            {
                col.SortDirection = null;
            }
            column.SortDirection = sortDirection;

            // Refresh items to display sort
            dataGrid.Items.Refresh();
        }

        private void buttonOpenClick(object sender, RoutedEventArgs e)
        {
            Process.Start(Project.Path);
        }

        private void buttonRefreshClick(object sender, RoutedEventArgs e)
        {
            AppController.Instance.RefreshElement(Project.Path);
        }
    }
}
