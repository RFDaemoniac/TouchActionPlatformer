using UnityEngine;

namespace NodeCanvas.StateMachines{

	///The connection object for FSM nodes. Transitions
	[AddComponentMenu("")]
	public class FSMConnection : ConditionalConnection {

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		protected override TipConnectionStyle tipConnectionStyle{
			get {return TipConnectionStyle.Arrow;}
		}
		
		#endif
	}
}