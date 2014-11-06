using UnityEngine;
using System.Collections;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	[AgentType(typeof(Blackboard))]
	[Description("Loads the blackboard variables previously saved if at all. Returns false if no saves found of load was failed")]
	public class LoadBlackboard : ActionTask {

		protected override void OnExecute(){
			EndAction( (agent as Blackboard).Load() );
		}
	}
}