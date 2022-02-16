using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Utils.Extensions;

namespace Wordle {
	public class SuccessLine : MonoBehaviour {
		[SerializeField] protected TMP_Text _successText;
		[SerializeField] protected Button   _newGameButton;

		public string text {
			get => _successText.text;
			set => _successText.text = value;
		}

		public static UnityEvent onNewGameClicked { get; } = new UnityEvent();

		public void Start() {
			_newGameButton.onClick.AddListenerOnce(onNewGameClicked.Invoke);
		}
	}
}