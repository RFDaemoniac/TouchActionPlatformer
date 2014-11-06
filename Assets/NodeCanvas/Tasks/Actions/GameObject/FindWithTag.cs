using UnityEngine;
using System.Collections.Generic;
using NodeCanvas.Variables;

namespace NodeCanvas.Actions{

	[Category("GameObject")]
	public class FindWithTag : ActionTask{

		[RequiredField] [TagField]
		public string searchTag = "Untagged";
		
		[RequiredField] [BlackboardOnly]
		public BBGameObject saveAs;

		protected override string info{
			get{return "GetObjects '" + searchTag + "' as " + saveAs;}
		}

		protected override void OnExecute(){

			saveAs.value = GameObject.FindWithTag(searchTag);
			EndAction(true);
		}
	}
}