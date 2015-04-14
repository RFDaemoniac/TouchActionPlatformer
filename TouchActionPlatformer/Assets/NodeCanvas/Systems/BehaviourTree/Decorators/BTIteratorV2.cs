using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using NodeCanvas.Variables;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("Iterate V2")]
	[Category("Decorators")]
	[Description("Iterate any type of list and execute the child node for each element in the list. Keeps iterating until the Termination Condition is met or the whole list is iterated and return the child node status")]
	[Icon("List")]
	///Iterate any list
	public class BTIteratorV2 : BTDecorator{

		[BlackboardOnly][VariableType(typeof(IList))]
		public BBVar list;
		[BlackboardOnly]
		public BBVar current;
		public BBInt maxIteration;

		public enum TerminationConditions {FirstSuccess, FirstFailure, None}
		public TerminationConditions terminationCondition = TerminationConditions.None;
		public bool resetIndex = true;

		private int currentIndex;

		private IList iList{
			get {return list != null && list.value != null? list.value as IList : null;}
		}
		
		protected override Status OnExecute(Component agent, Blackboard blackboard){

			if (!decoratedConnection)
				return Status.Resting;

			if (iList == null || iList.Count == 0)
				return Status.Failure;

			current.value = iList[currentIndex];
			status = decoratedConnection.Execute(agent, blackboard);

			if (status == Status.Success && terminationCondition == TerminationConditions.FirstSuccess)
				return Status.Success;

			if (status == Status.Failure && terminationCondition == TerminationConditions.FirstFailure)
				return Status.Failure;

			if (status != Status.Running){

				if (currentIndex == iList.Count - 1 || currentIndex == maxIteration.value - 1)
					return status;

				decoratedConnection.ResetConnection();
				currentIndex ++;
				return Status.Running;
			}

			return status;
		}


		protected override void OnReset(){

			if (resetIndex || currentIndex == iList.Count - 1 || currentIndex == maxIteration.value - 1)
				currentIndex = 0;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

			var leftLabelStyle = new GUIStyle(GUI.skin.GetStyle("label"));
			leftLabelStyle.alignment = TextAnchor.UpperLeft;

			GUILayout.Label("For Each \t" + current + "\nIn \t\t\t'<b>$" + list.dataName + "</b>'", leftLabelStyle);
			if (terminationCondition != TerminationConditions.None)
				GUILayout.Label("Exit on " + terminationCondition.ToString());

			if (Application.isPlaying)
				GUILayout.Label("Index: " + currentIndex.ToString() + " / " + (iList != null && iList.Count != 0? (iList.Count -1).ToString() : "?") );
		}

		protected override void OnNodeInspectorGUI(){
			DrawDefaultInspector();
			var iListType = iList != null? iList.GetType().GetGenericArguments()[0] : typeof(object);
			if (current.type != iListType)
				current.type = iListType;
		}

		#endif
	}
}