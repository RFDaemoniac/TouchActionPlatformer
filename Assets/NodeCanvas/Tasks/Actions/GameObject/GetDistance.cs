using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	public class GetDistance : ActionTask {

		[RequiredField]
		public BBGameObject target;
		[BlackboardOnly]
		public BBFloat saveAs;

		protected override void OnExecute(){
			
			saveAs.value = Vector3.Distance(agent.transform.position, target.value.transform.position);
			EndAction();
		}
	}
}