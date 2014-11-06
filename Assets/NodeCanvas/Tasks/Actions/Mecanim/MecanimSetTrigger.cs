using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set Mecanim Trigger")]
	public class MecanimSetTrigger : MecanimActions{

		[RequiredField]
		public BBString mecanimParameter;

		protected override string info{
			get{return "Mec.SetTrigger " + mecanimParameter;}
		}

		protected override void OnExecute(){

			animator.SetTrigger(mecanimParameter.value);
			EndAction();
		}
	}
}