using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.StateMachines{

	///This class is essentially a front-end to executing an FSM
	[AddComponentMenu("NodeCanvas/FSM Owner")]
	public class FSMOwner : GraphOwner {

		public FSM FSM;

		public override Graph behaviour{
			get{return FSM;}
			set {FSM = (FSM)value;}
		}

		public override System.Type graphType{
			get {return typeof(FSM);}
		}

		///The current state name
		public string currentStateName{
			get {return FSM != null? FSM.currentStateName : null;}
		}

		///The last state name
		public string lastStateName{
			get {return FSM != null? FSM.lastStateName : null;}
		}

		///Enter an FSM state by it's name
		public FSMState TriggerState(string stateName){

			if (FSM != null)
				return FSM.TriggerState(stateName);
			return null;
		}

		///Get all state names, excluding non-named states
		public List<string> GetStateNames(){
			if (FSM != null)
				return FSM.GetStateNames();
			return null;
		}
	}
}