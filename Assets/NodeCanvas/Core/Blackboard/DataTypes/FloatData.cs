using UnityEngine;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class FloatData : VariableData{

		public float value;

		public override object objectValue{
			get {return value;}
			set
			{
				if ((float)value != this.value){
					this.value = (float)value;
					OnValueChanged(value);
				}
			}
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR

		public override void ShowDataGUI(){
			GUI.backgroundColor = new Color(0.7f,0.7f,1);
			objectValue = UnityEditor.EditorGUILayout.FloatField(value, layoutOptions);
		}

		#endif
	}
}