using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    ParticleSystem.Particle[] particleArray;
    ParticleSystem system;
    public GameObject systemHolder;

    //How many particles will initially be emitted
    public int emitCount = 500;

    //The inverse factor for initial speed (ex. 0.5 will double init speed)
    public float speedFactor = 0.3f;

	void Start () {

        //get the particle system from the GameObject containing it
        system = systemHolder.GetComponent<ParticleSystem>();

        //create the specified count of particles
        system.Emit(emitCount);
        
        //create an array to hold the particles
        particleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem>().particleCount];

        //fill particle array with system's particles so they can be manipulated
        system.GetParticles(particleArray);
        
        float offset = 2.0f / emitCount;
        float increment = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));
        float x, y, z, r;

        //loop set the initial velocity of each of the particles in radial pattern using fibonnacci spiral
        for (float i = 0; i < system.particleCount; i++)
        {
            y = ((i * offset) - 1) + (offset / 2.0f);
            r = Mathf.Sqrt(1 - Mathf.Pow(y, 2.0f));

            //calculate an approximation of phi
            float phi = ((i + 1) % emitCount) * increment;

            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            //set particle position and velocity
            particleArray[(int)i].position = new Vector3(x, y, z);
            particleArray[(int)i].velocity = new Vector3(x / speedFactor, y / speedFactor, z / speedFactor);

        }
        //set the position and velocity of the particles in the system based on array
        system.SetParticles(particleArray, system.particleCount);
	}
	
	void Update () {
        //there should be a factor so that the particles slow down (maybe maxspeed?)
            //the slow down should be a steep falloff rather than linear
        //the color and dissapation can be set with the particle system itself

	    //get all the particles
        //loop through and change velocity based on perlin noise field
        //set particles
	}
}
