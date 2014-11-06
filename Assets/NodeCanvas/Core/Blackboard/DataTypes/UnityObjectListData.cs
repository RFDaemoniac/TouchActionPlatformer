using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class UnityObjectListData : VariableData {

		public List<Object> value = new List<Object>();
		public override object GetSerialized(){
			return null;
		}
	}
}