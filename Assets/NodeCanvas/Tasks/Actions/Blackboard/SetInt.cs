using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	[Description("Set a blackboard float variable")]
	public class SetInt : ActionTask{

		[BlackboardOnly]
		public BBInt valueA;
		public OperationMethod Operation = OperationMethod.Set;
		public BBInt valueB;

		protected override string info{
			get	{return valueA + TaskTools.GetOperationString(Operation) + valueB;}
		}

		protected override void OnExecute(){
			valueA.value = (int)TaskTools.Operate(valueA.value, valueB.value, Operation);
			EndAction(true);
		}
	}
}