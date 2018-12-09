using System.Collections.Generic;
using System.Xml;

namespace VSProjectManager
{
    public class Configuration
    {
        public string Name { get; set; }
        public string Attributes { get; set; }
        public List<Property> Properties;

        public Configuration(XmlNode node)
        {
            Name = node.Name;

            bool isItemGroup = Name == "ItemGroup";
            bool isPropertyGroup = Name == "PropertyGroup";

            // Если существуют атрибуты - данное свойство отвечает за конфигурацию 
            if (node.Attributes.Count != 0)
            {
                // Считываем конфигурацию
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    Attributes += attribute.Value + " ";
                }
                // Приводим к виду обычной строки
                Attributes = Attributes.GetTextBlock("== '", "'") ?? Attributes.GetTextBlock("=='", "'");
            }
            // Далее проверяем что перед нами - группа свойств или группа ресурсов
            if (isItemGroup || isPropertyGroup)
            {
                Properties = new List<Property>();
                foreach (XmlNode prop in node.ChildNodes)
                {
                    if (prop.NodeType != XmlNodeType.Comment)
                    {
                        if (isItemGroup)
                        {
                            // Если это ресурс добавляем значение его аттрибута Include
                            Properties.Add(new Property(prop.Name, prop.Attributes.GetNamedItem("Include").InnerText));
                        }
                        else
                        {
                            // Если свойство просто записываем имя и значение вершины, сохраняем возможные аттрибуты.
                            Properties.Add(new Property(prop.Name, prop.InnerText, Attributes));
                        }
                    }
                }
            }
            // Если это не свойство и не ресурс, перед нами список пользовательских конфигураций
            // Которые могут быть любой степени вложенности
            else
            {
                // Рекурсивный обход
                Properties = ParseAdditionalParams(node);
            }
        }
        private List<Property> ParseAdditionalParams(XmlNode node)
        {
            List<Property> propList = new List<Property>();
            TraverseTreeForParams(node, ref propList);
            return propList;
        }
        private void TraverseTreeForParams(XmlNode node, ref List<Property> propList)
        {
            if (node.FirstChild != null)
            {
                if (node.FirstChild.FirstChild != null && node.FirstChild.FirstChild.NodeType != XmlNodeType.Text)
                {
                    foreach (XmlNode childNode in node.ChildNodes)
                    {
                        TraverseTreeForParams(childNode, ref propList);
                    }
                }
                else
                {
                    foreach (XmlNode prop in node.ChildNodes)
                    {
                        propList.Add(new Property(prop.Name, prop.InnerText));
                    }
                }
            }
        }
    }
}
