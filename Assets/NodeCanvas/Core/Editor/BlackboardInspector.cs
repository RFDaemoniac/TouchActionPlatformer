using UnityEngine;
using System.Collections;
using UnityEditor;
using NodeCanvas;
using NodeCanvas.Variables;

namespace NodeCanvasEditor{

	[CustomEditor(typeof(Blackboard), true)]
	public class BlackboardInspector : Editor {

		void OnEnable(){
			foreach (VariableData data in (target as Blackboard).GetAllData()){
				if (data != null)
					data.hideFlags = HideFlags.HideInInspector;
			}
		}

		override public void OnInspectorGUI(){
			
			if (Event.current.isMouse)
				Repaint();

			(target as Blackboard).ShowBlackboardGUI();
			EditorUtils.EndOfInspector();
			if (Application.isPlaying)
				Repaint();

			if (GUI.changed)
				EditorUtility.SetDirty(target);
		}
	}
}