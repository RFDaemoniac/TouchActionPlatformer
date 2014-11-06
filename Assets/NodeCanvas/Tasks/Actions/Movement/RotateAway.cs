using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Movement")]
	[AgentType(typeof(Transform))]
	public class RotateAway : ActionTask {

		[RequiredField]
		public BBGameObject target;
		public BBFloat speed;
		[SliderField(1, 180)]
		public BBFloat angleDifference = new BBFloat{value = 5};
		public bool repeat;

		protected override void OnExecute(){Rotate();}
		protected override void OnUpdate(){Rotate();}

		void Rotate(){
			
			if (Vector3.Angle(target.value.transform.position - agent.transform.position, -agent.transform.forward) > angleDifference.value){
				var dir = target.value.transform.position - agent.transform.position;
				agent.transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(agent.transform.forward, dir, -speed.value * Time.deltaTime, 0));
			}
			
			if (!repeat)
				EndAction();
		}
	}
}