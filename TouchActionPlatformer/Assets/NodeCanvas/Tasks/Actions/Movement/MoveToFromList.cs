using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("Movement")]
	[AgentType(typeof(NavMeshAgent))]
	[Description("Move randomly between various game object positions taken from the list provided")]
	public class MoveToFromList : ActionTask{

		[RequiredField]
		public BBGameObjectList targetList = new BBGameObjectList{useBlackboard = true};
		public BBFloat speed = new BBFloat{value = 3};
		public float keepDistance = 0.1f;

		private int index;
		private Vector3 lastRequest;

		//for faster acccess
		private NavMeshAgent navAgent{
			get {return (NavMeshAgent)agent;}
		}

		protected override string info{
			get {return "Go Random " + targetList;}
		}

		protected override void OnExecute(){

			int newIndex = Random.Range(0, targetList.value.Count);
			while(newIndex == index)
				newIndex = Random.Range(0, targetList.value.Count);
			index = newIndex;

			var targetGo = targetList.value[index];
			if (targetGo == null){
				Debug.LogWarning("List's game object is null on MoveToFromList Action");
				EndAction(false);
				return;
			}

			var targetPos = targetGo.transform.position;

			navAgent.speed = speed.value;
			if ( (navAgent.transform.position - targetPos).magnitude < navAgent.stoppingDistance + keepDistance){
				EndAction(true);
				return;
			}

			Go();
		}

		protected override void OnUpdate(){
			Go();
		}

		void Go(){

			var targetPos = targetList.value[index].transform.position;
			if (lastRequest != targetPos){
				if ( !navAgent.SetDestination( targetPos) ){
					EndAction(false);
					return;
				}
			}

			lastRequest = targetPos;

			if (!navAgent.pathPending && navAgent.remainingDistance <= navAgent.stoppingDistance + keepDistance)
				EndAction(true);			
		}

		protected override void OnStop(){

			lastRequest = Vector3.zero;
			if (navAgent.gameObject.activeSelf)
				navAgent.ResetPath();
		}

		protected override void OnPause(){
			OnStop();
		}

		protected override void OnGizmosSelected(){
			if (agent && targetList.value != null){
				foreach (GameObject go in targetList.value){
					if (go)
						Gizmos.DrawSphere(go.transform.position, 0.1f);
				}
			}
		}
	}
}