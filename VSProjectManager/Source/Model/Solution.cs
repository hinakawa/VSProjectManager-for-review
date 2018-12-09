using System;
using System.Collections.Generic;
using Microsoft.Build.Construction;
using System.IO;

namespace VSProjectManager
{
    /// <summary>
    /// Класс представления решения.
    /// </summary>
    public class Solution : IDevelopmentSource
    {
        public bool Indexed { get; set; }
        public string GuID { get; private set; }
        public string Name { get; private set; }
        public string Path { get; private set; }
        public List<string> Config { get; private set; }
        public string IDE { get; private set; }
        public string IDEMinimumVersion { get; private set; }
        public string IDEVersion { get; private set; }
        public string FileFormatVersion { get; private set; }

        public List<IDevelopmentSource> Includes { get; set; }

        public SourceType Type
        {
            get
            {
                return SourceType.Solution;
            }
        }

        /// <summary>
        /// Принимает путь и обьект полученный с помощью MSBuild.
        /// </summary>
        public Solution(string path, SolutionFile solution)
        {
            Indexed = false;
            Name = System.IO.Path.GetFileName(path).Replace(System.IO.Path.GetExtension(path), String.Empty);
            Path = path;
            Config = new List<string>();
            foreach (var config in solution.SolutionConfigurations)
            {
                Config.Add(config.FullName);
            }
            ParseAdditionalParams(path);

            UpdateSettings();

            Includes = new List<IDevelopmentSource>();

            ParseProjects(solution);
        }

        private void ParseProjects(SolutionFile solution)
        {
            var projects = solution.ProjectsInOrder;
            foreach (var project in projects)
            {
                bool isProjectFileAndExist = System.IO.Path.GetExtension(project.AbsolutePath) != String.Empty &&
                                             File.Exists(project.AbsolutePath);
                if (isProjectFileAndExist)
                {
                    try
                    {
                        Includes.Add(new Project(project.AbsolutePath, IDE));
                    }
                    catch (System.Xml.XmlException)
                    {
                        //Если есть нарушения в структуре файла проекта, парсим все те данные которые можем
                        //TODO: добавить 
                    }
                }
            }
        }

        /// <summary>
        /// Получает дополнительные параметры из файла решения.
        /// </summary>
        private void ParseAdditionalParams(string file)
        {
            using (StreamReader fs = new StreamReader(file))
            {
                string source = fs.ReadToEnd();

                FileFormatVersion = source.GetTextBlock("Format Version ", "\r\n");
                GuID = source.GetTextBlock("SolutionGuid = ", "\r\n");
                if (GuID == null)
                {
                    GuID = source.GetTextBlock(", \"{", "}\"");
                }
                IDE = source.GetTextBlock("# Visual Studio", "\r\n");
                IDEMinimumVersion = source.GetTextBlock("MinimumVisualStudioVersion = ", "\r\n");
                if (IDEMinimumVersion == null)
                {
                    IDEMinimumVersion = "Limited only by framework version. (Located on project page)";
                }
                IDEVersion = source.GetTextBlock("VisualStudioVersion = ", "\r\n");
                if (IDEVersion == null)
                {
                    IDEVersion = "Not limited";
                }
            }
        }
        /// <summary>
        /// Обновляет глобальную память известных свойств в соотвествии с конфигурацией
        /// </summary>
        private void UpdateSettings()
        {
            var settings = Settings.Instance;
            settings.AddKnownParameter("Name", Name);
            settings.AddKnownParameter("GUID", GuID);
            settings.AddKnownParameter("Path", Path);
            foreach (string config in Config)
            {
                settings.AddKnownParameter("Configuration", config);
            }
            settings.AddKnownParameter("IDE", IDE);
            settings.AddKnownParameter("IDEMinimumVersion", IDEMinimumVersion);
            settings.AddKnownParameter("IDEVersion", IDEVersion);
            settings.AddKnownParameter("FileFormatVersion", FileFormatVersion);
        }
        /// <summary>
        /// Возвращает значения полей обьекта в виде коллекции
        /// </summary>
        public List<Property> GetProperties()
        {
            List<Property> parameters = new List<Property>()
            {
                new Property("Name", Name),
                new Property("GUID", GuID),
                new Property("Path", Path),
                new Property("IDE", IDE),
                new Property("IDEMinimumVersion", IDEMinimumVersion),
                new Property("IDEVersion", IDEVersion),
                new Property("FileFormatVersion", FileFormatVersion)
            };
            foreach (string config in Config)
            {
                parameters.Add(new Property("Configuration", config));
            }
            return parameters;
        }
    }
    public static class StringExt
    {
        public static string GetTextBlock(this string str, string start, string end)
        {
            int startIndex = str.IndexOf(start);
            if (startIndex != -1)
            {
                var block = str.Remove(0, startIndex + start.Length);
                int endIndex = block.IndexOf(end);
                if (endIndex != -1)
                {
                    return block.Substring(0, endIndex);
                }
            }
            return null;
        }
    }
}
