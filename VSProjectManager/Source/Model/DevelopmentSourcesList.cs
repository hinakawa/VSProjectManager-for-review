using System;
using System.Collections.Generic;
using System.Linq;

namespace VSProjectManager
{
    /// <summary>
    /// Преставляет собой контейнер для хранения структуры данных типа IDevelopmentSource
    /// </summary>
    public class DevelopmentSourcesList
    {
        private Dictionary<string, IDevelopmentSource> sourcesByPath;
        private List<IDevelopmentSource> Sources
        {
            get
            {
                var sources = sourcesByPath.Values.ToList();
                sources.RemoveAll(s => s.Type == SourceType.Project);
                return sources;
            }
        }
        public IEnumerator<IDevelopmentSource> GetEnumerator()
        {
            return Sources.GetEnumerator();
        }

        public DevelopmentSourcesList()
        {
            sourcesByPath = new Dictionary<string, IDevelopmentSource>();
        }
        public void Add(IDevelopmentSource source)
        {
            sourcesByPath.Add(source.Path, source);
            foreach(var include in source.Includes)
            {
                sourcesByPath.Add(include.Path, include);
            }
        }
        public void Reload()
        {
            HashSet<string> keys = new HashSet<string>(sourcesByPath.Keys);

            foreach (string path in keys)
            {
                if (sourcesByPath[path].Type == SourceType.Solution)
                {
                    Reload(path);
                }
            }
        }
        /// <summary>
        /// Выполняет глобальный репарсинг для всех источников списка и проверку существования конкретных источников
        /// </summary>
        public void Reload(string path)
        {
            if (sourcesByPath[path].Type == SourceType.Solution)
            {
                try
                {
                    var parsedData = Microsoft.Build.Construction.SolutionFile.Parse(path);
                    var solution = new Solution(path, parsedData);

                    sourcesByPath[path] = solution;

                    foreach (var include in solution.Includes)
                    {
                        sourcesByPath[include.Path] = include;
                    }
                }
                catch
                {
                    foreach (var include in sourcesByPath[path].Includes)
                    {
                        sourcesByPath.Remove(include.Path);
                    }
                    sourcesByPath.Remove(path);
                }
            }
            else
            {
                try
                {
                    var IDE = (sourcesByPath[path] as Project).IDE;
                    sourcesByPath[path] = new Project(path, IDE);
                }
                catch
                {
                    throw new Exception();
                }
            }
        }

        internal IDevelopmentSource GetItemByPath(string path)
        {
            return sourcesByPath[path];
        }
        /// <summary>
        /// Ищет по всем источникам те, что соотвествуют параметрам
        /// </summary>
        public List<IDevelopmentSource> FindSourcesAccordingTo(List<Property> parameters)
        {
            if (parameters.Count == 0)
            {
                return Sources;
            }

            var filter = parameters.Select(p => p.Name == "Name" ? p.Value : "").Aggregate((n1, n2) => n1 + n2);
            parameters.RemoveAll(p => p.Name == "Name");

            List<IDevelopmentSource> overlaps = new List<IDevelopmentSource>();

            // Обходим источники и включения
            foreach (var source in Sources)
            {
                if (source.CheckAccording(filter, parameters))
                {
                    source.Indexed = true;
                    overlaps.Add(source);
                }
                foreach (var include in source.Includes)
                {
                    if (include.CheckAccording(filter, parameters))
                    {
                        include.Indexed = true;

                        if (overlaps.GetLastAdded() != source)

                            overlaps.Add(source);

                    }
                }
            }
            return overlaps;
        }
    }

    public static class IDevelopmentSourceExt
    {
        static public IDevelopmentSource GetLastAdded(this List<IDevelopmentSource> sources)
        {
            IDevelopmentSource result;
            try
            {
                result = sources.Last();
            }
            catch
            {
                result = null;
            }
            return result;
        }
        /// <summary>
        /// Проверяет IDevelopmentSource на соотвествие определенному списку параметров
        /// </summary>
        static public bool CheckAccording(this IDevelopmentSource source, string filter, List<Property> parameters = null)
        {
            if (parameters == null)
            {
                parameters = new List<Property>();
            }

            foreach (var property in parameters)
            {
                if (!source.GetProperties().Contains(property))
                {
                    return false;
                }
            }
            return source.Name.ToLower().Contains(filter.ToLower());
        }
        /// <summary>
        /// Индексирует обьекты как (не)соотвествующие определенному списку параметров
        /// </summary>
        static public bool IndexSource(this IDevelopmentSource source, List<Property> parameters)
        {
            if (parameters.Count == 0)
            {
                return true;
            }

            bool result = false;
            var according = new List<Property>(parameters);
            string filter;

            filter = according.Select(p => p.Name == "Name" ? p.Value : "").Aggregate((n1, n2) => n1 + n2);
            according.RemoveAll(p => p.Name == "Name");

            if (source.CheckAccording(filter, according))
            {
                result = true;
                source.Indexed = true;
            }

            foreach (var include in source.Includes)
            {
                if (include.CheckAccording(filter, according))
                {
                    result = true;
                    include.Indexed = true;
                }
            }

            return result;
        }
    }
}
