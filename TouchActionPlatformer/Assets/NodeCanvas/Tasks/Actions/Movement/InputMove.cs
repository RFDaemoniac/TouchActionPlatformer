using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Movement")]
	[Description("Move & turn the agent based on input values provided ranging from -1 to 1. Per frame and in delta time")]
	[AgentType(typeof(Transform))]
	public class InputMove : ActionTask {

        [BlackboardOnly]
        public BBFloat strafe;
		[BlackboardOnly]
        public BBFloat turn;
        [BlackboardOnly]
        public BBFloat forward;
        public BBFloat moveSpeed = new BBFloat { value = 1.0f };
        public BBFloat rotationSpeed = new BBFloat { value = 1.0f };

        public bool repeat;

        protected override void OnExecute(){Move();}
		protected override void OnUpdate(){Move();}
        
        void Move() {
			var targetRotation = agent.transform.rotation * Quaternion.Euler(Vector3.up * turn.value * 10);
			agent.transform.rotation = Quaternion.Slerp(agent.transform.rotation, targetRotation, rotationSpeed.value * Time.deltaTime);

			var forwardMovement = agent.transform.forward * forward.value * moveSpeed.value * Time.deltaTime;
			var strafeMovement = agent.transform.right * strafe.value * moveSpeed.value * Time.deltaTime;
			agent.transform.position += strafeMovement + forwardMovement;

            if (!repeat)
	            EndAction();
        }
	}
}