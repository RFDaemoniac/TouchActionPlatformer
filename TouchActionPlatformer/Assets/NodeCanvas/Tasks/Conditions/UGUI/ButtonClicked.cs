
#if UNITY_4_6

using UnityEngine;
using UnityEngine.UI;
using NodeCanvas.Variables;

namespace NodeCanvas.Conditions{

	[Category("UGUI")]
	public class ButtonClicked : ConditionTask {

		[VariableType(typeof(Button))] [RequiredField]
		public BBObject button;

		protected override string OnInit(){
			(button.value as Button).onClick.AddListener(OnClick);
			return null;
		}

		protected override bool OnCheck(){
			return false;
		}

		void OnClick(){
			YieldReturn(true);
		}
	}
}

#endif