#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

namespace NodeCanvas.DialogueTrees{

	[AddComponentMenu("")]
	[Name("✫ Say")]
	[Description("Make the selected Dialogue Actor to talk. You can make the text more dynamic by using variable names in square brackets\ne.g. [myVarName]")]
	public class DLGStatementNode : DLGNodeBase{

		public Statement statement = new Statement("This is a dialogue text");

		public override string nodeName{
			get{return base.nodeName + " " + finalActorName;}
		}

		protected override Status OnExecute(){

			if (!finalActor){
				DLGTree.StopGraph();
				return Error("Actor not found");
			}

			DLGTree.currentNode = this;
			var finalStatement = statement.BlackboardReplace(finalBlackboard);
			finalActor.Say(finalStatement, Continue);
			return Status.Running;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){

			base.OnNodeGUI();
			var displayText = statement.text.Length > 60? statement.text.Substring(0, 60) + "..." : statement.text;
			GUILayout.Label("\"<i> " + displayText + "</i> \"");
		}

		protected override void OnNodeInspectorGUI(){

			base.OnNodeInspectorGUI();
			GUIStyle areaStyle = new GUIStyle(GUI.skin.GetStyle("TextArea"));
			areaStyle.wordWrap = true;
			
			GUILayout.Label("Dialogue Text");
			statement.text = EditorGUILayout.TextArea(statement.text, areaStyle, GUILayout.Height(100));
			statement.audio = EditorGUILayout.ObjectField("Audio File", statement.audio, typeof(AudioClip), false)  as AudioClip;
			statement.meta = EditorGUILayout.TextField("Metadata", statement.meta);
		}

		#endif
	}
}