using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	[Description("Set a blackboard float variable")]
	public class SetFloat : ActionTask{

		[BlackboardOnly]
		public BBFloat valueA;
		public OperationMethod Operation = OperationMethod.Set;
		public BBFloat valueB;

		protected override string info{
			get	{return valueA + TaskTools.GetOperationString(Operation) + valueB;}
		}

		protected override void OnExecute(){
			valueA.value = (float)TaskTools.Operate(valueA.value, valueB.value, Operation);
			EndAction(true);
		}
	}
}