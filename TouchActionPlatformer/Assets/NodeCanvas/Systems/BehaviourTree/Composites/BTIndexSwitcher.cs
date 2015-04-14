using UnityEngine;
using System.Collections;
using NodeCanvas.Variables;

namespace NodeCanvas.BehaviourTrees{

	[Name("Switch")]
	[Category("Composites")]
	[Description("Executes ONE child based on the provided index or enum and return it's status. If index change while a child is running, that child will be interrupted before the new child is executed\nWhen selection is based on an enum, the enum value is used")]
	[Icon("IndexSwitcher")]
	public class BTIndexSwitcher : BTComposite {

		public enum SelectionMode
		{
			IndexBased,
			EnumBased
		}

		public enum OutOfRangeMode
		{
			ReturnFailure,
			LoopIndex
		}

		[BlackboardOnly] [RequiredField]
		public BBEnum enumIndex;
		public BBInt index;
		public OutOfRangeMode outOfRangeMode;
		public SelectionMode selectionMode;

		private int current;
		private int runningIndex;

		public override string nodeName{
			get{return string.Format("<color=#b3ff7f>{0}</color>", base.nodeName.ToUpper());}
		}

		protected override Status OnExecute(Component agent, Blackboard blackboard){

			if (outConnections.Count == 0)
				return Status.Failure;

			current = selectionMode == SelectionMode.EnumBased? System.Convert.ToInt32(enumIndex.value) : index.value;

			if (outOfRangeMode == OutOfRangeMode.LoopIndex)
				current = Mathf.Abs(current) % outConnections.Count;

			if (runningIndex != current)
				outConnections[runningIndex].ResetConnection();

			if (current < 0 || current >= outConnections.Count)
				return Status.Failure;

			status = outConnections[current].Execute(agent, blackboard);

			if (status == Status.Running)
				runningIndex = current;

			return status;
		}

		////////////////////////////////////////
		///////////GUI AND EDITOR STUFF/////////
		////////////////////////////////////////
		#if UNITY_EDITOR
		
		protected override void OnNodeGUI(){

			GUILayout.Label("Current = " + (selectionMode == SelectionMode.IndexBased? index.ToString() : enumIndex.ToString()) );
		}

		protected override void OnNodeInspectorGUI(){

			selectionMode = (SelectionMode)UnityEditor.EditorGUILayout.EnumPopup("Selection Mode", selectionMode);
			if (selectionMode == SelectionMode.IndexBased)
			{
				index = (BBInt)EditorUtils.BBVariableField("Index", index);
			}
			else
			{
				enumIndex = (BBEnum)EditorUtils.BBVariableField("Enum", enumIndex);
			}

			outOfRangeMode = (OutOfRangeMode)UnityEditor.EditorGUILayout.EnumPopup("When Out Of Range", outOfRangeMode);
		}
		
		#endif
	}
}