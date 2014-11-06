using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("✫ Blackboard/Lists")]
	[Description("Add a number of GameObjects to the target GameObject list variable")]
	public class SetListGameObjects : ActionTask {

		[RequiredField] [BlackboardOnly]
		public BBGameObjectList targetList;
		public List<BBGameObject> objectsToAdd = new List<BBGameObject>();
		public bool onlyIfNotContained = true;

		protected override string info{
			get {return "Add " + objectsToAdd.Count.ToString() + " objects to " + targetList; }
		}

		protected override void OnExecute(){

			foreach (BBGameObject bbGO in objectsToAdd){
				if (onlyIfNotContained && targetList.value.Contains(bbGO.value))
					continue;
				targetList.value.Add(bbGO.value);
			}

			EndAction();
		}
	}
}