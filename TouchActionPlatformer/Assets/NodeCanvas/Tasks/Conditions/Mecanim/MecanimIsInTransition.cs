using UnityEngine;

namespace NodeCanvas.Conditions{

	[Name("Is In Transition")]
	public class MecanimIsInTransition : MecanimConditions {

		public int layerIndex;

		protected override string info{
			get {return "Mec.Is In Transition";}
		}

		protected override bool OnCheck(){

			return animator.IsInTransition(layerIndex);
		}
	}
}