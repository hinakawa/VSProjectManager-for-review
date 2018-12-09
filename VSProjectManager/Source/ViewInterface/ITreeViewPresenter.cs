using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;

namespace VSProjectManager
{
    /// <summary>
    /// Предоставляет набор методов и свойств для управления элементом TreeView
    /// </summary>
    internal interface ITreeViewPresenter
    {
        Dispatcher VisualQueue { get; set; }
        List<Property> Filter { get; set; }
        void AddTreeViewElements(ObservableCollection<TreeViewItem> overlap);
        void AsyncAddTreeViewElementInstantly(TreeViewItem fileHeader);
        void ShowProgressBar();
        void HideProgressBar();
        void Log(string str);
        void ReloadTreeView();
    }
}