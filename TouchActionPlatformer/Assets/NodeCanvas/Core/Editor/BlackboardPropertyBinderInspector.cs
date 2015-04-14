using UnityEngine;
using UnityEditor;
using NodeCanvas;
using System.Collections.Generic;
using System.Reflection;

namespace NodeCanvasEditor{

	[CustomEditor(typeof(BlackboardPropertyBinder))]
	public class BlackboardPropertyBinderInspector : Editor {

		private BlackboardPropertyBinder pb{
			get {return target as BlackboardPropertyBinder;}
		}

		public override void OnInspectorGUI(){

			pb.blackboard = (Blackboard)EditorGUILayout.ObjectField("Blackboard", pb.blackboard, typeof(Blackboard), true);
			pb.gameObject = (GameObject)EditorGUILayout.ObjectField("GameObject", pb.gameObject, typeof(GameObject), true);
			
			if (!pb.blackboard || !pb.gameObject){
				EditorGUILayout.HelpBox("Please assign both a blackboard and a target game object", MessageType.Info);
				return;
			}

			if (GUILayout.Button("Add Binder")){

				GenericMenu.MenuFunction2 Selected = delegate(object selectedName){
					var data = pb.blackboard.GetData((string)selectedName, null);
					pb.binders.Add(new BlackboardPropertyBinder.Binder(data) );
				};

				var menu = new GenericMenu();
				foreach (string name in pb.blackboard.GetDataNames())
					menu.AddItem(new GUIContent(name), false, Selected, name);
				menu.ShowAsContext();
				Event.current.Use();

				if (menu.GetItemCount() == 0)
					Debug.LogWarning("Blackboard has no variables");
			}

			GUI.color = Color.yellow;
			EditorGUILayout.LabelField("Variable Name", "Binded Property");
			GUI.color = Color.white;

			foreach (BlackboardPropertyBinder.Binder binder in pb.binders){

				GUILayout.BeginVertical("box");
				GUILayout.BeginHorizontal();
				string getOrSet = binder.bindingType == BlackboardPropertyBinder.Binder.BindingType.PropertyToVariable? "GET" : "SET";
				EditorGUILayout.LabelField(binder.variableName, string.IsNullOrEmpty(binder.componentName)? "No Binding" : string.Format("{0}.{1} ({2})", binder.componentName, binder.propertyName, getOrSet));

				if (!Application.isPlaying && GUILayout.Button("X", GUILayout.Width(18))){
					pb.binders.Remove(binder);
					return;
				}
				GUILayout.EndHorizontal();
				
				if (string.IsNullOrEmpty(binder.propertyName) && GUILayout.Button("Select Property")){
					EditorUtils.ShowGameObjectMethodSelectionMenu(pb.gameObject, new List<System.Type>{binder.type, typeof(void)}, new List<System.Type>{binder.type}, delegate(MethodInfo method){
						
						string name = method.Name;
						if (name.StartsWith("get_")){
							binder.bindingType = BlackboardPropertyBinder.Binder.BindingType.PropertyToVariable;
							name = method.Name.Replace("get_", "");
						}

						if (name.StartsWith("set_")){
							binder.bindingType = BlackboardPropertyBinder.Binder.BindingType.VariableToProperty;
							name = method.Name.Replace("set_", "");
						}

						binder.componentName = method.ReflectedType.Name;
						binder.propertyName = name;

					}, 1, true);
				}

				GUILayout.EndVertical();
			}

			if (GUI.changed)
				EditorUtility.SetDirty(target);
		}
	}
}