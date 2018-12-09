namespace VSProjectManager
{
    /// <summary>
    /// Структура для представления параметра
    /// </summary>
    public struct Property
    {
        public string Name { get; set; }
        private string value;
        public string Value
        {
            get
            {
                if (value == "")
                {
                    return ("Property not set or exist just as flag.");
                }
                return value;
            }
            set
            {
                this.value = value;
            }
        }
        public string Attributes { get; set; }
        public Property(string name, string value)
        {
            Attributes = null;
            Name = name;
            this.value = value;
        }
        public Property(string name, string value, string attributes)
        {
            Name = name;
            this.value = value;
            Attributes = attributes;
        }
    }
}
