using System;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace KanjiStrokes {
	public abstract class ViewModelBase : INotifyPropertyChanged {
		public event PropertyChangedEventHandler PropertyChanged;

		protected void OnPropertyChanged([CallerMemberName] string propertyName = null) {
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public sealed class MainWindowViewModel : ViewModelBase {

		public Settings Settings { get; } = new Settings();
		public KanjiDatabaseViewModel KanjiDatabase { get; } = new KanjiDatabaseViewModel();

		private readonly ImmutableArray<string> _startupKanji = new[] {
			"漢字", "学校", "電話", "日本", "学生", "先生", "今日", "昨日", "今夜", "番号",
			"兄弟", "兄妹", "姉弟", "姉妹"
		}.ToImmutableArray();

		public MainWindowViewModel() {
			void Callback(object sender, PropertyChangedEventArgs e) {
				if(e.PropertyName != nameof(KanjiDatabase.Status) ||
				   KanjiDatabase.Status != KanjiDatabaseViewModel.LoadingStatus.Ready) return;
				KanjiDatabase.PropertyChanged -= Callback;
				CurrentKanjiInfo = KanjiDatabase.GetKanjiInfo(ActiveCharacter);
			}
			KanjiDatabase.PropertyChanged += Callback;
			DarkMode = Settings.DarkMode;
			IsHelpPageOpen = Settings.FirstStart;
			Settings.FirstStart = false;
			Text = _startupKanji[new Random().Next(_startupKanji.Length)];
		}

		private bool _wasMaximizedBeforeFullscreen;
		private bool _fullscreen;
		public bool Fullscreen {
			get => _fullscreen;
			set {
				if(value == _fullscreen) return;
				_fullscreen = value;
				if(Fullscreen) {
					_wasMaximizedBeforeFullscreen = WindowState == WindowState.Maximized;
					UpdateWindowState(WindowState.Normal, false);
					OnPropertyChanged();
					UpdateWindowState(WindowState.Maximized, false);
				}
				else {
					OnPropertyChanged();
					UpdateWindowState(_wasMaximizedBeforeFullscreen ? WindowState.Maximized : WindowState.Normal, false);
				}
			}
		}

		private WindowState _windowState = WindowState.Normal;
		public WindowState WindowState {
			get => _windowState;
			set {
				if(value == _windowState) return;
				UpdateWindowState(value, true);
			}
		}

		private void UpdateWindowState(WindowState state, bool setFullscreen) {
			_windowState = state;
			if(setFullscreen && WindowState != WindowState.Maximized)
				Fullscreen = false;
			OnPropertyChanged(nameof(WindowState));
		}

		private bool _darkMode = true;
		public bool DarkMode {
			get => _darkMode;
			set {
				if(value == _darkMode) return;
				Settings.DarkMode = _darkMode = value;
				if(Application.Current.MainWindow is MainWindow mw)
					mw.ThemeDictionary.Source = ThemeUri;
				OnPropertyChanged();
				OnPropertyChanged(nameof(ThemeUri));
			}
		}

		public Uri ThemeUri => _darkMode
			? new Uri("ThemeDark.xaml", UriKind.Relative)
			: new Uri("ThemeLight.xaml", UriKind.Relative);

		private string _text;
		public string Text {
			get => _text;
			set {
				if(value == _text) return;
				_text = value;
				OnPropertyChanged();
			}
		}

		public void UpdateActiveCharacter(int caretIndex, int selectionStart, int selectionLength) {
			char? GetChar(int index) => index >= 0 && index < Text.Length ? Text[index] : (char?)null;
			char? GetKanji(int index) => GetChar(index) is char c && TextHelpers.IsKanji(c) ? c : (char?)null;

			char? kanji;

			if(selectionLength == 0) {
				kanji = GetKanji(caretIndex - 1) ?? GetKanji(caretIndex);
			}
			else {
				kanji = GetKanji(selectionStart);
			}

			if(kanji != null) ActiveCharacter = kanji.Value;
		}

		private char _activeCharacter;
		public char ActiveCharacter {
			get => _activeCharacter;
			set {
				if(value == _activeCharacter) return;
				_activeCharacter = value;
				OnPropertyChanged();
				CurrentKanjiInfo = KanjiDatabase.GetKanjiInfo(value);
			}
		}

		private KanjiInfo _currentKanjiInfo;
		public KanjiInfo CurrentKanjiInfo {
			get => _currentKanjiInfo;
			set {
				if(value == _currentKanjiInfo) return;
				_currentKanjiInfo = value;
				OnPropertyChanged();
			}
		}

		private bool _isInfoPanelExpanded = App.IsInDesignMode;
		public bool IsInfoPanelExpanded {
			get => _isInfoPanelExpanded;
			set {
				if(value == _isInfoPanelExpanded) return;
				_isInfoPanelExpanded = value;
				OnPropertyChanged();
			}
		}

		private bool _isCreditsPanelExpanded = App.IsInDesignMode;
		public bool IsCreditsPanelExpanded {
			get => _isCreditsPanelExpanded;
			set {
				if(value == _isCreditsPanelExpanded) return;
				_isCreditsPanelExpanded = value;
				OnPropertyChanged();
			}
		}

		private bool _isHelpPageOpen;
		public bool IsHelpPageOpen {
			get => _isHelpPageOpen;
			set {
				if(value == _isHelpPageOpen) return;
				_isHelpPageOpen = value;
				OnPropertyChanged();
				OnPropertyChanged(nameof(IsTextBoxVisible));
				IsInfoPanelExpanded = IsCreditsPanelExpanded = false;
				Keyboard.Focus(value ? (IInputElement)Application.Current.MainWindow : (Application.Current.MainWindow as MainWindow)?.TextBox);
			}
		}

		public bool IsTextBoxVisible => !IsHelpPageOpen;

		public ICommand ToggleFullscreenCommand => new ActionCommand(() => Fullscreen = !Fullscreen);
		public ICommand ToggleDarkModeCommand => new ActionCommand(() => DarkMode = !DarkMode);
		public ICommand ToggleInfoPanelCommand => new ActionCommand(() => IsInfoPanelExpanded = !IsInfoPanelExpanded && !IsHelpPageOpen);
		public ICommand ToggleCreditsPanelCommand => new ActionCommand(() => IsCreditsPanelExpanded = !IsCreditsPanelExpanded && !IsHelpPageOpen);
		public ICommand OpenHelpPageCommand => new ActionCommand(() => IsHelpPageOpen = true);
		public ICommand CollapsePanelsCommand => new ActionCommand(() => IsCreditsPanelExpanded = IsInfoPanelExpanded = IsHelpPageOpen = false);
		public ICommand OpenUrlCommand => new ActionCommand<string>(url => Process.Start("explorer", url));

		public void OnClosing() => Settings.Save();
	}
}
