#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Script Control")]
	[Description("Set a property on a script")]
	[AgentType(typeof(Transform))]
	public class SetProperty : ActionTask {

		public BBVariableSet setValue = new BBVariableSet();

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

				return string.Format("{0}.{1} = {2}", agentInfo, methodName, setValue.selectedBBVariable);
			}
		}

		//store the method info on init for performance
		protected override string OnInit(){
			method = agent.GetType().NCGetMethod(methodName);
			if (method == null)
				return "Missing Property Method";
			return null;
		}

		//do it by invoking method
		protected override void OnExecute(){
			method.Invoke(agent, new object[]{setValue.objectValue} );
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
					var newTypeName = t.AssemblyQualifiedName;
					if (scriptName != newTypeName){
						scriptName = newTypeName;
						methodName = null;
					}					
				};

				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Component), TypeSelected);				
			}

			if (!Application.isPlaying && GUILayout.Button("Select Property")){

				System.Action<MethodInfo> MethodSelected = delegate(MethodInfo method){
					if (!typeof(Component).IsAssignableFrom(method.DeclaringType) && !method.DeclaringType.IsInterface )
						return;
					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
					setValue.selectedType = method.GetParameters()[0].ParameterType;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, new List<System.Type>{typeof(void)}, setValue.availableTypes, MethodSelected, 1, true);
				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, new List<System.Type>{typeof(void)}, setValue.availableTypes, MethodSelected, 1, true);
					menu.ShowAsContext();
					Event.current.Use();
				}				
			}

			if (!string.IsNullOrEmpty(methodName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Type", agentType.Name);
				EditorGUILayout.LabelField("Property", methodName);
				EditorGUILayout.LabelField("Set Type", EditorUtils.TypeName(setValue.selectedType) );
				GUILayout.EndVertical();

				if (setValue.selectedType != null)
					EditorUtils.BBVariableField("Set Value", setValue.selectedBBVariable);
			}
		}

		#endif
	}
}