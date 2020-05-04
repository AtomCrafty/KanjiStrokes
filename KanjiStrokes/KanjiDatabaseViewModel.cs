using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KanjiStrokes {

	public sealed class KanjiInfo {
		public string Literal { get; }

		public int Grade { get; }
		public int JlptLevel { get; }
		public int Frequency { get; }
		public int StrokeCount { get; }

		public string ReadingOn { get; }
		public string ReadingKun { get; }
		public ImmutableArray<string> Nanori { get; }
		public ImmutableArray<string> Translations { get; }

		public ImmutableArray<char> Radicals { get; }

		public KanjiInfo(string literal, int grade, int jlptLevel, int frequency, int strokeCount,
			string readingOn, string readingKun, ImmutableArray<string> nanori,
			ImmutableArray<string> translations, ImmutableArray<char> radicals) {
			Literal = literal;
			Grade = grade;
			JlptLevel = jlptLevel;
			Frequency = frequency;
			StrokeCount = strokeCount;
			ReadingOn = readingOn;
			ReadingKun = readingKun;
			Nanori = nanori;
			Translations = translations;
			Radicals = radicals;
		}
	}

	public class KanjiDatabaseViewModel : ViewModelBase {
		public KanjiDatabaseViewModel() {
			if(App.IsInDesignMode) return;
			Task.Run(Load).ConfigureAwait(false);
		}

		private LoadingStatus _status = LoadingStatus.Loading;

		public LoadingStatus Status {
			get => _status;
			protected set {
				if(value == _status) return;
				_status = value;
				OnPropertyChanged();
			}
		}

		protected async Task Load() {
			await LoadKanjiData();
			Status = LoadingStatus.Ready;
		}

		public ImmutableDictionary<string, KanjiInfo> KanjiData { get; private set; } = ImmutableDictionary<string, KanjiInfo>.Empty;

		private async Task LoadKanjiData() {
			try {
				var radicalData = await LoadRadicalData();

				await using var stream = new GZipStream(App.OpenResource("resources/database/kanjidic2.xml.gz"), CompressionMode.Decompress);

				KanjiInfo ProcessCharacterNode(XElement node) {
					try {
						string literal = node?.Element("literal")?.Value;

						if(literal.Length != 1) return null;

						var misc = node?.Element("misc");
						int grade = int.Parse(misc?.Element("grade")?.Value ?? "-1");
						int jltpLevel = int.Parse(misc?.Element("jlpt")?.Value ?? "-1");
						int frequency = int.Parse(misc?.Element("freq")?.Value ?? "-1");
						int strokeCount = int.Parse(misc?.Element("stroke_count")?.Value ?? "-1");

						var rm = node?.Element("reading_meaning");
						var rmg = rm?.Element("rmgroup");
						string readingOn = rmg?.Elements("reading").FirstOrDefault(elem => elem.Attribute("r_type")?.Value == "ja_on")?.Value;
						string readingKun = rmg?.Elements("reading").FirstOrDefault(elem => elem.Attribute("r_type")?.Value == "ja_kun")?.Value;
						var translations = rmg?.Elements("meaning").Where(elem => elem.Attribute("m_lang") == null).Select(elem => elem.Value).ToImmutableArray() ?? ImmutableArray<string>.Empty;
						var nanori = rm?.Elements("nanori").Select(elem => elem.Value).ToImmutableArray() ?? ImmutableArray<string>.Empty;

						var radicals = radicalData.ContainsKey(literal[0])
							? radicalData[literal[0]].Except(new[] { ' ' }).ToImmutableArray()
							: ImmutableArray<char>.Empty;

						return new KanjiInfo(literal, grade, jltpLevel, frequency, strokeCount, readingOn, readingKun, nanori, translations, radicals);
					}
					catch(Exception e) {
						Console.WriteLine(e);
						throw;
					}
				}

				var root = await XElement.LoadAsync(stream, LoadOptions.None, CancellationToken.None);
				var header = root.Element("header");
				var charNodes = root.Elements("character");
				var kanjiInfo = charNodes.Select(ProcessCharacterNode).Where(info => info != null);

				KanjiData = kanjiInfo.ToImmutableDictionary(info => info.Literal);
			}
			catch(Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		protected async Task<Dictionary<char, string>> LoadRadicalData() {
			try {
				await using var stream = App.OpenResource("resources/database/kradfile");
				using var reader = new StreamReader(stream, Encoding.GetEncoding(20932 /* EUC-JP */));

				var kanjiToRadicals = new Dictionary<char, string>();
				string line;
				while((line = await reader.ReadLineAsync()) != null) {
					line = line.Trim();
					if(line.StartsWith('#')) continue;
					Debug.Assert(line.Length >= 5);
					Debug.Assert(line.Substring(1, 3) == " : ");
					char kanji = line[0];
					string radicals = line.Substring(4);
					kanjiToRadicals.Add(kanji, radicals);
				}
				return kanjiToRadicals;
			}
			catch(Exception e) {
				Console.WriteLine(e);
				throw;
			}
		}

		public enum LoadingStatus {
			Loading, Ready
		}

		public KanjiInfo? GetKanjiInfo(in char kanji) =>
			KanjiData.TryGetValue(kanji.ToString(), out var info) ? info : null;

		public ImmutableArray<char> GetRadicals(in char kanji) =>
			GetKanjiInfo(kanji)?.Radicals ?? ImmutableArray<char>.Empty;
	}
}
