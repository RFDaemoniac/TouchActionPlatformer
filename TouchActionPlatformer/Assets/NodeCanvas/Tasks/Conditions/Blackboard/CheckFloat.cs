using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	public class CheckFloat : ConditionTask{

		[BlackboardOnly]
		public BBFloat valueA;
		public CompareMethod checkType = CompareMethod.EqualTo;
		public BBFloat valueB;

		[SliderField(0,0.1f)]
		public float differenceThreshold = 0.05f;

		protected override string info{
			get	{return valueA + TaskTools.GetCompareString(checkType) + valueB;}
		}

		protected override bool OnCheck(){
			return TaskTools.Compare(valueA.value, valueB.value, checkType, differenceThreshold);
		}
	}
}