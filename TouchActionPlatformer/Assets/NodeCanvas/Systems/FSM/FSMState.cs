#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

namespace NodeCanvas.StateMachines{

	public interface IState{
		string name{get;}
		float elapsedTime{get;}
		FSM FSM{get;}
	}

	///The base class for all FSM system nodes. It basicaly 'converts' methods to more friendly FSM like ones.
	abstract public class FSMState : Node, IState{

		public enum TransitionEvaluation{
			CheckContinuously,
			CheckAfterStateFinished
		}
		
		[SerializeField]
		private TransitionEvaluation transitionEvaluation;

		public float elapsedTime{get;private set;}

		public override int maxInConnections{
			get{return -1;}
		}

		public override int maxOutConnections{
			get{return -1;}
		}

		sealed public override System.Type outConnectionType{
			get{return typeof(FSMConnection);}
		}

		///The FSM this state belongs to
		public FSM FSM{
			get{return (FSM)graph;}
		}

		public void Finish(){
			Finish(true);
		}

		///Declares that the state has finished
		protected void Finish(System.ValueType inSuccess){
			enabled = false;
			status = (bool)inSuccess? Status.Success : Status.Failure;
		}

		sealed public override void OnGraphStarted(){
			Init();
		}

		sealed public override void OnGraphStoped(){
			status = Status.Resting;
		}

		sealed public override void OnGraphPaused(){
			if (status == Status.Running)
				Pause();
		}

		//Enter...
		sealed protected override Status OnExecute(){

			if (status == Status.Resting || status == Status.Running){
				status = Status.Running;
/*
				//Disabled for now. If on transitions are checked instantely, but may cause infinite loops
				if (CheckTransition())
					return Status.Resting;
*/
				enabled = true;
				Enter();

			}

			return status;
		}

		//Stay...
		public void OnUpdate(){

			elapsedTime += Time.deltaTime;

			if (CheckTransition() == false){
				if (status == Status.Running)
					Stay();
			}
		}

		bool CheckTransition(){

			if (status == Status.Running && transitionEvaluation == TransitionEvaluation.CheckAfterStateFinished)
				return false;

			for (int i= 0; i < outConnections.Count; i++){
				var connection = outConnections[i] as FSMConnection;
				if (connection.CheckCondition(graphAgent, graphBlackboard)){
					if (status != Status.Running || connection.condition != null){
						FSM.EnterState(connection.targetNode as FSMState);

						//this is done for editor
						connection.connectionStatus = Status.Success;
						//

						return true;
					}
				}
			}

			return false;
		}

		//Exit...
		sealed protected override void OnReset(){

			elapsedTime = 0;
			Exit();
		}

		//Converted
		virtual protected void Init(){}
		virtual protected void Enter(){}
		virtual protected void Stay(){}
		virtual protected void Exit(){}
		virtual protected void Pause(){}
		//

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		private static Port clickedPort{get;set;}
		private Connection picked{get;set;}

		class Port{

			public FSMState parent;
			public Vector2 pos;

			public Port(FSMState parent, Vector2 pos){
				this.parent = parent;
				this.pos = pos;
			}
		}

		protected override void OnCreate(){
			enabled = false;
		}

		sealed public override void DrawNodeConnections(){

			var e = Event.current;

			if (maxOutConnections == 0){
				if (e.type == EventType.MouseUp && ID == graph.allNodes.Count){
					clickedPort = null;
					//e.Use();
				}
				return;
			}

			var portRectLeft = new Rect(0,0,20,20);
			var portRectRight = new Rect(0,0,20,20);
			var portRectBottom = new Rect(0,0,20,20);

			portRectLeft.center = new Vector2(nodeRect.x - 11, nodeRect.yMax - 10);
			portRectRight.center = new Vector2(nodeRect.xMax + 11, nodeRect.yMax - 10);
			portRectBottom.center = new Vector2(nodeRect.center.x, nodeRect.yMax + 11);

			EditorGUIUtility.AddCursorRect(portRectLeft, MouseCursor.ArrowPlus);
			EditorGUIUtility.AddCursorRect(portRectRight, MouseCursor.ArrowPlus);
			EditorGUIUtility.AddCursorRect(portRectBottom, MouseCursor.ArrowPlus);

			GUI.color = new Color(1,1,1,0.3f);
			GUI.Box(portRectLeft, "", "arrowLeft");
			GUI.Box(portRectRight, "", "arrowRight");
			if (maxInConnections == 0)
				GUI.Box(portRectBottom, "", "arrowBottom");
			GUI.color = Color.white;

			if (e.button == 0 && e.type == EventType.MouseDown){
				
				if (portRectLeft.Contains(e.mousePosition)){
					clickedPort = new Port(this, portRectLeft.center);
					e.Use();
				}
				
				if (portRectRight.Contains(e.mousePosition)){
					clickedPort = new Port(this, portRectRight.center);
					e.Use();
				}

				if (maxInConnections == 0 && portRectBottom.Contains(e.mousePosition)){
					clickedPort = new Port(this, portRectBottom.center);
					e.Use();
				}
			}

			if (clickedPort != null && clickedPort.parent == this)
				Handles.DrawBezier(clickedPort.pos, e.mousePosition, clickedPort.pos, e.mousePosition, new Color(0.5f,0.5f,0.8f,0.8f), null, 2);

			if (clickedPort != null && e.type == EventType.MouseUp){
				
				var port = clickedPort;

				if (ID == graph.allNodes.Count){
					clickedPort = null;
					e.Use();
				}

				if (nodeRect.Contains(e.mousePosition)){
					foreach(FSMConnection connection in inConnections){
						if (connection.sourceNode == port.parent){
							Debug.LogWarning("State is already connected to target state. Consider using a 'ConditionList' on the existing transition if you want to check multiple conditions");
							clickedPort = null;
							e.Use();
							return;
						}
					}
					graph.ConnectNode(port.parent, this);
					clickedPort = null;
					e.Use();
				}
			}

			for (int i = 0; i < outConnections.Count; i++){

				FSMConnection connection = outConnections[i] as FSMConnection;
				var targetPos = (connection.targetNode as FSMState).GetConnectedInPortPosition(connection);
				var sourcePos = Vector2.zero;

				if (nodeRect.center.x <= targetPos.x)
					sourcePos = portRectRight.center;
				
				if (nodeRect.center.x > targetPos.x)
					sourcePos = portRectLeft.center;

				if (maxInConnections == 0 && nodeRect.center.y < targetPos.y - 50 && Mathf.Abs(nodeRect.center.x - targetPos.x) < 200 )
					sourcePos = portRectBottom.center;

				connection.DrawConnectionGUI(sourcePos, targetPos);
			}
		}

