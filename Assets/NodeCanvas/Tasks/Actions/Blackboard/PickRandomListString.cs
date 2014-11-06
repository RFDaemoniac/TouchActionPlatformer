using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class PickRandomListString : ActionTask {

		[RequiredField]
		public BBStringList targetList;
		[BlackboardOnly]
		public BBString saveAs;

		protected override void OnExecute(){

			if (targetList.value.Count <= 0){
				EndAction(false);
				return;
			}

			saveAs.value = targetList.value[ Random.Range(0, targetList.value.Count) ];
			EndAction(true);
		}
	}
}