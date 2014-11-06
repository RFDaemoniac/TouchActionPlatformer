using UnityEngine;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class IntData : VariableData{

		public int value;

		public override object objectValue{
			get {return value;}
			set
			{
				if ((int)value != this.value){
					this.value = (int)value;
					OnValueChanged(value);
				}
			}
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR

		public override void ShowDataGUI(){
			GUI.backgroundColor = new Color(0.7f,1,0.7f);
			objectValue = UnityEditor.EditorGUILayout.IntField(value, layoutOptions);
		}

		#endif
	}
}