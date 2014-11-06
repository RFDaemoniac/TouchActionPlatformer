#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Script Control")]
	[Description("Check a property on a script and return if it's equal or not to the check value")]
	public class CheckProperty : ConditionTask {

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
					return "No Property Selected";
				return string.Format("{0}.{1}{2}", agentInfo, methodName, checkSet.selectedType == typeof(bool)? "" : TaskTools.GetCompareString(comparison) + checkSet.ToString());
			}
		}

		//store the method info on agent set for performance
		protected override string OnInit(){
			method = agent.GetType().NCGetMethod(methodName);
			if (method == null)
				return "Missing Property Method Info";
			return null;
		}

		//do it by invoking method
		protected override bool OnCheck(){

			if (checkSet.selectedType == typeof(float) || checkSet.selectedType == typeof(int))
				return TaskTools.Compare( (System.IComparable)method.Invoke(agent, null), (System.IComparable)checkSet.objectValue, comparison );

			return Equals( method.Invoke(agent, null), checkSet.objectValue );
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
					scriptName = method.DeclaringType.AssemblyQualifiedName;
					methodName = method.Name;
					checkSet.selectedType = method.ReturnType;
					comparison = CompareMethod.EqualTo;
				};

				if (agent != null){
					EditorUtils.ShowGameObjectMethodSelectionMenu(agent.gameObject, checkSet.availableTypes, null, MethodSelected, 0, true);
				} else {
					var menu = EditorUtils.GetMethodSelectionMenu(agentType, checkSet.availableTypes, null, MethodSelected, 0, true);
					menu.ShowAsContext();
					Event.current.Use();
				}				
			}			

			if (!string.IsNullOrEmpty(methodName)){
				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Selected Component", scriptName);
				EditorGUILayout.LabelField("Selected Property", methodName);
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