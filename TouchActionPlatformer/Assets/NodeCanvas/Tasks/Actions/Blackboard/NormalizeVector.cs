using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	public class NormalizeVector : ActionTask {

		public BBVector targetVector;
		public BBFloat multiply = new BBFloat{value = 1};

		protected override void OnExecute(){
			targetVector.value = targetVector.value.normalized * multiply.value;
			EndAction(true);
		}
	}
}