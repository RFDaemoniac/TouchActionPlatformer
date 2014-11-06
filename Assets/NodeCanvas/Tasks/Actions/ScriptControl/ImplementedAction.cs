using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace NodeCanvas.Actions{

	[Category("✫ Script Control")]
	[Description("Calls a function that has signature of 'public Status NAME()'. Return Status.Success, Failure or Running within that function")]
	public class ImplementedAction : ActionTask {

		[RequiredField]
		public string scriptName =  typeof(Component).AssemblyQualifiedName;
		[RequiredField]
		public string methodName;

		private MethodInfo method;
		private Status status = Status.Resting;

		public override System.Type agentType{
			get {return System.Type.GetType(scriptName);}
		}

		protected override string info{
			get {return string.Format("({0}.{1})", agentInfo, methodName);}
		}

		protected override string OnInit(){
			method = agent.GetType().NCGetMethod(methodName);
			if (method == null)
				return "Method not found";
			return null;
		}

		protected override void OnExecute(){
			Forward();
		}

		protected override void OnUpdate(){
			Forward();
		}

		void Forward(){

			status = (Status)method.Invoke(agent, null);

			if (status == Status.Success){
				EndAction(true);
				return;
			}

			if (status == Status.Failure){
				EndAction(false);
				return;
			}
		}

		protected override void OnStop(){
			status = Status.Resting;
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
					if (scriptName != newName){
						scriptName = newName;
						methodName = null;
					}
				};

				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Component), TypeSelected);
			}

			if (!Application.isPlaying && GUILayout.Button("Select Action Method")){

				System.Action<MethodInfo> MethodSelected = delegate(MethodInfo method){
					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, new List<System.Type>{typeof(Status)}, null, MethodSelected, 0, false);
				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, new List<System.Type>{typeof(Status)}, null, MethodSelected, 0, false);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}

			if (!string.IsNullOrEmpty(methodName))
				UnityEditor.EditorGUILayout.LabelField("Selected Action Method:", methodName);
		}
		
		#endif
	}
}