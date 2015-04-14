
#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NodeCanvas.Variables;

namespace NodeCanvas{

	//Have some commonly stuff used across most inspectors and helper functions. Keep outside of Editor folder since many runtime classes use this in #if UNITY_EDITOR
	public static class EditorUtils{

		private static Texture2D _tex;
		private static Texture2D tex{
			get
			{
				if (_tex == null){
					_tex = new Texture2D(1,1);
					_tex.hideFlags = HideFlags.HideAndDontSave;
				}
				return _tex;
			}
		}
		
		readonly public static Texture2D playIcon   = EditorGUIUtility.FindTexture("d_PlayButton");
		readonly public static Texture2D pauseIcon  = EditorGUIUtility.FindTexture("d_PauseButton");
		readonly public static Texture2D stepIcon   = EditorGUIUtility.FindTexture("d_StepButton");
		readonly public static Texture2D viewIcon   = EditorGUIUtility.FindTexture("d_ViewToolOrbit On");
		readonly public static Texture2D csIcon     = EditorGUIUtility.FindTexture("cs Script Icon");
		readonly public static Texture2D jsIcon     = EditorGUIUtility.FindTexture("Js Script Icon");
		readonly public static Texture2D tagIcon    = EditorGUIUtility.FindTexture("d_FilterByLabel");
		readonly public static Texture2D searchIcon = EditorGUIUtility.FindTexture("Search Icon");

		readonly public static Color lightOrange = new Color(1, 0.9f, 0.4f);
		readonly public static Color lightBlue   = new Color(0.8f,0.8f,1);
		readonly public static Color lightRed    = new Color(1,0.5f,0.5f, 0.8f);

		//a cool label :-P
		public static void CoolLabel(string text){

			GUI.skin.label.richText = true;
			GUI.color = lightOrange;
			GUILayout.Label("<b><size=14>" + text + "</size></b>");
			GUI.color = Color.white;
			GUILayout.Space(2);
		}

		//a thin separator
		public static void Separator(){
			
			GUI.backgroundColor = Color.black;
			GUILayout.Box("", GUILayout.MaxWidth(Screen.width), GUILayout.Height(2));
			GUI.backgroundColor = Color.white;
		}

