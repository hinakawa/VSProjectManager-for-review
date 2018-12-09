using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using VSProjectManager.Windows;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VSProjectManager
{
    public partial class MainWindow : Window, ITreeViewPresenter, IPagePresenter, INotifyPropertyChanged
    {
        private Settings settings = Settings.Instance;
        private AppController controller = AppController.Instance;

        private ObservableCollection<TreeViewItem> members;
        public ObservableCollection<TreeViewItem> Members
        {
            get
            {
                return members;
            }
            private set
            {
                members = value;
                OnPropertyChanged(nameof(Members));
            }
        }
        public ObservableCollection<TreeViewItem> Memory;
        private List<Property> filter;
        public List<Property> Filter
        {
            get
            {
                return filter;
            }
            set
            {
                Log("Current filter is : {");
                foreach (var pr in value)
                {
                    Log(string.Format("Parameter: \"{0}\" => Value: \"{1}\" ", pr.Name, pr.Value));
                }
                Log("}\n");
                filter = value;
            }
        }
        public Dispatcher VisualQueue { get; set; }


        public MainWindow()
        {
            Members = new ObservableCollection<TreeViewItem>();
            Memory = new ObservableCollection<TreeViewItem>();
            filter = new List<Property>();
            VisualQueue = Dispatcher;

            DataContext = this;

            InitializeComponent();

            controller.Initialize(this as ITreeViewPresenter, this as IPagePresenter);

            Show();

            if (settings.IsFirstEcexcution)
            {
                var message = new Message("You need to parametrize scanner before first \nstartup");
                message.ShowDialog();
                ButtonScanerSettingsClick(null, null);
            }
            controller.RunScaner();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void ButtonScanerSettingsClick(object sender, RoutedEventArgs e)
        {
            ScanerSetup setup = new ScanerSetup(settings.DefaultPaths, settings.SkipPaths)
            {
                Owner = this
            };
            if (setup.ShowDialog() == true)
            {
                settings.DefaultPaths = setup.DefaultPaths;
                settings.SkipPaths = setup.SkipPaths;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            settings.Save();
        }

        private void ButtonRefreshClick(object sender, RoutedEventArgs e)
        {
            controller.RunScaner();
        }

        public void PresentPage(IPresenterPage page)
        {
            ActivePage.Navigate(page);
            if (page != null)
            {
                page.Initialize();
            }
        }

        private void ActivePagePreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            ScrollViewer scv = (ScrollViewer)sender;
            scv.ScrollToVerticalOffset(scv.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        #region TreeSearch

        private void TreeSearchKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = (sender as TextBox);
            var filter = textBox.Text;
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Filter.Clear();
                Filter.Add(new Property("Name", filter));
                Members = controller.ReflectLogicalTree(Filter);
            }
            ButtonRemoveFilter.Visibility = Visibility.Hidden;
        }

        private void TreeSearchGotFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "Search" && (sender as TextBox).HorizontalContentAlignment == HorizontalAlignment.Center)
            {
                TreeSearch.Text = "";
                TreeSearch.HorizontalContentAlignment = HorizontalAlignment.Left;
            }
        }

        private void TreeSearchLostFocus(object sender, RoutedEventArgs e)
        {
            if ((sender as TextBox).Text == "")
            {
                TreeSearch.Text = "Search";
                TreeSearch.HorizontalContentAlignment = HorizontalAlignment.Center;
            }
        }

        private string prev;
        private void TreeSearchTextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = (sender as TextBox);
            var filter = textBox.Text;
            if (textBox.IsFocused && filter.Length > 1)
            {
                Filter.Clear();
                Filter.Add(new Property("Name", filter));
                Members = controller.ReflectLogicalTree(Filter);
            }
            else if (filter == "" && prev != "Search")
            {
                Filter.Clear();
                Members = controller.ReflectLogicalTree(Filter);
            }
            prev = filter;
        }

        private void ButtonFindClick(object sender, RoutedEventArgs e)
        {
            Search search = new Search(Filter)
            {
                Owner = this
            };
            if (search.ShowDialog() == true)
            {
                Filter = search.properties;
                Members = controller.ReflectLogicalTree(search.properties);
                ButtonRemoveFilter.Visibility = Visibility.Visible;
                TreeSearchLostFocus(TreeSearch, null);
            }
        }
        #endregion

        private void TreeViewItemSelected(object sender, RoutedEventArgs e)
        {
            controller.OnTreeViewPresenterItemSelected((e.OriginalSource as TreeViewItem).Tag as string);
        }

        private void ProgressBarMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ScanProgress.IsIndeterminate)
            {
                controller.SuspendScaning();
            }
            else
            {
                controller.ResumeScaning();
            }
            ScanProgress.IsIndeterminate = !ScanProgress.IsIndeterminate;
        }

        public void ShowProgressBar()
        {
            ScanProgress.Visibility = Visibility.Visible;
            ScanProgress.IsIndeterminate = true;
        }

        public void HideProgressBar()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ScanProgress.Visibility = Visibility.Hidden;
                ScanProgress.IsIndeterminate = true;
            });
        }

        private void ButtonRemoveFilterClick(object sender, RoutedEventArgs e)
        {
            ButtonRemoveFilter.Visibility = Visibility.Hidden;
            Filter.Clear();
            Members = controller.ReflectLogicalTree(Filter);
        }

        public void Log(string str)
        {
            if (tbLog != null)
            {
                Dispatcher.BeginInvoke(() => 
                {
                    tbLog.AppendText(str);
                    tbLog.ScrollToEnd();
                });
            }
        }

        public void ReloadTreeView()
        {
            Members = controller.ReflectLogicalTree(new List<Property>());
        }
    }

    /// <summary>
    /// Tree-view presenter part
    /// </summary>
    public partial class MainWindow : Window, ITreeViewPresenter, IPagePresenter
    {
        private void SolutionTreeItemSelected(object sender, RoutedEventArgs e)
        {
            controller.OnTreeViewPresenterItemSelected((e.OriginalSource as TreeViewItem).Tag as string);
        }

        public void AsyncAddTreeViewElementInstantly(TreeViewItem fileHeader)
        {
            Members.Add(fileHeader);
            Memory.Add(fileHeader);
        }

        public void AsyncAddTreeViewElementToMemory(TreeViewItem fileHeader)
        {
            Memory.Add(fileHeader);
        }

        public void AddTreeViewElements(ObservableCollection<TreeViewItem> overlap)
        {
            Members = overlap;
        }
    }
}
