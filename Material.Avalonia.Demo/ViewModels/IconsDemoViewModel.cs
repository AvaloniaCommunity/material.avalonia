using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Threading;
using Material.Avalonia.Demo.Models;
using Material.Icons;

namespace Material.Avalonia.Demo.ViewModels;

public class IconsDemoViewModel : ViewModelBase {
    private readonly Lazy<List<MaterialIconKindGroup>> _materialIconKinds;
    private IEnumerable<MaterialIconKindGroup>? _kinds;
    private string? _searchText;
    private MaterialIconKindGroup? _selectedGroup;

    public IconsDemoViewModel() {
        _materialIconKinds = new Lazy<List<MaterialIconKindGroup>>(() =>
            Enum.GetNames(typeof(MaterialIconKind))
                .GroupBy(k => (MaterialIconKind)Enum.Parse(typeof(MaterialIconKind), k))
                .Select(g => new MaterialIconKindGroup(this, g))
                .OrderBy(x => x.Kind)
                .ToList());

        CopyToClipboardCommand = new RelayCommand(o =>
            (o as TopLevel)?.Clipboard?.SetTextAsync($"<avalonia:MaterialIcon Kind=\"{o}\" />"));

        SearchCommand = new RelayCommand(DoSearchAsync);
    }

    public IEnumerable<MaterialIconKindGroup> Kinds {
        get => _kinds ?? _materialIconKinds.Value;
        set {
            _kinds = value;
            OnPropertyChanged();
        }
    }

    public MaterialIconKindGroup? SelectedGroup {
        get => _selectedGroup;
        set {
            _selectedGroup = value;
            OnPropertyChanged();
        }
    }

    public string? SearchText {
        get => _searchText;
        set {
            _searchText = value;
            OnPropertyChanged();
        }
    }

    public RelayCommand SearchCommand { get; }

    public RelayCommand CopyToClipboardCommand { get; }

    private async void DoSearchAsync(object? args) {
        if (string.IsNullOrWhiteSpace(SearchText))
            Kinds = _materialIconKinds.Value;
        else {
            var list = new ObservableCollection<MaterialIconKindGroup>();

            Kinds = list;

            foreach (var data in _materialIconKinds.Value
                         .Where(x => x.Aliases
                             .Any(a => a.Contains(SearchText, StringComparison.CurrentCultureIgnoreCase)))) {
                await Dispatcher.UIThread.InvokeAsync(delegate {
                    list.Add(data);
                });
            }
        }
    }
}