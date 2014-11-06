using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	public class CurveTransformTween : ActionTask {

		public enum TransformMode{
			Position,
			Rotation,
			Scale
		}

		public enum TweenMode{
			Absolute,
			Additive
		}

		public enum PlayMode{
			Normal,
			PingPong
		}

		public TransformMode transformMode;
		public TweenMode mode;
		public PlayMode playMode;
		public BBVector targetPosition;
		public BBAnimationCurve curve = new BBAnimationCurve{value = AnimationCurve.EaseInOut(0,0,1,1)};
		public BBFloat time;

		private Vector3 original;
		private Vector3 final;
		private bool ponging = false;

		protected override void OnExecute(){

			if (ponging)
				final = original;

			if (transformMode == TransformMode.Position)
				original = agent.transform.localPosition;
			if (transformMode == TransformMode.Rotation)
				original = agent.transform.localEulerAngles;
			if (transformMode == TransformMode.Scale)
				original = agent.transform.localScale;

			if (!ponging)
				final = targetPosition.value + (mode == TweenMode.Additive? original : Vector3.zero);

			ponging = playMode == PlayMode.PingPong;

			if ( (original - final).magnitude < 0.1f )
				EndAction();
		}

		protected override void OnUpdate(){
			
			var value = Vector3.Lerp(original, final, curve.value.Evaluate( elapsedTime/time.value ) );

			if (transformMode == TransformMode.Position)
				agent.transform.localPosition = value;
			if (transformMode == TransformMode.Rotation)
				agent.transform.localEulerAngles = value;
			if (transformMode == TransformMode.Scale)
				agent.transform.localScale = value;			

			if (elapsedTime >= time.value)
				EndAction(true);
		}
	}
}