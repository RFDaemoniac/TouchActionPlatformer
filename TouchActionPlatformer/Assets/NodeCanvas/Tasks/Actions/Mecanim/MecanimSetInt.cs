using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set Mecanim Int")]
	public class MecanimSetInt : MecanimActions{

		[RequiredField]
		public BBString parameter;
		public BBInt setTo;

		protected override string info{
			get {return "Mec.SetInt '" + parameter + "' to " + setTo;}
		}

		protected override void OnExecute(){
			animator.SetInteger(parameter.value, setTo.value);
			EndAction();
		}
	}
}