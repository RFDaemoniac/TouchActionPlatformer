using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("Optional")]
	[Category("Decorators")]
	[Description("Executes the decorated node without taking into account it's return status, thus making it optional to the parent node.\nThis has the same effect as disabling the node, but instead it executes normaly")]
	public class BTOptional : BTDecorator{

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			if (!decoratedConnection)
				return Status.Resting;

			status = decoratedConnection.Execute(agent, blackboard);
			if (status != Status.Running){
				decoratedConnection.ResetConnection();
				return Status.Resting;
			}

			return Status.Running;
		}
	}
}