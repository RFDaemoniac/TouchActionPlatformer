#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Script Control")]
	[Description("Get a variable of a script and save it to the blackboard")]
	public class GetField : ActionTask {

		public BBVariableSet saveAs = new BBVariableSet{blackboardOnly = true};

		[SerializeField]
		private string scriptName = typeof(Component).AssemblyQualifiedName;
		[SerializeField]
		private string fieldName;

		private FieldInfo field;

		public override System.Type agentType{
			get {return System.Type.GetType(scriptName);}
		}

		protected override string info{
			get
			{
				if (string.IsNullOrEmpty(fieldName))
					return "No Field Selected";

				return (saveAs.selectedBBVariable + " = " + agentInfo + "." + fieldName);
			}
		}

		protected override string OnInit(){
			field = agent.GetType().NCGetField(fieldName);
			if (field == null)
				return "Missing Field Info";
			return null;
		}

		protected override void OnExecute(){
			saveAs.objectValue = field.GetValue(agent);
			EndAction(true);
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

			EditorGUILayout.HelpBox(agent == null? "Agent is unknown.\nYou can select a type and a field" : "Agent is known.\nField selection will be done from existing components", MessageType.Info);

			if (!Application.isPlaying && agent == null && GUILayout.Button("Alter Type")){

				System.Action<System.Type> TypeSelected = delegate(System.Type t){
					var newName = t.AssemblyQualifiedName;
					if (scriptName != newName){
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
					saveAs.selectedType = field.FieldType;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectFieldSelectionMenu(agent.gameObject, saveAs.availableTypes, FieldSelected);
				} else {
					var menu = EditorUtils.GetFieldSelectionMenu(agentType, saveAs.availableTypes, FieldSelected);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}


			if (!string.IsNullOrEmpty(fieldName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Selected Component", scriptName);
				EditorGUILayout.LabelField("Selected Field", fieldName);
				EditorGUILayout.LabelField("Field Type", EditorUtils.TypeName(saveAs.selectedType) );
				GUILayout.EndVertical();

				if (saveAs.selectedType != null)
					EditorUtils.BBVariableField("Save As", saveAs.selectedBBVariable);
			}
		}

		#endif
	}
}