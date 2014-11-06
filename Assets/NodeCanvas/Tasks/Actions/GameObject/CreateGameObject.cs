using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	public class CreateGameObject : ActionTask {

		public BBString objectName;
		public BBVector position;
		public BBVector rotation;
		
		[BlackboardOnly]
		public BBGameObject saveAs;

		protected override void OnExecute(){
			var newGO = new GameObject(objectName.value);
			newGO.transform.position = position.value;
			newGO.transform.eulerAngles = rotation.value;
			saveAs.value = newGO;
			EndAction();
		}
	}
}