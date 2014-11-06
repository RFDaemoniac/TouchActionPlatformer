using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	[Description("Note that this is slow.\nAction will end in Failure if no objects are found")]
	public class FindAllWithName : ActionTask{

		[RequiredField]
		public BBString searchName = new BBString{value = "GameObject"};
		[BlackboardOnly]
		public BBGameObjectList saveAs;

		protected override string info{
			get{return "GetObjects '" + searchName + "' as " + saveAs;}
		}

		protected override void OnExecute(){

			List<GameObject> gos = new List<GameObject>();
			foreach (GameObject go in FindObjectsOfType<GameObject>()){
				if (go.name == searchName.value)
					gos.Add(go);
			}

			saveAs.value = gos;
			EndAction(gos.Count != 0);
		}
	}
}