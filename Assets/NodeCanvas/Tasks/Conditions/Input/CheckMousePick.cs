using UnityEngine;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("Input")]
	public class CheckMousePick : ConditionTask{

		public enum ButtonKeys{
			Left = 0,
			Right = 1,
			Middle = 2
		}
		
		public ButtonKeys buttonKey;
		[LayerField]
		public int layer;

		[BlackboardOnly]
		public BBGameObject saveGoAs;
		[BlackboardOnly]
		public BBFloat saveDistanceAs;
		[BlackboardOnly]
		public BBVector savePosAs;

		private int buttonID;
		private RaycastHit hit;

		protected override string info{
			get
			{
				string finalString= buttonKey.ToString() + " Click";
				if (!string.IsNullOrEmpty(savePosAs.dataName))
					finalString += "\nSavePos As " + savePosAs;
				if (!string.IsNullOrEmpty(saveGoAs.dataName))
					finalString += "\nSaveGo As " + saveGoAs;
				return finalString;
			}
		}

		protected override bool OnCheck(){

			buttonID = (int)buttonKey;
			if (Input.GetMouseButtonDown(buttonID)){
				if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1<<layer)){

					saveGoAs.value = hit.collider.gameObject;
					saveDistanceAs.value = hit.distance;
					savePosAs.value = hit.point;
					return true;
				}
			}
			return false;
		}
	}
}