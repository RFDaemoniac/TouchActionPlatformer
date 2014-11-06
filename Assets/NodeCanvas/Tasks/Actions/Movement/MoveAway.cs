using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Movement")]
	[AgentType(typeof(Transform))]
	public class MoveAway : ActionTask {

		[RequiredField]
		public BBGameObject target;
		public BBFloat speed = new BBFloat{value = 2};
		public BBFloat stopDistance = new BBFloat{value = 3f};
		public bool repeat;

		protected override void OnExecute(){Move();}
		protected override void OnUpdate(){Move();}

		void Move(){
			
			if ( (agent.transform.position - target.value.transform.position).magnitude < stopDistance.value )
				agent.transform.position = Vector3.MoveTowards(agent.transform.position, target.value.transform.position, -speed.value * Time.deltaTime);
			
			if (!repeat)
				EndAction();
		}
	}
}