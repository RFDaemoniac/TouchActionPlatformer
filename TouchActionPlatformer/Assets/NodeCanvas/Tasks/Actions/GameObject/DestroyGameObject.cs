﻿using UnityEngine;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[AgentType(typeof(Transform))]
	public class DestroyGameObject : ActionTask {

		protected override string info{
			get {return "Destroy GameObject";}
		}

		protected override void OnUpdate(){

			Destroy(agent.gameObject);
			EndAction();
		}
	}
}