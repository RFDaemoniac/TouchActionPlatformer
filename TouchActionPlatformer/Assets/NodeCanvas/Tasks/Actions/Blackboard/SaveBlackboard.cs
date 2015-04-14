using UnityEngine;
using System.Collections;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	[AgentType(typeof(Blackboard))]
	[Description("Saves the blackboard variables to be loaded later on")]
	public class SaveBlackboard : ActionTask {

		protected override void OnExecute(){
			(agent as Blackboard).Save();
			EndAction();
		}
	}
}