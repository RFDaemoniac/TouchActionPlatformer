using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	public class EvaluateCurve : ActionTask {

		[RequiredField]
		public BBAnimationCurve curve;
		public BBFloat from;
		public BBFloat to = new BBFloat{value = 1};
		public BBFloat time = new BBFloat{value = 1};

		[BlackboardOnly]
		public BBFloat saveAs;

		protected override void OnUpdate(){

			saveAs.value = curve.value.Evaluate( Mathf.Lerp(from.value, to.value, elapsedTime/time.value));
			if (elapsedTime > time.value)
				EndAction();
		}
	}
}