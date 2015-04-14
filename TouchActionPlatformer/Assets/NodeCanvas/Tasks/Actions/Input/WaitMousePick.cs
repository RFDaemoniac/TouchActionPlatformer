using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Input")]
	public class WaitMousePick : ActionTask {

		public enum ButtonKeys{
			Left = 0,
			Right = 1,
			Middle = 2
		}
		
		public ButtonKeys buttonKey;
		public LayerMask mask = -1;
		[BlackboardOnly]
		public BBGameObject saveObjectAs;
		[BlackboardOnly]
		public BBFloat saveDistanceAs;
		[BlackboardOnly]
		public BBVector savePositionAs;

		private int buttonID;
		private RaycastHit hit;

		protected override void OnUpdate(){
			
			buttonID = (int)buttonKey;
			if (Input.GetMouseButtonDown(buttonID)){
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask)){

					savePositionAs.value = hit.point;
					saveObjectAs.value = hit.collider.gameObject;
					saveDistanceAs.value = hit.distance;
					EndAction(true);
				}
			}
		}
	}
}