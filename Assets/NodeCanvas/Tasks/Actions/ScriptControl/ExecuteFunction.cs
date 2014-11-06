#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Script Control")]
	[Description("Execute a function on a script, of up to 3 parameters and save the return if any. If function is an IEnumerator it will start a coroutine and the action will run for as long as the coroutine is running. If the action stops, so will the coroutine.")]
	[AgentType(typeof(Transform))]
	public class ExecuteFunction : ActionTask {

		public BBVariableSet paramValue1 = new BBVariableSet();
		public BBVariableSet paramValue2 = new BBVariableSet();
		public BBVariableSet paramValue3 = new BBVariableSet();
		public BBVariableSet returnValue = new BBVariableSet{blackboardOnly = true};

		[SerializeField]
		private string scriptName = typeof(Component).AssemblyQualifiedName;
		[SerializeField]
		private string methodName;
		
		private MethodInfo method;
		private int paramCount;
		private bool routineRunning;

		public override System.Type agentType{
			get {return System.Type.GetType(scriptName);}
		}

		protected override string info{
			get
			{
				if (string.IsNullOrEmpty(methodName))
					return "No Method Selected";

				string paramInfo = "";
				paramInfo += paramValue1.selectedType != null? paramValue1.selectedBBVariable.ToString() : "";
				paramInfo += paramValue2.selectedType != null? ", " + paramValue2.selectedBBVariable.ToString() : "";
				paramInfo += paramValue3.selectedType != null? ", " + paramValue3.selectedBBVariable.ToString() : "";
				return (returnValue.selectedType != null && !returnValue.selectedBBVariable.isNone? returnValue.selectedBBVariable.ToString() + "= ": "") + agentInfo + "." + methodName + "(" + paramInfo + ")" ;
			}
		}

		//store the method info on init
		protected override string OnInit(){

			var paramTypes = new List<System.Type>();

			if (paramValue1.selectedType != null){
				paramTypes.Add(paramValue1.selectedType);
				if (paramValue2.selectedType != null){
					paramTypes.Add(paramValue2.selectedType);
					if (paramValue3.selectedType != null){
						paramTypes.Add(paramValue3.selectedType);
					}
				}
			}

			paramCount = paramTypes.Count;
			//the agent is always "casted" to the type so...
			method = agent.GetType().NCGetMethod(methodName, paramTypes.ToArray());

			if (method == null)
				return "Method not found";

			return null;
		}

		//do it by invoking method
		protected override void OnExecute(){

			object[] args = null;
			if (paramCount > 0){
				if (paramCount == 1)
					args = new object[]{paramValue1.objectValue};
				else if (paramCount == 2)
					args = new object[]{paramValue1.objectValue, paramValue2.objectValue};
				else if (paramCount == 3)
					args = new object[]{paramValue1.objectValue, paramValue2.objectValue, paramValue3.objectValue};
			}

			if (method.ReturnType == typeof(IEnumerator)){
				routineRunning = true;
				StartCoroutine( InternalCoroutine((IEnumerator)method.Invoke(agent, args)) );
			} else {
				returnValue.objectValue = method.Invoke(agent, args);
				EndAction(true);
			}
		}

		protected override void OnStop(){
			routineRunning = false;
		}


		IEnumerator InternalCoroutine(IEnumerator routine){

			while(routine.MoveNext()){
				
				yield return routine.Current;

				if (routineRunning == false)
					yield break;
			}

			EndAction(true);
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		[SerializeField]
		private List<string> paramNames = new List<string>{"Param1","Param2","Param3"}; //init for update
		[SerializeField]
		private bool isIEnumerator;

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

			if (!Application.isPlaying && GUILayout.Button("Select Method")){

				System.Action<MethodInfo> MethodSelected = delegate(MethodInfo method){
					
					if (!typeof(Component).IsAssignableFrom(method.DeclaringType) && !method.DeclaringType.IsInterface )
						return;

					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
					var parameters = method.GetParameters();
					paramNames = parameters.Select(p => EditorUtils.SplitCamelCase(p.Name) ).ToList();
					paramValue1.selectedType = parameters.Length >= 1? parameters[0].ParameterType : null;
					paramValue2.selectedType = parameters.Length >= 2? parameters[1].ParameterType : null;
					paramValue3.selectedType = parameters.Length >= 3? parameters[2].ParameterType : null;
					if (method.ReturnType == typeof(IEnumerator) || method.ReturnType == typeof(void) || method.ReturnType == typeof(Coroutine)){
						returnValue.selectedType = null;
					} else {
						returnValue.selectedType = method.ReturnType;
					}

					//for gui
					isIEnumerator = method.ReturnType == typeof(IEnumerator);
				};

				
				var returnTypes = returnValue.availableTypes;
				returnTypes.Add(typeof(void));
				returnTypes.Add(typeof(IEnumerator));
				returnTypes.Add(typeof(Coroutine));
				var paramTypes = paramValue1.availableTypes;

				if (agent != null){
					
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, returnTypes, paramTypes, MethodSelected, 3, false);

				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, returnTypes, paramTypes, MethodSelected, 3, false);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}



			if (!string.IsNullOrEmpty(methodName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Type", agentType.Name);
				EditorGUILayout.LabelField("Method", methodName);
				
				if (returnValue.selectedType != null)
					EditorGUILayout.LabelField("Returns", EditorUtils.TypeName(returnValue.selectedType));
				
				if (isIEnumerator)
					GUILayout.Label("<b>This will execute as a Coroutine</b>");

				GUILayout.EndVertical();

				if (paramValue1.selectedType != null){
					EditorUtils.BBVariableField(paramNames[0], paramValue1.selectedBBVariable);
					if (paramValue2.selectedType != null){
						EditorUtils.BBVariableField(paramNames[1], paramValue2.selectedBBVariable);
						if (paramValue3.selectedType != null){
							EditorUtils.BBVariableField(paramNames[2], paramValue3.selectedBBVariable);
						}
					}
				}

				if (returnValue.selectedType != null)
					EditorUtils.BBVariableField("Save Return Value", returnValue.selectedBBVariable);
			}
		}

		#endif
	}
}