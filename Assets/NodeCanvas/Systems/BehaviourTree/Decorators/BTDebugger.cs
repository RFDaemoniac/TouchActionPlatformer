#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("Debug")]
	[Category("Decorators")]
	[Description("Use this node to add a break point to the Behaviour Tree as well as log a UI label at the position of the agent when the node is in the selected status.")]
	[Icon("Log")]
	public class BTDebugger : BTDecorator{

		public bool breakPoint = true;
		public bool pauseUnity;

		public Status statusToLog = Status.Success;
		public string log;

		public bool forceReturnStatus;
		public Status forceStatus = Status.Success;

		private Component currentAgent;

		private Texture2D _tex;
		private Texture2D tex{
			get
			{
				if (!_tex){
					_tex = new Texture2D(1,1);
					_tex.SetPixel(0, 0, Color.white);
					_tex.Apply();
				}
				return _tex;			
			}
		}

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			useGUILayout = !string.IsNullOrEmpty(log);
			currentAgent = agent;

			if (breakPoint){
				graph.PauseGraph();
				Debug.Log("Behaviour Tree '" + graph.name + "' Breakpoint Reached...(Controls can be found on the BehaviourTreeOwner Inspector)");
				if (pauseUnity)
					Debug.Break();
			}
			
			if (decoratedConnection == null)
				return forceReturnStatus? forceStatus : status;

			status = decoratedConnection.Execute(agent, blackboard);

			return forceReturnStatus? forceStatus : status;
		}

		void OnGUI(){

			if (currentAgent == null || Camera.main == null)
				return;

			if (status != statusToLog)
				return;

			Vector2 point = Camera.main.WorldToScreenPoint(currentAgent.transform.position + new Vector3(0f, -0.5f, 0f));
			Vector2 finalSize = new GUIStyle("label").CalcSize(new GUIContent(log));
			Rect r = new Rect(0, 0, finalSize.x, finalSize.y);
			point.y = Screen.height - point.y;
			r.center = point;
			GUI.color = new Color(1f,1f,1f,0.5f);
			r.width += 6;
			GUI.DrawTexture(r, tex);
			GUI.color = new Color(0.2f, 0.2f, 0.2f, 1);
			r.x += 4;
			GUI.Label(r, log);
			GUI.color = Color.white;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

	        Rect breakRect = new Rect(nodeRect.width - 15, 5, 15, 15);
	        if (breakPoint)
	        	GUI.Label(breakRect, "<b>B</b>");

			if (!string.IsNullOrEmpty(log))
				GUILayout.Label("UI Log '" + log + "'");
			else
				GUILayout.Label("", GUILayout.Height(1));
		}

		protected override void OnNodeInspectorGUI(){

			GUILayout.BeginVertical("box");
			breakPoint = EditorGUILayout.Toggle("Break Point", breakPoint);
			if (breakPoint)
				pauseUnity = EditorGUILayout.Toggle("Also Pause Unity", pauseUnity);
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			statusToLog = (Status)EditorGUILayout.EnumPopup("When in Status", statusToLog);
			log = EditorGUILayout.TextField("UI Label", log);
			GUILayout.EndVertical();

			GUILayout.BeginVertical("box");
			forceReturnStatus = EditorGUILayout.Toggle("Force Return Status", forceReturnStatus);
			if (forceReturnStatus)
				forceStatus = (Status)EditorGUILayout.EnumPopup("Force Status", forceStatus);
			GUILayout.EndVertical();

			if (graph.isRunning){

				GUILayout.BeginVertical("box");
				EditorGUILayout.LabelField("Current Agent", currentAgent != null? currentAgent.name : "NONE");
				GUILayout.EndVertical();

			} else {

				EditorGUILayout.HelpBox("Current Agent to this point, will display here when the Behaviour Tree is running", MessageType.Info);
			}
		}

		#endif
	}
}