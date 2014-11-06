using UnityEngine;
using System.Collections;

namespace NodeCanvas.DialogueTrees{

	[AddComponentMenu("")]
	[Name("Condition")]
	[Description("Execute the first child node if a Condition is true, or the second one if that Condition is false. The Actor selected is used for the Condition check")]
	public class DLGConditionNode : DLGNodeBase, ITaskAssignable<ConditionTask>{

		[SerializeField]
		private Object _condition;

		[HideInInspector]
		public Task task{
			get {return condition;}
			set {condition = (ConditionTask)value;}
		}

		public Object serializedTask{
			get {return _condition;}
		}

		private ConditionTask condition{
			get{return _condition as ConditionTask;}
			set
			{
				_condition = value;
				if (value != null)
					value.SetOwnerSystem(this);
			}
		}

		public override string nodeName{
			get{return base.nodeName + " " + finalActorName;}
		}

		public override int maxOutConnections{
			get {return 2;}
		}

		protected override Status OnExecute(){

			if (outConnections.Count == 0){
				DLGTree.StopGraph();
				return Error("There are no connections.");
			}

			if (!finalActor){
				DLGTree.StopGraph();
				return Error("Actor not found");
			}

			if (!condition){
				Debug.LogWarning("No Condition on Dialoge Condition Node ID " + ID);
				outConnections[0].Execute(finalActor, finalBlackboard);
				return Status.Success;
			}

			if (condition.CheckCondition(finalActor, finalBlackboard)){
				outConnections[0].Execute(finalActor, finalBlackboard);
				return Status.Success;
			}

			if (outConnections.Count == 2){
				outConnections[1].Execute(finalActor, finalBlackboard);
				return Status.Failure;
			}

			graph.StopGraph();
			return Status.Success;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){

			if (outConnections.Count == 0)
				GUILayout.Label("Connect Outcomes");
		}

		#endif
	}
}