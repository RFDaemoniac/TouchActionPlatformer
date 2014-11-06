using UnityEngine;
using System.Collections;

namespace NodeCanvas.Conditions{

	[Category("Mecanim")]
	[AgentType(typeof(Animator))]
	abstract public class MecanimConditions : ConditionTask {

		protected Animator animator{
			get {return agent as Animator;}
		}
	}
}