using UnityEngine;
using System.Collections;

namespace NodeCanvas.StateMachines{

	[AddComponentMenu("")]
	[Name("Action State")]
	[Description("Execute a number of Action Tasks OnEnter. All actions will be stoped OnExit. This state is Finished when all Actions are finished as well")]
	public class FSMActionState : FSMState, ITaskAssignable{

		[SerializeField]
		private ActionList _actionList;

		[HideInInspector]
		public Task task{
			get {return actionList;}
			set {actionList = (ActionList)value;}
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

		protected override void Enter(){

			if (!actionList){
				graph.StopGraph();
				return;
			}

			actionList.ExecuteAction(graphAgent, graphBlackboard, Finish);
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

			base.OnNodeInspectorGUI();

			if (!actionList)
				return;

			EditorUtils.CoolLabel("Actions");
			actionList.ShowListGUI();
			actionList.ShowNestedActionsGUI();
		}

		#endif
	}
}