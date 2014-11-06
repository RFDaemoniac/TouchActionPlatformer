using UnityEngine;
using System.Collections;

namespace NodeCanvas.DialogueTrees{

	///Holds data of a dialogue actor that is contained in the argument of an OnActorSpeaking event
	[AddComponentMenu("NodeCanvas/Dialogue Actor")]
	public class DialogueActor : MonoBehaviour{

		[SerializeField]
		private string _actorName;

		public Texture2D portrait;
		public Color dialogueColor = Color.white;
		public Vector3 dialogueOffset;
		public Blackboard blackboard;

		private Sprite _portraitSprite;

		new public string name{
			get{return _actorName;}
			set{_actorName = value;}
		}

		///The actor name
		[System.Obsolete("Use 'name' instead")]
		public string actorName{
			get {return _actorName;}
			set {_actorName = value;}
		}

		public Sprite portraitSprite{
			get
			{
				if (_portraitSprite == null && portrait != null)
					_portraitSprite = Sprite.Create(portrait, new Rect(0,0,portrait.width, portrait.height), new Vector2(0.5f, 0.5f));
				return _portraitSprite;
			}
		}

		public string speech{get;set;}

		///The final position for the dialogue to show based on settings
		public Vector3 dialoguePosition{
			get{return Vector3.Scale(transform.position + dialogueOffset, transform.localScale);}
		}

		void Reset(){
			name = gameObject.name;
		}

		public void Say(string text, System.Action callback){
			Say(new Statement(text), callback);
		}

		///Helper function to make the actor talk. Dispatches the nescessary events
		public void Say(Statement statement, System.Action callback){
			speech = null;
			if (!EventHandler.Dispatch(DLGEvents.OnActorSpeaking, new DialogueSpeechInfo(this, statement, callback)))
				Debug.LogWarning("Make sure you have added the default <b>@DialogueGUI</b> prefab, or you have created your own UI to handle the events dispatched");
		}

		///Helper function to make the actor stop talking. Dispatches the nescessary events
		public void StopTalking(){
			speech = null;
			EventHandler.Dispatch(DLGEvents.OnDialogueFinished);
		}

		///An *optional* coroutine to process a statement writing text over time to the 'speech' property fo the actor as well as playing audio.
		public IEnumerator ProcessStatement(Statement statement, System.Action callback){

			if (statement.audio){

				var audioSource = gameObject.AddComponent<AudioSource>();
				audioSource.PlayOneShot(statement.audio);

				float timer = 0;
				speech = statement.text;
				while(timer < statement.audio.length){
					timer += Time.deltaTime;
					yield return null;
				}

				DestroyImmediate(audioSource);

			} else {

				for (int i= 0; i < statement.text.Length; i++){
					speech += statement.text[i];
					yield return new WaitForSeconds(0.05f);
					char c = statement.text[i];
					if (c == '.' || c == '!' || c == '?')
						yield return new WaitForSeconds(0.5f);
					if (c == ',')
						yield return new WaitForSeconds(0.1f);
				}

				yield return new WaitForSeconds(1.2f);
			}

			speech = null;
			callback();
		}

		void OnDrawGizmos(){
			Gizmos.DrawLine(transform.position, dialoguePosition);
		}

		///Finds an actor with provided name. Null if none found
		public static DialogueActor Find(string name){
			foreach (DialogueActor actor in FindObjectsOfType(typeof(DialogueActor))){
				if (actor.name == name)
					return actor;
			}

			return null;
		}

	}

	///Holds data of what's being said also contained in the argument of an OnActorSpeaking event
	[System.Serializable]
	public class Statement{

		public string text = string.Empty;
		public AudioClip audio;
		public string meta = string.Empty;

		public Statement(){
		}

		public Statement(string text){
			this.text = text;
		}

		///Replace the text of the statement found in brackets, with blackboard variables ToString
		public Statement BlackboardReplace(Blackboard bb){
			string s = text;
			int i = 0;
			while ( (i = s.IndexOf('[', i)) != -1){
				int end = s.Substring(i + 1).IndexOf(']');
				string varName = s.Substring(i + 1, end);
				string output = s.Substring(i, end + 2);
				object o = null;
				if (bb != null)
					o = bb.GetDataValue<object>(varName);
				s = s.Replace(output, o != null? o.ToString() : "*" + varName + "*");
				i++;
			}

			var replacedStatement = new Statement(s);
			replacedStatement.audio = audio;
			replacedStatement.meta = meta;
			return replacedStatement;			
		}

		public override string ToString(){
			return text;
		}
	}
}