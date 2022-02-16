using System.Collections.Generic;

namespace Wordle.Assistants {
	public interface IAssistant {
		void Init(IReadOnlyCollection<string> allPossibilities);
		void ApplyResult(string answer, IReadOnlyList<LetterValidity> result);
		string GetNextBestOption(out float score);
	}
}