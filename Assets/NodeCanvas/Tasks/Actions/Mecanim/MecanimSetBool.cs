using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set Mecanim Bool")]
	public class MecanimSetBool : MecanimActions{

		[RequiredField]
		public string mecanimParameter;
		public BBBool setTo;

		protected override string info{
			get{return "Mec.SetBool '" + mecanimParameter + "' to " + setTo;}
		}

		protected override void OnExecute(){

			animator.SetBool(mecanimParameter, (bool)setTo.value);
			EndAction(true);
		}
	}
}