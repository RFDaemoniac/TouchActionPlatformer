#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Script Control")]
	[Description("Call a function with none or one parameter on a script and return whether or not the return value is equal to the check value")]
	public class CheckFunction : ConditionTask {

		public BBVariableSet paramValue1 = new BBVariableSet();
		public BBVariableSet checkSet = new BBVariableSet();

		[SerializeField]
		private string scriptName = typeof(Component).AssemblyQualifiedName;
		[SerializeField]
		private string methodName;

		[SerializeField]
		private CompareMethod comparison;

		private MethodInfo method;

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
				return string.Format("{0}.{1}({2}){3}", agentInfo, methodName, paramInfo, checkSet.selectedType == typeof(bool)? "" : TaskTools.GetCompareString(comparison) + checkSet.selectedBBVariable);
			}
		}

		//store the method info on agent set for performance
		protected override string OnInit(){
			var paramTypes = new List<System.Type>();
			if (paramValue1.selectedType != null)
				paramTypes.Add(paramValue1.selectedType);

			method = agent.GetType().NCGetMethod(methodName, paramTypes.ToArray());

			if (method == null)
				return "Missing Method Info";
			
			return null;
		}

		//do it by invoking method
		protected override bool OnCheck(){

			object[] args = null;
			if (paramValue1.selectedType != null)
				args = new object[]{paramValue1.objectValue};

			if (checkSet.selectedType == typeof(float) || checkSet.selectedType == typeof(int))
				return TaskTools.Compare( (System.IComparable)method.Invoke(agent, args), (System.IComparable)checkSet.objectValue, comparison );

			return Equals( method.Invoke(agent, args), checkSet.objectValue );
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		[SerializeField]
		private List<string> paramNames = new List<string>{"Param1"}; //init for update

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
						methodName = null;
					}
				};

				EditorUtils.ShowConfiguredTypeSelectionMenu(typeof(Component), TypeSelected);
			}

			if (!Application.isPlaying && GUILayout.Button("Select Method")){
				System.Action<MethodInfo> MethodSelected = delegate(MethodInfo method){
					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
					var parameters = method.GetParameters();
					paramNames = parameters.Select(p => p.Name).ToList();
					paramValue1.selectedType = parameters.Length >= 1? parameters[0].ParameterType : null;
					checkSet.selectedType = method.ReturnType;
					comparison = CompareMethod.EqualTo;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, checkSet.availableTypes, paramValue1.availableTypes, MethodSelected, 1, false);
				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, checkSet.availableTypes, paramValue1.availableTypes, MethodSelected, 1, false);
					menu.ShowAsContext();
					Event.current.Use();
				}
			}

			if (!string.IsNullOrEmpty(methodName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Type", agentType.Name);
				EditorGUILayout.LabelField("Method", methodName);
				GUILayout.EndVertical();

				if (paramValue1.selectedType != null)
					EditorUtils.BBVariableField(paramNames[0], paramValue1.selectedBBVariable);

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