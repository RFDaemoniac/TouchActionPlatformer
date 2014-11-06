using UnityEngine;

namespace NodeCanvas{
	
	//Utility class for the nodes root game object
	[AddComponentMenu("")]
	[ExecuteInEditMode]
	public class NodesRootUtility : MonoBehaviour {

		[SerializeField]
		Graph _parentGraph;

		public Graph parentGraph{
			get {return _parentGraph;}
			set {_parentGraph = value;}
		}

		void OnEnable(){
			gameObject.hideFlags = Graph.doHide? HideFlags.HideInHierarchy : 0;
		}
	}
}