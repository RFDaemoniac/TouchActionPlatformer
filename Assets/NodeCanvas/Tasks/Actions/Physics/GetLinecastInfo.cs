using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Physics")]
	[AgentType(typeof(Transform))]
	public class GetLinecastInfo : ActionTask {

		[RequiredField]
		public BBGameObject target;
		public LayerMask mask = -1;
		[BlackboardOnly]
		public BBGameObject saveHitGameObjectAs;
		[BlackboardOnly]
		public BBFloat saveDistanceAs;
		[BlackboardOnly]
		public BBVector savePointAs;
		[BlackboardOnly]
		public BBVector saveNormalAs;

		private RaycastHit hit = new RaycastHit();

		protected override void OnExecute(){

			if (Physics.Linecast(agent.transform.position, target.value.transform.position, out hit, mask)){
				saveHitGameObjectAs.value = hit.collider.gameObject;
				saveDistanceAs.value = hit.distance;
				savePointAs.value = hit.point;
				saveNormalAs.value = hit.normal;
				EndAction(true);
				return;
			}

			EndAction(false);
		}

		protected override void OnGizmosSelected(){
			if (agent && target.value)
				Gizmos.DrawLine(agent.transform.position, target.value.transform.position);
		}
	}
}