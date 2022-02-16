using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Events;
using Utils.Extensions;

namespace Wordle {
	public class AssistLine : MonoBehaviour {
		[SerializeField] protected Button   _copyOptionButton;
		[SerializeField] protected TMP_Text _text;

		private string proposition { get; set; }

		public static StringEvent onCopyButtonClicked { get; } = new StringEvent();

		private void Start() {
			_copyOptionButton.onClick.AddListenerOnce(() => onCopyButtonClicked.Invoke(proposition));
		}

		public void Set(string proposition, float confidenceLevel) {
			this.proposition = proposition;
			_text.text = $"I would play <#00FF00>{proposition}</color>, I'm confident it is the answer at {confidenceLevel:0%}";
		}

		public void Close() {
			_copyOptionButton.gameObject.SetActive(false);
		}
	}
}