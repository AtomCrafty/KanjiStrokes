using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace KanjiStrokes {
	/// <summary>
	/// Interaction logic for IconButton.xaml
	/// </summary>
	public partial class IconButton : Button {

		public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon), typeof(PathGeometry), typeof(IconButton));

		public PathGeometry Icon {
			get => (PathGeometry)GetValue(IconProperty);
			set => SetValue(IconProperty, value);
		}

		public static readonly DependencyProperty AlternateIconProperty = DependencyProperty.Register(nameof(AlternateIcon), typeof(PathGeometry), typeof(IconButton));

		public PathGeometry AlternateIcon {
			get => (PathGeometry)GetValue(AlternateIconProperty);
			set => SetValue(AlternateIconProperty, value);
		}

		public static readonly DependencyProperty UseAlternateIconProperty = DependencyProperty.Register(nameof(UseAlternateIcon), typeof(bool), typeof(IconButton));

		public bool UseAlternateIcon {
			get => (bool)GetValue(UseAlternateIconProperty);
			set => SetValue(UseAlternateIconProperty, value);
		}

		public IconButton() {
			InitializeComponent();
		}
	}
}
