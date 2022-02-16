using System.Collections.Generic;
using System.Linq;
using Utils.Extensions;

namespace Wordle.Assistants {
	public class PossibilityRemoverAssistant : IAssistant {
		private HashSet<string> remainingPossibilities { get; } = new HashSet<string>();

		public void Init(IReadOnlyCollection<string> allPossibilities) {
			remainingPossibilities.Clear();
			remainingPossibilities.AddAll(allPossibilities);
		}

		public void ApplyResult(string answer, IReadOnlyList<LetterValidity> result) {
			for (var i = 0; i < result.Count; ++i) {
				if (result[i] == LetterValidity.Correct) {
					remainingPossibilities.RemoveWhere(t => t[i] != answer[i]);
				}
				else if (result[i] == LetterValidity.Incorrect) {
					remainingPossibilities.RemoveWhere(t => t.Contains(answer[i]));
				}
				else if (result[i] == LetterValidity.WrongPosition) {
					remainingPossibilities.RemoveWhere(t => !t.Contains(answer[i]));
					remainingPossibilities.RemoveWhere(t => t[i] == answer[i]);
				}
			}
		}

		public string GetNextBestOption(out float score) {
			score = 1f / remainingPossibilities.Count;
			return remainingPossibilities.Random();
		}
	}
}