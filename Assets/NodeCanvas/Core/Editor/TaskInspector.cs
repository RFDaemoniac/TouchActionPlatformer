using UnityEngine;
using System.Collections;
using UnityEditor;
using NodeCanvas;

namespace NodeCanvasEditor{

	[CustomEditor(typeof(Task), true)]
	public class TaskInspector : Editor {

		public override void OnInspectorGUI(){

			(target as Task).ShowInspectorGUI(false);
			if (target == null)
				return;

			EditorUtils.EndOfInspector();
			if (GUI.changed){
				EditorUtility.SetDirty(target);
				Repaint();
			}
		}
	}
}
