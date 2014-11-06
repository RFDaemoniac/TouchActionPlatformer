using UnityEngine;
using System.Linq;
using NodeCanvas;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[AgentType(typeof(Transform))]
	[Category("✫ Blackboard/Lists")]
	[Description("Will sort the gameobjects in the target list by their distance to the agent (closer first) and save that list to the blackboard")]
	public class SortObjectListByDistance : ActionTask {

		[RequiredField]
		public BBGameObjectList targetList;
		[BlackboardOnly]
		public BBGameObjectList saveAs;
		public bool reverse;

		protected override string info{
			get {return "Sort " + targetList + " by distance as " + saveAs;}
		}

		protected override void OnExecute(){

			saveAs.value = targetList.value.OrderBy(go => Vector3.Distance(go.transform.position, agent.transform.position)).ToList();
			if (reverse)
				saveAs.value.Reverse();

			EndAction();
		}
	}
}