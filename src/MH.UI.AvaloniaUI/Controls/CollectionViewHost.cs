using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class CollectionViewHost : TreeDataGridHost, UIC.ICollectionViewHost {
  public new static readonly StyledProperty<UIC.TreeView?> ViewModelProperty =
    AvaloniaProperty.Register<CollectionViewHost, UIC.TreeView?>(nameof(ViewModel));

  public new UIC.CollectionView? ViewModel {
    get => (UIC.CollectionView?)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  private static void _onViewModelChanged(CollectionViewHost o, AvaloniaPropertyChangedEventArgs e) {
    o.SetValue(TreeDataGridHost.ViewModelProperty, o.ViewModel);
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static CollectionViewHost() {
    ViewModelProperty.Changed.AddClassHandler<CollectionViewHost>(_onViewModelChanged);
  }

  public static RelayCommand<SizeChangedEventArgs> SetGroupWidthCommand { get; } = new(_setGroupWidth);

  private static void _setGroupWidth(SizeChangedEventArgs? e) {
    if (e is { WidthChanged: true, Source: StyledElement { DataContext: ICollectionViewGroup group } }) {
      group.Width = e.NewSize.Width;
    }
  }
}

public class CollectionViewTemplateSelector : IDataTemplate {
  public Control? Build(object? param) {
    if (param == null || Application.Current == null) return null;

    var key = param switch {
      ICollectionViewGroup => "MH.DT.CollectionViewGroup",
      ICollectionViewRow => "MH.DT.CollectionViewRow",
      _ => throw new ArgumentOutOfRangeException(nameof(param), param, null)
    };

    if (Application.Current.TryFindResource(key, out var res) && res is IDataTemplate dt)
      return dt.Build(param);

    return null;
  }

  public bool Match(object? data) =>
    data is ICollectionViewGroup or ICollectionViewRow;
}