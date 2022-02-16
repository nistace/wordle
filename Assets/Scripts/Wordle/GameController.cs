using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using Utils.Extensions;
using Utils.Libraries;
using Wordle.Assistants;

namespace Wordle {
	public class GameController : MonoBehaviour {
		private HashSet<string> dictionary         { get; } = new HashSet<string>();
		private HashSet<char>   excludedCharacters { get; } = new HashSet<char>();

		[SerializeField] protected TextAsset      _txtFile;
		[SerializeField] protected int            _wordLength = 5;
		[SerializeField] protected Assistant.Type _assistantType;

		private int        attemptIndex        { get; set; }
		private string     currentAttemptInput { get; set; }
		private string     currentWord         { get; set; }
		private IAssistant assistant           { get; set; }

		private void Start() {
			assistant = Assistant.Create(_assistantType);
			dictionary.Clear();
			dictionary.AddAll(_txtFile.Lines().Select(t => t.ToUpper().Trim()).Where(t => Regex.IsMatch(t, $"^[a-zA-Z]{{{_wordLength}}}$") && t.Distinct().Count() == _wordLength).Distinct());
			Colors.LoadLibrary(Resources.LoadAll<ColorLibrary>("Libraries").First());
			Debug.Log($"{dictionary.Count} words in dictionary");
			StartNewGame();
			SuccessLine.onNewGameClicked.AddListenerOnce(StartNewGame);
			AssistLine.onCopyButtonClicked.AddListenerOnce(ChangeCurrentAttemptInput);
		}

		private void StartNewGame() {
			StopAllCoroutines();
			excludedCharacters.Clear();
			GamePanel.Clear();
			AssistantPanel.Clear();
			currentWord = dictionary.Random();
			assistant.Init(dictionary);
			attemptIndex = 0;
			currentAttemptInput = string.Empty;
			StartNextAttempt();
		}

		private void StartNextAttempt() {
			var bestOption = currentAttemptInput = string.Empty;
			AssistantPanel.NextAssist(assistant.GetNextBestOption(out var score), score);
			Debug.Log($"I think that the answer is {bestOption} with score {score:0%}");
			GamePanel.NextAttempt(attemptIndex, _wordLength);
			StartCoroutine(ListenWordInput());
			GamePanel.GetCurrentAttempt().onValidate.AddListenerOnce(ValidateAttempt);
		}

		private void ValidateAttempt() {
			GamePanel.GetCurrentAttempt().SetInput(currentAttemptInput, new LetterValidity[0], dictionary.Contains(currentAttemptInput));
			StopAllCoroutines();
			var result = currentAttemptInput.Select(CheckLetter).ToArray();
			GamePanel.CloseCurrentAttempt(result);
			AssistantPanel.GetCurrentAssist().Close();
			attemptIndex++;
			if (result.Count(t => t == LetterValidity.Correct) == _wordLength) {
				GamePanel.SetSuccess(currentWord, attemptIndex);
			}
			else {
				excludedCharacters.AddAll(currentAttemptInput.Where((t, i) => result[i] == LetterValidity.Incorrect));
				assistant.ApplyResult(currentAttemptInput, result);
				StartNextAttempt();
			}
		}

		private LetterValidity CheckLetter(char inputCharacter, int index) {
			if (inputCharacter == currentWord[index]) return LetterValidity.Correct;
			if (currentWord.Contains(inputCharacter)) return LetterValidity.WrongPosition;
			return LetterValidity.Incorrect;
		}

		private LetterValidity[] HelpWithExcluded() =>
			_wordLength.CreateArray(i => i < currentAttemptInput.Length && excludedCharacters.Contains(currentAttemptInput[i]) ? LetterValidity.Excluded : LetterValidity.Unchecked);

		private IEnumerator ListenWordInput() {
			while (!Input.GetKeyDown(KeyCode.Space) || !dictionary.Contains(currentAttemptInput)) {
				if (currentAttemptInput.Length > 0 && Input.GetKeyDown(KeyCode.Backspace)) {
					ChangeCurrentAttemptInput(currentAttemptInput.Substring(0, currentAttemptInput.Length - 1));
				}
				if (currentAttemptInput.Length < _wordLength && !string.IsNullOrEmpty(Input.inputString) && Regex.IsMatch(Input.inputString, "^[a-zA-Z]$") &&
					!currentAttemptInput.Contains(Input.inputString.ToUpper())) {
					ChangeCurrentAttemptInput(currentAttemptInput + Input.inputString.ToUpper());
				}
				yield return null;
			}
			ValidateAttempt();
		}

		private void ChangeCurrentAttemptInput(string newValue) {
			currentAttemptInput = newValue;
			GamePanel.GetCurrentAttempt().SetInput(currentAttemptInput, HelpWithExcluded(), dictionary.Contains(currentAttemptInput));
		}
	}
}