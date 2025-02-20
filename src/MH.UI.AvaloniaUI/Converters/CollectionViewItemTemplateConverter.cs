using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;

namespace MH.UI.AvaloniaUI.Converters;

public sealed class CollectionViewItemTemplateConverter : BaseConverter {
  private static readonly object _lock = new();
  private static CollectionViewItemTemplateConverter? _inst;
  public static CollectionViewItemTemplateConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object? Convert(object? value, object? parameter) =>
    value is Visual v && v.FindAncestorOfType<StackPanel>() is
      { DataContext: ITreeItem { Parent: ICollectionViewGroup g } }
      ? ResourceConverter.Inst.Convert(g.GetItemTemplateName(), null) as DataTemplate
      : null;
}