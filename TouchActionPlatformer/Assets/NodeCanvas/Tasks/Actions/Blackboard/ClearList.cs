using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class ClearList : ActionTask {

		[VariableType(typeof(IList))] [RequiredField]
		public BBVar targetList;

		protected override void OnExecute(){

			(targetList.value as IList).Clear();
			EndAction(true);
		}
	}
}