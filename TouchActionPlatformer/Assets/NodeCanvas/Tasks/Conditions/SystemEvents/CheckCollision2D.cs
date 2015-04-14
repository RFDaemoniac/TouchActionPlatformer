using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("System Events")]
	[Name("Check Collision 2D")]
	[AgentType(typeof(Collider2D))]
	[EventListener("OnCollisionEnter2D", "OnCollisionExit2D")]
	public class CheckCollision2D : ConditionTask {

		public enum CheckTypes
		{
			CollisionEnter = 0,
			CollisionExit  = 1,
			CollisionStay  = 2
		}

		public CheckTypes checkType = CheckTypes.CollisionEnter;
		public bool specifiedTagOnly;
		[TagField]
		public string objectTag = "Untagged";
		
		[BlackboardOnly]
		public BBGameObject saveGameObjectAs;
		[BlackboardOnly]
		public BBVector saveContactPoint;

		private bool stay;

		protected override string info{
			get {return checkType.ToString() + ( specifiedTagOnly? (" '" + objectTag + "' tag") : "" );}
		}

		protected override bool OnCheck(){
			if (checkType == CheckTypes.CollisionStay)
				return stay;
			return false;
		}

		public void OnCollisionEnter2D(Collision2D info){
			
			if (!specifiedTagOnly || info.gameObject.tag == objectTag){
				stay = true;
				if (checkType == CheckTypes.CollisionEnter || checkType == CheckTypes.CollisionStay){
					saveGameObjectAs.value = info.gameObject;
					saveContactPoint.value = info.contacts[0].point;
					YieldReturn(true);
				}
			}
		}

		public void OnCollisionExit2D(Collision2D info){
			
			if (!specifiedTagOnly || info.gameObject.tag == objectTag){
				stay = false;
				if (checkType == CheckTypes.CollisionExit){
					saveGameObjectAs.value = info.gameObject;
					YieldReturn(true);
				}
			}
		}
	}
}