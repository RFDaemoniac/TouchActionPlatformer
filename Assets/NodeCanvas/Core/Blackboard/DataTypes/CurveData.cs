using UnityEngine;
using System.Collections.Generic;

namespace NodeCanvas.Variables{

	[AddComponentMenu("")]
	public class CurveData : VariableData{

		public AnimationCurve value = AnimationCurve.EaseInOut(0,0,1,1);

		public override object objectValue{
			get {return value;}
			set
			{
				if ((AnimationCurve)value != this.value){
					this.value = (AnimationCurve)value;
					OnValueChanged(value);
				}
			}
		}

		public override object GetSerialized(){
			var serKeys = new List<SerializedKey>();
			foreach (Keyframe key in value.keys){
				var newKey = new SerializedKey();
				newKey.time = key.time;
				newKey.value = key.value;
				newKey.inTangent = key.inTangent;
				newKey.outTangent = key.outTangent;
				serKeys.Add(newKey);
			}
			return serKeys;
		}

		public override void SetSerialized(object obj){
			var keyframes = new List<Keyframe>();
			foreach (SerializedKey serKey in (List<SerializedKey>)obj )
				keyframes.Add(new Keyframe(serKey.time, serKey.value, serKey.inTangent, serKey.outTangent));
			value.keys = keyframes.ToArray();
		}

		[System.Serializable]
		class SerializedKey{

			public float time;
			public float value;
			public float inTangent;
			public float outTangent;
		}

		//////////////////////////
		///////EDITOR/////////////
		//////////////////////////
		#if UNITY_EDITOR

		public override void ShowDataGUI(){
			value = UnityEditor.EditorGUILayout.CurveField(value, layoutOptions);
		}

		#endif
	}
}