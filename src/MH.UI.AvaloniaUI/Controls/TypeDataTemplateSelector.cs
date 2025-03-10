using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace MH.UI.AvaloniaUI.Controls;

public abstract class TypeDataTemplateSelector(TypeTemplateMapping[] mappings) : IDataTemplate {
  protected TypeTemplateMapping[] TemplateMappings { get; } = mappings;

  public bool Match(object? data) =>
    _getTemplateMapping(data) != null;

  public Control? Build(object? data) {
    if (_getTemplateMapping(data) is not { } tm) return null;

    if (Application.Current?.TryFindResource(tm.Key, out var res) == true && res is IDataTemplate dt)
      return dt.Build(data);

    return null;
  }

  private TypeTemplateMapping? _getTemplateMapping(object? data) {
    if (data == null) return null;
    var itemType = data.GetType();
    if (TemplateMappings.FirstOrDefault(x => x.Type.IsAssignableFrom(itemType)) is { } tm) return tm;
    var itemTypeName = string.Join('.', itemType.Namespace, itemType.Name);
    return TemplateMappings.FirstOrDefault(x => itemTypeName.Equals(string.Join('.', x.Type.Namespace, x.Type.Name)));
  }
}

public class TypeTemplateMapping(Type type, string key) {
  public readonly Type Type = type;
  public readonly string Key = key;
}