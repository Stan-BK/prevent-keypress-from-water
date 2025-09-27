using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace prevent_keypress_from_water
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private readonly KeyboardHook _globalHook = new ();

        private readonly Color disabledColor = Color.FromArgb(255, 248, 64, 64);

        private readonly Color enabledColor = Color.FromArgb(255, 170, 170, 170);

        private string _blockedKeyInfo = "Last Blocked Key:";
        public string BlockedKeyInfo
        {
            get => _blockedKeyInfo;
            set
            {
                if (_blockedKeyInfo != value)
                {
                    _blockedKeyInfo = value;
                    OnPropertyChanged();
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            _globalHook.KeyBlocked += OnKeyBlocked;
            _globalHook.Start();
        }

        private void OnKeyBlocked(Key key)
        {
            Dispatcher.Invoke(() =>
            {
                BlockedKeyInfo = $"Last Blocked Key: {key}";
            });
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        protected override void OnClosed(EventArgs e)
        {
            _globalHook.Dispose();
            base.OnClosed(e);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            Key key = (Key)Enum.Parse(typeof(Key), btn.Tag.ToString()!);
            if (IsKeyBlocked(key))
            {
                _globalHook.RemoveBlockedKey(key);
                btn.BorderBrush = new SolidColorBrush(enabledColor);
            }
            else
            {
                _globalHook.AddBlockedKey(key);
                btn.BorderBrush = new SolidColorBrush(disabledColor);
            }
        }

        private Boolean IsKeyBlocked(Key key)
        {
            return _globalHook.IsKeyBlocked(key);
        }
    }
}