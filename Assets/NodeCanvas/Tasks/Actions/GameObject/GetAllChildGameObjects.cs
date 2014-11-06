using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	public class GetAllChildGameObjects : ActionTask {

		[BlackboardOnly]
		public BBGameObjectList saveAs;
		public bool recursive = false;

		protected override void OnExecute(){

			var found = new List<Transform>();
			foreach (Transform t in agent.transform){
				found.Add(t);
				if (recursive)
					found.AddRange(Get(t));
			}
			saveAs.value = found.Select(t => t.gameObject).ToList();
			EndAction();
		}

		List<Transform> Get(Transform parent){
			var found = new List<Transform>();
			foreach (Transform t in parent){
				found.Add(t);
				found.AddRange(Get(t));
			}
			return found;
		}
	}
}