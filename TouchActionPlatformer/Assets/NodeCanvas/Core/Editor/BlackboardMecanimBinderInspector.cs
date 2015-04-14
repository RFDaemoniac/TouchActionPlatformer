using UnityEditor;
using UnityEngine;
using System.Collections;
using NodeCanvas;

namespace NodeCanvasEditor{

	[CustomEditor(typeof(BlackboardMecanimBinder))]
	public class BlackboardMecanimBinderInspector : Editor {

		BlackboardMecanimBinder sync{
			get {return (BlackboardMecanimBinder)target;}
		}

		public override void OnInspectorGUI(){

			GUI.enabled = !Application.isPlaying;

			sync.blackboard = (Blackboard)EditorGUILayout.ObjectField("Blackboard Target", sync.blackboard, typeof(Blackboard), true);
			sync.animator = (Animator)EditorGUILayout.ObjectField("Animator Target", sync.animator, typeof(Animator), true);

			if (GUILayout.Button("Add New"))
				sync.parameters.Add(string.Empty);

			for (int i = 0; i < sync.parameters.Count; i++){

				GUILayout.BeginHorizontal();
				sync.parameters[i] = EditorGUILayout.TextField("Synced Parameter", sync.parameters[i]);

				if (GUILayout.Button("X", GUILayout.Width(20)))
					sync.parameters.RemoveAt(i);

				GUILayout.EndHorizontal();
			}

			GUI.enabled = true;
		}
	}
}