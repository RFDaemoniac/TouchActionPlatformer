using UnityEngine;

namespace NodeCanvas.Conditions{
	
	[Category("TouchPlatformer")]
	public class Grounded : ConditionTask{
		
		
		protected override string info{
			get {return "Grounded";}
		}
		
		protected override bool OnCheck(){
			// update
			if (agent.transform.position.y <= 0) return true;
			return false;
		}
	}
}