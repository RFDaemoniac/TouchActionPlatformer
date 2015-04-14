using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.BehaviourTrees{

	[Name("Step Iterator")]
	[Category("Composites")]
	[Description("Executes AND immediately returns children node status ONE-BY-ONE. Step Iterator always moves forward by one and loops it's index")]
	[Icon("StepIterator")]
	public class BTStepIterator : BTComposite {

		private int current;

		public override string nodeName{
			get{return string.Format("<color=#bf7fff>{0}</color>", base.nodeName.ToUpper());}
		}

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			current = current % outConnections.Count;
			return outConnections[current].Execute(agent, blackboard);
		}

		protected override void OnReset(){
			current += 1;
		}
	}
}