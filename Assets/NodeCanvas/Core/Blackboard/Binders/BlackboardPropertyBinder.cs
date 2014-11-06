using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Variables;
using System.Reflection;

namespace NodeCanvas{

	[AddComponentMenu("NodeCanvas/Blackboard Property Binder")]
	public class BlackboardPropertyBinder : MonoBehaviour {

		[System.Serializable]
		public class Binder{
			
			public enum BindingType{
				VariableToProperty,
				PropertyToVariable
			}

			public BindingType bindingType = BindingType.VariableToProperty;

			public string variableName;
			public string componentName;
			public string propertyName;

			private Component component;
			private MethodInfo setter;
			private MethodInfo getter;

			private VariableData data;

			[SerializeField] [HideInInspector]
			private string _typeName = typeof(object).AssemblyQualifiedName;

			public System.Type type{
				get {return System.Type.GetType(_typeName);}
				set {_typeName = value.AssemblyQualifiedName;}
			}

			public Binder(VariableData data){
				this.variableName = data.dataName;
				this.type = data.varType;
				this.data = data;
			}

			public void Init(Blackboard bb, GameObject go){

				component = go.GetComponent(componentName);
				if (component == null){
					Debug.LogWarning(string.Format("<b>Property Binder:</b> GameObject doesn't have '{0}' component type", componentName), go);
					return;
				}

				data = bb.GetData(variableName, type);
				if (data == null){
					Debug.LogWarning(string.Format("<b>Property Binder:</b> Blackboard doesn't have variable with name '{0}' and type '{1}'", variableName, type.Name), bb);
					return;
				}

				if (bindingType == BindingType.VariableToProperty){
					setter = component.GetType().NCGetMethod("set_" + propertyName);
					if (setter == null){
						Debug.LogWarning(string.Format("<b>Property Binder:</b> Component '{0}' doesn't have '{1}' setter property", componentName, propertyName), go);
						return;
					}

					data.onValueChanged += OnValueChanged;
					OnValueChanged(variableName, data.objectValue);
				}
				else
				if (bindingType == BindingType.PropertyToVariable){
					getter = component.GetType().NCGetMethod("get_" + propertyName);
					if (getter == null){
						Debug.LogWarning(string.Format("<b>Property Binder:</b> Component '{0}' doesn't have '{1}' getter property", componentName, propertyName), go);
						return;
					}
				}

				Debug.Log(string.Format("Binded blackboard variable '{0}' with '{1}.{2}' property", variableName, componentName, propertyName), go );
			}

			void OnValueChanged(string name, object value){
				setter.Invoke(component, new object[]{value});
			}

			object lastValue;
			object currentValue;
			public void Update(){
				if (bindingType != BindingType.PropertyToVariable)
					return;

				currentValue = getter.Invoke(component, null);
				if (lastValue != currentValue){
					data.objectValue = currentValue;
					lastValue = currentValue;
				}
			}
		}

		public Blackboard blackboard;
		new public GameObject gameObject;
		public List<Binder> binders = new List<Binder>();

		private bool binded;

		void Reset(){
			blackboard = GetComponent<Blackboard>();
			gameObject = this.transform.gameObject;
		}

		void OnEnable(){
			
			if (binded)
				return;

			binded = true;

			if (!blackboard)
				blackboard = GetComponent<Blackboard>();
			
			if (!gameObject)
				gameObject = this.transform.gameObject;
			
			if (!blackboard || !gameObject)
				return;

			foreach (Binder binder in binders)
				binder.Init(blackboard, gameObject);
		}

		void LateUpdate(){
			foreach (Binder binder in binders)
				binder.Update();
		}
	}
}