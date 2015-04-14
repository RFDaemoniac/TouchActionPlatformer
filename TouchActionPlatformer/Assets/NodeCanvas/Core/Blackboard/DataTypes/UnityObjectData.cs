using UnityEngine;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class UnityObjectData : VariableData {

		public Object value;

		[SerializeField]
		private string _typeName = typeof(Object).AssemblyQualifiedName;

		private System.Type type{
			get {return value != null? value.GetType() : System.Type.GetType(_typeName); }
			set
			{
				_typeName = value.AssemblyQualifiedName;
				if (this.value != null && !value.NCIsAssignableFrom(this.value.GetType()))
					this.value = null;
			}
		}

		public override System.Type varType{
			get {return type;}
		}

		public override object objectValue{
			get {return value;}
			set
			{
				if (this.value != (Object)value){
					this.value = (Object)value;
					if (value != null && !type.NCIsAssignableFrom(value.GetType()))
						type = value.GetType();
					OnValueChanged(value);
				}
			}
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR

		void OnValidate(){
			if (type == null)
				type = typeof(Object);
		}

		void Reset(){
			type = typeof(Object);
		}

		public override void ShowDataGUI(){
			
			objectValue = (Object)UnityEditor.EditorGUILayout.ObjectField((Object)objectValue, varType, true, layoutOptions) as Object;

			if (GUILayout.Button("T", GUILayout.Width(10), GUILayout.Height(14)))
				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Object), delegate(System.Type t){type = t;} );
		}

		#endif
	}
}