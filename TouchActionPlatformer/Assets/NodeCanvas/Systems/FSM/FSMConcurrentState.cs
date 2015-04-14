using UnityEngine;
using System.Collections;

namespace NodeCanvas.StateMachines{

	[AddComponentMenu("")]
	[Name("Concurrent")]
	[Description("Execute a number of Action Tasks and in parallel to any other state, as soon as the FSM is started. All Action Tasks will prematurely be stoped if the FSM stops as well.\nThis is not a state per se and thus it has no transitions as well as it can't be Entered by transitions.")]
	public class FSMConcurrentState : FSMState, ITaskAssignable{

		[SerializeField]
		private ActionList _actionList;
		private bool hasBeenExecuted;

		public Task task{
			get{return actionList;}
			set{actionList = (ActionList)value;}
		}

		public Object serializedTask{
			get {return _actionList;}
		}

		private ActionList actionList{
			get
			{
				if (_actionList == null){
					_actionList = gameObject.AddComponent<ActionList>();
					_actionList.runInParallel = true;
				}
				return _actionList;
			}
			set
			{
				_actionList = value;
				if (_actionList != null)
					_actionList.SetOwnerSystem(graph);
			}
		}

		public override string nodeName{
			get {return string.Format("<color=#ff64cb>{0}</color>", base.nodeName.ToUpper());}
		}

		public override int maxInConnections{
			get {return 0;}
		}

		public override int maxOutConnections{
			get {return 0;}
		}

		public override bool allowAsPrime{
			get {return false;}
		}

		protected override void Init(){
			hasBeenExecuted = false;
		}

		protected override void Enter(){

			if (!actionList){
				graph.StopGraph();
				return;
			}

			if (hasBeenExecuted){
				Finish();
				return;
			}

			actionList.ExecuteAction(graphAgent, graphBlackboard, OnActionListFinished);
		}

		private void OnActionListFinished(System.ValueType didSucceed){
			Finish();
			hasBeenExecuted = true;
		}

		protected override void Exit(){
			actionList.EndAction(false);
		}

		protected override void Pause(){
			actionList.PauseAction();
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnCreate(){
			actionList = gameObject.AddComponent<ActionList>();
			actionList.runInParallel = true;
		}
	
		protected override void OnNodeInspectorGUI(){

			if (!actionList)
				return;

			EditorUtils.CoolLabel("Actions");
			actionList.ShowListGUI();
			actionList.ShowNestedActionsGUI();
		}

		#endif
	}
}