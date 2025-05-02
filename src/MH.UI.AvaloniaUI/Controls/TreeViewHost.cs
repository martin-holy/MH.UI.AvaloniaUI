using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Threading;
using Avalonia.VisualTree;
using MH.Utils.BaseClasses;
using MH.Utils.Interfaces;
using System.ComponentModel;
using Avalonia.Controls.Templates;
using MH.Utils;
using MH.Utils.Extensions;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class TreeViewHost : TreeView, UIC.ITreeViewHost {
  private bool _isScrollingTo;
  private bool _resetHScroll;
  private double _resetHOffset;
  private ScrollViewer _sv = null!;

  public static readonly StyledProperty<UIC.TreeView?> ViewModelProperty =
    AvaloniaProperty.Register<TreeViewHost, UIC.TreeView?>(nameof(ViewModel));

  public UIC.TreeView? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(TreeViewHost o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static TreeViewHost() {
    ViewModelProperty.Changed.AddClassHandler<TreeViewHost>(_onViewModelChanged);
  }

  public RelayCommand<RequestBringIntoViewEventArgs> TreeItemIntoViewCommand { get; }

  public event EventHandler<bool>? HostIsVisibleChangedEvent;

  public TreeViewHost() {
    TreeItemIntoViewCommand = new(_onTreeItemIntoView);
  }

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

  private void _onTreeItemIntoView(RequestBringIntoViewEventArgs? e) {
    _resetHScroll = true;
    _resetHOffset = _sv.Offset.X;
  }

  private void _onScrollChanged(object? sender, ScrollChangedEventArgs e) {
    if (!_isScrollingTo && ViewModel != null && _sv.IsVisible)
      ViewModel.TopTreeItem = _getHitTestItem(10, 10);

    if (_resetHScroll) {
      _resetHScroll = false;
      _sv.Offset.WithX(_resetHOffset);
    }
  }

  private void _setItemsSource() {
    if (ViewModel == null) return;
    var expand = false;
    var root = ViewModel.RootHolder.FirstOrDefault() as ITreeItem;
    if (root is { IsExpanded: true }) {
      expand = true;
      root.IsExpanded = false;
    }
    ItemsSource = ViewModel.RootHolder;
    if (expand) ExpandRootWhenReady(root!);
  }

  public void ScrollToTop() {
    if (_sv.Offset.Y == 0) return;
    _sv.ScrollToHome();
    _sv.UpdateLayout();

    if (!_sv.IsVisible && ViewModel != null)
      ViewModel.TopTreeItem = _getHitTestItem(10, 10);
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

  /// <summary>
  /// Scroll TreeView when the mouse is near the top or bottom during a drag-and-drop operation.
  /// </summary>
  private protected void _dragDropAutoScroll(DragEventArgs e) {
    const double px = 25;
    const double scrollSpeed = 10;
    var pos = e.GetPosition(this);
    var newOffsetY = _sv.Offset.Y;

    if (pos.Y < px)
      newOffsetY -= scrollSpeed;
    else if (_sv.Bounds.Height - pos.Y < px)
      newOffsetY += scrollSpeed;

    _sv.Offset = _sv.Offset.WithY(Math.Max(0, newOffsetY));
  }
}

public class TreeViewHost2 : ItemsControl, UIC.ITreeViewHost {
  private bool _isScrollingTo;
  private ScrollViewer _sv = null!;
  private static readonly FuncTemplate<Panel?> _defaultPanel = new(() => new VirtualizingStackPanel());

  public static readonly StyledProperty<UIC.TreeView?> ViewModelProperty =
    AvaloniaProperty.Register<TreeViewHost2, UIC.TreeView?>(nameof(ViewModel));

  public UIC.TreeView? ViewModel { get => GetValue(ViewModelProperty); set => SetValue(ViewModelProperty, value); }

  private static void _onViewModelChanged(TreeViewHost2 o, AvaloniaPropertyChangedEventArgs e) {
    if (o.ViewModel != null) o.ViewModel.Host = o;
    if (e.NewValue is UIC.TreeView newTreeView)
      newTreeView.RootHolder.CollectionChanged += o._onRootHolderChanged;
    if (e.OldValue is UIC.TreeView oldTreeView)
      oldTreeView.RootHolder.CollectionChanged -= o._onRootHolderChanged;
  }

  static TreeViewHost2() {
    ItemsPanelProperty.OverrideDefaultValue<TreeViewHost2>(_defaultPanel);
    ViewModelProperty.Changed.AddClassHandler<TreeViewHost2>(_onViewModelChanged);
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

  protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) =>
    new FlatTreeItemHost();

  protected  override bool NeedsContainerOverride(object? item, int index, out object? recycleKey) =>
    NeedsContainer<FlatTreeItemHost>(item, out recycleKey);

  private void _raiseHostIsVisibleChanged(bool value) => HostIsVisibleChangedEvent?.Invoke(this, value);

  private void _onScrollChanged(object? sender, ScrollChangedEventArgs e) {
    if (!_isScrollingTo && ViewModel != null && _sv.IsVisible)
      ViewModel.TopTreeItem = _getHitTestItem(10, 10);
  }

  private void _setItemsSource() {
    if (ViewModel == null) return;
    var newFlatItems = Tree.ToFlatTreeItems(ViewModel.RootHolder);
    _updateTreeItemSubscriptions(ItemsSource as IEnumerable<FlatTreeItem>, newFlatItems);
    ItemsSource = newFlatItems;
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

  internal virtual bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e) {
    if (source.DataContext is FlatTreeItem fti)
      ViewModel?.SelectItemCommand.Execute(fti.TreeItem);

    return true;
  }

  private void _updateTreeItemSubscriptions(IEnumerable<FlatTreeItem>? oldItems, IEnumerable<FlatTreeItem>? newItems) {
    var o = oldItems?.Except(newItems ?? []).ToArray() ?? [];
    var n = newItems?.Except(oldItems ?? []).ToArray() ?? [];

    foreach (var item in o)
      item.TreeItem.PropertyChanged -= _onTreeItemPropertyChanged;

    foreach (var item in n)
      item.TreeItem.PropertyChanged += _onTreeItemPropertyChanged;
  }

  private void _onTreeItemPropertyChanged(object? sender, PropertyChangedEventArgs e) {
    if (e.Is(nameof(TreeItem.IsExpanded)))
      _setItemsSource();
  }

  private void _onRootHolderChanged(object? sender, NotifyCollectionChangedEventArgs e) =>
    _setItemsSource();
}