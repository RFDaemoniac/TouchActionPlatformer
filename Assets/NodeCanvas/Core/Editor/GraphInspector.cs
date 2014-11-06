using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using NodeCanvas;
using NodeCanvas.Variables;
using System.Linq;

namespace NodeCanvasEditor{

    [CustomEditor(typeof(Graph), true)]
    public class GraphInspector : Editor {

        private Graph graph{
            get {return target as Graph;}
        }


        void OnEnable(){
            if (graph)
                graph.nodesRoot.gameObject.hideFlags = Graph.doHide? HideFlags.HideInHierarchy : 0;
        }

        //hack
        void OnDestroy(){
            if (graph == null){
                var root = graph._nodesRoot;
                if (root != null)
                    DestroyImmediate(root.gameObject, true);
            }
        }

        public override void OnInspectorGUI(){

            GUI.skin.label.richText = true;
            Undo.RecordObject(graph, "Graph Inspector");
            ShowBasicGUI();
            ShowTargetsGUI();
            graph.ShowDefinedBBVariablesGUI();
            //ShowUsedByGUI();

            if (GUI.changed)
                EditorUtility.SetDirty(graph);
        }

        void ShowBasicGUI(){

           if (graph.isRunning)
                EditorUtils.CoolLabel("Now Running!");

            GUILayout.Space(10);
            graph.name = EditorGUILayout.TextField("Graph Name", graph.name);
            if (string.IsNullOrEmpty(graph.name))
              graph.name = graph.gameObject.name;
            graph.graphComments = GUILayout.TextArea(graph.graphComments, GUILayout.Height(45));
            EditorUtils.TextFieldComment(graph.graphComments);

            if (PrefabUtility.GetPrefabType(graph) != PrefabType.Prefab){
                GUI.backgroundColor = new Color(0.8f,0.8f,1);
                if (GUILayout.Button("EDIT IN NODECANVAS"))
                    GraphEditor.OpenWindow(graph);
                GUI.backgroundColor = Color.white;
            } else {
                EditorGUILayout.HelpBox("Editing is not allowed when prefab asset is selected. Please place the prefab in a scene, edit and apply it", MessageType.Info);
            }
        }

        void ShowTargetsGUI(){

            EditorUtils.BoldSeparator();

            GUI.color = new Color(1f,1f,1f,0.2f);
            GUILayout.Label("Current Owner References (These are automaticaly set):");
            graph.agent = (Component)EditorGUILayout.ObjectField("Agent", graph.agent, typeof(Component), true);
            graph.blackboard = (Blackboard)EditorGUILayout.ObjectField("Blackboard", graph.blackboard, typeof(Blackboard), true);
            GUI.color = Color.white;
        }


        void ShowUsedByGUI(){

            EditorUtils.BoldSeparator();
            EditorUtils.CoolLabel("Used By (in scene)");
            EditorUtils.Separator();

            var owners = Resources.FindObjectsOfTypeAll<GraphOwner>();
            var nestedNodes = Resources.FindObjectsOfTypeAll<Node>();

            foreach (GraphOwner owner in owners){
                if (owner.behaviour == graph)
                    GUILayout.Label(string.Format("{0} ({1})", owner.name, owner.GetType().Name));
            }

            foreach (INestedNode nested in nestedNodes.OfType<INestedNode>()){
                if (nested.nestedGraph == graph)
                    GUILayout.Label(string.Format("{0} ({1})", (nested as Node).graph.name, (nested as Node).graph.GetType().Name ));
            }
        }
    }
}