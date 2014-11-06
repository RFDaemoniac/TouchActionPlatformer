using UnityEngine;
using UnityEditor;
using System.Collections;
using NodeCanvas;
using NodeCanvas.BehaviourTrees;

namespace NodeCanvasEditor{

	[CustomEditor(typeof(BehaviourTreeOwner))]
	public class BehaviourTreeOwnerInspector : GraphOwnerInspector {

		BehaviourTreeOwner owner{
			get {return target as BehaviourTreeOwner; }
		}

		protected override void OnExtraOptions(){
			
			owner.runForever = EditorGUILayout.Toggle("Run Forever", owner.runForever);
			if (owner.runForever){
				GUI.color = owner.updateInterval > 0? Color.white : new Color(1,1,1,0.5f);
				owner.updateInterval = EditorGUILayout.FloatField("Update Interval", owner.updateInterval );
				GUI.color = Color.white;
			}
		}

		protected override void OnGrapOwnerControls(){
			if (GUILayout.Button(EditorUtils.stepIcon))
				owner.Tick();
		}
	}
}