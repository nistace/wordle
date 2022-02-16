using System;

namespace Wordle.Assistants {
	public class Assistant {
		public enum Type {
			PossibilityRemover = 0,
			Score              = 1
		}

		public static IAssistant Create(Type type) {
			switch (type) {
				case Type.PossibilityRemover: return new PossibilityRemoverAssistant();
				case Type.Score: return new ScoreAssistant();
				default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
			}
		}
	}
}