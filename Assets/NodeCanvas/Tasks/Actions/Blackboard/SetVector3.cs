using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	[Description("Set a blackboard Vector3 variable")]
	public class SetVector3 : ActionTask {

		[BlackboardOnly]
		public BBVector valueA;
		public OperationMethod operation;
		public BBVector valueB;

		protected override string info{
			get {return valueA + TaskTools.GetOperationString(operation) + valueB;}
		}

		protected override void OnExecute(){
			valueA.value = (Vector3)TaskTools.Operate(valueA.value, valueB.value, operation);
			EndAction();
		}
	}
}