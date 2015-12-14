
using UnityEngine;
using System.Collections;

//add using System.Collections.Generic; to use the generic list format
using System.Collections.Generic;

public class GameManager : MonoBehaviour {


    public GameObject dude;
    public GameObject target;

    public GameObject dudePrefab;
    public GameObject targetPrefab;
    public GameObject obstaclePrefab;

	private Vector3 flockCentroid;
	private Vector3 flockDirection;
    private GameObject[] obstacles;
	private GameObject[] flockers;
	public GameObject[] Obstacles {
		get { return obstacles; }
	}

	public GameObject[] Flockers {
		get { return flockers; }
	}

	public Vector3 FlockCentroid {
		get { return flockCentroid; }
	}

	public Vector3 FlockDirection {
		get { return flockDirection; } 
	}

	void Start () {
        
        //make a target at (0, 0, 0) with no rotation
        Vector3 pos = new Vector3(0, 0, 0);
        //no rotation = Quaternion.identity
        //target = (GameObject)Instantiate(targetPrefab, pos, Quaternion.identity);

		flockers = new GameObject[10];
		for (int i = 0; i < 10; i++) {

            pos = new Vector3(6 * i - 100, 0f, 6 * i - 30);
			dude = (GameObject)Instantiate (dudePrefab, pos, Quaternion.identity);

			
			dude.GetComponent<Seeker> ().seekerTarget = target;
			

			flockers[i] = dude;
		}


		obstacles = new GameObject[20];
		//Create obstacles and place them in the obstacles array
        /*
		for(int i = 0; i < 20; i++){
			pos = new Vector3(Random.Range(-30, 30), 4f, Random.Range(-30, 30));
			Quaternion rot = Quaternion.Euler(new Vector3(0, Random.Range(0,180), 0));
			obstacles[i] = (GameObject)Instantiate(obstaclePrefab, pos, rot);
		}*/
		//Camera.main.GetComponent<SmoothFollow>().target = this.transform;
	}


	void Update () {
		/*float dist = Vector3.Distance(target.transform.position, this.transform.position);
		if( dist < 6f){
			do{
				target.transform.position = new Vector3(Random.Range(-30, 30), 4f, Random.Range(-30, 30));
			}
			while(NearAnObstacle());
		}*/
		CalcCentroid ();
		CalcFlockDirection ();
		this.transform.position = flockCentroid;
		this.transform.rotation = dude.transform.rotation;
	}
    /*
	bool NearAnObstacle(){

		
		for(int i = 0; i < obstacles.Length; i++){
			if(Vector3.Distance(target.transform.position, obstacles[i].transform.position) < 5.0f){
				return true;
			}
		}
		
		return false;
	}*/



	private void CalcCentroid(){
		flockCentroid = Vector3.zero;
		float distance;
		int count = 0;
		foreach (GameObject dude in Flockers) {
			distance = Vector3.Distance(this.transform.position, dude.transform.position);		
			flockCentroid += dude.transform.position;
			count++;
		}
		if (count > 0) {
			 flockCentroid /= count;
		}
	}

	private void CalcFlockDirection(){
		flockDirection = Vector3.zero;
		float distance;
		int count = 0;
		foreach (GameObject dude in Flockers){
			flockDirection += dude.GetComponent<Seeker>().Velocity;
			count++;
		}
		flockDirection /= count;
	}
}