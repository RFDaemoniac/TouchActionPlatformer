using UnityEngine;
using System.Collections;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("SubTree")]
	[Category("Nested")]
	[Description("SubTree Node can be assigned an entire Sub BehaviorTree. The prime node of that behaviour will be considered child node of this node and will return whatever it returns")]
	[Icon("BT")]
	public class BTSubTree : BTNodeBase, INestedNode{

		[SerializeField]
		private BehaviourTree _nestedTree;
		private bool instantiated;

		private BehaviourTree nestedTree{
			get {return _nestedTree;}
			set
			{
				_nestedTree = value;
				if (_nestedTree != null){
					_nestedTree.agent = graphAgent;
					_nestedTree.blackboard = graphBlackboard;
				}
			}
		}

		public Graph nestedGraph{
			get {return nestedTree;}
			set {nestedTree = (BehaviourTree)value;}
		}

		public override string nodeName{
			get {return base.nodeName.ToUpper();}
		}

		/////////
		/////////

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			CheckInstance();

			if (nestedTree && nestedTree.primeNode)
				return nestedTree.Tick(agent, blackboard);
				//return nestedTree.primeNode.Execute(agent, blackboard);

			return Status.Success;
		}

		protected override void OnReset(){

			if (nestedTree && nestedTree.primeNode)
				nestedTree.primeNode.ResetNode();
		}

		public override void OnGraphStarted(){
			if (nestedTree){
				CheckInstance();
				foreach(Node node in nestedTree.allNodes)
					node.OnGraphStarted();				
			}
		}

		public override void OnGraphStoped(){
			if (nestedTree){
				foreach(Node node in nestedTree.allNodes)
					node.OnGraphStoped();				
			}			
		}

		public override void OnGraphPaused(){
			if (nestedTree){
				foreach(Node node in nestedTree.allNodes)
					node.OnGraphPaused();
			}
		}

		void CheckInstance(){

			if (!instantiated && nestedTree != null && nestedTree.transform.parent != graph.transform){
				nestedTree = (BehaviourTree)Instantiate(nestedTree, transform.position, transform.rotation);
				nestedTree.transform.parent = graph.transform;
				instantiated = true;	
			}
		}

		////////////////////////////
		//////EDITOR AND GUI////////
		////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){
		    
		    if (nestedTree){

		    	GUILayout.Label("'" + nestedTree.name + "'");

			} else {
				
				if (GUILayout.Button("CREATE NEW"))
					nestedTree = (BehaviourTree)Graph.CreateNested(this, typeof(BehaviourTree), "SubTree");
			}
		}

		protected override void OnNodeInspectorGUI(){

		    nestedTree = UnityEditor.EditorGUILayout.ObjectField("Behaviour Tree", nestedTree, typeof(BehaviourTree), true) as BehaviourTree;
	    	if (nestedTree == this.graph){
		    	Debug.LogWarning("You can't have a Graph nested to iteself! Please select another");
		    	nestedTree = null;
		    }

		    if (nestedTree != null){
		    	nestedTree.name = UnityEditor.EditorGUILayout.TextField("Name", nestedTree.name);
		    	nestedTree.ShowDefinedBBVariablesGUI();
		    }
		}

		#endif
	}
}