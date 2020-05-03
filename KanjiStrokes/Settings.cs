using System.Configuration;
using System.Runtime.CompilerServices;

namespace KanjiStrokes {
	public class Settings : ApplicationSettingsBase {

		private T Get<T>([CallerMemberName] string propertyName = null) => (T)this[propertyName];
		private void Set<T>(T value, [CallerMemberName] string propertyName = null) => this[propertyName] = value;

		[UserScopedSetting]
		[DefaultSettingValue("True")]
		public bool DarkMode { get => Get<bool>(); set => Set(value); }
		
		[UserScopedSetting]
		[DefaultSettingValue("True")]
		public bool FirstStart { get => Get<bool>(); set => Set(value); }
	}
}