		Vector2 GetConnectedInPortPosition(Connection connection){

			var sourcePos = connection.sourceNode.nodeRect.center;
			var thisPos = nodeRect.center;

			if (sourcePos.x > nodeRect.x && sourcePos.x < nodeRect.xMax)
				return new Vector2(nodeRect.center.x, nodeRect.y);
				//return new Vector2(nodeRect.xMax, nodeRect.y + 10);
			
			if (sourcePos.y > nodeRect.y - 100 && sourcePos.y < nodeRect.yMax){
				if (sourcePos.x <= thisPos.x)
					return new Vector2(nodeRect.x, nodeRect.y + 10);
				if (sourcePos.x > thisPos.x)
					return new Vector2(nodeRect.xMax, nodeRect.y + 10);
			}

			if (sourcePos.y <= thisPos.y)
				return new Vector2(nodeRect.center.x, nodeRect.y);
			if (sourcePos.y > thisPos.y)
				return new Vector2(nodeRect.center.x, nodeRect.yMax);

			return thisPos;
		}
		
		protected override void OnNodeGUI(){

			if (inIconMode)
				GUILayout.Label("<i>" + nodeName + "</i>");

			if (Application.isPlaying){
				if (allowAsPrime && Event.current.type == EventType.MouseDown && Event.current.alt)
					FSM.EnterState(this);
			} 
		}

		protected override void OnNodeInspectorGUI(){

			ShowBaseFSMInspectorGUI();
			DrawDefaultInspector();
		}

		protected override void OnContextMenu(GenericMenu menu){
			
			if (allowAsPrime){
				if (Application.isPlaying){
					menu.AddItem (new GUIContent ("Enter State (ALT+Click)"), false, delegate{FSM.EnterState(this);});
				} else {
					menu.AddDisabledItem(new GUIContent ("Enter State (ALT+Click)"));
				}
			}
		}

		void ShowBaseFSMInspectorGUI(){

			EditorUtils.CoolLabel("Transitions");

			if (outConnections.Count == 0){
				GUI.backgroundColor = new Color(1,1,1,0.5f);
				GUILayout.BeginHorizontal("box");
				GUILayout.Label("No Transitions");
				GUILayout.EndHorizontal();
				GUI.backgroundColor = Color.white;
			}

			var onFinishExists = false;
			EditorUtils.ReorderableList(outConnections, delegate(int i){

				FSMConnection connection = (FSMConnection)outConnections[i];
				GUI.backgroundColor = new Color(1,1,1,0.5f);
				GUILayout.BeginHorizontal("box");
				if (connection.condition){
					GUILayout.Label(connection.condition.summaryInfo);
				} else {
					GUILayout.Label("OnFinish" + (onFinishExists? " (exists)" : "" ));
					onFinishExists = true;
				}

				GUILayout.FlexibleSpace();
				GUILayout.Label("--> '" + connection.targetNode.nodeName + "'");
				if (GUILayout.Button(">"))
					Graph.currentSelection = connection;

				GUILayout.EndHorizontal();
				GUI.backgroundColor = Color.white;
			});

			if (this.GetType() != typeof(FSMAnyState))
				transitionEvaluation = (TransitionEvaluation)EditorGUILayout.EnumPopup(transitionEvaluation);

			EditorUtils.BoldSeparator();
		}

		#endif
	}
}