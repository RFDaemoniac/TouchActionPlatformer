using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Name("Check Line Of Sight")]
	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	[Description("Check of agent is in line of sight with target by doing a linecast and optionaly save the distance")]
	public class CheckLOS : ConditionTask{

		[RequiredField]
		public BBGameObject LosTarget;
		public Vector3 Offset;
		[BlackboardOnly]
		public BBFloat saveDistanceAs;

		private RaycastHit hit = new RaycastHit();

		protected override string info{
			get {return "LOS with " + LosTarget.ToString();}
		}

		protected override bool OnCheck(){

			var t = LosTarget.value.transform;

			if (Physics.Linecast(agent.transform.position + Offset, t.position + Offset, out hit)){
				if (hit.collider != t.collider){
					saveDistanceAs.value = hit.distance;
					return false;
				}
			}

			return true;
		}

		protected override void OnGizmosSelected(){
			if (agent && LosTarget.value)
				Gizmos.DrawLine(agent.transform.position + Offset, LosTarget.value.transform.position + Offset);
		}
	}
}