using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	public class CheckComponent : ConditionTask {

		[BlackboardOnly]
		public BBComponent valueA;
		public BBComponent valueB;

		protected override string info{
			get {return valueA + " == " + valueB;}
		}

		protected override bool OnCheck(){
			return valueA.value == valueB.value;
		}
	}
}