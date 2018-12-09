using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
#pragma warning disable CS0618
namespace VSProjectManager
{
    public class SolutionScaner
    {
        private HashSet<string> roots;
        private HashSet<string> ignores;
        private static HashSet<string> knownPaths = new HashSet<string>();

        public SolutionScaner()
        {
            this.roots = new HashSet<string>();
            this.ignores = new HashSet<string>();
        }

        private List<Thread> threads;
        public void Initialize(List<string> roots, List<string> ignores)
        {
            if (threads != null)
            {
                threads.Clear();
            }
            else
            {
                threads = new List<Thread>();
            }

            this.roots = new HashSet<string>(roots);
            this.ignores = new HashSet<string>(ignores);

            foreach (string drive in Environment.GetLogicalDrives())
            {
                if (!ignores.Contains(drive))
                {
                    this.roots.Add(drive);
                }
            }
        }
        public void ScanFS()
        {
            foreach (var root in roots)
            {
                var finder = new ParameterizedThreadStart(FindSolutions);
                Thread solutionFinder = new Thread(finder)
                {
                    IsBackground = true,
                    Priority = ThreadPriority.BelowNormal
                };
                threads.Add(solutionFinder);
                solutionFinder.Start(root);
            }
        }
        public void Pause()
        {
            foreach (var thread in threads)
            {
                try
                {
                    thread.Suspend();
                }
                catch (ThreadStateException) { }
            }
        }
        public void Resume()
        {
            foreach (var thread in threads)
            {
                try
                {
                    thread.Resume();
                }
                catch (ThreadStateException) { }
            }
        }
        private void FindSolutions(object pathRoot)
        {
            string root = pathRoot as string;
            if (!System.IO.Directory.Exists(root))
            {
                return;
            }

            Stack<string> dirs = new Stack<string>();
            dirs.Push(root);

            while (dirs.Count > 0 && threads.Contains(Thread.CurrentThread))
            {
                string currentDir = dirs.Pop();
                if (!ignores.Contains(currentDir))
                {
                    string[] subDirs;
                    try
                    {
                        subDirs = System.IO.Directory.GetDirectories(currentDir);
                    }
                    catch
                    {
                        continue;
                    }
                    string[] files;
                    try
                    {
                        files = System.IO.Directory.GetFiles(currentDir);
                    }
                    catch
                    {
                        continue;
                    }
                    foreach (string file in files)
                    {
                        if (file.EndsWith(".sln") && knownPaths.Add(file) && !ignores.Contains(file))
                        {
                            Trace.WriteLine(file);
                            AppController.Instance.OnSolutionFound(file);
                        }
                    }
                    foreach (string subDir in subDirs)
                    {
                        if (!ignores.Contains(subDir))
                        {
                            dirs.Push(subDir);
                        }
                    }
                }
            }

            threads.Remove(Thread.CurrentThread);
            if (threads.Count == 0)
            {
                AppController.Instance.OnThreadsFinished();
            }
        }
    }
}
