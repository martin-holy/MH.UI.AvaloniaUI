﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Markup.Xaml.Templates;
using Avalonia.VisualTree;
using MH.UI.AvaloniaUI.Converters;
using MH.UI.Interfaces;
using MH.Utils.BaseClasses;
using UIC = MH.UI.Controls;

namespace MH.UI.AvaloniaUI.Controls;

public class CollectionViewHost : TreeViewHost, UIC.ICollectionViewHost {
  private double _openTime;
  private DateTime _lastClickTime = DateTime.Now;

  public static IDataTemplate? GroupByDialogDataTemplateSelector { get; set; }

  public new static readonly StyledProperty<UIC.TreeView?> ViewModelProperty =
    AvaloniaProperty.Register<CollectionViewHost, UIC.TreeView?>(nameof(ViewModel));

  public new UIC.CollectionView? ViewModel {
    get => (UIC.CollectionView?)GetValue(ViewModelProperty);
    set => SetValue(ViewModelProperty, value);
  }

  private static void _onViewModelChanged(CollectionViewHost o, AvaloniaPropertyChangedEventArgs e) {
    o.SetValue(TreeViewHost.ViewModelProperty, o.ViewModel);
    if (o.ViewModel != null) o.ViewModel.Host = o;
  }

  static CollectionViewHost() {
    ViewModelProperty.Changed.AddClassHandler<CollectionViewHost>(_onViewModelChanged);
  }

  public static RelayCommand<TappedEventArgs> OpenItemCommand { get; } = new(_openItem);
  public static RelayCommand<SizeChangedEventArgs> SetGroupWidthCommand { get; } = new(_setGroupWidth);

  private static void _openItem(TappedEventArgs? e) {
    if ((e?.Source as Control)?.FindAncestorOfType<CollectionViewHost>() is not { ViewModel.CanOpen: true } cv) return;
    cv._openItem(_getDataContext(e.Source));
  }

  private void _openItem(object? item) {
    var startTime = DateTime.Now;
    ViewModel?.OpenItem(item);
    _openTime = (DateTime.Now - startTime).TotalMilliseconds;
  }

  internal override bool UpdateSelectionFromPointerEvent(Control source, PointerEventArgs e) {
    if ((e.Source as Control)?.FindAncestorOfType<CollectionViewHost>() is not { ViewModel.CanOpen: true } cv
        || cv._doubleClicking()) return false;

    var item = _getDataContext(e.Source);
    var row = ((e.Source as Control)?.FindAncestorOfType<CollectionViewRowItemsControl>()?.DataContext as FlatTreeItem)?.TreeItem;
    var btn = e.Source as Button ?? (e.Source as Control)?.FindAncestorOfType<Button>();

    if (item == null || row == null || btn != null) return false;

    bool isCtrlOn;
    bool isShiftOn;

    if (e.GetCurrentPoint(null).Properties.PointerUpdateKind != PointerUpdateKind.LeftButtonReleased) {
      isCtrlOn = true;
      isShiftOn = false;
    }
    else {
      isCtrlOn = (e.KeyModifiers & KeyModifiers.Control) > 0;
      isShiftOn = (e.KeyModifiers & KeyModifiers.Shift) > 0;
    }

    cv.ViewModel.SelectItem(row, item, isCtrlOn, isShiftOn);

    return true;
  }

  private bool _doubleClicking() {
    var sinceLastClick = (DateTime.Now - _lastClickTime).TotalMilliseconds;
    _lastClickTime = DateTime.Now;
    return sinceLastClick - _openTime < 300;
  }

  // TODO PORT
  private static object? _getDataContext(object source) =>
    (source as Control)?.DataContext;

  private static void _setGroupWidth(SizeChangedEventArgs? e) {
    if (e is { WidthChanged: true, Source: StyledElement { DataContext: FlatTreeItem { TreeItem: ICollectionViewGroup group} } }) {
      group.Width = e.NewSize.Width;
    }
  }
}

public class CollectionViewTemplateSelector : IDataTemplate {
  public Control? Build(object? param) {
    if (param is not FlatTreeItem fti || Application.Current == null) return null;

    var key = fti.TreeItem switch {
      ICollectionViewGroup => "MH.DT.CollectionViewGroup",
      ICollectionViewRow => "MH.DT.CollectionViewRow",
      _ => throw new ArgumentOutOfRangeException(nameof(param), param, null)
    };

    if (Application.Current.TryFindResource(key, out var res) && res is IDataTemplate dt)
      return dt.Build(param);

    return null;
  }

  public bool Match(object? data) =>
    data is FlatTreeItem { TreeItem: ICollectionViewGroup or ICollectionViewRow };
}

public class CollectionViewItemContainer : ContentControl {
  public static readonly StyledProperty<IDataTemplate?> InnerContentTemplateProperty =
    AvaloniaProperty.Register<CollectionViewItemContainer, IDataTemplate?>(nameof(InnerContentTemplate));

  public IDataTemplate? InnerContentTemplate {
    get => GetValue(InnerContentTemplateProperty);
    set => SetValue(InnerContentTemplateProperty, value);
  }

  static CollectionViewItemContainer() {
    AffectsRender<CollectionViewItemContainer>(InnerContentTemplateProperty);
  }
}

public class CollectionViewRowItemsControl : ItemsControl {
  private ICollectionViewGroup? _group;
  private IDataTemplate? _innerItemTemplate;
  private string? _innerItemTemplateKey;

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _group = (DataContext as FlatTreeItem)?.TreeItem.Parent as ICollectionViewGroup;
  }

  protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey) {
    var templateKey = _group?.GetItemTemplateName();
    if (!string.Equals(_innerItemTemplateKey, templateKey)) {
      _innerItemTemplateKey = templateKey;
      _innerItemTemplate = ResourceConverter.Inst.Convert(templateKey, null) as DataTemplate;
    }
    
    var container = new CollectionViewItemContainer { InnerContentTemplate = _innerItemTemplate };

    if (item != null && _group != null) {
      container.Width = _group.GetItemSize(item, true);
      container.Height = _group.GetItemSize(item, false);
    }

    return container;
  }
}