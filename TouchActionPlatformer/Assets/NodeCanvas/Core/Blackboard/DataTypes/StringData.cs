using UnityEngine;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class StringData : VariableData{

		public string value = string.Empty;

		public override object objectValue{
			get {return value;}
			set
			{
				if (this.value != (string)value){
					this.value = (string)value;
					OnValueChanged(value);
				}
			}
		}


		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR	

		public override void ShowDataGUI(){
			GUI.backgroundColor = new Color(0.5f,0.5f,0.5f);
			objectValue = (string)UnityEditor.EditorGUILayout.TextField((string)objectValue, layoutOptions);
		}

		#endif
	}
}