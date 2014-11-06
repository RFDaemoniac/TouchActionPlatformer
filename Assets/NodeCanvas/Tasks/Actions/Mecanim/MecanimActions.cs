using UnityEngine;
using System.Collections;

namespace NodeCanvas.Actions{

	[Category("Mecanim")]
	[AgentType(typeof(Animator))]
	abstract public class MecanimActions : ActionTask {

		protected Animator animator{
			get {return (Animator)agent;}
		}
	}
}