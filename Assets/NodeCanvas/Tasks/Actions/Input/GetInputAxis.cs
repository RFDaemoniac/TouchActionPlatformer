using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Input")]
	public class GetInputAxis : ActionTask {

		public string xAxisName = "Horizontal";
		public string yAxisName;
		public string zAxisName = "Vertical";
		public BBFloat multiplier = new BBFloat{value = 1};
		[BlackboardOnly]
		public BBVector saveAs;
		[BlackboardOnly]
		public BBFloat saveXAs;
		[BlackboardOnly]
		public BBFloat saveYAs;
		[BlackboardOnly]
		public BBFloat saveZAs;

		public bool forever;

		protected override void OnExecute(){
			Do();
		}

		protected override void OnUpdate(){
			Do();
		}

		void Do(){

			var x = string.IsNullOrEmpty(xAxisName)? 0 : Input.GetAxis(xAxisName);
			var y = string.IsNullOrEmpty(yAxisName)? 0 : Input.GetAxis(yAxisName);
			var z = string.IsNullOrEmpty(zAxisName)? 0 : Input.GetAxis(zAxisName);
			
			saveXAs.value = x * multiplier.value;
			saveYAs.value = y * multiplier.value;
			saveZAs.value = z * multiplier.value;
			saveAs.value = new Vector3(x, y, z) * multiplier.value;
			if (!forever)
				EndAction();
		}
	}
}