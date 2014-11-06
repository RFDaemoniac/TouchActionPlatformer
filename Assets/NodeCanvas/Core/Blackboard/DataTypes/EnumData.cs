using UnityEngine;
using System;
using System.Linq;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class EnumData : VariableData {

		enum DefaultEnum
		{
			One,
			Two,
			Three,
			Four,
			Five
		}

		public Enum value;
		public string stringValue;


		[SerializeField]
		private string _typeName = typeof(DefaultEnum).AssemblyQualifiedName;

		public override System.Type varType{
			get {return type;}
		}

		private System.Type type{
			get {return System.Type.GetType(_typeName);}
			set
			{
				_typeName = value.AssemblyQualifiedName;
				stringValue = Enum.GetNames(value)[0];
			}
		}

		public override object objectValue{
			get {return Enum.Parse(type, stringValue);}
			set
			{
				if (!Equals(this.value, value)){
					if (value != null && typeof(Enum).NCIsAssignableFrom(value.GetType()) && type != value.GetType() )
						type = value.GetType();

					if (value.GetType() == typeof(string)){
						if (Enum.GetNames(type).Contains(value)){
							stringValue = (string)value;
						} else {
							Debug.LogError(string.Format("{0} is not a valid name of the {1} enum type", value, type));
						}
					} else {
						stringValue = Enum.GetName(type, value);
						this.value = (Enum)value;
					}

					OnValueChanged(value);
				}
			}
		}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		void Reset(){
			type = typeof(DefaultEnum);
		}

		void OnValidate(){
			if (type == null || type == typeof(Enum))
				type = typeof(DefaultEnum);
		}

		public override void ShowDataGUI(){

			objectValue = (Enum)UnityEditor.EditorGUILayout.EnumPopup((Enum)objectValue, layoutOptions);

			if (GUILayout.Button("T", GUILayout.Width(10), GUILayout.Height(14))){
				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Enum), delegate(System.Type t){
					type = t;
				}, false);
			}
		}

		#endif
	}
}