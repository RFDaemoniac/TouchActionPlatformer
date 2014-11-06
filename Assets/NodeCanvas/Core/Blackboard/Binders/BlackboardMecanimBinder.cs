using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Variables;

namespace NodeCanvas{

	[AddComponentMenu("NodeCanvas/Blackboard Mecanim Binder")]
	public class BlackboardMecanimBinder : MonoBehaviour {

		public Blackboard blackboard;
		public Animator animator;
		public List<string> parameters = new List<string>();

		private List<VariableData> syncedVariables = new List<VariableData>();

		void Reset(){
			blackboard = GetComponent<Blackboard>();
			animator = GetComponent<Animator>();
		}

		void Awake(){
			
			if (!blackboard || !animator)
				return;

			foreach (string parameter in parameters){
				var data = blackboard.GetData(parameter, null);
				if (data == null){
					Debug.LogWarning(string.Format("MecanimSync: Blackboard does not have variable with name '{0}'", parameter));
					continue;
				}

				if (!syncedVariables.Contains(data)){
					syncedVariables.Add(data);
					data.onValueChanged += OnValueChanged;
				}
			}
		}

		void OnDestroy(){

			foreach (VariableData data in syncedVariables)
				data.onValueChanged -= OnValueChanged;
		}

		void OnValueChanged(string name, object value){
			
			if (value.GetType() == typeof(bool)){
				animator.SetBool(name, (bool)value);
				return;
			}

			if (value.GetType() == typeof(float)){
				animator.SetFloat(name, (float)value);
				return;
			}

			if (value.GetType() == typeof(int)){
				animator.SetInteger(name, (int)value);
				return;
			}
		}
	}
}