namespace MH.UI.Avalonia.Sample;

public class CoreUI {
  public CoreUI() {
    MH.UI.Avalonia.Utils.Init.SetDelegates();
    MH.UI.Avalonia.Resources.Dictionaries.IconToBrush = Avalonia.Sample.Resources.Res.IconToBrushDic;
  }
}