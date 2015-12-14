using UnityEngine;
using System.Collections;

namespace UnityStandardAssets.Vehicles.Car
{
    public class WaypointSwitcher : MonoBehaviour
    {
        Vector3[] waypointlist = new Vector3[7];
        public CarAIControl car;
        public Transform waypointholder;
        int index = 0;
        public Explosion explosion;
        // Use this for initialization
        void Start()
        {
            waypointlist[0] = new Vector3(20, 0, 0);
            waypointlist[1] = new Vector3(0, 0, 0);
            waypointlist[2] = new Vector3(0, 0, 30);
            waypointlist[3] = new Vector3(10, 0, 10);
            waypointlist[4] = new Vector3(0, 0, 20);
            waypointlist[5] = new Vector3(20, 0, 15);
            waypointlist[6] = new Vector3(0, 0, 0);
            car = GetComponent(typeof(CarAIControl)) as CarAIControl;
            waypointholder.position = waypointlist[0];
            car.SetTarget(waypointholder);
        }

        // Update is called once per frame
        void Update()
        {
            if (Vector3.Distance(waypointholder.position, car.transform.position) < 3)
            {
                index++;
                index %= 7;
                waypointholder.position = waypointlist[index];
                car.SetTarget(waypointholder);
                GameObject.Instantiate(explosion);
            }
        }
    }
}