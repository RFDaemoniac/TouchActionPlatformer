using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Name("Check Mecanim Bool")]
	public class MecanimCheckBool : MecanimConditions {

		[RequiredField]
		public string mecanimParameter;
		public BBBool value;

		protected override string info{
			get{return "Mec.Bool '" + mecanimParameter + "' == " + value;}
		}

		protected override bool OnCheck(){

			return animator.GetBool(mecanimParameter) == value.value;
		}
	}
}