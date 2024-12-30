using Avalonia;
using System.Collections;

namespace MH.UI.AvaloniaUI.Converters;

public enum CheckFor { NotNull, Null, True, False, NotEmpty, NullOrEmpty, MoreThan0, EqualsParam, NotEqualsParam }

// TODO PORT Visibility.Hidden

public class VisibilityConverter : BaseConverter {
  private static VisibilityConverter? _nullToVisible;
  private static VisibilityConverter? _notNullToVisible;
  private static VisibilityConverter? _trueToVisible;
  private static VisibilityConverter? _falseToVisible;
  private static VisibilityConverter? _trueToHidden;
  private static VisibilityConverter? _notEmptyToVisible;
  private static VisibilityConverter? _nullOrEmptyToVisible;
  private static VisibilityConverter? _intToVisible;
  private static VisibilityConverter? _equalsParamToVisible;
  private static VisibilityConverter? _notEqualsParamToVisible;

  public static VisibilityConverter NullToVisible => _nullToVisible ??= new() { CheckFor = CheckFor.Null, ToVisible = true };
  public static VisibilityConverter NotNullToVisible => _notNullToVisible ??= new() { CheckFor = CheckFor.NotNull, ToVisible = true };
  public static VisibilityConverter TrueToVisible => _trueToVisible ??= new() { CheckFor = CheckFor.True, ToVisible = true };
  public static VisibilityConverter FalseToVisible => _falseToVisible ??= new() { CheckFor = CheckFor.False, ToVisible = true };
  public static VisibilityConverter TrueToHidden => _trueToHidden ??= new() { CheckFor = CheckFor.True, ToHidden = true };
  public static VisibilityConverter NotEmptyToVisible => _notEmptyToVisible ??= new() { CheckFor = CheckFor.NotEmpty, ToVisible = true };
  public static VisibilityConverter NullOrEmptyToVisible => _nullOrEmptyToVisible ??= new() { CheckFor = CheckFor.NullOrEmpty, ToVisible = true };
  public static VisibilityConverter IntToVisible => _intToVisible ??= new() { CheckFor = CheckFor.MoreThan0, ToVisible = true };
  public static VisibilityConverter EqualsParamToVisible => _equalsParamToVisible ??= new() { CheckFor = CheckFor.EqualsParam, ToVisible = true };
  public static VisibilityConverter NotEqualsParamToVisible => _notEqualsParamToVisible ??= new() { CheckFor = CheckFor.NotEqualsParam, ToVisible = true };

  public CheckFor CheckFor { get; init; }

  public bool ToCollapsed { get; init; }
  public bool ToHidden { get; init; }
  public bool ToVisible { get; init; }

  public override object Convert(object? value, object? parameter) =>
    CheckFor switch {
      CheckFor.NotNull => GetFor(value != null),
      CheckFor.Null => GetFor(value == null),
      CheckFor.True => GetFor(value is true),
      CheckFor.False => GetFor(value is false),
      CheckFor.NotEmpty => GetFor((value is string s && !string.IsNullOrEmpty(s)) || (value is IList { Count: > 0 })),
      CheckFor.NullOrEmpty => GetFor(value is null || string.IsNullOrEmpty(value as string) || value is IList { Count: 0 }),
      CheckFor.MoreThan0 => GetFor(value is > 0),
      CheckFor.EqualsParam => GetFor(Equals(value, parameter)),
      CheckFor.NotEqualsParam => GetFor(!Equals(value, parameter)),
      _ => AvaloniaProperty.UnsetValue
    };

  private bool GetFor(bool flag) {
    if (flag) {
      if (ToVisible) return true;
      if (ToCollapsed) return false;
      if (ToHidden) return false;
    }
    else {
      if (ToVisible) return false;
      if (ToCollapsed || ToHidden) return true;
    }

    return false;
  }
}