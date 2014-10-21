using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public enum primaryState {ground, jumpLeft, jumpRight, fall, dive};
	public enum secondaryState {none, turn, melee, rangecharge, rangefire};
	public enum directionState {none, left, right};
	public primaryState primary = primaryState.ground;
	public secondaryState secondary = secondaryState.none;
	public directionState direction = directionState.none;

	public float primaryChangeTime = 0;
	public float secondaryChangeTime = 0;

	public float doubleClickTime = 0.1f;

	public float runSpeed = 8f;
	public float runExplosion = 2f;
	public float runAcceleration = 4f;
	public float jumpTime = 0.8f;
	public float jumpAcceleration = 2f;
	public float jumpExplosive = 8.5f;
	public float airSpeed = 10f;
	public float airAcceleration = 30f;
	public float velocityDampening = 0.6f;
	public float airDampening = 0.95f;
	public float fallExplosive = 3f;

	public float gravity = 10f;
	private float hiddenGravity = 0f;

	public Vector3 velocity;


	// Update is called once per frame
	void Update () {

		// preliminary grounded test, expand later
		if (transform.position.y <= 0) {
			primary = primaryState.ground;
			primaryChangeTime = Time.time;
			transform.position -= Vector3.up * transform.position.y;
		}

		// process controls
		if (direction == directionState.none) {
			if (Input.GetButtonDown("leftTouch") && !Input.GetButtonDown("rightTouch")) {
				direction = directionState.left;
				if ( primary == primaryState.ground) {
					velocity.x = -runExplosion;
				}
			} else if (Input.GetButtonDown("rightTouch") && !Input.GetButtonDown("leftTouch")) {
				direction = directionState.right;
				if ( primary == primaryState.ground) {
					velocity.x = runExplosion;
				}
			} else if (primary == primaryState.ground && Input.GetButtonDown("rightTouch") && Input.GetButtonDown("leftTouch")) {
				jump ();
			}
		} else if (direction == directionState.right) {
			if (Input.GetButtonUp ("rightTouch")) {
				direction = directionState.none;
			} else if (Input.GetButtonDown ("leftTouch") && primary == primaryState.ground) {
				jump ();
			} else if (Input.GetButtonUp("leftTouch") && primary == primaryState.jumpRight) {
				primary = primaryState.fall;
				primaryChangeTime = Time.time;
				// velocity.y = -fallExplosive;
			}
		} else if (direction == directionState.left) {
			if (Input.GetButtonUp ("leftTouch")) {
				direction = directionState.none;
			} else if (Input.GetButtonDown ("rightTouch") && primary == primaryState.ground) {
				jump ();
			} else if (Input.GetButtonUp("rightTouch") && primary == primaryState.jumpLeft) {
				primary = primaryState.fall;
				primaryChangeTime = Time.time;
				// velocity.y = -fallExplosive;
			}
		}

		// do actions
		/*
		if (primary == primaryState.ground) {
			if (direction == directionState.right) {
				transform.position += Vector3.right * runSpeed * Time.deltaTime;
			} else if (direction == directionState.left) {
				transform.position -= Vector3.right * runSpeed * Time.deltaTime;
			}
		} else if (primary == primaryState.jumpLeft || primary == primaryState.jumpRight) {
			transform.position += Vector3.up * jumpAcceleration * Time.deltaTime;
		}

		if ( primary == primaryState.jumpLeft || primary == primaryState.jumpRight
		    	|| primary == primaryState.fall || primary == primaryState.dive) {
			if (direction == directionState.right) {
				transform.position += Vector3.right * airSpeed * Time.deltaTime;
			} else if (direction == directionState.left) {
				transform.position -= Vector3.right * airSpeed * Time.deltaTime;
			}
		}
		*/
		// change of state due to actions finishing
		if ((primary == primaryState.jumpLeft || primary == primaryState.jumpRight)
		    	&& primaryChangeTime + jumpTime < Time.time) {
			primary = primaryState.fall;
			primaryChangeTime = Time.time;
			// velocity.y = -fallExplosive;
		}
		// apply velocity
		if (primary == primaryState.ground) {
			velocity.y = 0;
			if (direction == directionState.right && velocity.x < runSpeed) {
				velocity.x += runAcceleration * Time.deltaTime;
			} else if (direction == directionState.left && velocity.x > -runSpeed) {
				velocity.x -= runAcceleration * Time.deltaTime;
			}
			if (direction == directionState.none) {
				velocity.x *= velocityDampening * (1 - Time.deltaTime);
			} else if (direction == directionState.right && velocity.x > runSpeed) {
				velocity.x = runSpeed;
			} else if (direction == directionState.left && velocity.x < -runSpeed) {
				velocity.x = -runSpeed;
			}
		} else {
			hiddenGravity += gravity * Time.deltaTime;
			velocity.y -= hiddenGravity * Time.deltaTime;
			if (primary == primaryState.jumpRight) {
				velocity.y += jumpAcceleration * Time.deltaTime;
				if (direction == directionState.right && velocity.x < runSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (direction == directionState.left && velocity.x > -airSpeed) {
					velocity.x -= airAcceleration * Time.deltaTime;
				}
			} else if (primary == primaryState.jumpLeft) {
				velocity.y += jumpAcceleration * Time.deltaTime;
				if (direction == directionState.right && velocity.x < airSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (direction == directionState.left && velocity.x > -runSpeed) {
					velocity.x -= airAcceleration * Time.deltaTime;
				}
			} else {
				if (direction == directionState.right && velocity.x < airSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (direction == directionState.left && velocity.x > -airSpeed) {
					velocity.x -= airAcceleration * Time.deltaTime;
				}
				velocity.x *= airDampening * (1 - Time.deltaTime);
			}
		}
		if (Mathf.Abs (velocity.x) < 0.001f) {
			velocity.x = 0;
		}
		transform.position += velocity * Time.deltaTime;
	}

	private void jump() {
		if (direction == directionState.left) {
			primary = primaryState.jumpLeft;
			velocity.x = -runSpeed;
		} else {
			primary = primaryState.jumpRight;
			velocity.x = runSpeed;
		}
		primaryChangeTime = Time.time;
		velocity.y = jumpExplosive;
		hiddenGravity = gravity;
	}

}
