using Project;
using UnityEngine;
using UnityEngine.UI;

namespace HelloPico2{
	public class AudioPosterExample : MonoBehaviour{
		[SerializeField] private InputField inputField;
		[SerializeField] private Button button;

		private void Start(){
			button.onClick.AddListener(OnButtonClick);
		}

		private void OnButtonClick(){
			var inputFieldText = inputField.text;
			EventBus.Post(new AudioEventRequested(inputFieldText, transform.position));
		}
	}
}