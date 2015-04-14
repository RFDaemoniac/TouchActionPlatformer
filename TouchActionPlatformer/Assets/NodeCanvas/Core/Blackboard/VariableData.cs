using UnityEngine;
using System.Reflection;
using System.Collections;

namespace NodeCanvas.Variables{
	
	///Variables are mostly stored in Blackboard. Derived classes of this store the correct type respectively in a public value named variable
	abstract public class VariableData : MonoBehaviour{

		public string dataName;
		public event System.Action<string, object> onValueChanged;

		private System.Type _type;
		private FieldInfo _valueField;
		private object currentValue;

		new public string name{
			get {return dataName;}
		}

		///The Type this Variable holds
		virtual public System.Type varType{
			get
			{
				if (_type == null)
					_type = GetType().NCGetField("value").FieldType;
				return _type;
			}
		}

		///The System.Object value of the contained variable
		virtual public object objectValue{
			get
			{
				if (_valueField == null)
					_valueField = GetType().NCGetField("value");
				return _valueField.GetValue(this);
			}
			set
			{
				if (currentValue != value){
					if (_valueField == null)
						_valueField = GetType().NCGetField("value");
					_valueField.SetValue(this, value);
					currentValue = value;
					OnValueChanged(value);
				}
			}
		}

		protected void OnValueChanged(object value){
			if (onValueChanged != null)
				onValueChanged(dataName, value);
		}

		//Used when saving to get the object information
		virtual public object GetSerialized(){
			return objectValue;
		}

		//Used when loading to set the object information
		virtual public void SetSerialized(object obj){
			objectValue = obj;
		}

		public override string ToString(){
			return objectValue != null? objectValue.ToString() : "NULL";
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR

		protected static GUILayoutOption[] layoutOptions = new GUILayoutOption[]{GUILayout.MaxWidth(100), GUILayout.ExpandWidth(true)};

		virtual public void ShowDataGUI(){

			var field = this.GetType().GetField("value");
			if (field == null){
				GUILayout.Label("No public field 'value' found");
				return;
			}

			if (varType == typeof(object)){
				GUILayout.Label("(System.Object)", layoutOptions);
				return;
			}

			if (typeof(Object).IsAssignableFrom(varType)){
				objectValue = (Object)UnityEditor.EditorGUILayout.ObjectField((Object)objectValue, varType, true, layoutOptions);
				return;
			}

			var value = field.GetValue(this);
			var actualType = value != null? value.GetType() : varType;

			if (actualType.IsAbstract){
				GUILayout.Label( string.Format("({0})", EditorUtils.TypeName(actualType)), layoutOptions );
				return;
			}

			if (!varType.IsValueType){
	
				var isList = typeof(IList).IsAssignableFrom(varType) && value != null && !varType.IsArray;
				if (GUILayout.Button("(" + EditorUtils.TypeName(actualType) + ")" + (isList? (value as IList).Count.ToString() : ""), layoutOptions))
					NodeCanvasEditor.PopObjectEditor.Show(value, actualType);
				return;
			}

			//objectValue = EditorUtils.GenericField(null, objectValue, objectValue.GetType());
			GUILayout.Label(string.Format("({0})", EditorUtils.TypeName(varType) ), layoutOptions);
		}

		#endif
	}
}