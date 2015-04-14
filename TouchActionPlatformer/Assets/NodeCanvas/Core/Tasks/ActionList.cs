#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NodeCanvas{

	[Category("✫ Utility")]
	[ExecuteInEditMode]
	///ActionList is an ActionTask itself that though holds multilple Action Tasks which can executes either in parallel or in sequence.
	public class ActionList : ActionTask{

		public bool runInParallel;

		[SerializeField] //Serialized as Object so that we can handle missing/refactored tasks
		private List<Object> actions = new List<Object>();

		private int currentActionIndex;

		public override float estimatedLength{
			get
			{
				float total = 0;
				foreach (ActionTask action in actions.OfType<ActionTask>())
					total += action.estimatedLength;
				return total;			
			}
		}

		protected override string info{
			get
			{
				if (actions.Count == 0)
					return "No Actions";

				var finalText= string.Empty;
				for (int i= 0; i < actions.Count; i++){
					
					if (IsTrullyNull(actions[i]))
						continue;

					var action = actions[i] as ActionTask;
					if (action == null){
						finalText += MissingTaskText(actions[i]) + "\n";
						continue;
					}

					if (action.isActive){
						var prefix = action.isRunning? "► " : action.isPaused? "<b>||</b> " : "";
						finalText += prefix + action.summaryInfo + (i == actions.Count -1? "" : "\n");
					}
				}

				return finalText;	
			}
		}

		protected override void OnExecute(){

			if (actions.Count == 0){
				EndAction();
				return;
			}

			currentActionIndex = 0;
			
			if (runInParallel){
		
				for (int i= 0; i < actions.Count; i++){

					var action = actions[i] as ActionTask;
					if (!action || !action.isActive){
						OnNestedActionEnd(true);
						continue;
					}

					action.ExecuteAction(agent, blackboard, OnNestedActionEnd);
				}

			} else {

				MoveNext();
			}
		}

		void MoveNext(){

			var action = actions[currentActionIndex] as ActionTask;
			if (!action || !action.isActive){
				OnNestedActionEnd(true);
				return;
			}
			action.ExecuteAction(agent, blackboard, OnNestedActionEnd);
		}

		//This is the callback from a nested action
		void OnNestedActionEnd(System.ValueType didSucceed){

			if (!(bool)didSucceed){
				EndAction(false);
				return;
			}

			currentActionIndex ++;

			if (runInParallel){

				if (currentActionIndex == actions.Count){
					EndAction(true);
					return;
				}

			} else {

				if (currentActionIndex < actions.Count)
					MoveNext();
				else
					EndAction(true);
			}
		}

		protected override void OnStop(){
			foreach (ActionTask action in actions.OfType<ActionTask>())
				action.EndAction(false);
		}

		protected override void OnPause(){
			foreach (ActionTask action in actions.OfType<ActionTask>())
				action.PauseAction();			
		}

		protected override void OnGizmos(){
			foreach (ActionTask action in actions.OfType<ActionTask>())
				action.DrawGizmos();			
		}

		protected override void OnGizmosSelected(){
			foreach (ActionTask action in actions.OfType<ActionTask>())
				action.DrawGizmosSelected();
		}

		string MissingTaskText(Object o){
			var s = Equals(o, null)? "NULL ENTRY" : o.ToString();
			s = s.Replace(gameObject.name + " ", "");
			return string.Format("<color=#ff6457>* {0} *</color>", s);
		}

		//meaning that it's not a missing action. We want to keep those in
		bool IsTrullyNull(Object o){
			try {return o == null && o.GetType() != typeof(Object);}
			catch{return true;}
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		private ActionTask currentViewAction;

		private void OnDestroy(){
			EditorApplication.delayCall += delegate {
				foreach (Object o in actions)
					if (o) DestroyImmediate(o, true);
			};
		}

		protected override void OnEditorValidate(){
			ValidateList();
		}

		void ValidateList(){
			for (int i = 0; i < actions.Count; i++){
				if (IsTrullyNull(actions[i]))
					actions.RemoveAt(i);
			}			
		}

		public override Task CopyTo(GameObject go){

			if (this == null)
				return null;

			var newList = (ActionList)go.AddComponent<ActionList>();
			Undo.RegisterCreatedObjectUndo(newList, "Copy List");
			Undo.RecordObject(newList, "Copy List");
			EditorUtility.CopySerialized(this, newList);
			newList.actions.Clear();

			foreach (ActionTask action in actions.OfType<ActionTask>()){
				var copiedAction = action.CopyTo(go);
				newList.AddAction(copiedAction as ActionTask);
			}

			return newList;
		}

		protected override void OnTaskInspectorGUI(){

			ShowListGUI();
			ShowNestedActionsGUI();

			if (GUI.changed && this != null)
	            EditorUtility.SetDirty(this);
		}

		//The action list gui
		public void ShowListGUI(){

			if (this == null)
				return;

			//button to add new actions
			EditorUtils.TaskSelectionButton(gameObject, typeof(ActionTask), delegate(Task a){ AddAction((ActionTask)a); });

			//check list and possibly remove trully null entries
			ValidateList();

			if (actions.Count == 0){
				EditorGUILayout.HelpBox("No Actions", MessageType.None);
				return;
			}

			//show the actions
			EditorUtils.ReorderableList(actions, delegate(int i){

				var o = actions[i];
				var action = actions[i] as ActionTask;
				GUI.color = new Color(1, 1, 1, 0.25f);
				EditorGUILayout.BeginHorizontal("box");

				if (action != null){

					GUI.color = action.isActive? new Color(1,1,1,0.8f) : new Color(1,1,1,0.25f);
					Undo.RecordObject(action, "Mute");
					action.isActive = EditorGUILayout.Toggle(action.isActive, GUILayout.Width(18));

					GUI.backgroundColor = action == currentViewAction? Color.grey : Color.white;
					if (GUILayout.Button(EditorUtils.viewIcon, GUILayout.Width(25), GUILayout.Height(18)))
						currentViewAction = action == currentViewAction? null : action;
					EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
					GUI.backgroundColor = Color.white;

					GUILayout.Label( (action.isRunning? "► " : action.isPaused? "<b>||</b> " : "") + action.summaryInfo);

				} else {

					GUILayout.Label(MissingTaskText(o));
					GUI.color = Color.white;
				}

				if (GUILayout.Button("X", GUILayout.Width(20))){
					Undo.RecordObject(this, "List Remove Task");
					actions.RemoveAt(i);
					Undo.DestroyObjectImmediate(o);
				}

				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				EditorGUILayout.EndHorizontal();
				GUI.color = Color.white;
			});

			if (actions.Count > 1)
				runInParallel = EditorGUILayout.ToggleLeft("Run In Parallel", runInParallel);
		}


		public void ShowNestedActionsGUI(){

			if (actions.Count == 1)
				currentViewAction = actions[0] as ActionTask;

			if (currentViewAction != null){
				EditorUtils.BoldSeparator();
				currentViewAction.ShowInspectorGUI();
			}
		}

		public void AddAction(ActionTask action){
			
			Undo.RecordObject(this, "List Add Task");
			Undo.RecordObject(action, "List Add Task");
			currentViewAction = action;
			actions.Add(action);
			action.SetOwnerSystem(ownerSystem);
		}

		#endif
	}
}