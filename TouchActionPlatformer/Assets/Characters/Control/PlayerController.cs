﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public enum primaryState {ground, jumpLeft, jumpRight, fall, dive, land};
	public enum secondaryState {none, turn, land};
	public enum directionState {none, left, right};
	public enum attackState {none, melee, rangecharge, rangefire};
	public primaryState primary = primaryState.ground;
	public secondaryState secondary = secondaryState.none;
	public directionState direction = directionState.none;

	public bool turning = false;
	public float turnChangeTime = 0;

	public float primaryChangeTime = 0;
	public float secondaryChangeTime = 0;

	public float turnBuffer = 0.2f;
	
	
	public float diveExplosion = 25f;
	public float landBuffer = 0.15f;
	
	public int airJumpsMax = 1;
	private int airJumps = 0;

	public float runSpeed = 14f;
	public float runExplosion = 2f;
	public float runAcceleration = 100f;
	public float jumpTime = 0.5f;
	public float jumpAcceleration = 2f;
	public float jumpSpeed = 2f;
	public float jumpExplosion = 10f;
	public float airSpeed = 10f;
	public float airAcceleration = 30f;
	public float velocityDampening = 0.6f;
	public float airDampening = 0.95f;
	public float fallExplosion = 3f;
	public float turnExplosion = 10f;

	public float gravity = 25f;
	private float hiddenGravity = 25f;

	public Vector3 velocity;

	// Update is called once per frame
	void Update () {

		// preliminary grounded test, expand later
		if (transform.position.y <= 0) {
			primary = primaryState.ground;
			primaryChangeTime = Time.time;
			transform.position -= Vector3.up * transform.position.y;
			airJumps = 0;
		}

		// process controls
		bool leftHold = Input.GetButton ("leftTouch");
		bool rightHold = Input.GetButton ("rightTouch");
		bool leftDown = Input.GetButtonDown ("leftTouch");
		bool rightDown = Input.GetButtonDown ("rightTouch");
		bool leftUp = Input.GetButtonUp ("leftTouch");
		bool rightUp = Input.GetButtonUp ("rightTouch");

		// android controls
		#if UNITY_ANDROID
		if (Input.touchCount > 0) {
			foreach (Touch t in Input.touches) {
				if (t.phase == TouchPhase.Began) {
					if (t.position.x < Screen.width/2) {
						leftDown = true;
						leftHold = true;
					} else {
						rightDown = true;
						rightHold = true;
					}
				} else if (t.phase == TouchPhase.Ended) {
					if (t.position.x < Screen.width/2) {
						leftUp = true;
					} else {
						rightUp = true;
					}
				} else {
					if (t.position.x < Screen.width/2) {
						leftHold = true;
					} else {
						rightHold = true;
					}
				}
			}
		}
		#endif
		
		bool dive = Input.GetButtonDown("leftDrop") || Input.GetButtonDown("rightDrop");
		
		if (!leftHold && !rightHold) {
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
				if (leftDown && (airJumps < airJumpsMax)) {
					turning = true;
					velocity.y = 0;
					turnChangeTime = Time.time;
					direction = directionState.left;
					velocity.x = -turnExplosion;
					//if (primary == primaryState.ground) {
					//}
				} else if (rightUp && leftHold) {
					direction = directionState.left;
				} else if (rightUp) {
					direction = directionState.none;
				}
			} else if (turning) {
				if (leftDown) {
					if (primary == primaryState.ground) {
						jump ();
					} else if (airJumps < airJumpsMax) {
						airJumps += 1;
						jump ();
					}
				} else if (rightUp) {
					direction = directionState.left;
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
				if (rightDown && (airJumps < airJumpsMax)) {
					turning = true;
					velocity.y = 0;
					turnChangeTime = Time.time;
					direction = directionState.right;
					velocity.x = turnExplosion;
					//if (primary == primaryState.ground) {
					//}
				} else if (leftUp && rightHold) {
					direction = directionState.right;
				} else if (leftUp) {
					direction = directionState.none;
				}
			} else if (turning) {
				if (rightDown) {
					if (primary == primaryState.ground) {
						jump ();
					} else if (airJumps < airJumpsMax) {
						airJumps += 1;
						jump ();
					}
				} else if (leftUp) {
					direction = directionState.right;
					if (primary == primaryState.ground) {
						if (velocity.x < runExplosion) {
							velocity.x = runExplosion;
						}
					}
				}
			}
			if (rightUp && primary == primaryState.jumpLeft) {
				primary = primaryState.fall;
				primaryChangeTime = Time.time;
				// velocity.y = -fallExplosive;
			}
		}
		
		if (dive) {
			primary = primaryState.dive;
			velocity.y = -diveExplosion;
		}

		// change of state due to actions finishing
		if ((primary == primaryState.jumpLeft || primary == primaryState.jumpRight)
		    	&& primaryChangeTime + jumpTime < Time.time) {
			primary = primaryState.fall;
			primaryChangeTime = Time.time;
			// velocity.y = -fallExplosive;
		}
		
		if (turning && turnChangeTime + turnBuffer < Time.time) {
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
				if (airJumps == 0 && direction == directionState.right && velocity.x < runSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (direction == directionState.right && velocity.x < airSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (direction == directionState.left && velocity.x > -airSpeed) {
					velocity.x -= airAcceleration * Time.deltaTime;
				}
			} else if (primary == primaryState.jumpLeft) {
				velocity.y += jumpAcceleration * Time.deltaTime;
				if (direction == directionState.right && velocity.x < airSpeed) {
					velocity.x += airAcceleration * Time.deltaTime;
				} else if (airJumps == 0 && direction == directionState.left && velocity.x > -runSpeed) {
					velocity.x -= airAcceleration * Time.deltaTime;
				} else if (direction == directionState.left && velocity.x > -airSpeed) {
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
		if ( primary == primaryState.jumpLeft || primary == primaryState.jumpRight) {
			transform.position += Vector3.up * jumpSpeed * Time.deltaTime;	
		}
	}

	private void jump() {
		turning = false;
		if (direction == directionState.left) {
			if (primary == primaryState.ground) {
				velocity.x = -runSpeed;
			} else {
				velocity.x = -airSpeed;
			}
			primary = primaryState.jumpLeft;
			
		} else {
			if (primary == primaryState.ground) {
				velocity.x = runSpeed;
			} else {
				velocity.x = airSpeed;
			}
			primary = primaryState.jumpRight;
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
