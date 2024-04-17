using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Testing.UIChangeInducedByMIDI;

public class MyViewModel : INotifyPropertyChanged
{
    private string _myLabelText;

    public string MyLabelText
    {
        get => _myLabelText;
        set
        {
            _myLabelText = value;
            OnPropertyChanged("MyLabelText");
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}