		//A thick separator similar to ngui. Thanks
		public static void BoldSeparator(){

			var lastRect= GUILayoutUtility.GetLastRect();

			GUILayout.Space(14);
			GUI.color = new Color(0, 0, 0, 0.25f);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 1), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 9, Screen.width, 1), tex);
			GUI.color = Color.white;
		}

		//Combines the rest functions for a header style label
		public static void TitledSeparator(string title){

			GUILayout.Space(1);
			BoldSeparator();
			CoolLabel(title + " ▼");
			Separator();
		}

		//Just a fancy ending for inspectors
		public static void EndOfInspector(){

			var lastRect= GUILayoutUtility.GetLastRect();

			GUILayout.Space(8);
			GUI.color = new Color(0, 0, 0, 0.4f);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 6, Screen.width, 4), tex);
			GUI.DrawTexture(new Rect(0, lastRect.yMax + 4, Screen.width, 1), tex);
			GUI.color = Color.white;
		}

		//Used just after a textfield with no prefix to show an italic transparent text inside when empty
		public static void TextFieldComment(string check, string comment = "Comments..."){
			if (string.IsNullOrEmpty(check)){
				var lastRect = GUILayoutUtility.GetLastRect();
				GUI.color = new Color(1,1,1,0.3f);
				GUI.Label(lastRect, " <i>" + comment + "</i>");
				GUI.color = Color.white;
			}
		}

		//Show an automatic editor gui for arbitrary objects, taking into account custom attributes
		public static void ShowAutoEditorGUI(object o){

			foreach (FieldInfo field in o.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public)){

				if (field.GetCustomAttributes(typeof(HideInInspector), true ).FirstOrDefault() as HideInInspector != null)
					continue;

				field.SetValue(o, GenericField(field.Name, field.GetValue(o), field.FieldType, field) );
				GUI.backgroundColor = Color.white;
			}

			foreach (PropertyInfo prop in o.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)){

				if (prop.GetCustomAttributes(typeof(HideInInspector), true ).FirstOrDefault() as HideInInspector != null)
					continue;

				if (!prop.CanRead || !prop.CanWrite)
					continue;

				prop.SetValue(o, GenericField(prop.Name, prop.GetValue(o, null), prop.PropertyType, prop), null);
				GUI.backgroundColor = Color.white;				
			}
		}

		//For generic automatic editors. Passing a MemberInfo will also check for custom attributes
		public static object GenericField(string name, object value, Type t, MemberInfo member = null){

			if (t == null){
				GUILayout.Label("NO TYPE PROVIDED!");
				return null;
			}

			name = SplitCamelCase(name);


			if ( !typeof(UnityEngine.Object).IsAssignableFrom(t) && (value != null && value.GetType().IsAbstract) || (value == null && t.IsAbstract) ){
				GUILayout.Label(name + " (Abstract)");
				return value;
			}

			if (!(typeof(UnityEngine.Object)).IsAssignableFrom(t) && value == null && t != null && !t.IsAbstract && t.GetConstructor(Type.EmptyTypes) != null)
				value = Activator.CreateInstance(t);

			if (member != null){
				if (member.GetCustomAttributes(typeof(RequiredFieldAttribute), true).FirstOrDefault() as RequiredFieldAttribute != null){
					if ( (value == null || value.Equals(null) ) || 
						(t == typeof(string) && string.IsNullOrEmpty((string)value) ) ||
						(typeof(BBVariable).IsAssignableFrom(t) && (value as BBVariable).isNull) )
					{
						GUI.backgroundColor = lightRed;
					}
				}
			}


			if (t == typeof(string)){

				if (member != null){
					if (member.GetCustomAttributes(typeof(TagFieldAttribute), true).FirstOrDefault() as TagFieldAttribute != null)
						return EditorGUILayout.TagField(name, (string)value);
					var areaAtt = member.GetCustomAttributes(typeof(TextAreaFieldAttribute), true).FirstOrDefault() as TextAreaFieldAttribute;
					if (areaAtt != null){
						GUILayout.Label(name);
						return EditorGUILayout.TextArea((string)value, GUILayout.Height(areaAtt.height));
					}
				}

				return EditorGUILayout.TextField(name, (string)value);
			}

			if (t == typeof(bool))
				return EditorGUILayout.Toggle(name, (bool)value);

			if (t == typeof(int)){
				
				if (member != null && member.GetCustomAttributes(typeof(LayerFieldAttribute), true).FirstOrDefault() as LayerFieldAttribute != null)
					return EditorGUILayout.LayerField(name, (int)value);

				return EditorGUILayout.IntField(name, (int)value);
			}

			if (t == typeof(float)){
				
				if (member != null){
					var sField = member.GetCustomAttributes(typeof(SliderFieldAttribute), true).FirstOrDefault() as SliderFieldAttribute;
					if (sField != null)
						return EditorGUILayout.Slider(name, (float)value, sField.left, sField.right);
				}

				return EditorGUILayout.FloatField(name, (float)value);
			}

			if (t == typeof(Vector2))
				return EditorGUILayout.Vector2Field(name, (Vector2)value);

			if (t == typeof(Vector3))
				return EditorGUILayout.Vector3Field(name, (Vector3)value);

			if (t == typeof(Vector4))
				return EditorGUILayout.Vector4Field(name, (Vector3)value);

			if (t == typeof(Quaternion)){
				var quat = (Quaternion)value;
				var vec4 = new Vector4(quat.x, quat.y, quat.z, quat.w);
				vec4 = EditorGUILayout.Vector4Field(name, vec4);
				return new Quaternion(vec4.x, vec4.y, vec4.z, vec4.w);
			}

			if (t == typeof(Color))
				return EditorGUILayout.ColorField(name, (Color)value);

			if (t == typeof(Rect))
				return EditorGUILayout.RectField(name, (Rect)value);

			if (t == typeof(AnimationCurve))
				return EditorGUILayout.CurveField(name, (AnimationCurve)value);

			if (t == typeof(Bounds))
				return EditorGUILayout.BoundsField(name, (Bounds)value);

			if (t == typeof(LayerMask))
				return LayerMaskField(name, (LayerMask)value);

			if (typeof(System.Enum).IsAssignableFrom(t)){
				if (value != null && value.GetType() == typeof(string))
					return StringPopup(name, (string)value, Enum.GetNames(t).ToList(), false, false );
				return EditorGUILayout.EnumPopup(name, (System.Enum)value);
			}

			if (typeof(BBVariable).IsAssignableFrom(t))
				return BBVariableField(name, (BBVariable)value, member);

			if (t == typeof(Component) && value as Component != null)
				return ComponentField(name, (Component)value, typeof(Component));

			if (typeof(UnityEngine.Object).IsAssignableFrom(t))
				return EditorGUILayout.ObjectField(name, (UnityEngine.Object)value, t, true);

			if (typeof(IList).IsAssignableFrom(t)){
				if (!t.IsArray)
					return ListEditor(name, (IList)value, t.GetGenericArguments()[0]);
			}

			GUILayout.BeginVertical();
			GUILayout.Label(TypeName(t));
			EditorGUI.indentLevel ++;
			foreach (FieldInfo field in value.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public))
				field.SetValue(value, GenericField(field.Name, field.GetValue(value), field.FieldType, field) );
			EditorGUI.indentLevel --;
			GUILayout.EndVertical();

			return value;
		}

		public static LayerMask LayerMaskField(string prefix, LayerMask mask){

			var options = new List<string>();
			for (int i = 0; i <= 31; i++){
				var name = LayerMask.LayerToName(i);
				if (!string.IsNullOrEmpty(name) || i <= 8)
					options.Add(name);
			}

			mask = EditorGUILayout.MaskField(prefix, mask.value, options.ToArray());
			return mask;
		}

		public static IList ListEditor(string prefix, IList list, Type argType = null){

			if (list == null){
				GUILayout.Label("Null List");
				return null;
			}

			argType = argType == null? list.GetType().GetGenericArguments()[0] : argType;

			GUILayout.BeginVertical();
			Separator();
			GUILayout.BeginHorizontal();
			EditorGUILayout.PrefixLabel(prefix);

			if (GUILayout.Button("Add Element")){
				
				if (argType.IsValueType){
					list.Add(Activator.CreateInstance(argType));
				} else {
					list.Add(null);
				}
			}

			GUILayout.EndHorizontal();

			EditorGUI.indentLevel ++;

			for (int i = 0; i < list.Count; i++){
				GUILayout.BeginHorizontal();
				list[i] = GenericField("Element " + i, list[i], argType);
				if (GUILayout.Button("X", GUILayout.Width(18)))
					list.RemoveAt(i);
				GUILayout.EndHorizontal();				
			}

			EditorGUI.indentLevel --;
			Separator();

			GUILayout.EndVertical();
			return list;
		}

		//Convert camelCase to words as the name implies.
		public static string SplitCamelCase(string s){

			if (string.IsNullOrEmpty(s)) return s;

			s = char.ToUpper(s[0]) + s.Substring(1);
			return System.Text.RegularExpressions.Regex.Replace(s, "(?<=[a-z])([A-Z])", " $1").Trim();
		}

		//a special object field for the BBVariable class to let user choose either a real value or enter a string to read data from a Blackboard
		public static BBVariable BBVariableField(string prefix, BBVariable bbVar, MemberInfo member = null){

			if (bbVar == null){
				EditorGUILayout.LabelField(prefix, "NULL");
				return null;
			}

			GUILayout.BeginVertical();
			GUILayout.BeginHorizontal();
			if (!bbVar.blackboardOnly && !bbVar.useBlackboard){

				var field = bbVar.GetType().GetField("_value", BindingFlags.Instance | BindingFlags.NonPublic);
				field.SetValue(bbVar, GenericField(prefix, field.GetValue(bbVar), bbVar.varType, member) );

			} else {

				GUI.color = new Color(0.9f,0.9f,1f,1f);
				if (bbVar.bb){

					List<string> dataNames = bbVar.bb.GetDataNames(bbVar.varType).ToList();

					dataNames.Add("/ ");
					foreach (KeyValuePair<string, Blackboard> pair in Blackboard.allBlackboards){
						
						if (pair.Value == bbVar.bb || !pair.Value.isGlobal)
							continue;

						dataNames.Add(pair.Key + "/");
						foreach (string dName in pair.Value.GetDataNames(bbVar.varType))
							dataNames.Add(pair.Key + "/" + dName);
					}

					dataNames.Add("(DynamicVar)");

					if (dataNames.Contains(bbVar.dataName) || string.IsNullOrEmpty(bbVar.dataName) ){

						if (bbVar.isDynamic){
							GUIUtility.keyboardControl = 0;
							bbVar.isDynamic = false;
						}

						bbVar.dataName = StringPopup(prefix, bbVar.dataName, dataNames, false, true);
						if (bbVar.dataName == "(DynamicVar)"){
							bbVar.isDynamic = true;
							bbVar.dataName = "_";
						}

					} else {

						bbVar.dataName = EditorGUILayout.TextField(prefix + " (" + TypeName(bbVar.varType) + ")", bbVar.dataName);
					}	

				} else {

					bbVar.dataName = EditorGUILayout.TextField(prefix + " (" + TypeName(bbVar.varType) + ")", bbVar.dataName);
				}
			}

			GUI.color = Color.white;
			GUI.backgroundColor = Color.white;

			if (!bbVar.blackboardOnly)
				bbVar.useBlackboard = EditorGUILayout.Toggle(bbVar.useBlackboard, EditorStyles.radioButton, GUILayout.Width(18));

			GUILayout.EndHorizontal();
		
			if (bbVar.bb && bbVar.useBlackboard && string.IsNullOrEmpty(bbVar.dataName)){	
				
				GUI.backgroundColor = new Color(0.8f,0.8f,1f,0.5f);
				GUI.color = new Color(1f,1f,1f,0.5f);
				GUILayout.BeginHorizontal("textfield");

				if (string.IsNullOrEmpty(bbVar.dataName)){
					GUILayout.Label("Select a '" + TypeName(bbVar.varType) + "' Blackboard Variable");
				} else {
					GUILayout.Label("Variable name does not exist in blackboard");
				}
				
				GUILayout.EndHorizontal();
				GUILayout.Space(2);
			}

			GUILayout.EndVertical();

			//RequiresCoponent attribute should be checked here unfortunately
			if (member != null && bbVar.GetType() == typeof(BBGameObject)){
				var rc = member.GetCustomAttributes(typeof(RequiresComponentAttribute), true).FirstOrDefault() as RequiresComponentAttribute;
				if (rc != null && typeof(Component).IsAssignableFrom(rc.type) && !bbVar.isNull){
					if ( (bbVar as BBGameObject).value.GetComponent(rc.type) == null ){
						GUI.backgroundColor = new Color(1f,0.8f,0.8f,0.5f);
						GUI.color = new Color(1f,1f,1f,0.5f);
						GUILayout.BeginHorizontal("textfield");
						GUILayout.Label("GameObject requires '" + TypeName(rc.type) + "' Component");
						GUILayout.EndHorizontal();
						GUILayout.Space(2);
					}
				}
			}

			GUI.backgroundColor = Color.white;
			GUI.color = Color.white;			
			return bbVar;
		}


		///A simple reorderable list. Pass the list and a function to call for GUI. The callback comes with the current iterated element and index
		private static Dictionary<IList, object> pickedObjectList = new Dictionary<IList, object>();
		public static void ReorderableList(IList list, System.Action<int> GUICallback, UnityEngine.Object undoObject = null){

			if (list.Count == 1){
				GUICallback(0);
				return;
			}

			if (!pickedObjectList.ContainsKey(list))
				pickedObjectList[list] = null;

			var e = Event.current;
			var lastRect = new Rect();
			object picked = pickedObjectList[list];
			GUILayout.BeginVertical();
			for (int i= 0; i < list.Count; i++){

				GUILayout.BeginVertical();
				GUICallback(i);
				GUILayout.EndVertical();

				GUI.color = Color.white;
				GUI.backgroundColor = Color.white;

				lastRect = GUILayoutUtility.GetLastRect();
				EditorGUIUtility.AddCursorRect(lastRect, MouseCursor.MoveArrow);

				if (picked != null && picked == list[i])
					GUI.Box(lastRect, "");

				if (picked != null && lastRect.Contains(e.mousePosition) && picked != list[i]){

					var markRect = new Rect(lastRect.x,lastRect.y-2,lastRect.width, 2);
					if (list.IndexOf(picked) < i)
						markRect.y = lastRect.yMax - 2;

					GUI.Box(markRect, "");
					if (e.type == EventType.MouseUp){
						if (undoObject != null)
							Undo.RecordObject(undoObject, "Reorder");
						list.Remove(picked);
						list.Insert(i, picked);
						pickedObjectList[list] = null;
						e.Use();
						return;
					}
				}

				if (lastRect.Contains(e.mousePosition) && e.type == EventType.MouseDown)
					pickedObjectList[list] = list[i];
			}
			GUILayout.EndVertical();

			if (e.type == EventType.MouseUp)
				pickedObjectList[list] = null;
		}

		//An editor field where if the component is null simply shows an object field, but if its not, shows a dropdown popup to select the specific component
		//from within the gameobject
		public static Component ComponentField(string prefix, Component comp, Type type, bool allowNone = true){

			if (!comp){

				if (prefix != string.Empty){

					comp = EditorGUILayout.ObjectField(prefix, comp, type, true, GUILayout.ExpandWidth(true)) as Component;

				} else {

					comp = EditorGUILayout.ObjectField(comp, type, true, GUILayout.ExpandWidth(true)) as Component;
				}

				return comp;
			}

			var allComp = new List<Component>(comp.GetComponents(type));
			var compNames = new List<string>();

			foreach (Component c in allComp.ToArray()){
				
				if (c == null)
					continue;

				if (c.hideFlags == HideFlags.HideInInspector){
					allComp.Remove(c);
					continue;
				}

				compNames.Add(TypeName(c.GetType()) + " (" + c.gameObject.name + ")");
			}

			if (allowNone)
				compNames.Add("|NONE|");

			int index;
			if (prefix != string.Empty)
				index = EditorGUILayout.Popup(prefix, allComp.IndexOf(comp), compNames.ToArray(), GUILayout.ExpandWidth(true));
			else
				index = EditorGUILayout.Popup(allComp.IndexOf(comp), compNames.ToArray(), GUILayout.ExpandWidth(true));
			
			if (allowNone && index == compNames.Count - 1)
				return null;

			return allComp[index];
		}


		public static string StringPopup(string selected, List<string> options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[] GUIOptions){
			return StringPopup(string.Empty, selected, options, showWarning, allowNone, GUIOptions);
		}

		//a popup that is based on the string rather than the index
		public static string StringPopup(string prefix, string selected, List<string> options, bool showWarning = true, bool allowNone = false, params GUILayoutOption[] GUIOptions){

			EditorGUILayout.BeginVertical();
			if (options.Count == 0 && showWarning){
				EditorGUILayout.HelpBox("There are no options to select for '" + prefix + "'", MessageType.Warning);
				EditorGUILayout.EndVertical();
				return null;
			}

			if (allowNone)
				options.Insert(0, "|NONE|");

			int index;

			if (options.Contains(selected))	index = options.IndexOf(selected);
			else index = allowNone? 0 : -1;

			if (!string.IsNullOrEmpty(prefix)) index = EditorGUILayout.Popup(prefix, index, options.ToArray(), GUIOptions);
			else index = EditorGUILayout.Popup(index, options.ToArray(), GUIOptions);

			if (index == -1 || (allowNone && index == 0)){

				if (showWarning){
					if (!string.IsNullOrEmpty(selected))
						EditorGUILayout.HelpBox("The previous selection '" + selected + "' has been deleted or changed. Please select another", MessageType.Warning);
					else
						EditorGUILayout.HelpBox("Please make a selection", MessageType.Warning);
				}
			}

			EditorGUILayout.EndVertical();
			if (allowNone)
				return index == 0? string.Empty : options[index];

			return index == -1? string.Empty : options[index];
		}

		///Generic Popup for selection of any element within a list
		public static T Popup<T>(string prefix, T selected, List<T> options, params GUILayoutOption[] GUIOptions){

			EditorGUILayout.BeginVertical();
			int index;

			if (options.Contains(selected))	index = options.IndexOf(selected);
			else index = -1;

			var stringedOptions = options.Select(e => e.ToString()).ToArray();

			if (!string.IsNullOrEmpty(prefix)) index = EditorGUILayout.Popup(prefix, index, stringedOptions, GUIOptions);
			else index = EditorGUILayout.Popup(index, stringedOptions, GUIOptions);

			EditorGUILayout.EndVertical();
			return index == -1? options[0] : options[index];
		}

		//A generic menu selection
		public static void ShowMenu<T>(List<T> options, Action<T> callback){
			
			GenericMenu.MenuFunction2 Selected = delegate(object selection){
				callback((T)selection);
			};

			var menu = new GenericMenu();
			foreach (T element in options)
				menu.AddItem(new GUIContent(element.ToString()), false, Selected, element );
			menu.ShowAsContext();
			Event.current.Use();
		}


		//Shows a button that when clicked, pops a context menu with a list of tasks deriving the base type specified. When something is selected the callback is called
		//On top of that it also shows a search field for Tasks
		static string lastSearch = string.Empty;
		static List<ScriptInfo> searchResults = new List<ScriptInfo>();
		public static void TaskSelectionButton(GameObject target, Type baseType, Action<Task> callback, bool hide = false){

			Action<Type> ContextAction = delegate (Type script){

				if (!target)
					target = new GameObject(baseType.Name);

				var newTask = target.AddComponent(script) as Task;
				Undo.RegisterCreatedObjectUndo(newTask, "New Task");
				Undo.RecordObject(newTask, "New Task");
				newTask.hideFlags = hide? HideFlags.HideInInspector : 0;
				
				callback(newTask);
			};

			GUI.backgroundColor = lightBlue;
			if (GUILayout.Button("Add " + SplitCamelCase(baseType.Name) )){

				var menu = GetTypeSelectionMenu(baseType, ContextAction);
				if (Task.copiedTask != null && baseType.IsAssignableFrom( Task.copiedTask.GetType()) )
					menu.AddItem(new GUIContent(string.Format("Paste ({0})", Task.copiedTask.name) ), false, delegate { callback( Task.copiedTask.CopyTo(target) ); });
				menu.ShowAsContext();
				Event.current.Use();
			}


			GUI.backgroundColor = Color.white;

			GUILayout.BeginHorizontal();
			var search = EditorGUILayout.TextField(lastSearch, (GUIStyle)"ToolbarSeachTextField");
			if (GUILayout.Button("", (GUIStyle)"ToolbarSeachCancelButton")){
				search = string.Empty;
				GUIUtility.keyboardControl = 0;
			}
			GUILayout.EndHorizontal();

			if (!string.IsNullOrEmpty(search)){

				if (search != lastSearch)
					searchResults = GetAllScriptsOfTypeCategorized(baseType);

				GUILayout.BeginVertical("TextField");
				foreach (ScriptInfo taskInfo in searchResults){
					if (taskInfo.name.ToLower().Contains(search.ToLower())){
						if (GUILayout.Button(taskInfo.name)){
							search = string.Empty;
							GUIUtility.keyboardControl = 0;
							ContextAction(taskInfo.type);
						}
					}
				}
				GUILayout.EndVertical();
			}

			lastSearch = search;
		}

		public static GenericMenu GetTypeSelectionMenu(Type baseType, Action<Type> callback, string subCategory = null){
			GenericMenu.MenuFunction2 Selected = delegate(object selectedType){
				callback((Type)selectedType);
			};

			var scriptInfos = GetAllScriptsOfTypeCategorized(baseType);
			var menu = new GenericMenu();
			subCategory = string.IsNullOrEmpty(subCategory)? "" : subCategory + "/";

			foreach (ScriptInfo script in scriptInfos){
				if (string.IsNullOrEmpty(script.category))
					menu.AddItem(new GUIContent(subCategory + script.name), false, Selected, script.type);
			}

			menu.AddSeparator("/");

			foreach (ScriptInfo script in scriptInfos){
				if (!string.IsNullOrEmpty(script.category))
					menu.AddItem(new GUIContent(subCategory + script.category + "/" + script.name), false, Selected, script.type);
			}

			return menu;
		}

		//Get all scripts of a type excluding: the base type, abstract classes, Obsolete classes and those with the DoNotList attribute, from within the project categorized as a list of ScriptInfo
		public static List<ScriptInfo> GetAllScriptsOfTypeCategorized(Type baseType){

			var allRequestedScripts = new List<ScriptInfo>();
			var assetPaths = AssetDatabase.GetAllAssetPaths().Select(p => Strip(p, "/")).ToList();

			foreach (Type subType in GetAssemblyTypes(baseType)){
				
				if (subType.GetCustomAttributes(typeof(DoNotListAttribute), false).FirstOrDefault() == null && subType.GetCustomAttributes(typeof(ObsoleteAttribute), false).FirstOrDefault() == null ){

					if (subType.IsAbstract)
						continue;

					if (typeof(MonoBehaviour).IsAssignableFrom(subType)){
						if (!assetPaths.Contains(subType.Name+".cs") && !assetPaths.Contains(subType.Name+".js") && !assetPaths.Contains(subType.Name+".boo")){
							Debug.LogWarning(string.Format("Class Name {0} is different from it's script name", subType.Name));
							continue;
						}
					}

					var scriptName = SplitCamelCase( TypeName(subType) );
					var scriptCategory = string.Empty;

					var nameAttribute = subType.GetCustomAttributes(typeof(NameAttribute), false).FirstOrDefault() as NameAttribute;
					if (nameAttribute != null)
						scriptName = nameAttribute.name;

					var categoryAttribute = subType.GetCustomAttributes(typeof(CategoryAttribute), true).FirstOrDefault() as CategoryAttribute;
					if (categoryAttribute != null)
						scriptCategory = categoryAttribute.category;

					allRequestedScripts.Add(new ScriptInfo(subType, scriptName, scriptCategory));
				}
			}

			allRequestedScripts = allRequestedScripts.OrderBy(script => script.name).ToList();
			allRequestedScripts = allRequestedScripts.OrderBy(script => script.category).ToList();

			return allRequestedScripts;
		}

		//yeah this is very special but....
		public static void ShowConfiguredTypeSelectionMenu(Type type, Action<Type> callback, bool showInterfaces = true){
			GenericMenu.MenuFunction2 Selected = delegate(object t){
				callback((Type)t);
			};	
			
			var menu = new UnityEditor.GenericMenu();
			foreach (System.Type t in NCPrefs.GetCommonlyUsedTypes(typeof(object))){
				if (type.IsAssignableFrom(t) || (t.IsInterface && showInterfaces) ){
					var category = "Classes/";
					if (t.IsInterface) category = "Interfaces/";
					if (t.IsEnum) category = "Enumerations/";
					var nsString = string.IsNullOrEmpty(t.Namespace)? "No Namespace/" : (t.Namespace.Replace(".","/") + "/") ;
					menu.AddItem(new GUIContent(category + nsString + TypeName(t) ), false, Selected, t);
				}
			}

			menu.AddDisabledItem(new GUIContent("Add more in Type Configurator"));
			menu.ShowAsContext();
			Event.current.Use();
		}


		//Get all base derived types in the current loaded assemplies, excluding the base type itself
		public static List<Type> GetAssemblyTypes(Type baseType){
			
			var types = new List<Type>();
			foreach (Assembly ass in System.AppDomain.CurrentDomain.GetAssemblies()){

				if (ass.GetName().Name.Contains("Editor"))
					continue;
					
				try
				{
					foreach (Type t in ass.GetExportedTypes()){
						if (t.IsSubclassOf(baseType))
							types.Add(t);
					}
				}
				catch
				{
					Debug.Log(ass.FullName + " will be excluded");
					continue;
				}
			}
			types = types.OrderBy(type => TypeName(type) ).ToList();
			types = types.OrderBy(type => type.Namespace).ToList();
			return types;
		}

		//Gets the first type found by providing just the name of the type. Rarely used (currently for upgrading ScriptControl tasks)
		public static Type GetType(string name, Type fallback){
			foreach (Type t in GetAssemblyTypes(typeof(object))){
				if (t.Name == name)
					return t;
			}
			return fallback;
		}


		//get the right friendly name for a type
		public static string TypeName(Type t){

			if (t == null)
				return "NONE";

			string s = t.Name;
			
			if (s == "Single") s = "Float";
			if (s == "Int32") s = "Integer";

			if (t.IsGenericParameter)
				s = "T";

			if (t.IsGenericType){
				
				Type[] args= t.GetGenericArguments();
				
				if (args.Length != 0){
				
					s = s.Replace("`" + args.Length.ToString(), "");

					s += "<";
					for (int i= 0; i < args.Length; i++)
						s += (i == 0? "":", ") + TypeName(args[i]);
					s += ">";
				}
			}

			return Strip(s, ".");
		}

		//strips anything before the specified character. Mostly use to strip namespaces
		public static string Strip(string text, string before){

			int index= text.LastIndexOf(before);
			return index >= 0? text.Substring(index + 1) : text;
		}



		///Get a GenericMenu for field selection in a type
		public static GenericMenu GetFieldSelectionMenu(Type type, List<Type> availableTypes, Action<FieldInfo> callback, GenericMenu menu = null){
			
			if (menu == null)
				menu = new GenericMenu();

			GenericMenu.MenuFunction2 Selected = delegate(object selectedField){
				callback((FieldInfo)selectedField);
			};

			bool separatorAdded = false;
			bool itemAdded = false;
			foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public)){
				foreach (Type t in availableTypes){

					if (!separatorAdded && field.DeclaringType != type){
						menu.AddSeparator( TypeName(type) + "/");
						separatorAdded = true;
					}

					if (t.IsAssignableFrom(field.FieldType)){
						menu.AddItem(new GUIContent( TypeName(type) + "/" + field.Name), false, Selected, field);
						itemAdded = true;
					}
				}
			}

			if (!itemAdded)
				menu.AddDisabledItem(new GUIContent(TypeName(type)));

			return menu;
		}

		///Get a GenericMenu for method or property selection in a type
		public static GenericMenu GetMethodSelectionMenu(Type type, List<Type> returnTypes, List<Type> paramTypes, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly, GenericMenu menu = null){

			if (menu == null)
				menu = new GenericMenu();

			GenericMenu.MenuFunction2 Selected = delegate(object selectedMethod){
				callback((MethodInfo)selectedMethod);
			};

			bool itemAdded = false;
			foreach (MethodInfo method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public)){

				if (propertiesOnly && !method.IsSpecialName)
					continue;

				if (!propertiesOnly && method.IsSpecialName)
					continue;

				if (method.IsGenericMethod)
					continue;

				var isAssignable = false;
				foreach(Type t in returnTypes){
					if (t.IsAssignableFrom(method.ReturnType) ){
						isAssignable = true;
						break;
					}
				}

				if (!isAssignable)
					continue;

				if (paramTypes == null)
					maxParameters = 0;

				var parameters = method.GetParameters();
				if (parameters.Length > maxParameters && maxParameters != -1)
					continue;

				if (parameters.Length > 0){

					foreach(ParameterInfo param in parameters){
						isAssignable = false;
						foreach (Type t in paramTypes){
							if (t.IsAssignableFrom(param.ParameterType) ){
								isAssignable = true;
								break;
							}
						}
					}

				} else {

					isAssignable = true;
				}

				if (!isAssignable)
					continue;

				var methodName = method.Name + " (";
				for (int i = 0; i < parameters.Length; i++){
					var p = parameters[i];
					methodName += TypeName(p.ParameterType) + (i < parameters.Length-1? ", " : "");
				}
				methodName += ") : " + TypeName(method.ReturnType);

				menu.AddItem(new GUIContent( TypeName(type) + "/" + methodName), false, Selected, method);
				itemAdded = true;
			}
			
			if (!itemAdded)
				menu.AddDisabledItem(new GUIContent(TypeName(type)) );

			return menu;
		}


		///Shows a GenericMenu for methods of all components of a game object
		public static void ShowGameObjectMethodSelectionMenu(GameObject go, List<Type> returnTypes, List<Type> paramTypes, System.Action<MethodInfo> callback, int maxParameters, bool propertiesOnly){

			var menu = new GenericMenu();
			foreach (Component comp in go.GetComponents(typeof(Component)) ){
				if (!comp || comp.hideFlags == HideFlags.HideInInspector)
					continue;
				menu = GetMethodSelectionMenu(comp.GetType(), returnTypes, paramTypes, callback, maxParameters, propertiesOnly, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}

		///Shows a GenericMenu for methods of all components of a game object
		public static void ShowGameObjectFieldSelectionMenu(GameObject go, List<Type> availableTypes, System.Action<FieldInfo> callback){

			var menu = new GenericMenu();
			foreach (Component comp in go.GetComponents(typeof(Component)) ){
				if (!comp || comp.hideFlags == HideFlags.HideInInspector)
					continue;
				menu = GetFieldSelectionMenu(comp.GetType(), availableTypes, callback, menu);
			}
			menu.ShowAsContext();
			Event.current.Use();
		}


		public static void ShowStaticMethodSelectionMenu(List<Type> types, Action<MethodInfo> callback){

			GenericMenu.MenuFunction2 Selected = delegate(object selectedMethod){
				callback((MethodInfo)selectedMethod);
			};			

			var menu = new GenericMenu();
			foreach (Type t in GetAssemblyTypes(typeof(object)) ){
				if (t.Namespace == "UnityEngine"){
					foreach (MethodInfo method in t.GetMethods(BindingFlags.Static | BindingFlags.Public))
						menu.AddItem(new GUIContent(t.Namespace + "/" + t.Name + "/" + method.Name), false, Selected, method);
				}
			}

			menu.ShowAsContext();
			Event.current.Use();
		}




		///Returns event names found on type, that are either of the default EventHandler delegate type or of a parametless delegate handler
		public static List<string> GetAvailableEvents(Type type){

			var eventNames = new List<string>();
			foreach(EventInfo e in type.GetEvents(BindingFlags.Instance | BindingFlags.Public)){

				if (e.EventHandlerType == typeof(System.EventHandler)){
					eventNames.Add(e.Name);
					continue;
				}

				var m = e.EventHandlerType.GetMethod("Invoke");
				if (m.GetParameters().Length == 0 && m.ReturnType == typeof(void))
					eventNames.Add(e.Name);
			}

			return eventNames;
		}

		//Determines if a type can be casted to another
		public static bool CanConvert(Type fromType, Type toType) {
		    try
		    {
		        System.Linq.Expressions.Expression.Convert(System.Linq.Expressions.Expression.Parameter(fromType, null), toType);
		        return true;
		    }
		    catch
		    {
		        return false;
		    }
		}

		//all scene names (added in build settings)
		public static List<string> GetSceneNames(){

			var allSceneNames = new List<string>();

			foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes){

				if (scene.enabled){

					string name= scene.path.Substring(scene.path.LastIndexOf("/") + 1);
					name = name.Substring(0,name.Length-6);
					allSceneNames.Add(name);
				}
			}

			return allSceneNames;
		}

/*
		public static GameObject NewPrefabSafeGameObject(string name, Transform parent){

			var newGO = new GameObject();

			#if UNITY_EDITOR
			if (PrefabUtility.GetPrefabType(parent.gameObject) == PrefabType.Prefab){
				var clone = PrefabUtility.InstantiatePrefab(parent.gameObject) as GameObject;
				var root = PrefabUtility.FindPrefabRoot(clone);
				newGO.transform.parent = clone.transform;
				newGO.transform.localPosition = Vector3.zero;
				var index = newGO.transform.GetSiblingIndex();
				PrefabUtility.ReplacePrefab(root, PrefabUtility.GetPrefabParent(root), ReplacePrefabOptions.ConnectToPrefab);
				UnityEngine.Object.DestroyImmediate(root, true);
				return parent.GetChild(index).gameObject;
			}
			#endif
			
			newGO.transform.parent = parent;
			newGO.transform.localPosition = Vector3.zero;
			return newGO;
		}
*/
		//for when getting scripts
		public class ScriptInfo{

			public Type type;
			public string name;
			public string category;

			public ScriptInfo(Type type, string name, string category){
				this.type = type;
				this.name = name;
				this.category = category;
			}
		}
	}
}

#endif