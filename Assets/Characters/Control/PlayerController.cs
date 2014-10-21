using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public enum primaryState {ground, jumpLeft, jumpRight, fall, dive};
	public enum secondaryState {none, melee, rangecharge, rangefire};
	public enum directionState {none, left, right};
	public primaryState primary = primaryState.ground;
	public secondaryState secondary = secondaryState.none;
	public directionState direction = directionState.none;

	public bool turning = false;
	public float turnTime = 0;

	public float primaryChangeTime = 0;
	public float secondaryChangeTime = 0;

	public float doubleTapTime = 0.1f;

	public float runSpeed = 8f;
	public float runExplosion = 2f;
	public float runAcceleration = 4f;
	public float jumpTime = 0.8f;
	public float jumpAcceleration = 2f;
	public float jumpExplosion = 8.5f;
	public float airSpeed = 10f;
	public float airAcceleration = 30f;
	public float velocityDampening = 0.6f;
	public float airDampening = 0.95f;
	public float fallExplosion = 3f;
	public float turnExplosion = 6f;

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
		bool leftHold = Input.GetButton ("leftTouch");
		bool rightHold = Input.GetButton ("rightTouch");
		bool leftDown = Input.GetButtonDown ("leftTouch");
		bool rightDown = Input.GetButtonDown ("rightTouch");
		bool leftUp = Input.GetButtonUp ("leftTouch");
		bool rightUp = Input.GetButtonUp ("rightTouch");

		if (! leftHold && !rightHold) {
			direction = directionState.none;
			turning = false;
		}

		if (direction == directionState.none) {
			if (rightDown) {
				direction = directionState.right;
				if (primary == primaryState.ground) {
					velocity.x = runExplosion;
				}
			} else if (leftDown) {
				direction = directionState.left;
				if (primary == primaryState.ground) {
					velocity.x = -runExplosion;
				}
			}
		} else if (mRight()) {
			if (!turning) {
				if (leftDown) {
					turning = true;
					turnTime = Time.time;
					direction = directionState.left;
					if (primary == primaryState.ground) {
						velocity.x = -turnExplosion;
					}
				} else if (rightUp) {
					direction = directionState.none;
				}
			} else if (turning) {
				if (leftDown) {
					jump ();
				} else if (rightUp) {
					direction = directionState.left;
					turning = false;
					if (primary == primaryState.ground) {
						if (velocity.x > -runExplosion) {
							velocity.x = -runExplosion;
						}
					}
				}
			}
			if (leftUp && primary == primaryState.jumpRight) {
				primary = primaryState.fall;
				primaryChangeTime = Time.time;
				// velocity.y = -fallExplosive;
			}
		} else if (mLeft()) {
			if (!turning) {
				if (rightDown) {
					turning = true;
					turnTime = Time.time;
					direction = directionState.right;
					if (primary == primaryState.ground) {
						velocity.x = turnExplosion;
					}
				} else if (leftUp) {
					direction = directionState.none;
				}
			} else if (turning) {
				if (leftUp) {
					direction = directionState.right;
					if (primary == primaryState.ground) {
						velocity.x = turnExplosion;
					}
				} else if (rightDown) {
					jump ();
				}
			}
			if (rightUp && primary == primaryState.jumpLeft) {
				primary = primaryState.fall;
				primaryChangeTime = Time.time;
				// velocity.y = -fallExplosive;
			}
		}

		// change of state due to actions finishing
		if ((primary == primaryState.jumpLeft || primary == primaryState.jumpRight)
		    	&& primaryChangeTime + jumpTime < Time.time) {
			primary = primaryState.fall;
			primaryChangeTime = Time.time;
			// velocity.y = -fallExplosive;
		}
		
		if (turning && turnTime + doubleTapTime < Time.time) {
			turning = false;
			/*
			if (mLeft () && rightHold) {
				direction = directionState.right;
			} else if (mRight () && leftHold) {
				direction = directionState.left;
			}
			*/
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
		turning = false;
		if (direction == directionState.left) {
			primary = primaryState.jumpLeft;
			velocity.x = -runSpeed;
		} else {
			primary = primaryState.jumpRight;
			velocity.x = runSpeed;
		}
		primaryChangeTime = Time.time;
		velocity.y = jumpExplosion;
		hiddenGravity = gravity;
	}

	private bool mRight() {
		return (direction == directionState.right);
	}

	private bool mLeft() {
		return (direction == directionState.left);
	}

}
