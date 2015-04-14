using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class AddStringToList : ActionTask {

		[RequiredField] [BlackboardOnly]
		public BBStringList targetList;
		public BBString targetString;

		protected override void OnExecute(){
			
			targetList.value.Add(targetString.value);
			EndAction(true);
		}
	}
}