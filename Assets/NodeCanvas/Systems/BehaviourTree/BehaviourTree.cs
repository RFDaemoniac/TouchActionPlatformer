using UnityEngine;
using System.Collections;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	///The actual Behaviour Tree
	public class BehaviourTree : Graph{

		///Should the tree repeat forever?
		public bool runForever = true;
		///The frequency in seconds for the tree to repeat if set to run forever.
		public float updateInterval = 0;
		///This event is called when the root status of the behaviour is changed
		public event System.Action<BehaviourTree, Status> onRootStatusChanged;

		private float intervalCounter = 0;
		private Status _rootStatus = Status.Resting;

		///The last status of the root
		public Status rootStatus{
			get{return _rootStatus;}
			private set
			{
				if (_rootStatus != value){
					_rootStatus = value;
					if (onRootStatusChanged != null)
						onRootStatusChanged(this, value);
				}
			}
		}

		public override System.Type baseNodeType{
			get {return typeof(BTNodeBase);}
		}

		protected override void OnGraphStarted(){

			intervalCounter = updateInterval;
			rootStatus = primeNode.status;
		}

		protected override void OnGraphUpdate(){

			if (intervalCounter >= updateInterval){
				intervalCounter = 0;
				if ( Tick(agent, blackboard) != Status.Running && !runForever)
					StopGraph();
			}

			intervalCounter += Time.deltaTime;
		}

		///Tick the tree once for the provided agent and with the provided blackboard
		public Status Tick(Component agent, Blackboard blackboard){

			if (rootStatus != Status.Running)
				primeNode.ResetNode();

			rootStatus = primeNode.Execute(agent, blackboard);
			return rootStatus;
		}


		////////////////////////////////////////
		#if UNITY_EDITOR
		
		[UnityEditor.MenuItem("Window/NodeCanvas/Create Behaviour Tree")]
		public static void CreateBehaviourTree(){
			BehaviourTree newBT = new GameObject("BehaviourTree").AddComponent(typeof(BehaviourTree)) as BehaviourTree;
			UnityEditor.Selection.activeObject = newBT;
		}		
		#endif
	}
}