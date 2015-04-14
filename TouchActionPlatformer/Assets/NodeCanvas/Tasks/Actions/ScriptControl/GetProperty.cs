#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Script Control")]
	[Description("Get a property of a script and save it to the blackboard")]
	public class GetProperty : ActionTask {

		public BBVariableSet saveAs = new BBVariableSet{blackboardOnly = true};

		[SerializeField]
		private string scriptName = typeof(Component).AssemblyQualifiedName;
		[SerializeField]
		private string methodName;

		private MethodInfo method;

		public override System.Type agentType{
			get {return System.Type.GetType(scriptName);}
		}

		protected override string info{
			get
			{
				if (string.IsNullOrEmpty(methodName))
					return "No Property Selected";

				return string.Format("{0} = {1}.{2}", saveAs.selectedBBVariable, agentInfo, methodName);
			}
		}

		//store the method info on init for performance
		protected override string OnInit(){
			method = agent.GetType().NCGetMethod(methodName);
			if (method == null)
				return "Missing Property Method Info";
			return null;
		}

		//do it by invoking method
		protected override void OnExecute(){
			saveAs.objectValue = method.Invoke(agent, null);
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

			EditorGUILayout.HelpBox(agent == null? "Agent is unknown.\nYou can select a type and a method" : "Agent is known.\nMethod selection will be done from existing components", MessageType.Info);

			if (!Application.isPlaying && agent == null && GUILayout.Button("Alter Type")){

				System.Action<System.Type> TypeSelected = delegate(System.Type t){
					var newName = t.AssemblyQualifiedName;
					if (scriptName != newName){
						scriptName = newName;
						methodName = null;
					}
				};

				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Component), TypeSelected);
			}

			if (!Application.isPlaying && GUILayout.Button("Select Property")){
				System.Action<MethodInfo> MethodSelected = delegate(MethodInfo method){
					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
					saveAs.selectedType = method.ReturnType;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, saveAs.availableTypes, null, MethodSelected, 0, true);
				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, saveAs.availableTypes, null, MethodSelected, 0, true);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}


			if (!string.IsNullOrEmpty(methodName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Type", agentType.Name);
				EditorGUILayout.LabelField("Property", methodName);
				EditorGUILayout.LabelField("Property Type", EditorUtils.TypeName(saveAs.selectedType) );
				GUILayout.EndVertical();

				if (saveAs.selectedType != null)
					EditorUtils.BBVariableField("Save As", saveAs.selectedBBVariable);
			}
		}

		#endif
	}
}