#if UNITY_4_6

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("UGUI")]
	[Description("Returns true when the selected event is triggered on the selected agent.\nYou can use this for both GUI and 3D objects.\nPlease make sure that Unity Event Systems are setup correctly")]
	[AgentType(typeof(Transform))]
	public class InterceptEvent : ConditionTask {

		public EventTriggerType eventType;

		protected override string info{
			get {return eventType.ToString();}
		}

		protected override string OnInit(){
			var handler = agent.GetComponent<AgentUtilities>();
			if (handler == null)
				handler = agent.gameObject.AddComponent<AgentUtilities>();
			handler.Listen(this, "On" + eventType.ToString());
			return null;
		}

		protected override bool OnCheck(){
			return false;
		}

		void OnPointerEnter(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerExit(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerDown(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerUp(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnPointerClick(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnDrag(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnDrop(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnScroll(PointerEventData eventData){
			YieldReturn(true);
		}

		void OnUpdateSelected(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnSelect(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnDeselect(BaseEventData eventData){
			YieldReturn(true);
		}

		void OnMove(AxisEventData eventData){
			YieldReturn(true);
		}

		void OnSubmit(BaseEventData eventData){
			YieldReturn(true);
		}
	}
}

#endif