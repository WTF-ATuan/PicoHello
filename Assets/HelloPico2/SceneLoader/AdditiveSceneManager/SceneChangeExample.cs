using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Project.AdditiveSceneManager{
	public class SceneChangeExample : MonoBehaviour{
		private Button _button;
		private Text _text;

		[SerializeField] private bool isActivate;
		[SerializeField] [Required] private SceneController controller;
		[SerializeField] private Text stateText;

		private void Start(){
			stateText.text = isActivate ? "Activate" : "Deactivate";
			stateText.color = isActivate ? Color.green : Color.red;
			_button = GetComponent<Button>();
			_text = GetComponentInChildren<Text>();
			_button.onClick.AddListener(OnClick);
		}

		private void OnClick(){
			if(isActivate){
				controller.UnloadLevels(_text.text);
				isActivate = false;
				stateText.text = "Deactivate";
				stateText.color = Color.red;
			}
			else{
				controller.LoadLevel(_text.text);
				isActivate = true;
				stateText.text = "Activate";
				stateText.color = Color.green;
			}
		}
	}
}