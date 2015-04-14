using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set IK")]
	[EventListener("OnAnimatorIK")]
	public class MecanimSetIK : MecanimActions{

		public AvatarIKGoal IKGoal;
		[RequiredField]
		public BBGameObject goal;
		public BBFloat weight;

		protected override string info{
			get{return "Set '" + IKGoal + "' " + goal;}
		}

		public void OnAnimatorIK(){

			animator.SetIKPositionWeight(IKGoal, weight.value);
			animator.SetIKPosition(IKGoal, goal.value.transform.position);
			EndAction();
		}
	}
}