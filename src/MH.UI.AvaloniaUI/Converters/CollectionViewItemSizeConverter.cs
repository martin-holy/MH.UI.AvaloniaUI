using Avalonia.Controls;
using MH.UI.Interfaces;
using MH.Utils.Interfaces;

namespace MH.UI.AvaloniaUI.Converters;

public class CollectionViewItemSizeConverter : BaseConverter {
  private static readonly object _lock = new();
  private static CollectionViewItemSizeConverter? _inst;
  public static CollectionViewItemSizeConverter Inst { get { lock (_lock) { return _inst ??= new(); } } }

  public override object? Convert(object? value, object? parameter) {
    if (value is Control { Parent.DataContext: ITreeItem { Parent: ICollectionViewGroup g } } c) {
      c.Width = g.GetItemSize(c.DataContext!, true);
      c.Height = g.GetItemSize(c.DataContext!, false);
    }
    
    return null;
  }
}