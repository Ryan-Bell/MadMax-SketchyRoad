using UnityEngine;
using System.Collections;

public class Seeker : Vehicle {

    //what is this Seeker going after?
    public GameObject seekerTarget;

    //weighting
    public float seekWeight = 75.0f;

    //what is my steering force at the moment?
    Vector3 steeringForce;

	public float safeDistance = 10.0f;
	public float separationDistance = 18f;
	public float avoidWeight = 100.0f;
	public float separationWeight = 20f;
	public float alignmentWeight = 10f;
	public float cohesionWeight = 10f;

	// Call Inherited Start and then do our own
	override public void Start () {
        //call parent's start
		base.Start();

        //initialize the steering force
        steeringForce = Vector3.zero;
	}

    protected override void CalcSteeringForces() {
        //reset the steering force
        steeringForce = Vector3.zero;


        //call methods for each steering force

		steeringForce = seekWeight * Seek (seekerTarget.transform.position);
		//steeringForce += avoidWeight * AvoidObstacle(FindNearestObstacle() , safeDistance);
		steeringForce += separationWeight * Separate (separationDistance);
		steeringForce += alignmentWeight * Alignment ();
		steeringForce += cohesionWeight * Seek (gm.FlockCentroid);

        //limit the 1 steering force (ultimate force)
        steeringForce = Vector3.ClampMagnitude(steeringForce, maxForce);


        //apply them as 1 force (ultimate force) in ApplyForce()
        ApplyForce(steeringForce);

    }

    /*
	GameObject FindNearestObstacle(){
		float distanceTo = 1000;
		GameObject returned = null;
		foreach (GameObject obj in gm.Obstacles) {
			if(Vector3.Distance(obj.transform.position, this.transform.position) < distanceTo){
				returned = obj;
				distanceTo = Vector3.Distance(obj.transform.position, this.transform.position);
			}
		}
		return returned;
	}*/

}
