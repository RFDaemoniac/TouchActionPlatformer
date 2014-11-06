using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[Description("Note that this is very slow")]
	public class FindObjectOfType : ActionTask {

		public BBString type = new BBString{value = "Camera"};
		[BlackboardOnly]
		public BBGameObject saveGameObjectAs;

		protected override void OnExecute(){

			foreach(GameObject go in FindObjectsOfType<GameObject>()){
				if (go.GetComponent(type.value) != null){
					saveGameObjectAs.value = go;
					EndAction(true);
					return;
				}
			}
			saveGameObjectAs.value = null;
			EndAction(false);
		}
	}
}