using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Windows;

namespace KanjiStrokes {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {
#if NETCOREAPP
		static App() {
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
		}
#endif

		public static bool IsInDesignMode => DesignerProperties.GetIsInDesignMode(Current.MainWindow ?? new DependencyObject());

		public static ResourceManager ResourceManager = new ResourceManager("KanjiStrokes.g", Assembly.GetExecutingAssembly());

		public static Stream OpenResource(string name) => ResourceManager.GetStream(name, CultureInfo.InvariantCulture);
	}
}
