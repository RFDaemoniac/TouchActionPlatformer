#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Script Control")]
	[Description("Check a field on a script and return if it's equal or not to a value")]
	public class CheckField : ConditionTask {

		public BBVariableSet checkSet = new BBVariableSet();

		[SerializeField]
		private string scriptName = typeof(Component).AssemblyQualifiedName;
		[SerializeField]
		private string fieldName;

		[SerializeField]
		private CompareMethod comparison;

		private FieldInfo field;

		public override System.Type agentType{
			get {return System.Type.GetType(scriptName);}
		}

		protected override string info{
			get
			{
				if (string.IsNullOrEmpty(fieldName))
					return "No Field Selected";
				return string.Format("{0}.{1}{2}", agentInfo, fieldName, checkSet.selectedType == typeof(bool)? "" : TaskTools.GetCompareString(comparison) + checkSet.ToString());
			}
		}

		//store the field info on agent set for performance
		protected override string OnInit(){
			field = agent.GetType().NCGetField(fieldName);
			if (field == null)
				return "Missing Field Info";
			return null;
		}

		//do it by invoking field
		protected override bool OnCheck(){

			if (checkSet.selectedType == typeof(float) || checkSet.selectedType == typeof(int))
				return TaskTools.Compare( (System.IComparable)field.GetValue(agent), (System.IComparable)checkSet.objectValue, comparison );

			return Equals( field.GetValue(agent), checkSet.objectValue );
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR


		/////UPDATING
		protected override void OnEditorValidate(){
			if (agentType == null)
				scriptName = EditorUtils.GetType(scriptName, typeof(Component)).AssemblyQualifiedName;
		}
		///////	
		
		protected override void OnTaskInspectorGUI(){

			if (!Application.isPlaying && agent == null && GUILayout.Button("Alter Type")){
				System.Action<System.Type> TypeSelected = delegate(System.Type t){
					var newName = t.AssemblyQualifiedName;
					if (newName != scriptName){
						scriptName = newName;
						fieldName = null;
					}
				};

				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Component), TypeSelected);
			}

			if (!Application.isPlaying && GUILayout.Button("Select Field")){
				System.Action<FieldInfo> FieldSelected = delegate(FieldInfo field){
					scriptName = field.DeclaringType.AssemblyQualifiedName;
					fieldName = field.Name;
					checkSet.selectedType = field.FieldType;
					comparison = CompareMethod.EqualTo;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectFieldSelectionMenu(agent.gameObject, checkSet.availableTypes, FieldSelected);
				} else {
					var menu = EditorUtils.GetFieldSelectionMenu(agentType, checkSet.availableTypes, FieldSelected);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}

			if (!string.IsNullOrEmpty(fieldName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Type", agentType.Name);
				EditorGUILayout.LabelField("Field", fieldName);
				GUILayout.EndVertical();

				if (checkSet.selectedType != null){

					GUI.enabled = checkSet.selectedType == typeof(float) || checkSet.selectedType == typeof(int);
					comparison = (CompareMethod)EditorGUILayout.EnumPopup("Comparison", comparison);
					GUI.enabled = true;

					EditorUtils.BBVariableField("Value", checkSet.selectedBBVariable);
				}
			}
		}

		#endif
	}
}