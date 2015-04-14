#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NodeCanvas{

	[ExecuteInEditMode]
	[Category("✫ Utility")]
	public class ConditionList : ConditionTask{

		public bool allSuccessRequired = true;

		[SerializeField]
		private List<Object> conditions = new List<Object>();

		protected override string info{
			get
			{
				string finalText = conditions.Count != 0? "" : "No Conditions";
				if (conditions.Count > 1)
					finalText += "<b>(" + (allSuccessRequired? "ALL True" : "ANY True") + ")</b>\n";

				for (int i= 0; i < conditions.Count; i++){
					
					if (IsTrullyNull(conditions[i]))
						continue;

					var condition = conditions[i] as ConditionTask;
					if (condition == null){
						finalText += MissingTaskText(conditions[i]) + "\n";
						continue;
					}

					if (condition.isActive)
						finalText += condition.summaryInfo + (i == conditions.Count -1? "" : "\n" );
				}
				return finalText;
			}
		}

		protected override bool OnCheck(){

			int succeedChecks = 0;

			foreach (ConditionTask condition in conditions.OfType<ConditionTask>()){

				if (!condition.isActive){
					succeedChecks ++;
					continue;
				}

				if (condition.CheckCondition(agent, blackboard)){

					if (!allSuccessRequired)
						return true;

					succeedChecks ++;
				
				} else {
					
					if (allSuccessRequired)
						return false;
				}
			}

			return succeedChecks == conditions.Count;
		}

		protected override void OnGizmos(){
			foreach (ConditionTask condition in conditions.OfType<ConditionTask>())
				condition.DrawGizmos();
		}

		protected override void OnGizmosSelected(){
			foreach (ConditionTask condition in conditions.OfType<ConditionTask>())
				condition.DrawGizmosSelected();
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

		private ConditionTask currentViewCondition;

		private void OnDestroy(){
			EditorApplication.delayCall += delegate {
				foreach (Object o in conditions)
					if (o) DestroyImmediate(o, true);
			};
		}

		protected override void OnEditorValidate(){
			ValidateList();
		}

		void ValidateList(){
			for (int i = 0; i < conditions.Count; i++){
				if (IsTrullyNull(conditions[i]))
					conditions.RemoveAt(i);
			}			
		}

		public override Task CopyTo(GameObject go){

			if (this == null)
				return null;

			var newList = (ConditionList)go.AddComponent<ConditionList>();
			Undo.RegisterCreatedObjectUndo(newList, "Copy List");
			Undo.RecordObject(newList, "Copy List");
			EditorUtility.CopySerialized(this, newList);
			newList.conditions.Clear();

			foreach (ConditionTask condition in conditions.OfType<ConditionTask>()){
				var copiedCondition = condition.CopyTo(go);
				newList.AddCondition(copiedCondition as ConditionTask);
			}

			return newList;
		}

		override protected void OnTaskInspectorGUI(){

			ShowListGUI();
			ShowNestedConditionsGUI();

			if (GUI.changed && this != null)
	            EditorUtility.SetDirty(this);
		}

		public void ShowListGUI(){

			if (this == null)
				return;

			EditorUtils.TaskSelectionButton(gameObject, typeof(ConditionTask), delegate(Task c){ AddCondition((ConditionTask)c) ;});

			ValidateList();

			if (conditions.Count == 0){
				EditorGUILayout.HelpBox("No Conditions", MessageType.None);
				return;
			}
			
			EditorUtils.ReorderableList(conditions, delegate(int i){

				var o = conditions[i];
				var condition = conditions[i] as ConditionTask;
				GUI.color = new Color(1, 1, 1, 0.25f);
				GUILayout.BeginHorizontal("box");

				if (condition != null){

					GUI.color = condition.isActive? new Color(1,1,1,0.8f) : new Color(1,1,1,0.25f);

					Undo.RecordObject(condition, "Mute");
					condition.isActive = EditorGUILayout.Toggle(condition.isActive, GUILayout.Width(18));

					GUI.backgroundColor = condition == currentViewCondition? Color.grey : Color.white;
					if (GUILayout.Button(EditorUtils.viewIcon, GUILayout.Width(25), GUILayout.Height(18)))
						currentViewCondition = condition == currentViewCondition? null : condition;
					EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
					GUI.backgroundColor = Color.white;
					GUILayout.Label(condition.summaryInfo);
				
				} else {

					GUILayout.Label(MissingTaskText(o));
					GUI.color = Color.white;
				}

				if (GUILayout.Button("X", GUILayout.MaxWidth(20))){
					Undo.RecordObject(this, "List Remove Task");
					conditions.RemoveAt(i);
					Undo.DestroyObjectImmediate(o);
				}

				EditorGUIUtility.AddCursorRect(GUILayoutUtility.GetLastRect(), MouseCursor.Link);
				GUILayout.EndHorizontal();
				GUI.color = Color.white;
			});

			EditorUtils.Separator();

			if (conditions.Count > 1){
				GUI.backgroundColor = new Color(0.5f,0.5f,0.5f);
				if (GUILayout.Button(allSuccessRequired? "ALL True Required":"ANY True Suffice"))
					allSuccessRequired = !allSuccessRequired;
				GUI.backgroundColor = Color.white;
			}
		}


		public void ShowNestedConditionsGUI(){

			if (conditions.Count == 1)
				currentViewCondition = conditions[0] as ConditionTask;

			if (currentViewCondition){
				EditorUtils.BoldSeparator();
				currentViewCondition.ShowInspectorGUI();
			}
		}

		public void AddCondition(ConditionTask condition){
			Undo.RecordObject(this, "List Add Task");
			Undo.RecordObject(condition, "List Add Task");
			currentViewCondition = condition;
			conditions.Add(condition);
			condition.SetOwnerSystem(ownerSystem);
		}

		#endif
	}
}