using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Libraries;

namespace Wordle {
	public class LetterInput : MonoBehaviour {
		[SerializeField] protected Image    _background;
		[SerializeField] protected TMP_Text _letterText;

		public string letter {
			get => _letterText.text;
			set => _letterText.text = value;
		}

		public void SetValidity(LetterValidity validity) {
			_background.color = Colors.Of($"letter.validity.{validity}");
		}
	}
}