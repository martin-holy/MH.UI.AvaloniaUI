using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;

namespace MH.UI.AvaloniaUI.AttachedProperties;

public class Slot : AvaloniaObject {
  public static readonly AttachedProperty<object> LeftContentProperty = AvaloniaProperty.RegisterAttached<Slot, Control, object>("LeftContent");
  public static readonly AttachedProperty<object> TopContentProperty = AvaloniaProperty.RegisterAttached<Slot, Control, object>("TopContent");
  public static readonly AttachedProperty<object> RightContentProperty = AvaloniaProperty.RegisterAttached<Slot, Control, object>("RightContent");
  public static readonly AttachedProperty<object> BottomContentProperty = AvaloniaProperty.RegisterAttached<Slot, Control, object>("BottomContent");

  public static readonly AttachedProperty<IDataTemplate> LeftContentTemplateProperty = AvaloniaProperty.RegisterAttached<Slot, Control, IDataTemplate>("LeftContentTemplate");
  public static readonly AttachedProperty<IDataTemplate> TopContentTemplateProperty = AvaloniaProperty.RegisterAttached<Slot, Control, IDataTemplate>("TopContentTemplate");
  public static readonly AttachedProperty<IDataTemplate> RightContentTemplateProperty = AvaloniaProperty.RegisterAttached<Slot, Control, IDataTemplate>("RightContentTemplate");
  public static readonly AttachedProperty<IDataTemplate> BottomContentTemplateProperty = AvaloniaProperty.RegisterAttached<Slot, Control, IDataTemplate>("BottomContentTemplate");

  public static object GetLeftContent(AvaloniaObject element) => element.GetValue(LeftContentProperty);
  public static void SetLeftContent(AvaloniaObject element, object value) => element.SetValue(LeftContentProperty, value);
  public static object GetTopContent(AvaloniaObject element) => element.GetValue(TopContentProperty);
  public static void SetTopContent(AvaloniaObject element, object value) => element.SetValue(TopContentProperty, value);
  public static object GetRightContent(AvaloniaObject element) => element.GetValue(RightContentProperty);
  public static void SetRightContent(AvaloniaObject element, object value) => element.SetValue(RightContentProperty, value);
  public static object GetBottomContent(AvaloniaObject element) => element.GetValue(BottomContentProperty);
  public static void SetBottomContent(AvaloniaObject element, object value) => element.SetValue(BottomContentProperty, value);

  public static IDataTemplate GetLeftContentTemplate(AvaloniaObject element) => element.GetValue(LeftContentTemplateProperty);
  public static void SetLeftContentTemplate(AvaloniaObject element, IDataTemplate value) => element.SetValue(LeftContentTemplateProperty, value);
  public static IDataTemplate GetTopContentTemplate(AvaloniaObject element) => element.GetValue(TopContentTemplateProperty);
  public static void SetTopContentTemplate(AvaloniaObject element, IDataTemplate value) => element.SetValue(TopContentTemplateProperty, value);
  public static IDataTemplate GetRightContentTemplate(AvaloniaObject element) => element.GetValue(RightContentTemplateProperty);
  public static void SetRightContentTemplate(AvaloniaObject element, IDataTemplate value) => element.SetValue(RightContentTemplateProperty, value);
  public static IDataTemplate GetBottomContentTemplate(AvaloniaObject element) => element.GetValue(BottomContentTemplateProperty);
  public static void SetBottomContentTemplate(AvaloniaObject element, IDataTemplate value) => element.SetValue(BottomContentTemplateProperty, value);
}