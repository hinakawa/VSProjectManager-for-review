using System.Collections.Generic;
using System.Xml;

namespace VSProjectManager
{
    /// <summary>
    /// Класс представления проекта.
    /// </summary>
    public class Project : IDevelopmentSource
    {
        public bool Indexed { get; set; }
        public string Name { get; }
        public string Type { get; }
        public string Path { get; }
        public string IDE { get; }
        public List<Configuration> Configuration { get; }
        public List<IDevelopmentSource> Includes { get; private set; }

        public List<Property> GetProperties()
        {
            List<Property> properties = new List<Property>()
            {
                new Property("Name", Name),
                new Property("Type", Type),
                new Property("Path", Path)
            };
            foreach (Configuration config in Configuration)
            {
                foreach(var property in config.Properties)
                {
                    properties.Add(property);
                }
            }
            return properties;
        }

        SourceType IDevelopmentSource.Type
        {
            get
            {
                return SourceType.Project;
            }
        }

        public Project(string file, string IDEVersion)
        {
            IDE = IDEVersion;
            Indexed = false;
            Name = System.IO.Path.GetFileName(file)
                   .Replace(System.IO.Path.GetExtension(file), "");
            Type = System.IO.Path.GetExtension(file)
                   .Remove(0, 1);
            Path = file;

            Includes = new List<IDevelopmentSource>();

            var doc = new XmlDocument();
            doc.Load(file);
            var root = doc.DocumentElement;
            HashSet<string> targetNodes = new HashSet<string>
            {
                "PropertyGroup",
                "ItemGroup",
                "ProjectExtensions",
                "ItemDefinitionGroup"
            };

            Configuration = new List<Configuration>();
            // Проходим по всем вершинам первого уровня
            foreach (XmlNode node in root.ChildNodes)
            {
                if (targetNodes.Contains(node.Name))
                {
                    // Создаем конфигурацию
                    Configuration.Add(new Configuration(node));
                }
            }
            UpdateSettings();
        }

        private void UpdateSettings()
        {
            var settings = Settings.Instance;
            settings.AddKnownParameter("Name", Name);
            settings.AddKnownParameter("Type", Type);
            settings.AddKnownParameter("Path", Path);
            foreach (var configuration in Configuration)
            {
                foreach (var parameter in configuration.Properties)
                {
                    if (parameter.Attributes != null)
                    {
                        settings.KnownConfigurations.Add(parameter.Attributes);
                    }
                    settings.AddKnownParameter(parameter.Name, parameter.Value);
                }
            }
        }
    }
}
