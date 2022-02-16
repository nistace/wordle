using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Extensions;

namespace Wordle {
	public class GamePanel : MonoBehaviour {
		private static GamePanel instance { get; set; }

		[SerializeField] protected Transform   _attemptsListContainer;
		[SerializeField] protected AttemptLine _attemptLinePrefab;
		[SerializeField] protected SuccessLine _successLinePrefab;

		private List<AttemptLine> attempts { get; } = new List<AttemptLine>();

		private void Awake() {
			instance = this;
		}

		public static void Clear() {
			instance._attemptsListContainer.ClearChildren();
			instance.attempts.Clear();
		}

		public static void CloseCurrentAttempt(LetterValidity[] result) => GetCurrentAttempt().Close(result);

		public static void SetSuccess(string foundWord, int attempts) =>
			Instantiate(instance._successLinePrefab, instance._attemptsListContainer).text = "Success! You found " + foundWord + " in " + attempts + " attempts!";

		public static void NextAttempt(int attemptIndex, int wordLength) {
			instance.attempts.Add(Instantiate(instance._attemptLinePrefab, instance._attemptsListContainer));
			GetCurrentAttempt().Create(attemptIndex, wordLength);
		}

		public static AttemptLine GetCurrentAttempt() => instance.attempts.Last();
	}
}