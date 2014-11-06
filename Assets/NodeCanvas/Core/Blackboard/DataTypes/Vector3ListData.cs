using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Variables{

	public class Vector3ListData : VariableData {

		public List<Vector3> value = new List<Vector3>();
		public override object GetSerialized(){
			return null;
		}
	}
}