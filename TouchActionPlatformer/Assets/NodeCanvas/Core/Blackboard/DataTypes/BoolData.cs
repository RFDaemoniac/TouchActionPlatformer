using UnityEngine;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class BoolData : VariableData{

		public bool value;

		public override object objectValue{
			get {return value;}
			set
			{
				if ((bool)value != this.value){
					this.value = (bool)value;
					OnValueChanged(value);
				}
			}
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////

		#if UNITY_EDITOR
		override public void ShowDataGUI(){
			objectValue = UnityEditor.EditorGUILayout.Toggle(value, layoutOptions);
		}

		#endif
	}
}