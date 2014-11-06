using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	[Description("Checks if target string is contained within the target list")]
	public class ListContainsString : ConditionTask {

		[RequiredField] [BlackboardOnly]
		public BBStringList targetList;
		public BBString targetString;

		protected override bool OnCheck(){

			return targetList.value.Contains(targetString.value);
		}
	}
}