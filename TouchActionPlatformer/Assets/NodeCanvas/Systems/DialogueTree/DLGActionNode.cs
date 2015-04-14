using UnityEngine;
using System.Collections;
namespace NodeCanvas.DialogueTrees{

	[AddComponentMenu("")]
	[Name("Action")]
	[Description("Execute an Action Task for the Dialogue Actor selected. The Blackboard will be taken from the selected Actor.")]
	public class DLGActionNode : DLGNodeBase, ITaskAssignable<ActionTask>{

		[SerializeField]
		private Object _action;

		[HideInInspector]
		public Task task{
			get {return action;}
			set {action = (ActionTask)value;}
		}

		public Object serializedTask{
			get {return _action;}
		}

		private ActionTask action{
			get {return _action as ActionTask;}
			set
			{
				_action = value;
				if (value != null)
					value.SetOwnerSystem(this);
			}
		}

		public override string nodeName{
			get{return base.nodeName + " " + finalActorName;}
		}

		protected override Status OnExecute(){

			if (!finalActor){
				DLGTree.StopGraph();
				return Error("Actor not found");
			}

			if (!action){
				OnActionEnd(true);
				return Status.Success;
			}

			DLGTree.currentNode = this;

			status = Status.Running;
			action.ExecuteAction(finalActor, finalBlackboard, OnActionEnd);
			return status;
		}

		private void OnActionEnd(System.ValueType success){

			if ( (bool)success ){
				Continue();
				return;
			}

			status = Status.Failure;
			DLGTree.StopGraph();
		}

		protected override void OnReset(){
			if (action)
				action.EndAction(false);
		}

		public override void OnGraphPaused(){
			if (action)
				action.PauseAction();
		}
	}
}