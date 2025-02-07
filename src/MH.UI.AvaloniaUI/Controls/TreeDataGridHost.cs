using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using MH.Utils.Interfaces;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class TreeDataGridHost : TreeDataGrid, UIC.ITreeViewHost {
  private ScrollViewer _sv = null!;

  private readonly IDataTemplate _defaultSingleColumnTemplate = new FuncDataTemplate<object>((item, _) =>
    new ContentPresenter { Content = item });

  public static readonly StyledProperty<UIC.TreeView?> ViewModelProperty =
    AvaloniaProperty.Register<TreeDataGridHost, UIC.TreeView?>(nameof(ViewModel));
  public static readonly StyledProperty<IDataTemplate?> SingleColumnTemplateProperty =
    AvaloniaProperty.Register<TreeDataGridHost, IDataTemplate?>(nameof(SingleColumnTemplate));

  public UIC.TreeView? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }
  public IDataTemplate? SingleColumnTemplate { get => GetValue(SingleColumnTemplateProperty); set => SetValue(SingleColumnTemplateProperty, value); }

  private static void _onViewModelChanged(TreeDataGridHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static TreeDataGridHost() {
    ViewModelProperty.Changed.AddClassHandler<TreeDataGridHost>(_onViewModelChanged);
  }

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);

    if (e.NameScope.Find<ScrollViewer>("PART_ScrollViewer") is { } sv) {
      _sv = sv;
      _sv.GetObservable(IsVisibleProperty).Subscribe(_raiseHostIsVisibleChanged);
      _sv.ScrollChanged += _onScrollChanged;
      _raiseHostIsVisibleChanged(_sv.IsVisible);
    }
    
    _setItemsSource();
  }

  private void _raiseHostIsVisibleChanged(bool value) => HostIsVisibleChangedEvent?.Invoke(this, value);

  private void _onScrollChanged(object? sender, ScrollChangedEventArgs e) {
    // TODO PORT
  }


  private void _setItemsSource() {
    if (ViewModel == null) return;
    var expand = false;
    var root = ViewModel.RootHolder.FirstOrDefault();
    if (root is { IsExpanded: true }) {
      expand = true;
      root.IsExpanded = false;
    }

    Source = new HierarchicalTreeDataGridSource<ITreeItem>(ViewModel.RootHolder) {
      Columns = {
        new HierarchicalExpanderColumn<ITreeItem>(
          new TemplateColumn<ITreeItem>(null, SingleColumnTemplate ?? _defaultSingleColumnTemplate),
          x => x.Items,
          null,
          x => x.IsExpanded)
      },
    };

    if (expand) ExpandRootWhenReady(root!);
  }

  public void ScrollToTop() {
    // TODO PORT
  }

  public void ScrollToItems(object[] items, bool exactly) =>
    Dispatcher.UIThread.Post(() => _scrollToItems(items, exactly), DispatcherPriority.Background);

  private void _scrollToItems(object[] items, bool exactly) {
    // TODO PORT
  }

  /// <summary>
  /// TreeView loads all items when everything is expanded.
  /// So I collapsed the root on reload and expanded it after to load just what is in the view.
  /// </summary>
  public void ExpandRootWhenReady(ITreeItem root) =>
    Dispatcher.UIThread.Post(() => root.IsExpanded = true, DispatcherPriority.Loaded);
}