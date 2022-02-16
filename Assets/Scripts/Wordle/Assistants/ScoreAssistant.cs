using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace Wordle.Assistants {
	public class ScoreAssistant : IAssistant {
		private HashSet<string>                        dictionary         { get; } = new HashSet<string>();
		private Dictionary<int, Dictionary<char, int>> charPositionScores { get; } = new Dictionary<int, Dictionary<char, int>>();

		public void Init(IReadOnlyCollection<string> allPossibilities) {
			dictionary.Clear();
			dictionary.AddAll(allPossibilities);
			charPositionScores.Clear();
			LoadCharPositionScores();
		}

		private void LoadCharPositionScores() {
			charPositionScores.Clear();
			foreach (var word in dictionary) {
				for (var i = 0; i < word.Length; ++i) {
					if (!charPositionScores.ContainsKey(i)) charPositionScores.Add(i, new Dictionary<char, int>());
					if (!charPositionScores[i].ContainsKey(word[i])) charPositionScores[i].Add(word[i], 0);
					charPositionScores[i][word[i]]++;
				}
			}
		}

		public void ApplyResult(string answer, IReadOnlyList<LetterValidity> result) {
			for (var i = 0; i < result.Count; ++i) {
				if (result[i] == LetterValidity.Correct) {
					foreach (var c in charPositionScores[i].Keys.Where(c => c != answer[i]).ToArray()) {
						charPositionScores[i][c] = 0;
					}
				}
				else if (result[i] == LetterValidity.Incorrect) {
					foreach (var t in charPositionScores.Values.Where(t => t.ContainsKey(answer[i]))) {
						t[answer[i]] = 0;
					}
				}
				else if (result[i] == LetterValidity.WrongPosition) {
					charPositionScores[i][answer[i]] = 0;
					foreach (var position in charPositionScores.Values.Where(position => position.ContainsKey(answer[i]))) {
						position[answer[i]] *= 2;
					}
				}
			}
		}

		public string GetNextBestOption(out float score) {
			var sumScore = 0f;
			var bestWord = string.Empty;
			var bestScore = 0f;
			foreach (var word in dictionary) {
				var wordScore = word.Select((t, i) => (float) charPositionScores[i][t]).Aggregate((t, u) => t * u);
				sumScore += wordScore;
				if (wordScore > bestScore) {
					bestScore = wordScore;
					bestWord = word;
				}
			}
			score = bestScore / sumScore;
			return bestWord;
		}
	}
}