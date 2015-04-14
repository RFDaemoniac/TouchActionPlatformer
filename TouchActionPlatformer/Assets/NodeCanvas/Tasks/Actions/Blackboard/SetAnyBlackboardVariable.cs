using UnityEngine;
using System.Linq;
using NodeCanvas;
using NodeCanvas.Variables;
 
namespace NodeCanvas.Actions{
 
    [Category("✫ Blackboard")]
    [AgentType(typeof(Blackboard))]
    [Description("Use this to set a variable on any blackboard by overriding the agent")]
    public class SetAnyBlackboardVariable : ActionTask {

        [RequiredField]
        public string targetVariableName;

        [SerializeField]
    	private BBVariableSet variableSet = new BBVariableSet();
       
        protected override string info{
            get {return string.Format("<b>'${0}'</b> = {1}", targetVariableName, variableSet.selectedBBVariable != null? variableSet.selectedBBVariable.ToString() : ""); }
        }

        protected override void OnExecute(){
           
            (agent as Blackboard).SetDataValue(targetVariableName, variableSet.objectValue);
            EndAction();
        }

        ////////////////////////////////////////
        ///////////GUI AND EDITOR STUFF/////////
        ////////////////////////////////////////
        #if UNITY_EDITOR
        
        protected override void OnTaskInspectorGUI(){

        	DrawDefaultInspector();
            variableSet.selectedType = EditorUtils.Popup<System.Type>("Type", variableSet.selectedType, variableSet.availableTypes);
        	if (variableSet.selectedBBVariable != null)
        		EditorUtils.BBVariableField("Value", variableSet.selectedBBVariable);
        }
        
        #endif
    }
}
 