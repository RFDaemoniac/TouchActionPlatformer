using UnityEngine;
using NodeCanvas.Variables;
using System.Linq;

namespace NodeCanvas.Conditions{

	[Category("Movement")]
	[AgentType(typeof(NavMeshAgent))]
	[Description("Check if a path exists for the agent and optionaly save the resulting path positions")]
	public class PathExists : ConditionTask {

		public BBVector targetPosition;
		[BlackboardOnly]
		public BBVectorList savePathAs;

		protected override bool OnCheck(){
			
			var path = new NavMeshPath();
			(agent as NavMeshAgent).CalculatePath(targetPosition.value, path);
			savePathAs.value = path.corners.ToList();
			return path.status == NavMeshPathStatus.PathComplete;
		}
	}
}