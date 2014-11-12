using UnityEngine;
using System.Collections;


[RequireComponent (typeof (Animator))]
public class ToggleTurnAway : MonoBehaviour {


	private Animator animator;


	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator>();
		//StartCoroutine(ToggleMechanimTurnAway());
	}

	IEnumerator ToggleMechanimTurnAway() {
		while(true) {
			animator.SetFloat("Away", 0);
			yield return new WaitForSeconds(Random.Range(0.5f, 5f));
			animator.SetFloat("Away", 1);
			yield return new WaitForSeconds(Random.Range(0.5f, 5f));
		}
	}

	public void ProbablyToggleTurnAway() {
		if (Random.value < 0.3f) {
			animator.SetFloat("Away", 1 - animator.GetFloat("Away"));
		}
	}
}
