using UnityEngine;
using System.Collections;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[Description("Find the the closest game object of tag to the agent")]
	[AgentType(typeof(Transform))]
	public class FindClosestWithTag : ActionTask {

		[TagField] [RequiredField]
		public BBString searchTag;
		[BlackboardOnly]
		public BBGameObject saveObjectAs;
		[BlackboardOnly]
		public BBFloat saveDistanceAs;

		protected override void OnExecute(){

			var found = GameObject.FindGameObjectsWithTag(searchTag.value).ToList();
			if (found.Count == 0){
				saveObjectAs.value = null;
				saveDistanceAs.value = 0;
				EndAction(false);
				return;
			}

			GameObject closest = null;
			float dist = Mathf.Infinity;
			foreach (GameObject go in found){
				
				if (go == agent.gameObject)
					continue;

				var newDist = Vector3.Distance(go.transform.position, agent.transform.position);
				if (newDist < dist){
					dist = newDist;
					closest = go;
				}
			}

			saveObjectAs.value = closest;
			saveDistanceAs.value = dist;
			EndAction();
		}
	}
}