using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Name("Set Mecanim Float")]
	public class MecanimSetFloat : MecanimActions{

		[RequiredField]
		public string MecanimParameter;
		public BBFloat SetTo;
		[SliderField(0,1)]
		public float TransitTime= 0.25f;

		private float currentValue;

		protected override string info{
			get {return "Mec.SetFloat '" + MecanimParameter + "' to " + SetTo.ToString();}
		}

		protected override void OnExecute(){

			if (TransitTime <= 0){
				animator.SetFloat(MecanimParameter, SetTo.value);
				EndAction();
				return;
			}

			currentValue = animator.GetFloat(MecanimParameter);
		}

		protected override void OnUpdate(){

			animator.SetFloat(MecanimParameter, Mathf.Lerp(currentValue, SetTo.value, elapsedTime/TransitTime));
			if (elapsedTime >= TransitTime)
				EndAction(true);
		}
	}
}