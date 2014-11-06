using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_4_6
using UnityEngine.EventSystems;
#endif


namespace NodeCanvas{

	///Automaticaly added to the agent when needed.
	///Handles forwarding Unity event messages to the Tasks that need them as well as Custom event forwarding.
	///A task can listen to an event message by using [EventListener(param string[])] attribute.
	///This can also be used as a general event message forwarding for other puporses than just tasks
	public class AgentUtilities : MonoBehaviour

		#if UNITY_4_6
			, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IDragHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler, IMoveHandler, ISubmitHandler
		#endif
	{

		private List<Listener> listeners = new List<Listener>();


		#if UNITY_4_6

		public void OnPointerEnter(PointerEventData eventData){
			Send("OnPointerEnter", eventData);
		}

		public void OnPointerExit(PointerEventData eventData){
			Send("OnPointerExit", eventData);
		}

		public void OnPointerDown(PointerEventData eventData){
			Send("OnPointerDown", eventData);
		}

		public void OnPointerUp(PointerEventData eventData){
			Send("OnPointerUp", eventData);
		}

		public void OnPointerClick(PointerEventData eventData){
			Send("OnPointerClick", eventData);
		}

		public void OnDrag(PointerEventData eventData){
			Send("OnDrag", eventData);
		}

		public void OnDrop(BaseEventData eventData){
			Send("OnDrop", eventData);
		}

		public void OnScroll(PointerEventData eventData){
			Send("OnScroll", eventData);
		}

		public void OnUpdateSelected(BaseEventData eventData){
			Send("OnUpdateSelected", eventData);
		}

		public void OnSelect(BaseEventData eventData){
			Send("OnSelect", eventData);
		}

		public void OnDeselect(BaseEventData eventData){
			Send("OnDeselect", eventData);
		}

		public void OnMove(AxisEventData eventData){
			Send("OnMove", eventData);
		}

		public void OnSubmit(BaseEventData eventData){
			Send("OnSubmit", eventData);
		}

		#endif


		void OnAnimatorIK(int layerIndex){
			Send("OnAnimatorIK", layerIndex);
		}

		void OnAnimatorMove(){
			Send("OnAnimatorMove", null);
		}

		void OnBecameInvisible(){
			Send("OnBecameInvisible", null);
		}

		void OnBecameVisible(){
			Send("OnBecameVisible", null);
		}

		void OnCollisionEnter(Collision collisionInfo){
			Send("OnCollisionEnter", collisionInfo);
		}

		void OnCollisionExit(Collision collisionInfo){
			Send("OnCollisionExit", collisionInfo);
		}

		void OnCollisionStay(Collision collisionInfo){
			Send("OnCollisionStay", collisionInfo);
		}

		void OnCollisionEnter2D(Collision2D collisionInfo){
			Send("OnCollisionEnter2D", collisionInfo);
		}

		void OnCollisionExit2D(Collision2D collisionInfo){
			Send("OnCollisionExit2D", collisionInfo);
		}

		void OnCollisionStay2D(Collision2D collisionInfo){
			Send("OnCollisionStay2D", collisionInfo);
		}

		void OnTriggerEnter(Collider other){
			Send("OnTriggerEnter", other);
		}

		void OnTriggerExit(Collider other){
			Send("OnTriggerExit", other);
		}

		void OnTriggerStay(Collider other){
			Send("OnTriggerStay", other);
		}

		void OnTriggerEnter2D(Collider2D other){
			Send("OnTriggerEnter2D", other);
		}

		void OnTriggerExit2D(Collider2D other){
			Send("OnTriggerExit2D", other);
		}

		void OnTriggerStay2D(Collider2D other){
			Send("OnTriggerStay2D", other);
		}

		void OnMouseDown(){
			Send("OnMouseDown", null);
		}

		void OnMouseDrag(){
			Send("OnMouseDrag", null);
		}

		void OnMouseEnter(){
			Send("OnMouseEnter", null);
		}

		void OnMouseExit(){
			Send("OnMouseExit", null);
		}

		void OnMouseOver(){
			Send("OnMouseOver", null);
		}

		void OnMouseUp(){
			Send("OnMouseUp", null);
		}

		void OnCustomEvent(string eventName){
			Send("OnCustomEvent", eventName);
		}


		///Add a listener
		public void Listen(object target, string toMessage){

			foreach ( Listener listener in listeners){
				if (listener.target == target){
					if (!listener.messages.Contains(toMessage))
						listener.messages.Add(toMessage);
					return;
				}
			}

			listeners.Add(new Listener(target, toMessage));
		}

		///Remove a listener completely
		public void Forget(object target){

			foreach ( Listener listener in listeners){
				if (listener.target == target){
					listeners.Remove(listener);
					return;
				}
			}
		}

		///Remove a listener from a specific message
		public void Forget(object target, string forgetMessage){

			foreach ( Listener listener in listeners){
				if (listener.target == target){
					listener.messages.Remove(forgetMessage);
					if (listener.messages.Count == 0)
						listeners.Remove(listener);
					return;
				}
			}
		}

		///Call the functions
		public void Send(string eventName, object arg){

			if (listeners.Count == 0)
				return;

			foreach ( Listener listener in listeners){

				foreach ( string msg in listener.messages){

					if (eventName == msg && listener.target != null){

						var method = listener.target.GetType().NCGetMethod(eventName, true);
						if (method == null){
							Debug.LogWarning("Method '" + eventName + "' not found on subscribed type " + listener.target.GetType().Name);
							return;
						}

						var args = method.GetParameters().Length == 1? new object[]{arg} : null;
						if (method.ReturnType == typeof(IEnumerator)){
							MonoManager.current.StartCoroutine( (IEnumerator)method.Invoke(listener.target, args ));
						} else {
							method.Invoke(listener.target, args);
						}
					}
				}
			}
		}

		///Describes a listener
		class Listener{

			public object target;
			public List<string> messages = new List<string>();

			public Listener(object target, string message){

				this.target = target;
				this.messages.Add(message);
			}
		}
	}
}