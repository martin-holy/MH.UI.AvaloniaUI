using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Models.TreeDataGrid;
using Avalonia.Controls.Presenters;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MH.Utils.Interfaces;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class TreeDataGridHost : TreeDataGrid, UIC.ITreeViewHost {
  private bool _isScrollingTo;
  private ScrollViewer _sv = null!;
  private HierarchicalTreeDataGridSource<ITreeItem>? _source;

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

    SelectionChanging += (_, se) => se.Cancel = true;
    
    _setItemsSource();
  }

  private void _raiseHostIsVisibleChanged(bool value) => HostIsVisibleChangedEvent?.Invoke(this, value);

  private void _onScrollChanged(object? sender, ScrollChangedEventArgs e) {
    if (!_isScrollingTo && ViewModel != null && _sv.IsVisible)
      ViewModel.TopTreeItem = _getHitTestItem(10, 10);
  }

  private void _setItemsSource() {
    if (ViewModel == null) return;
    var expand = false;
    var root = ViewModel.RootHolder.FirstOrDefault();
    if (root is { IsExpanded: true }) {
      expand = true;
      root.IsExpanded = false;
    }

    _source = new(ViewModel.RootHolder) {
      Columns = {
        new HierarchicalExpanderColumn<ITreeItem>(
          new TemplateColumn<ITreeItem>(null, SingleColumnTemplate ?? _defaultSingleColumnTemplate, null, GridLength.Star),
          x => x.Items,
          null,
          x => x.IsExpanded)
      }
    };

    Source = _source;

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

  private ITreeItem? _getHitTestItem(double x, double y) =>
    _sv.GetVisualsAt(new(x, y))
      .OfType<Control>()
      .Select(c => c.DataContext)
      .OfType<ITreeItem>()
      .FirstOrDefault(ViewModel!.IsHitTestItem);

  /// <summary>
  /// TreeView loads all items when everything is expanded.
  /// So I collapsed the root on reload and expanded it after to load just what is in the view.
  /// </summary>
  public void ExpandRootWhenReady(ITreeItem root) =>
    Dispatcher.UIThread.Post(() => root.IsExpanded = true, DispatcherPriority.Loaded);
}