using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class PickListFloat : ActionTask {

		[RequiredField]
		public BBFloatList targetList;
		public BBInt index;
		[BlackboardOnly]
		public BBFloat saveAs;

		protected override void OnExecute(){

			if (index.value < 0 || index.value >= targetList.value.Count){
				EndAction(false);
				return;
			}

			saveAs.value = targetList.value[index.value];
			EndAction(true);
		}
	}
}