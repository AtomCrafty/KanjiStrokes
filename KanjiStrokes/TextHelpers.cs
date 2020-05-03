using System.Collections.Generic;
using System.Linq;

namespace KanjiStrokes {
	public static class TextHelpers {

		public static bool IsCharInRange(char ch, int min, int max) => ch >= min && ch <= max;
		public static bool IsHiragana(char ch) => IsCharInRange(ch, 0x3040, 0x309F);
		public static bool IsKatakana(char ch) => IsCharInRange(ch, 0x30A0, 0x30FF);
		public static bool IsKanji(char ch) => IsCharInRange(ch, 0x4E00, 0x9FBF);

		public static IEnumerable<char> GetCharsInRange(string text, int min, int max) => text.Where(ch => IsCharInRange(ch, min, max));
		public static IEnumerable<char> FindHiragana(string text) => text.Where(IsHiragana);
		public static IEnumerable<char> FindKatakana(string text) => text.Where(IsKatakana);
		public static IEnumerable<char> FindKanji(string text) => text.Where(IsKanji);
	}
}
