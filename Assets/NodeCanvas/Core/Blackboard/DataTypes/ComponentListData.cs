using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class ComponentListData : VariableData {

		public List<Component> value = new List<Component>();
		public override object GetSerialized(){
			return null;
		}
	}
}