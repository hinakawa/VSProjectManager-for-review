using System.Collections.Generic;

namespace VSProjectManager
{
    /// <summary>
    /// Отписывает сущность которая хранит данные из конкретного источника
    /// И логически организуется в дерево.
    /// </summary>
    public interface IDevelopmentSource
    {
        string Name { get; }
        string Path { get; }
        bool Indexed { get; set; }

        SourceType Type { get; }
        // Этот функционал является членом интерфейса, а не просто полем в Solution
        // Для возможности расширения и добавления новых уровней представления данных
        List<IDevelopmentSource> Includes { get; }
        List<Property> GetProperties();
    }
    public enum SourceType
    {
        Solution, Project,
        SourceFile // Возможный путь расширения дерева.
    }
}
