using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	[Description("Add a number of strings to the target string list variable")]
	public class SetListStrings : ActionTask {

		[RequiredField] [BlackboardOnly]
		public BBStringList targetList;
		public List<BBString> stringsToAdd = new List<BBString>();
		public bool onlyIfNotContained = true;

		protected override string info{
			get {return "Add " + stringsToAdd.Count.ToString() + " strings to " + targetList; }
		}

		protected override void OnExecute(){

			foreach (BBString bbString in stringsToAdd){
				if (onlyIfNotContained && targetList.value.Contains(bbString.value))
					continue;
				targetList.value.Add(bbString.value);
			}

			EndAction();
		}
	}
}