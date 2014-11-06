using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	public class SampleCurve : ActionTask {

		[RequiredField]
		public BBAnimationCurve curve;
		[SliderField(0,1)]
		public BBFloat sampleAt;

		[BlackboardOnly]
		public BBFloat saveAs;

		protected override void OnExecute(){
			
			saveAs.value = curve.value.Evaluate(sampleAt.value);
			EndAction();
		}
	}
}