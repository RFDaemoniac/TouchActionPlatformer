
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System.Collections;

namespace NodeCanvasEditor{

	///A generic popup editor for all types
	public class PopObjectEditor : EditorWindow{

		private object targetObject;
		private System.Type targetType;
		private Vector2 scrollPos;

		void OnEnable(){
			title = "NC Object Editor";
			//EditorApplication.playmodeStateChanged += Close;
		}

		void OnGUI(){

			if (EditorApplication.isCompiling || targetObject == null || targetType == null){
				Close();
				return;
			}

			GUI.skin.label.richText = true;
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label(string.Format("<size=14><b>{0}</b></size>", NodeCanvas.EditorUtils.TypeName(targetType) ) );
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();

			GUILayout.Space(10);
			scrollPos = GUILayout.BeginScrollView(scrollPos);
			NodeCanvas.EditorUtils.GenericField(NodeCanvas.EditorUtils.TypeName(targetType), targetObject, targetType);
			GUILayout.EndScrollView();
			Repaint();
		}

		public static void Show(object o, System.Type t){

			var window = ScriptableObject.CreateInstance(typeof(PopObjectEditor)) as PopObjectEditor;
			//if (o == null)
			//	o = System.Activator.CreateInstance(t);
			window.targetObject = o;
			window.targetType = t;
			window.ShowUtility();
		}
	}
}

#endif