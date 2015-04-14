using UnityEngine;

namespace NodeCanvas.Conditions{

	[AgentType(typeof(Transform))]
	[Category("GameObject")]
	public class IsActive : ConditionTask {

		protected override bool OnCheck(){
			return agent.gameObject.activeInHierarchy;
		}
	}
}