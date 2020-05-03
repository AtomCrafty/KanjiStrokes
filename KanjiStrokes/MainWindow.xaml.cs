using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace KanjiStrokes {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public readonly ResourceDictionary ThemeDictionary = new ResourceDictionary { Source = new Uri("ThemeDark.xaml", UriKind.Relative) };

		public MainWindow() {
			InitializeComponent();
			Resources.MergedDictionaries.Remove(Resources.MergedDictionaries.First(dict => dict.Source.OriginalString == "ThemeDark.xaml"));
			Resources.MergedDictionaries.Add(ThemeDictionary);
		}

		private void TextArea_OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e) {
			InputLanguageManager.Current.CurrentInputLanguage = new CultureInfo("ja-JP");
			if(InputMethod.Current != null) InputMethod.Current.ImeState = InputMethodState.On;
		}

		private void TextArea_OnSelectionChanged(object sender, RoutedEventArgs e) {
			if(!(sender is TextBox tb)) return;
			if(!(DataContext is MainWindowViewModel vm)) return;
			vm.UpdateActiveCharacter(tb.CaretIndex, tb.SelectionStart, tb.SelectionLength);
		}

		private void MainWindow_OnClosing(object sender, CancelEventArgs e) {
			if(!(DataContext is MainWindowViewModel vm)) return;
			vm.OnClosing();
		}

		private void HelpOverlay_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			if(!(DataContext is MainWindowViewModel vm)) return;
			vm.IsHelpPageOpen = false;
		}
	}
}
