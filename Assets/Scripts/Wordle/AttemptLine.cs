using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Extensions;
using Utils.Libraries;

namespace Wordle {
	public class AttemptLine : MonoBehaviour {
		[SerializeField] protected Button        _validateButton;
		[SerializeField] protected TMP_Text      _attemptTitle;
		[SerializeField] protected LetterInput   _letterInputPrefab;
		[SerializeField] protected RectTransform _letterInputContainer;
		[SerializeField] protected float         _letterWidth = 48;
		[SerializeField] protected float         _spaceWidth  = 12;

		private List<LetterInput> letterInputs { get; } = new List<LetterInput>();

		public bool       complete   => letterInputs.All(t => !string.IsNullOrEmpty(t.letter));
		public UnityEvent onValidate => _validateButton.onClick;

		public void Create(int attemptIndex, int letterCount) {
			_attemptTitle.color = Colors.Of("attempt.current");
			_attemptTitle.text = $"Attempt {attemptIndex + 1}";
			var width = letterCount * _letterWidth + _spaceWidth * (letterCount + 1);
			_letterInputContainer.offsetMin = new Vector2(-width / 2, 0);
			_letterInputContainer.offsetMax = new Vector2(width / 2, 0);

			for (var i = 0; i < letterCount; ++i) {
				letterInputs.Add(Instantiate(_letterInputPrefab, _letterInputContainer));
			}
			letterInputs.ForEach(t => {
				t.SetValidity(LetterValidity.Unchecked);
				t.letter = string.Empty;
			});
			_validateButton.interactable = false;
		}

		public void SetInput(string input, IReadOnlyList<LetterValidity> helperValidity, bool validAttempt) {
			letterInputs.ForEach((t, i) => letterInputs[i].letter = input.Length > i ? $"{input[i]}" : "");
			_validateButton.interactable = validAttempt;
			helperValidity.ForEach((t, i) => letterInputs[i].SetValidity(t));
		}

		public void Close(LetterValidity[] result) {
			_attemptTitle.color = Colors.Of("attempt.closed");
			result.ForEach((t, i) => letterInputs[i].SetValidity(t));
			_validateButton.gameObject.SetActive(false);
		}
	}
}