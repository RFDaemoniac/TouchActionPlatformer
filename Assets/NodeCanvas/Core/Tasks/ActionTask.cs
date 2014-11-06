#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

namespace NodeCanvas{

	///Base class for all actions. Extend this to create your own.
	abstract public class ActionTask : Task{

		[SerializeField] [HideInInspector]
		private float _deltaDelay;
		private System.Action<System.ValueType> FinishCallback;

		public float deltaDelay{
			get {return _deltaDelay;}
			set {_deltaDelay = value;}
		}

		///The time in seconds this action is running if at all
		public float elapsedTime{get; private set;}

		///Is the action currently running?
		public bool isRunning{get; private set;}

		///Is the action currently paused?
		public bool isPaused{get; private set;}

		///The estimated length this action will take to complete
		virtual public float estimatedLength{get; private set;}

		sealed override public string summaryInfo{
			get {return (agentIsOverride? "* " : "") + info;}
		}

		//Override in your own actions to provide the visible editor action info whenever it's shown
		virtual protected string info{
			get {return name;}
		}

		
		////////
		////////

		public void ExecuteAction(Component agent, System.Action<System.ValueType> callback){

			ExecuteAction(agent, this.blackboard, callback);
		}

		///Executes the action for the provided agent, optionaly providing a blackboard and a callback function that will be called with a ValueType argument
		///once the action is completed. The argument in most cases will be a boolean specifying if the action did succeed or failed.
		public void ExecuteAction(Component agent, Blackboard blackboard, System.Action<System.ValueType> callback){

			if (!isActive){
				callback(false);
				return;
			}

			if (isRunning)
				return;

			if (!Set(agent, blackboard)){
				isActive = false;
				callback(false);
				return;
			}

			FinishCallback = callback;
			isRunning = true;
			isPaused = false;
			enabled = true;

			OnExecute();

			if (isRunning)
				MonoManager.current.AddMethod(UpdateAction);
		}

		private void UpdateAction(){
			elapsedTime += Time.deltaTime;
			OnUpdate();
		}

		///Override in your own actions. Called once when the actions is executed.
		virtual protected void OnExecute(){

		}

		///Override in your own actions. Called every frame, if and while the action is running and until it ends.
		virtual protected void OnUpdate(){

		}

		///End the action is Success
		public void EndAction(){
			EndAction(true);
		}

		///Ends the action either in success or failure. The callback function (passed on ExecuteAction) is called with the same parameter that this function was called.
		///If not called, the action will run indefinetely.
		public void EndAction(System.ValueType param){

			if (!isRunning && !isPaused)
				return;

			//do these if the action actually entered update after all
			if (elapsedTime > 0){
				MonoManager.current.RemoveMethod(UpdateAction);
				estimatedLength = elapsedTime;
				elapsedTime = 0;
			}

			isRunning = false;
			isPaused = false;
			enabled = false;
			OnStop();

			if (FinishCallback != null)
				FinishCallback(param);
			FinishCallback = null;
		}

		///Called whenever the action ends due to any reason.
		virtual protected void OnStop(){

		}

		///Pause the action from updating and calls OnPause
		public void PauseAction(){

			if (!isRunning)
				return;

			MonoManager.current.RemoveMethod(UpdateAction);

			enabled = false;
			isRunning = false;
			isPaused = true;
			OnPause();
		}

		///Called when the action is paused
		virtual protected void OnPause(){

		}

		void OnDestroy(){
			MonoManager.current.RemoveMethod(UpdateAction);
		}

		//////////////////////////////////
		/////////GUI & EDITOR STUFF///////
		//////////////////////////////////
		#if UNITY_EDITOR

		///Editor: Draw the action's controls.
		sealed protected override void SealedInspectorGUI(){
			if (Application.isPlaying){
				if (elapsedTime > 0) GUI.color = Color.yellow;
				EditorGUILayout.LabelField("Elapsed Time", elapsedTime.ToString());
				GUI.color = Color.white;
			}
		}

		#endif
	}
}