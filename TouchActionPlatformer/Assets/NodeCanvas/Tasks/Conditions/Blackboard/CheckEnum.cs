using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("✫ Blackboard")]
	public class CheckEnum : ConditionTask {

		[BlackboardOnly]
		public BBEnum valueA;
		public BBEnum valueB;

		protected override string info{
			get {return valueA + " == " + valueB;}
		}

		protected override bool OnCheck(){
			return Equals(valueA.value, valueB.value);
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnTaskInspectorGUI(){
		
			EditorUtils.BBVariableField("Value A", valueA);
			(valueB as IMultiCastable).type = typeof(System.Enum);
			
			if (!valueA.isNone){
				(valueB as IMultiCastable).type = valueA.value.GetType();
				EditorUtils.BBVariableField("Value B", valueB);
			}
		}

		#endif
	}
}