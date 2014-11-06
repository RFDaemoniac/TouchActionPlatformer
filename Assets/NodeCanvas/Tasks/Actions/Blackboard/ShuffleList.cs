using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	public class ShuffleList : ActionTask {

		[VariableType(typeof(IList))] [RequiredField]
		public BBVar targetList;

		protected override void OnExecute(){
			
			var list = targetList.value as IList;

			for ( int i= list.Count -1; i > 0; i--){
				var j = (int)Mathf.Floor(Random.value * (i + 1));
				var temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}

			EndAction();
		}
	}
}