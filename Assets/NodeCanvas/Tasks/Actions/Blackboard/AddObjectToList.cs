using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class AddObjectToList : ActionTask {

		[RequiredField] [BlackboardOnly]
		public BBGameObjectList targetList;
		public BBGameObject targetObject;

		protected override void OnExecute(){
			
			targetList.value.Add(targetObject.value);
			EndAction(true);
		}
	}
}