﻿using MH.UI.Controls;
using System;
using System.IO;
using MH.UI.AvaloniaUI.Sample.Models;
using MH.UI.AvaloniaUI.Sample.Resources;

namespace MH.UI.AvaloniaUI.Sample.ViewModels.Controls;

public class FolderTreeViewVM : TreeView {
  public FolderTreeViewVM() {
    ShowTreeItemSelection = true;
    AddDrives();
  }

  private void AddDrives() {
    var drives = Environment.GetLogicalDrives();
    RootHolder.Clear();
    SelectedTreeItems.DeselectAll();

    foreach (var drive in drives) {
      var di = new DriveInfo(drive);
      if (!di.IsReady) continue;

      var item = new FolderM(null, di.Name.TrimEnd(Path.DirectorySeparatorChar)) {
        Icon = GetDriveIcon(di.DriveType)
      };

      // add placeholder so the Drive can be expanded
      item.Items.Add(new FolderM(null, string.Empty));

      RootHolder.Add(item);
    }
  }

  public static string GetDriveIcon(DriveType type) =>
    type switch {
      DriveType.CDRom => Icons.Cd,
      DriveType.Network => Icons.Drive,
      DriveType.NoRootDirectory or DriveType.Unknown => Icons.DriveError,
      _ => Icons.Drive,
    };
}