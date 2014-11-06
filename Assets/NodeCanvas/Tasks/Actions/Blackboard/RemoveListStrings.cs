using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	[Description("Remove a number of strings from the target list")]
	public class RemoveListStrings : ActionTask {

		[RequiredField][BlackboardOnly]
		public BBStringList targetList;

		public List<BBString> stringsToRemove;

		protected override void OnExecute(){
			
			foreach(string s in stringsToRemove.Select(bbString => bbString.value))
				targetList.value.Remove(s);

			EndAction(true);
		}
	}
}