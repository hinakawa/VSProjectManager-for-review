using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace VSProjectManager
{
    public class SolutionMonitor
    {
        #region Singletone
        private static SolutionMonitor _instance;
        public static SolutionMonitor Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SolutionMonitor();
                }
                return _instance;
            }
        }
        #endregion

        private static int _counter = 0;
        private HashSet<string> knownPaths;
        private Dictionary<int, Solution> solutionsById;
        private Dictionary<string, Solution> solutionsByFile;
        private SolutionList solutions;


        private Thread monitor;
        public bool Enable;
        public SolutionMonitor()
        {
            knownPaths = new HashSet<string>();
            solutionsById = new Dictionary<int, Solution>();
            solutionsByFile = new Dictionary<string, Solution>();
            
        }
        public void Add(string path)
        {
            knownPaths.Add(path);
        }
        public void Remove(string path)
        {
            knownPaths.Remove(path);
        }
        private void MonitoringProcess()
        {
            while (Enable)
            {
                foreach (var path in knownPaths)
                {
                    if (System.IO.File.Exists(path))
                    {

                    }
                    else
                    {

                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}
