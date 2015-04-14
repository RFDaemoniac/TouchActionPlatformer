using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard")]
	public class SetEnum : ActionTask {

		[BlackboardOnly] [RequiredField]
		public BBEnum valueA;
		public BBEnum valueB;

		protected override string info{
			get {return valueA + " = " + valueB;}
		}

		protected override void OnExecute(){
			valueA.value = valueB.value;
			EndAction();
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