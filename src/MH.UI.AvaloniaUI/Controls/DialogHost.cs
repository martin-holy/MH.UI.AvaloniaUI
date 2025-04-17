using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using MH.UI.Controls;
using System.Collections.ObjectModel;

namespace MH.UI.AvaloniaUI.Controls;

public class DialogHost : TemplatedControl {
  private static DialogHost? _inst;

  public static readonly ObservableCollection<object> ActiveDialogs = [];

  public static readonly StyledProperty<IDataTemplate?> DialogTemplateProperty =
    AvaloniaProperty.Register<DialogHost, IDataTemplate?>(nameof(DialogTemplate));

  public IDataTemplate? DialogTemplate {
    get => GetValue(DialogTemplateProperty);
    set => SetValue(DialogTemplateProperty, value);
  }

  static DialogHost() {
    ActiveDialogs.CollectionChanged += (_, _) => {
      if (_inst != null)
        _inst.IsVisible = ActiveDialogs.Count > 0;
    };
  }

  protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
    base.OnApplyTemplate(e);
    _inst = this;
    IsVisible = ActiveDialogs.Count > 0;
  }

  public static async Task<int> ShowAsync(Dialog dialog) {
    if (!ActiveDialogs.Contains(dialog))
      ActiveDialogs.Add(dialog);

    var result = await dialog.TaskCompletionSource.Task;
    ActiveDialogs.Remove(dialog);

    return result;
  }
}