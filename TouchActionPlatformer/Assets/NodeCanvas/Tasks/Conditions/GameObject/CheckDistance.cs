using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	public class CheckDistance : ConditionTask{

		[RequiredField]
		public BBGameObject CheckTarget;
		public CompareMethod checkType = CompareMethod.LessThan;
		public BBFloat distance;

		[SliderField(0,0.1f)]
		public float differenceThreshold = 0.05f;

		protected override string info{
			get {return "Distance " + TaskTools.GetCompareString(checkType) + distance + " to " + CheckTarget;}
		}

		protected override bool OnCheck(){
			return TaskTools.Compare(Vector3.Distance(agent.transform.position, CheckTarget.value.transform.position), distance.value, checkType, differenceThreshold);
		}
	}
}