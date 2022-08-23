using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Tools", menuName = "Tools/PreviewWindows Demo")]
public class CustomPreviewWindows : ScriptableObject{
	public List<PiceInfo> PiecsInfo;
}
#if UNITY_EDITOR
[CustomPreview(typeof(CustomPreviewWindows))]
public class LanguageImageSwitcherPreview : ObjectPreview{
	CustomPreviewWindows displayLanguage;

	public override bool HasPreviewGUI(){
		return true;
	}

	public override GUIContent GetPreviewTitle(){
		return new GUIContent("Editor");
	}

	public override void OnInteractivePreviewGUI(Rect r, GUIStyle background){
		int D = 50;
		int hintSize = 10;
		var target = this.target as CustomPreviewWindows;
		//        return;
		Vector2 centerPos = new Vector2(r.x + r.width / 2 - D / 2, r.y + r.height / 2 - D / 2);

		Dictionary<Vector2Int, PiceInfo> piceInfos = new Dictionary<Vector2Int, PiceInfo>();

		foreach(var info in target.PiecsInfo){
			if(!piceInfos.ContainsKey(info.Pos)) piceInfos.Add(info.Pos, info);
		}

		Dictionary<Element, Color> elementHintPos = new Dictionary<Element, Color>();
		elementHintPos.Add(Element.Fire, Color.red);
		elementHintPos.Add(Element.Magic, Color.green);
		elementHintPos.Add(Element.Rock, Color.white);
		elementHintPos.Add(Element.Thunder, Color.yellow);
		elementHintPos.Add(Element.Water, Color.cyan);

		List<Vector2> hintPos = new List<Vector2>();
		hintPos.Add(new Vector2(D - hintSize, D - hintSize));
		hintPos.Add(new Vector2(D - hintSize * 2, D - hintSize));
		hintPos.Add(new Vector2(D - hintSize * 3, D - hintSize));
		hintPos.Add(new Vector2(D - hintSize * 4, D - hintSize));
		hintPos.Add(new Vector2(D - hintSize * 5, D - hintSize));

		for(int x = -2; x < 3; x++){
			for(int y = -2; y < 3; y++){
				var pos = new Vector2Int(x, y);
				Rect t = new Rect(centerPos.x + x * D, centerPos.y - y * D, D, D);
				if(piceInfos.ContainsKey(pos)){
					PiceInfo info = piceInfos[pos];
					EditorGUI.DrawRect(t, Color.gray);
					t.width = 20;
					t.height = 20;
					if(EditorGUI.DropdownButton(t, new GUIContent(""), FocusType.Keyboard)){
						List<string> alls = new List<string>();
						foreach(string name in Enum.GetNames(typeof(Element))){
							alls.Add(name);
						}

						GenericMenu _menu = new GenericMenu();
						for(int i = 0; i < alls.Count; i++){
							_menu.AddItem(new GUIContent(alls[i]), info.IsSelectEnumType((Element)i), (e) => {
								if(Enum.TryParse(e.ToString(), out Element myStatus)){
									int index = 1 << (int)myStatus;
									info.enumType = info.enumType ^ (Element)index;
									if((int)info.enumType == 0) target.PiecsInfo.Remove(info);
								}
							}, alls[i]);
						}

						_menu.ShowAsContext();
					}

					int hintCount = 0;
					foreach(var hintInfo in elementHintPos){
						if(info.IsSelectEnumType(hintInfo.Key)){
							var addPos = hintPos[hintCount];
							Rect tt = new Rect(t.x + addPos.x, t.y + addPos.y, 10, 10);
							EditorGUI.DrawRect(tt, hintInfo.Value);
							hintCount++;
						}
					}
				}
				else{
					t.x += D / 2;
					t.width = D / 2;
					t.height = D / 2;
					if(EditorGUI.Toggle(t, false)){
						target.PiecsInfo.Add(new PiceInfo(new Vector2Int(x, y), (Element)1));
					}
				}
			}
		}
	}
}

#endif

[Serializable]
public class PiceInfo{
	[EnumMultiAttribute] public Element enumType;
	public Vector2Int Pos;

	public PiceInfo(Vector2Int pos, Element element){
		Pos = pos;
		enumType = element;
	}

	public bool IsSelectEnumType(Element type){
		int index = 1 << (int)type;
		int eventTypeResult = (int)enumType;
		return (eventTypeResult & index) == index;
	}
}

public class EnumMultiAttribute : PropertyAttribute{ }

[CustomPropertyDrawer(typeof(EnumMultiAttribute))]
public class EnumMultiAttributeDrawer : PropertyDrawer{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label){
		property.intValue = EditorGUI.MaskField(position, label, property.intValue, property.enumNames);
	}
}

public enum Element{
	Space,
	Fire,
	Water,
	Rock,
	Thunder,
	Magic
}