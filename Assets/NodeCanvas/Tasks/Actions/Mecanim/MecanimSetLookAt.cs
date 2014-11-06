using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set Mecanim Look At")]
	[EventListener("OnAnimatorIK")]
	public class MecanimSetLookAt : MecanimActions{

		public BBGameObject targetPosition;
		public BBFloat targetWeight;

		protected override string info{
			get{return "Mec.SetLookAt " + targetPosition;}
		}

		public void OnAnimatorIK(){

			animator.SetLookAtPosition(targetPosition.value.transform.position);
			animator.SetLookAtWeight(targetWeight.value);
			EndAction();
		}
	}
}