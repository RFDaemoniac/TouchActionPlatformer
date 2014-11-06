using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[System.Obsolete("Use 'Set List Objects instead'")]
	[Category("✫ Blackboard")]
	[Description("Set a blackboard GameObject list variable")]
	public class SetGameObjectList_obsolete : ActionTask {

		[RequiredField] [BlackboardOnly]
		public BBGameObjectList valueA;
		public BBGameObjectList valueB;

		protected override string info{
			get {return "Set " + valueA + " = " + valueB;}
		}

		protected override void OnExecute(){

			valueA.value = valueB.value;
			EndAction();
		}
	}
}