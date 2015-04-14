using UnityEngine;
using System.Collections;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("Condition")]
	[Description("Check a condition and return Success or Failure")]
	[Icon("Condition")]
	public class BTCondition : BTNodeBase, ITaskAssignable<ConditionTask>{

		[SerializeField]
		private Object _condition;
		[SerializeField]
		private BTCondition _referencedNode;

		public Task task{
			get {return condition;}
			set {condition = (ConditionTask)value;}
		}

		public Object serializedTask{
			get {return _condition;}
		}

		private ConditionTask condition{
			get
			{
				if (referencedNode != null)
					return referencedNode.condition;
				return _condition as ConditionTask;
			}
			set
			{
				if (referencedNode != null) referencedNode.condition = value;
				else _condition = value;

				if (value != null)
					value.SetOwnerSystem(graph);
			}
		}

		public BTCondition referencedNode{
			get {return _referencedNode;}
			private set {_referencedNode = value;}
		}

		public override string nodeName{
			get{return base.nodeName.ToUpper();}
		}

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			if (condition)
				return condition.CheckCondition(agent, blackboard)? Status.Success: Status.Failure;
			return Status.Failure;
		}

		/////////////////////////////////////////
		/////////GUI AND EDITOR STUFF////////////
		/////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){
			
	        if (referencedNode != null)
	        	GUI.Label(new Rect(nodeRect.width - 15, 5, 15, 15), "<b>R</b>");
		}

		protected override void OnNodeInspectorGUI(){

			if (referencedNode != null){
				if (GUILayout.Button("Select Reference"))
					Graph.currentSelection = referencedNode;

				if (GUILayout.Button("Break Reference"))
					BreakReference();
			}
		}

		protected override void OnContextMenu(UnityEditor.GenericMenu menu){
			menu.AddItem (new GUIContent ("Duplicate Referenced"), false, DuplicateReference);
		}
		
		private void DuplicateReference(){
			var newNode = graph.AddNode(typeof(BTCondition)) as BTCondition;
			newNode.nodeRect.center = this.nodeRect.center + new Vector2(50, 50);
			newNode.referencedNode = referencedNode != null? referencedNode : this;
		}

		public void BreakReference(){

			if (referencedNode == null)
				return;

			if (referencedNode.condition != null)
				_condition = (ConditionTask)referencedNode.condition.CopyTo(this.gameObject);

			referencedNode = null;
		}

		#endif
	}
}