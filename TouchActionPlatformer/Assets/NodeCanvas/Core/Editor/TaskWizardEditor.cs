#if !UNITY_WEBPLAYER

using UnityEditor;
using UnityEngine;

namespace NodeCanvasEditor{

	public class TaskWizardEditor : EditorWindow {

		enum TaskType{Action, Condition}
		TaskType type = TaskType.Action;

		string taskName;
		string category;
		string agentType;
		string ns;

		void OnEnable(){
			title = "NC Task Wizard";
		}

		void OnGUI(){

			type = (TaskType)EditorGUILayout.EnumPopup("Task Type", type);
			ns = EditorGUILayout.TextField("Namespace", ns);
			taskName = EditorGUILayout.TextField("Task Name", taskName);
			category = EditorGUILayout.TextField("Category(?)", category);
			agentType = EditorGUILayout.TextField("Agent Type(?)", agentType);

			if (GUILayout.Button("CREATE")){

				if (string.IsNullOrEmpty(taskName)){
					EditorUtility.DisplayDialog("Empty Task Name", "Please give the new task a name","OK");
					return;
				}

				if (type == TaskType.Action)
					CreateFile(GetActionTemplate());

				if (type == TaskType.Condition)
					CreateFile(GetCoditionTemplate());

				taskName = "";
				GUIUtility.hotControl = 0;
				GUIUtility.keyboardControl = 0;
			}

			if (type == TaskType.Action)
				GUILayout.Label(GetActionTemplate());

			if (type == TaskType.Condition)
				GUILayout.Label(GetCoditionTemplate());
		}

		void CreateFile(string template){
			
			var path = AssetDatabase.GenerateUniqueAssetPath(GetPath() + "/" + taskName + ".cs");

			if (System.IO.File.Exists(path)){
				if (!EditorUtility.DisplayDialog("File Exists", "Overwrite file?","YES", "NO"))
					return;
			}

			System.IO.File.WriteAllText(path, template);
			UnityEditor.AssetDatabase.Refresh();
			Debug.LogWarning("New Task is placed under: " + path);
		}

		string GetPath(){
			var path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "")
			    path = "Assets";
			if (System.IO.Path.GetExtension(path) != "")
			    path = path.Replace(System.IO.Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			return path;
		}

		string GetActionTemplate(){
			return
			"using UnityEngine;\n" +
			(string.IsNullOrEmpty(ns)? "" : "using NodeCanvas;\n") + 
			"using NodeCanvas.Variables;\n\n" +
			"namespace " + (string.IsNullOrEmpty(ns)? "NodeCanvas.Actions" : ns) + "{\n\n" + 
			(!string.IsNullOrEmpty(category)? "\t[Category(\"" + category + "\")]\n" : "") +
			(!string.IsNullOrEmpty(agentType)? "\t[AgentType(typeof(" + agentType + "))]\n" : "") +
			"\tpublic class " + taskName + " : ActionTask {\n\n" +
			"\t\tprotected override string OnInit(){\n" +
			"\t\t\treturn null;\n" +
			"\t\t}\n\n" +
			"\t\tprotected override void OnExecute(){\n" +
			"\t\t\tEndAction(true);\n" +
			"\t\t}\n\n" +
			"\t\tprotected override void OnUpdate(){\n" +
			"\t\t\t\n" + 
			"\t\t}\n\n" +
			"\t\tprotected override void OnStop(){\n" +
			"\t\t\t\n" +
			"\t\t}\n\n" + 
			"\t\tprotected override void OnPause(){\n" +
			"\t\t\t\n" +
			"\t\t}\n" + 
			"\t}\n" +
			"}";
		}

		string GetCoditionTemplate(){	
			return
			"using UnityEngine;\n" +
			(string.IsNullOrEmpty(ns)? "" : "using NodeCanvas;\n") + 
			"using NodeCanvas.Variables;\n\n" +
			"namespace " + (string.IsNullOrEmpty(ns)? "NodeCanvas.Conditions" : ns) + "{\n\n" + 
			(!string.IsNullOrEmpty(category)? "\t[Category(\"" + category + "\")]\n" : "") +
			(!string.IsNullOrEmpty(agentType)? "\t[AgentType(typeof(" + agentType + "))]\n" : "") +
			"\tpublic class " + taskName + " : ConditionTask {\n\n" +
			"\t\tprotected override string OnInit(){\n" +
			"\t\t\treturn null;\n" +
			"\t\t}\n\n" +
			"\t\tprotected override bool OnCheck(){\n" +
			"\t\t\treturn true;\n" +
			"\t\t}\n" + 
			"\t}\n" +
			"}";
		}

		[MenuItem("Assets/Create/NodeCanvas Task")]
		[MenuItem("Window/NodeCanvas/Create New Task")]
		public static void ShowWindow(){

			var window = ScriptableObject.CreateInstance(typeof(TaskWizardEditor)) as TaskWizardEditor;
			window.ShowUtility();
		}
	}
}

#endif
