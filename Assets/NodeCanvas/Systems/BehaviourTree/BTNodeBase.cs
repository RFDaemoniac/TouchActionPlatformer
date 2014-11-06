using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	///The base class for all Behaviour Tree system nodes
	abstract public class BTNodeBase : Node, IAutoSortable{

		public override System.Type outConnectionType{
			get{return typeof(BTConnection);}
		}

		public override int maxInConnections{
			get{return 1;}
		}

		public override int maxOutConnections{
			get{return 0;}
		}

		///Fetch all child nodes of the node, optionaly including this
		public List<BTNodeBase> GetAllChildNodesRecursively(bool includeThis){

			var childList = new List<BTNodeBase>();
			if (includeThis)
				childList.Add(this);

			foreach (BTNodeBase child in outConnections.Select(c => c.targetNode))
				childList.AddRange(child.GetAllChildNodesRecursively(true));

			return childList;
		}

		///Fetch all child nodes of this node with their depth in regards to this node.
		public Dictionary<BTNodeBase, int> GetAllChildNodesWithDepthRecursively(bool includeThis, int startIndex){

			var childList = new Dictionary<BTNodeBase, int>();
			if (includeThis)
				childList[this] = startIndex;

			foreach (BTNodeBase child in outConnections.Select(c => c.targetNode)){
				foreach (KeyValuePair<BTNodeBase, int> pair in child.GetAllChildNodesWithDepthRecursively(true, startIndex + 1))
					childList[pair.Key] = pair.Value;
			}

			return childList;
		}

	}
}