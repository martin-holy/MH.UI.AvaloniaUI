﻿using MH.UI.AvaloniaUI.Sample.Models;
using MH.UI.Controls;
using MH.UI.AvaloniaUI.Sample.ViewModels.Controls;
using MH.Utils.BaseClasses;

namespace MH.UI.AvaloniaUI.Sample.ViewModels.Layout;

public class RightContentVM : ObservableObject {
  private FolderM? _selectedFolder;

  public FolderM? SelectedFolder { get => _selectedFolder; private set { _selectedFolder = value; OnPropertyChanged(); } }
  public FolderTreeViewVM FolderTreeView { get; } = new();
  public SlidePanelPinButton SlidePanelPinButton { get; } = new();

  public RightContentVM() {
    FolderTreeView.ItemSelectedEvent += (_, item) => SelectedFolder = item as FolderM;
  }
}