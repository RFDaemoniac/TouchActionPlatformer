using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	public class IsInFront : ConditionTask{

		[RequiredField]
		public BBGameObject CheckTarget;
		[SliderField(1, 180)]
		public float AngleToCheck = 70f;

		protected override string info{
			get {return CheckTarget + " in front";}
		}

		protected override bool OnCheck(){
			return Vector3.Angle(CheckTarget.value.transform.position - agent.transform.position, agent.transform.forward) < AngleToCheck;
		}
	}
}