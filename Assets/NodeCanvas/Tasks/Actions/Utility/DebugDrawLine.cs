using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Utility")]
	public class DebugDrawLine : ActionTask {

		public BBVector from;
		public BBVector to;
		public Color color = Color.white;
		public float timeToShow = 0.1f;

		protected override void OnExecute(){
			
			Debug.DrawLine(from.value, to.value, color, timeToShow);
			EndAction(true);
		}
	}
}