namespace MH.UI.AvaloniaUI.Sample;

public class CoreUI {
  public CoreUI() {
    MH.UI.AvaloniaUI.Utils.Init.SetDelegates();
    MH.UI.AvaloniaUI.Resources.Dictionaries.IconToBrush = AvaloniaUI.Sample.Resources.Res.IconToBrushDic;
  }
}