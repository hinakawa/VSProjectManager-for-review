using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace VSProjectManager
{
    /// <summary>
    /// Управляющий модуль приложения
    /// Обьединяет в себе функции Presenter'a и Controller'a
    /// </summary>
    public class AppController
    {
        #region Singleton
        private static AppController _instance;
        public static AppController Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AppController();
                }
                return _instance;
            }
        }
        #endregion

        private Settings settings;
        private SolutionScaner scaner;
        private DevelopmentSourcesList sourcesList;
        private AppController()
        {
            settings = Settings.Instance;
            scaner = new SolutionScaner();
            sourcesList = new DevelopmentSourcesList();
        }

        internal ITreeViewPresenter treeViewPresenter;
        internal IPagePresenter pagePresenter;
        internal List<Property> Filter
        {
            get
            {
                return new List<Property>(treeViewPresenter.Filter);
            }
        }
        internal void Initialize(ITreeViewPresenter treeViewPresenter, IPagePresenter pagePresenter)
        {
            this.treeViewPresenter = treeViewPresenter;
            this.pagePresenter = pagePresenter;
        }

        private string lastSelectedItem;
        
        public void SuspendScaning()
        {
            scaner.Pause();
        }

        public void ResumeScaning()
        {
            scaner.Resume();
        }

        internal void OnThreadsFinished()
        {
            treeViewPresenter.HideProgressBar();
            treeViewPresenter.Log("Scan process completed.\n");
        }
        /// <summary>
        /// Конструирует обьект преставления и добавляет новый источник
        /// При нахождении файла с нужным расширением.
        /// </summary>
        public void OnSolutionFound(string file)
        {
            try
            {
                var parsedData = Microsoft.Build.Construction.SolutionFile.Parse(file);
                var solution = new Solution(file, parsedData);

                sourcesList.Add(solution);

                treeViewPresenter.Log("New solution found on path: " + file + "\n");

                if (solution.IndexSource(Filter))
                {
                    treeViewPresenter.VisualQueue.BeginInvoke(() =>
                    {
                        var header = new TreeViewItem()
                        {
                            Header = solution.Name,
                            Tag = solution.Path
                        };
                        if (solution.Indexed)
                        {
                            BrushItem(header);
                        }
                        foreach (var include in solution.Includes)
                        {
                            var child = new TreeViewItem()
                            {
                                Header = include.Name,
                                Tag = include.Path
                            };
                            if (include.Indexed)
                            {
                                BrushItem(child);
                                header.IsExpanded = true;
                                include.Indexed = false;
                            }
                            header.Items.Add(child);
                        }
                        solution.Indexed = false;
                        treeViewPresenter.AsyncAddTreeViewElementInstantly(header);
                    });
                }
            }
            catch (Microsoft.Build.Exceptions.InvalidProjectFileException) {}
        }
        /// <summary>
        /// Локальная перезагрузка страницы
        /// Если элемент больше не сушествует запускает глобальную перезагрузку
        /// </summary>
        public void RefreshElement(string path)
        {
            try
            {
                sourcesList.Reload(path);
                OnTreeViewPresenterItemSelected(path);
            }
            catch
            {
                Reload();
                pagePresenter.PresentPage(null);
            }
        }
        /// <summary>
        /// Запускает сканирование
        /// </summary>
        public void RunScaner()
        {
            treeViewPresenter.Log("Scan process started.\n");
            scaner.Initialize(settings.DefaultPaths, settings.SkipPaths);
            scaner.ScanFS();
            sourcesList.Reload();
            treeViewPresenter.Log("Sources reloaded.\n");
            if (lastSelectedItem != null)
            {
                OnTreeViewPresenterItemSelected(lastSelectedItem);
            }
            treeViewPresenter.ShowProgressBar();
            treeViewPresenter.ReloadTreeView();
        }

        public void Reload()
        {
            treeViewPresenter.Log("Sources reloaded.\n");
            sourcesList.Reload();
            treeViewPresenter.ReloadTreeView();
        }
        /// <summary>
        /// Конструирует нужную страницу
        /// </summary>
        internal void OnTreeViewPresenterItemSelected(string id)
        {
            lastSelectedItem = id;
            var targetItem = sourcesList.GetItemByPath(id);
            if (targetItem != null)
            {
                if (targetItem.Type == SourceType.Solution)
                {
                    SolutionPage solutionPage = new SolutionPage(targetItem as Solution);
                    pagePresenter.PresentPage(solutionPage);
                }
                if (targetItem.Type == SourceType.Project)
                {
                    ProjectPage projectPage = new ProjectPage(targetItem as Project);
                    pagePresenter.PresentPage(projectPage);
                }
            }
            else
            {
                pagePresenter.PresentPage(null);
            }
        }
        /// <summary>
        /// Отражает коллекцию обьектов модели в виде понятном привязке представления
        /// </summary>
        internal ObservableCollection<TreeViewItem> ReflectLogicalTree(List<Property> parameters)
        {
            var filter = new List<Property>(parameters);
            List<IDevelopmentSource> reflectedSources = sourcesList.FindSourcesAccordingTo(filter);
            ObservableCollection<TreeViewItem> reflectedHeaders = new ObservableCollection<TreeViewItem>();
            foreach (var source in reflectedSources)
            {
                var header = new TreeViewItem()
                {
                    Header = source.Name,
                    Tag = source.Path
                };
                if (source.Indexed)
                {
                    BrushItem(header);
                }

                foreach (var include in source.Includes)
                {
                    var child = new TreeViewItem()
                    {
                        Header = include.Name,
                        Tag = include.Path
                    };
                    if (include.Indexed)
                    {
                        BrushItem(child);
                        header.IsExpanded = true;
                        include.Indexed = false;
                    }
                    header.Items.Add(child);
                }
                reflectedHeaders.Add(header);
                source.Indexed = false;
            }
            return reflectedHeaders;
        }

        private void BrushItem(TreeViewItem item)
        {
            var bc = new BrushConverter();
            item.Background = (Brush)bc.ConvertFrom("#B2A8842A");
            item.Foreground = (Brush)bc.ConvertFrom("#FF1B0F03");
        }
    }
}
