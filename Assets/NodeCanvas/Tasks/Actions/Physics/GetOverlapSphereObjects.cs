using UnityEngine;
using System.Linq;
using NodeCanvas;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Physics")]
	[Description("Gets a lists of game objects that are in the physics overlap sphere at the position of the agent, excluding the agent")]
	[AgentType(typeof(Transform))]
	public class GetOverlapSphereObjects : ActionTask {

		public LayerMask layerMask = -1;
		public BBFloat radius = new BBFloat{value = 2f};
		[BlackboardOnly]
		public BBGameObjectList saveObjectsAs;

		protected override void OnExecute(){

			var hitColliders = Physics.OverlapSphere(agent.transform.position, radius.value, layerMask);
			saveObjectsAs.value = hitColliders.Select(c => c.gameObject).ToList();
			saveObjectsAs.value.Remove(agent.gameObject);

			if (saveObjectsAs.value.Count == 0){
				EndAction(false);
				return;
			}

			EndAction(true);
		}

		protected override void OnGizmosSelected(){
			if (agent != null){
				Gizmos.color = new Color(1,1,1,0.2f);
				Gizmos.DrawSphere(agent.transform.position, radius.value);
			}
		}
	}
}