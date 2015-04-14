#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas{

	///The base class for all Actions and Conditions. You dont actually use or derive this class. Instead derive from ActionTask and ConditionTask
	abstract public class Task : MonoBehaviour{


		///Designates what type of component to get and set the agent from the agent itself on initialization.
		///That component type is also considered required for correct task init.
		[AttributeUsage(AttributeTargets.Class)]
		protected class AgentTypeAttribute : Attribute{
			public Type type;
			public AgentTypeAttribute(Type type){
				this.type = type;
			}
		}

		///Designates that the task requires Unity messages to be forwarded from the agent and to this task
		[AttributeUsage(AttributeTargets.Class)]
		protected class EventListenerAttribute : Attribute{
			public string[] messages;
			public EventListenerAttribute(params string[] args){
				this.messages = args;
			}
		}

		///If the field is deriving Component then it will be retrieved from the agent. The field is also considered Required for correct initialization
		[AttributeUsage(AttributeTargets.Field)]
		protected class GetFromAgentAttribute : Attribute{}

		///A special BBVariable for the task agent
		[Serializable]
		class TaskAgent : BBVariable{

			[SerializeField]
			private bool _isOverride;
			public bool isOverride{
				get {return _isOverride;}
				set {_isOverride = value;}
			}

			//Runtime checks
			public Component current{get;set;}
			
			[SerializeField]
			private Component _value;
			public Component value{
				get
				{
					if (useBlackboard){
						var o = Read<UnityEngine.Object>();
						if (o != null){
							if (o is GameObject)
								return (o as GameObject).transform;
							if (o is Component)
								return (Component)o;
						}
						return null;
					}
					return _value;
				}
				set {_value = value;}
			}

			public override Type varType{
				get {return typeof(UnityEngine.Object);}
			}

			public override string ToString(){
				if (!isOverride) return "<b>owner</b>";
				if (useBlackboard) return base.ToString();
				return string.Format("<b>{0}</b>", _value != null? _value.name : "NULL");
			}
		}

		[SerializeField]
		private MonoBehaviour _ownerSystem;
		[SerializeField]
		private bool _isActive = true;

		[SerializeField]
		private TaskAgent taskAgent = new TaskAgent();
		private Blackboard _blackboard;
		
		//store to avoid spamming
		private Type _agentType;
		private string _taskName;
		private string _taskDescription;
		//


		//These are special so I write them first
		public void SetOwnerSystem(ITaskSystem newOwnerSystem){

			if (newOwnerSystem == null)
				return;

			_ownerSystem = (MonoBehaviour)newOwnerSystem;
			UpdateBBFields(newOwnerSystem.blackboard);
		}

		///The system this task belongs to from which defaults are taken from.
		public ITaskSystem ownerSystem{
			get {return _ownerSystem as ITaskSystem;}
		}

		///The owner system's assigned agent
		private Component ownerAgent{
			get	{return ownerSystem != null? ownerSystem.agent : null;}
		}

		///The owner system's assigned blackboard
		private Blackboard ownerBlackboard{
			get	{return ownerSystem != null? ownerSystem.blackboard : null;}
		}

		///The time in seconds that the owner system is running
		protected float ownerElapsedTime{
			get {return ownerSystem != null? ownerSystem.elapsedTime : 0;}
		}
		///

		///Is the Task active?
		public bool isActive{
			get {return _isActive;}
			set {_isActive = value;}
		}

		///The friendly task name. This can be overriden with the [Name] attribute
		new public string name{
			get
			{
				if (string.IsNullOrEmpty(_taskName)){
					var nameAtt = this.GetType().NCGetAttribute(typeof(NameAttribute), false) as NameAttribute;
					_taskName = nameAtt != null? nameAtt.name : GetType().Name;
					#if UNITY_EDITOR
					_taskName = EditorUtils.SplitCamelCase(_taskName);
					#endif
				}
				return _taskName;
			}
		}

		///The help description of the task if it has any through [Description] attribute
		public string description{
			get
			{
				if (_taskDescription == null ){
					var descAtt = this.GetType().NCGetAttribute(typeof(DescriptionAttribute), true) as DescriptionAttribute;
					_taskDescription = descAtt != null? descAtt.description : string.Empty;
				}
				return _taskDescription;				
			}
		}

		///The type that the agent will be set to by getting component from itself on task initialize. Defined with [AgentType] attribute.
		///You can omit this to keep the agent propagated as is or if there is no need for a specific type.
		virtual public Type agentType{
			get
			{
				if (_agentType == null){
					var typeAtt = this.GetType().NCGetAttribute(typeof(AgentTypeAttribute), true) as AgentTypeAttribute;
					_agentType = typeAtt != null && (typeof(Component).NCIsAssignableFrom(typeAtt.type) || typeAtt.type.NCIsInterface() )? typeAtt.type : null;
				}
				return _agentType;
			}
		}

		///A short summary of what the task will finaly do
		virtual public string summaryInfo{
			get {return name;}
		}

		///Helper summary info to display final agent string within task info if needed
		public string agentInfo{
			get	{ return taskAgent.ToString(); }
		}

		///Is the agent overriden or the default taken from owner system will be used?
		public bool agentIsOverride{
			get { return taskAgent.isOverride; }
			private set
			{
				if (value == false && taskAgent.isOverride == true){
					taskAgent.useBlackboard = false;
					taskAgent.value = null;
				}
				taskAgent.isOverride = value;
			}
		}

		///The current or last executive agent of this task
		protected Component agent{
			get { return taskAgent.current? taskAgent.current : (agentIsOverride? taskAgent.value : ownerAgent); }
		}

		///The current or last blackboard to be used by this task
		protected Blackboard blackboard{
			get
			{
				if (_blackboard == null){
					_blackboard = ownerBlackboard;
					UpdateBBFields(_blackboard);
				}

				return _blackboard;
			}
			private set
			{
				if (_blackboard != value){
					_blackboard = value;
					UpdateBBFields(_blackboard);
				}
			}
		}

		//////////

		protected void Awake(){
			enabled = false;
			InitBBFields();
			OnAwake();
		}

		///Override in your own Tasks. Use this instead of Awake
		virtual protected void OnAwake(){

		}

		//Tasks can start coroutine through MonoManager for even when they are disabled
		new protected Coroutine StartCoroutine(IEnumerator routine){
			return MonoManager.current.StartCoroutine(routine);
		}

		///Sends an event to the owner system to handle (same as calling ownerSystem.SendEvent)
		protected void SendEvent(string eventName){

			if (ownerSystem != null)
				ownerSystem.SendEvent(eventName);
		}

		///Override in your own Tasks. This is called after a NEW agent is set, after initialization and before execution
		///Return null if everything is ok, or a string with the error if not.
		virtual protected string OnInit(){
			return null;
		}

		//Actions and Conditions call this before execution. Returns if the task was sucessfully initialized as well
		protected bool Set(Component newAgent, Blackboard newBB){

			//set blackboard with normal setter first
			blackboard = newBB;

			if (agentIsOverride){
				if (taskAgent.current && taskAgent.value && taskAgent.current.gameObject == taskAgent.value.gameObject)
					return true;
				return Initialize(TransformAgent(taskAgent.value, agentType));
			}

			if (taskAgent.current && taskAgent.current.gameObject == newAgent.gameObject)
				return true;			
			return Initialize(TransformAgent(newAgent, agentType));
		}

		//helper function
		Component TransformAgent(Component newAgent, Type newType){
			return (newType != null && newType != typeof(Component) && newAgent != null)? newAgent.GetComponent(newType) : newAgent;
		}

		//Initialize whenever agent is set to a new value. Essentially usage of the attributes
		bool Initialize(Component newAgent){

			taskAgent.current = newAgent;

			if (newAgent == null){
				Debug.LogError("<b>Task Init:</b> Failed to change Agent to type '" + agentType + "', for Task '" + name + "' or new Agent is NULL. Does the Agent has that Component?", this);
				return false;			
			}

			//Subscribe to events for the agent
			SubscribeToEvents(newAgent);

			//Usage of [RequiredField] and [GetFromAgent] attributes
			foreach (FieldInfo field in this.GetType().NCGetFields()){
				
				var value = field.GetValue(this);

				var requiredAttribute = field.GetCustomAttributes(typeof(RequiredFieldAttribute), true).FirstOrDefault() as RequiredFieldAttribute;
				if (requiredAttribute != null){

					if (value == null || value.Equals(null)){
						Debug.LogError("<b>Task Init:</b> A required field for Task '" + name + "', is not set! Field: '" + field.Name + "' ", this);
						return false;
					}

					if (field.FieldType == typeof(string) && string.IsNullOrEmpty((string)value) ){
						Debug.LogError("<b>Task Init:</b> A required string for Task '" + name + "', is not set! Field: '" + field.Name + "' ", this);
						return false;
					}

					if (typeof(BBVariable).NCIsAssignableFrom(field.FieldType) && (value as BBVariable).isNull ) {
						Debug.LogError("<b>Task Init:</b> A required BBVariable value for Task '" + name + "', is not set! Field: '" + field.Name + "' ", this);
						return false;
					}
				}

				var getterAttribute = field.GetCustomAttributes(typeof(GetFromAgentAttribute), true).FirstOrDefault() as GetFromAgentAttribute;
				if (getterAttribute != null){

					if (typeof(Component).NCIsAssignableFrom(field.FieldType)){

						field.SetValue(this, newAgent.GetComponent(field.FieldType));
						if ( (field.GetValue(this) as UnityEngine.Object) == null){
							Debug.LogError("<b>Task Init:</b> GetFromAgent Attribute failed to get the required component of type '" + field.FieldType + "' from '" + agent.gameObject.name + "'. Does it exist?", agent.gameObject);
							return false;
						}
					
					} else {

						Debug.LogError("<b>Task Init:</b> You've set a GetFromAgent Attribute on a field (" + field.Name + ") whos type does not derive Component on Task '" + name + "'", this);
						return false;
					}
				}

				//BBGameObject has a special attribute [RequiresComponent]. We do that here.
				if (field.FieldType == typeof(BBGameObject)){
					var rc = field.GetCustomAttributes(typeof(RequiresComponentAttribute), true).FirstOrDefault() as RequiresComponentAttribute;
					var bbGO = value as BBGameObject;
					if (rc != null && !bbGO.isNull && bbGO.value.GetComponent(rc.type) == null){
						Debug.LogError("<b>Task Init:</b> BBGameObject requires missing Component of type '" + rc.type + "'.", agent.gameObject);
						return false;
					}
				}
			}

			//let user make further adjustments and inform us if there was an error
			string errorString = OnInit();
			if (errorString != null){
				Debug.LogError("<b>Task Init:</b> " + errorString + ". Task '" + name + "'");
				return false;
			}
			return true;
		}

		//Usage of [EventListener]. This is done in init
		void SubscribeToEvents(Component newAgent){

			var msgAttribute = this.GetType().NCGetAttribute(typeof(EventListenerAttribute), true) as EventListenerAttribute;
			if (msgAttribute != null){
				var agentUtils = newAgent.GetComponent<AgentUtilities>();
				if (agentUtils == null)
					agentUtils = newAgent.gameObject.AddComponent<AgentUtilities>();

				foreach (string msg in msgAttribute.messages)
					agentUtils.Listen(this, msg);
			}
		}

		void UnsubscribeFromEvents(){
			var agentUtils = agent.GetComponent<AgentUtilities>();
			if (agentUtils != null)
				agentUtils.Forget(this);
		}

		//Set the target blackboard for all BBVariables found in class. This is done every time the blackboard of the Task is set to a new value
		void UpdateBBFields(Blackboard bb){
			BBVariable.SetBBFields(bb, this);
			taskAgent.bb = bb;
		}

		//Helper to ensure that BBVariables are not null.
		void InitBBFields(){
			BBVariable.InitBBFields(this);
		}

		sealed public override string ToString(){
			return string.Format("{0} ({1})", name, summaryInfo);
		}


		public void DrawGizmos(){
			OnGizmos();
		}

		public void DrawGizmosSelected(){
			OnGizmosSelected();
		}

		virtual protected void OnGizmos(){}
		virtual protected void OnGizmosSelected(){}


		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR

		[SerializeField]
		private bool _unfolded = true;
		private Texture2D _icon;

		public Texture2D icon{
			get
			{
				if (_icon == null){
					var iconAtt = this.GetType().NCGetAttribute(typeof(IconAttribute), true) as IconAttribute;
					if (iconAtt != null) _icon = (Texture2D)Resources.Load(iconAtt.iconName);
				}
				return _icon;
			}
		}

		public bool unfolded{
			get {return _unfolded;}
			set {_unfolded = value;}
		}

		public System.ObsoleteAttribute isObsolete{
			get	{ return this.GetType().NCGetAttribute(typeof(System.ObsoleteAttribute), true) as System.ObsoleteAttribute;	}
		}

		public static Task copiedTask{get;set;}

		protected void Reset(){
			enabled = false;
			InitBBFields();
			OnEditorReset();
		}

		protected void OnValidate(){
			enabled = false;
			InitBBFields();
			OnEditorValidate();
			if (isObsolete != null)
				Debug.LogWarning(string.Format("You are using an obsolete Task '{0}': <b>'{1}'</b>", name, isObsolete.Message));
		}

		virtual protected void OnEditorReset(){}
		virtual protected void OnEditorValidate(){}

		public void ShowInspectorGUI(bool showTitlebar = true){

			Undo.RecordObject(this, "Task Inspector");
			if (!showTitlebar || ShowTaskTitlebar()){

				if (!string.IsNullOrEmpty(description) )
					EditorGUILayout.HelpBox(description, MessageType.None);

				SealedInspectorGUI();
				ShowAgentField();

				OnTaskInspectorGUI();
			}

			if (GUI.changed && this != null)
				EditorUtility.SetDirty(this);
		}

		virtual protected void SealedInspectorGUI(){}

		///Editor: Optional override to show custom controls whenever the ShowInspectorGUI is called. By default controls will automaticaly show for most types
		virtual protected void OnTaskInspectorGUI(){
			DrawDefaultInspector();
		}

		///Draw an auto editor inspector for this task.
		protected void DrawDefaultInspector(){
			EditorUtils.ShowAutoEditorGUI(this);
		}


		//a Custom titlebar for tasks
		bool ShowTaskTitlebar(){

			//this.hideFlags = HideFlags.HideInInspector;
			this.hideFlags = 0;
			
			GUI.backgroundColor = new Color(1,1,1,0.8f);
			GUILayout.BeginHorizontal("box");
			GUI.backgroundColor = Color.white;

			if (GUILayout.Button("X", GUILayout.Width(20))){
				Undo.DestroyObjectImmediate(this);
				return false;
			}

			GUILayout.Label("<b>" + (this.unfolded? "▼ " :"► ") + this.name + "</b>" + (this.unfolded? "" : "\n<i><size=10>(" + this.summaryInfo + ")</size></i>") );

			if (GUILayout.Button(EditorUtils.csIcon, "label", GUILayout.Width(20), GUILayout.Height(20)))
				AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour(this));

			GUILayout.EndHorizontal();
			var titleRect = GUILayoutUtility.GetLastRect();
			EditorGUIUtility.AddCursorRect(titleRect, MouseCursor.Link);

			var e = Event.current;

			if (e.type == EventType.ContextClick && titleRect.Contains(e.mousePosition)){
				var menu = new GenericMenu();
				menu.AddItem(new GUIContent("Open Script"), false, delegate{AssetDatabase.OpenAsset(MonoScript.FromMonoBehaviour(this)) ;} );
				menu.AddItem(new GUIContent("Copy"), false, delegate{Task.copiedTask = this;} );
				menu.AddItem(new GUIContent("Delete"), false, delegate{Undo.DestroyObjectImmediate(this);} );
				menu.ShowAsContext();
				e.Use();
			}

			if (e.button == 0 && e.type == EventType.MouseDown && titleRect.Contains(e.mousePosition))
				e.Use();

			if (e.button == 0 && e.type == EventType.MouseUp && titleRect.Contains(e.mousePosition)){
				this.unfolded = !this.unfolded;
				e.Use();
			}
			return this.unfolded;
		}

		virtual public Task CopyTo(GameObject go){

			if (this == null)
				return null;

			Task newTask = (Task)go.AddComponent(this.GetType());
			Undo.RegisterCreatedObjectUndo(newTask, "Copy Task");
			UnityEditor.EditorUtility.CopySerialized(this, newTask);
			return newTask;
		}

		//Shows the agent field
		void ShowAgentField(){

			if (agentType == null)
				return;

			if (Application.isPlaying && agentIsOverride && taskAgent.value == null){
				GUI.color = EditorUtils.lightRed;
				EditorGUILayout.LabelField("Missing Agent Reference!");
				GUI.color = Color.white;
				return;
			}

			Undo.RecordObject(this, "Agent Field Change");

			var isMissing = agent == null || agent.GetComponent(agentType) == null;
			var infoString = isMissing? "<color=#ff5f5f>" + EditorUtils.TypeName(agentType) + "</color>": EditorUtils.TypeName(agentType);

			GUI.color = new Color(1f,1f,1f, agentIsOverride? 1f : 0.5f);
			GUI.backgroundColor = GUI.color;
			GUILayout.BeginVertical("box");
			GUILayout.BeginHorizontal();
			
			if (!taskAgent.useBlackboard){

				if (agentIsOverride){
					taskAgent.value = EditorGUILayout.ObjectField(taskAgent.value, agentType, true) as Component;
					//if (taskAgent.value != null && !agentType.IsAssignableFrom(taskAgent.value.GetType()))
					//	taskAgent.value = taskAgent.value.GetComponent(agentType);

				} else {

					GUILayout.BeginHorizontal();
					var compIcon = EditorGUIUtility.ObjectContent(null, agentType).image;
					if (compIcon != null)
						GUILayout.Label(compIcon, GUILayout.Width(16), GUILayout.Height(16));
					GUILayout.Label("Use Owner (" + infoString + ")");
					GUILayout.EndHorizontal();
				}

			} else {

				GUI.color = new Color(0.9f,0.9f,1f,1f);
				if (taskAgent.bb){

					var dataNames = taskAgent.bb.GetDataNames(typeof(GameObject)).ToList();
					dataNames.AddRange( taskAgent.bb.GetDataNames(agentType));

					if (dataNames.Contains(taskAgent.dataName) || string.IsNullOrEmpty(taskAgent.dataName) ){
						taskAgent.dataName = EditorUtils.StringPopup(taskAgent.dataName, dataNames, false, true);
					} else {
						taskAgent.dataName = EditorGUILayout.TextField(taskAgent.dataName);
					}

				} else {

					taskAgent.dataName = EditorGUILayout.TextField(taskAgent.dataName);

				}
			}

			GUI.color = Color.white;

			if (agentIsOverride){
				if (isMissing)
					GUILayout.Label("(" + infoString + ")", GUILayout.Height(15));
				taskAgent.useBlackboard = EditorGUILayout.Toggle(taskAgent.useBlackboard, EditorStyles.radioButton, GUILayout.Width(18));
			}

			if (!Application.isPlaying)
				agentIsOverride = EditorGUILayout.Toggle(agentIsOverride, GUILayout.Width(18));

			GUILayout.EndHorizontal();
			GUILayout.EndVertical();
			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;

			if (GUI.changed)
				EditorUtility.SetDirty(this);
		}

		#endif
	}
}