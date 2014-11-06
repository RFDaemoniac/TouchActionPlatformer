using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	public class ListIsEmpty : ConditionTask {

		[VariableType(typeof(IList))] [RequiredField]
		public BBVar targetList;

		protected override bool OnCheck(){
			return (targetList.value as IList).Count == 0;
		}
	}
}