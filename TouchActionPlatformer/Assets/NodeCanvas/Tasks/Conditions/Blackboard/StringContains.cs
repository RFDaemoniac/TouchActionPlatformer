using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	public class StringContains : ConditionTask {

		[RequiredField] [BlackboardOnly]
		public BBString targetString;
		public BBString checkString;

		protected override bool OnCheck(){
			return targetString.value.Contains(checkString.value);
		}
	}
}