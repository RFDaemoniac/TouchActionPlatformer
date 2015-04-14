using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class PickListObject : ActionTask {

		[RequiredField]
		public BBGameObjectList targetList;
		public BBInt index;
		[BlackboardOnly]
		public BBGameObject saveAs;

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