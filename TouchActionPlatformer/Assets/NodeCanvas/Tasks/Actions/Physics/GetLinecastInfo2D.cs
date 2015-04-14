using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Physics")]
	[AgentType(typeof(Transform))]
	public class GetLinecastInfo2D : ActionTask {

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

		protected override void OnExecute(){

            var hit = Physics2D.Linecast(agent.transform.position, target.value.transform.position, mask);

            if (hit.collider != null) {

                saveHitGameObjectAs.value = hit.collider.gameObject;
                saveDistanceAs.value = hit.fraction;
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