using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    public class WaypointSwitcher : MonoBehaviour
    {
        public Vector3[] waypointlist = new Vector3[10];

        //the car that will be following the path. Targets are calculated
        //with reynolds algorithms and passed into the AI controller to give
        //the vehicle more natural physics
        public CarAIControl carai;
        public CarController car;
        //vehicle target is represented by the waypoint holder which changes position
        public Transform waypointholder;
        //int index = 0;
        public Explosion explosion;
        public GameObject marker;
        float lasttime;


        public Camera[] cameras;
        private int currentCameraIndex;

        // Use this for initialization
        void Start()
        {
            waypointlist[0] = new Vector3(-20.6f, 0f, 126.2f);
            waypointlist[1] = new Vector3(35.6f, 0f, 138.8f);
            waypointlist[2] = new Vector3(78.7f, 0f, 105.3f);
            waypointlist[3] = new Vector3(98.4f, 0f, 43.9f);
            waypointlist[4] = new Vector3(75.8f , 0f, -13.3f);
            waypointlist[5] = new Vector3(28.4f, 0f, -18.4f);
            waypointlist[6] = new Vector3(-4.2f, 0f, -2.3f);
            waypointlist[7] = new Vector3(-18.7f, 0f, 26f);
            waypointlist[8] = new Vector3(-31.6f, 0f, 57.4f);
            waypointlist[9] = new Vector3(-30f, 0f, 95f);
            carai = GetComponent(typeof(CarAIControl)) as CarAIControl;
            car = carai.GetComponent(typeof(CarController)) as CarController;
            waypointholder.position = waypointlist[0];
            carai.SetTarget(waypointholder);
            for (int i = 0; i < 10; i++)
            {
                GameObject.Instantiate(marker);
                marker.transform.position = waypointlist[i];
            }
            lasttime = 0;



            //In start initialize both
            currentCameraIndex = 0;
            //Turn all cameras off, except the first default one
            for (int i=1; i<cameras.Length; i++)
            {
                cameras[i].gameObject.SetActive(false);
            }
    
            //If any cameras were added to the controller, enable the first one
            if (cameras.Length>0)
            {
                cameras [0].gameObject.SetActive (true);
            }
        }

        //The basic steps for path following:
        //find future location
        //loop through all line segments and get the normal points
        //check validaty of the points
        //move towards the closest valid normal point
        void Update()
        {
            Vector3 futurepos = car.transform.position + car.m_Rigidbody.velocity;
            float distanceRecord = 20000000000;
            for (int i = 0; i < 9; i++)
            {
                Vector3 normalPoint = getNormalPoint(futurepos, waypointlist[i], waypointlist[i + 1]);
                if (Vector3.Distance(normalPoint, waypointlist[i]) > Vector3.Distance(waypointlist[i + 1], waypointlist[i]) || Vector3.Distance(normalPoint, waypointlist[i+1]) > Vector3.Distance(waypointlist[i + 1], waypointlist[i]))
                {
                    normalPoint = waypointlist[i + 1];
                }
                float tempDist = Vector3.Distance(futurepos, normalPoint);
                if ( tempDist < distanceRecord)
                {
                    distanceRecord = tempDist;
                    waypointholder.position = normalPoint;
                }
            }
            //create explosion behind car every five seconds
            if(Time.time > lasttime + 5){
                GameObject.Instantiate(explosion, car.transform.position - car.m_Rigidbody.velocity / 2, Quaternion.identity);
                lasttime = Time.time;
            }

            if (Input.GetKeyDown(KeyCode.C)) //can be any key you want
            {
                currentCameraIndex++;
                if (currentCameraIndex < cameras.Length)
                {
                    cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                    cameras[currentCameraIndex].gameObject.SetActive(true);
                }
                else
                {
                    cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                    currentCameraIndex = 0;
                    cameras[currentCameraIndex].gameObject.SetActive(true);
                }
            }
            return;
        }

        public Vector3 getNormalPoint(Vector3 predictedLoc, Vector3 startpoint, Vector3 endpoint)
        {
            Vector3 shadowline = predictedLoc - startpoint;
            Vector3 lineSegment = endpoint - startpoint;

            lineSegment.Normalize();
            lineSegment *= (Vector3.Dot(shadowline, lineSegment));

            return (startpoint + lineSegment);
        }
    }
}