using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.Extensions;

namespace Wordle {
	public class AssistantPanel : MonoBehaviour {
		private static AssistantPanel instance { get; set; }

		[SerializeField] protected Transform  _assistsContainer;
		[SerializeField] protected AssistLine _assistLinePrefab;

		private List<AssistLine> assists { get; } = new List<AssistLine>();

		private void Awake() {
			instance = this;
		}

		public static void Clear() {
			instance._assistsContainer.ClearChildren();
			instance.assists.Clear();
		}

		public static void NextAssist(string proposition, float confidenceLevel) {
			instance.assists.Add(Instantiate(instance._assistLinePrefab, instance._assistsContainer));
			GetCurrentAssist().Set(proposition, confidenceLevel);
		}

		public static AssistLine GetCurrentAssist() => instance.assists.Last();
	}
}