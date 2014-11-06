using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	[Description("Remove a number of game objects from the target list")]
	public class RemoveListObjects : ActionTask {

		[RequiredField][BlackboardOnly]
		public BBGameObjectList targetList;

		public List<BBGameObject> objectsToRemove;

		protected override void OnExecute(){
			
			foreach(GameObject go in objectsToRemove.Select(bbGo => bbGo.value))
				targetList.value.Remove(go);

			EndAction(true);
		}
	}
}