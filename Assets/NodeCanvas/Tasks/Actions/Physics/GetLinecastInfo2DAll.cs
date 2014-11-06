using UnityEngine;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Physics")]
	[AgentType(typeof(Transform))]
	[Description("Get hit info for ALL objects in the linecast, in Lists")]
	public class GetLinecastInfo2DAll : ActionTask {

		[RequiredField]
		public BBGameObject target;
		public LayerMask mask = -1;
		[BlackboardOnly]
		public BBGameObjectList saveHitGameObjectsAs;
		[BlackboardOnly]
		public BBFloatList saveDistancesAs;
		[BlackboardOnly]
		public BBVectorList savePointsAs;
		[BlackboardOnly]
		public BBVectorList saveNormalsAs;

		protected override void OnExecute(){

            var hits = Physics2D.LinecastAll(agent.transform.position, target.value.transform.position, mask);

            if (hits.Length > 0) {
                saveHitGameObjectsAs.value = hits.Select(h => h.collider.gameObject).ToList();
                saveDistancesAs.value = hits.Select(h => h.fraction).ToList();
                savePointsAs.value = hits.Select(h => h.point).Cast<Vector3>().ToList();
                saveNormalsAs.value = hits.Select(h => h.normal).Cast<Vector3>().ToList();
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