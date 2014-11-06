using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.BehaviourTrees{

	[AddComponentMenu("")]
	[Name("Repeat")]
	[Category("Decorators")]
	[Description("Repeat the child either x times or until it returns the specified status, or forever")]
	[Icon("Repeat")]
	public class BTRepeater : BTDecorator{

		public enum RepeatTypes {
			RepeatTimes,
			RepeatUntil,
			RepeatForever
		}

		public enum RepeatUntil {
			Failure = 0,
			Success = 1
		}

		public RepeatTypes repeatType= RepeatTypes.RepeatTimes;
		public RepeatUntil repeatUntil= RepeatUntil.Success;
		public BBInt repeatTimes = new BBInt{value = 1};

		private int currentIteration = 1;

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			if (!decoratedConnection)
				return Status.Resting;

			status = decoratedConnection.Execute(agent, blackboard);

			if (status == Status.Resting)
				return Status.Running;

			if (status != Status.Running){

				if (repeatType == RepeatTypes.RepeatTimes){

					repeatTimes.value = Mathf.Max(repeatTimes.value, 1);
					if (currentIteration >= repeatTimes.value)
						return status;

					currentIteration ++;

				} else if (repeatType == RepeatTypes.RepeatUntil){

					if ((int)status == (int)repeatUntil)
						return status;
				}

				decoratedConnection.ResetConnection();
				return Status.Running;
			}

			return status;
		}

		protected override void OnReset(){

			currentIteration = 1;
		}


		/////////////////////////////////////////
		/////////GUI AND EDITOR STUFF////////////
		/////////////////////////////////////////
		#if UNITY_EDITOR

		protected override void OnNodeGUI(){

			if (repeatType == RepeatTypes.RepeatTimes){

				GUILayout.Label(repeatTimes + " Times");
				if (Application.isPlaying)
					GUILayout.Label("Itteration: " + currentIteration.ToString());

			} else if (repeatType == RepeatTypes.RepeatUntil){

				GUILayout.Label("Until " + repeatUntil);
			
			} else {

				GUILayout.Label("Repeat Forever");
			}
		}

		protected override void OnNodeInspectorGUI(){

			repeatType = (RepeatTypes)UnityEditor.EditorGUILayout.EnumPopup("Repeat Type", repeatType);

			if (repeatType == RepeatTypes.RepeatTimes){

				repeatTimes = (BBInt)EditorUtils.BBVariableField("Repeat Times", repeatTimes);

			} else if (repeatType == RepeatTypes.RepeatUntil){

				repeatUntil = (RepeatUntil)UnityEditor.EditorGUILayout.EnumPopup("Repeat Until", repeatUntil);
			}
		}

		#endif
	}
}