using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[Description("Note that this is very slow")]
	public class FindObjectsOfType : ActionTask {

		public BBString type = new BBString{value = "Camera"};
		[BlackboardOnly]
		public BBGameObjectList saveAs;

		protected override void OnExecute(){

			var all = FindObjectsOfType<GameObject>();
			var found = new List<GameObject>();
			foreach(GameObject go in all){
				if (go.GetComponent(type.value) != null)
					found.Add(go);
			}
			saveAs.value = found;
			EndAction(found.Count > 0);
		}
	}
}