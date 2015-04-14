using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class PickRandomListFloat : ActionTask {

		[RequiredField]
		public BBFloatList targetList;
		[BlackboardOnly]
		public BBFloat saveAs;

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