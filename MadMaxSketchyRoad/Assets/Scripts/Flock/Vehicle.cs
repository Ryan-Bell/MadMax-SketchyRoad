using UnityEngine;
using System.Collections;

//use the Generic system here to make use of a Flocker list later on
using System.Collections.Generic;

[RequireComponent(typeof(CharacterController))]

abstract public class Vehicle : MonoBehaviour {

    //get access to Character Controller component
    CharacterController charControl;

    //fields necessry for movement
    protected Vector3 acceleration;
    protected Vector3 velocity;
    public Vector3 Velocity {
        get { return velocity; }
    }
    protected Vector3 desired;
    protected Vector3 steer;

    //fields 
    public float maxSpeed = 6.0f;
    public float maxForce = 12.0f;
    public float radius = 1.0f;
    public float mass = 1.0f;
    public float gravity = 20.0f;

	//Access to GameManager script
	protected GameManager gm;

    abstract protected void CalcSteeringForces();


	virtual public void Start(){
        acceleration = Vector3.zero;
        velocity = transform.forward;
        charControl = GetComponent<CharacterController>();
		gm = GameObject.Find("GameManagerGO").GetComponent<GameManager>(); 
	}

	
	// Update is called once per frame
	protected void Update () {
        CalcSteeringForces();

        //"movement formula"
        velocity += acceleration * Time.deltaTime;
        velocity.y = 0;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        if(velocity != Vector3.zero){
            transform.forward = velocity.normalized;
        }

       charControl.Move(velocity * Time.deltaTime);

       acceleration = Vector3.zero;
	}

    protected void ApplyForce(Vector3 steeringForce) {
        acceleration += steeringForce/mass;
    }

    protected Vector3 Seek(Vector3 targetPos) {
        desired = targetPos - transform.position;
        desired.Normalize();
        desired = desired * maxSpeed;
        steer = desired - velocity;
        steer.y = 0;
        return steer;
    }
    /*
	protected Vector3 AvoidObstacle(GameObject ob, float safe) {
		
		//reset desired velocity
		desired = Vector3.zero;
		//get radius from obstacle's script
		float obRad = ob.GetComponent<ObstacleScript>().Radius;
		//get vector from vehicle to obstacle
		Vector3 vecToCenter = ob.transform.position - transform.position;
		//zero-out y component (only necessary when working on X-Z plane)
		vecToCenter.y = 0;
		//if object is out of my safe zone, ignore it
		if(vecToCenter.magnitude > safe){
			return Vector3.zero;
		}
		//if object is behind me, ignore it
		if(Vector3.Dot(vecToCenter, transform.forward) < 0){
			return Vector3.zero;
		}
		//if object is not in my forward path, ignore it
		if(Mathf.Abs(Vector3.Dot(vecToCenter, transform.right)) > obRad + radius){
			return Vector3.zero;
		}
		
		//if we get this far, we will collide with an obstacle!
		//object on left, steer right
		if (Vector3.Dot(vecToCenter, transform.right) < 0) {
			desired = transform.right * maxSpeed;
			//debug line to see if the dude is avoiding to the right
			Debug.DrawLine(transform.position, ob.transform.position, Color.red);
		}
		else {
			desired = transform.right * -maxSpeed;
			//debug line to see if the dude is avoiding to the left
			Debug.DrawLine(transform.position, ob.transform.position, Color.green);
		}

		return desired;
	}*/

	public Vector3 Separate(float desiredSeparation){
		Vector3 sum = Vector3.zero;
		int count = 0;
		float distance;
		foreach (GameObject dude in gm.Flockers) {
			if ((distance = Vector3.Distance(this.transform.position, dude.transform.position)) < desiredSeparation){
				sum += (this.transform.position - dude.transform.position); 
				count++;
			}
		}
		if (count > 0) {
			sum /= count;
			sum.Normalize();
			sum *= maxSpeed;
			sum -= velocity;
		}
		Debug.DrawLine(transform.position, transform.position + sum, Color.blue);
		return sum;
	}

	public Vector3 Alignment(){
		Vector3 sum = Vector3.zero;
		sum.Normalize ();
		sum *= maxSpeed;
		sum -= velocity;
		return sum;
	}

	public Vector3 Cohesion(Vector3 center){
		center = Seek (center);
		Debug.DrawLine(transform.position, transform.position + center, Color.cyan);
		return center;
	}

	public Vector3 StayInBounds(float radius, Vector3 center){
		if (Vector3.Distance (this.transform.position, center) > radius) {
			return Seek(center);
		}
		return Vector3.zero;
	}
}
