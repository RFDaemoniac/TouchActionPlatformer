using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class GetListCount : ActionTask {

		[VariableType(typeof(IList))] [RequiredField]
		public BBVar targetList;

		[BlackboardOnly]
		public BBInt saveAs;

		protected override void OnExecute(){

			saveAs.value = (targetList.value as IList).Count;
			EndAction(true);
		}
	}
}