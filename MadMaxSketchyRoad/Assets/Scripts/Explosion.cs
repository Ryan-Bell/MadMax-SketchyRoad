using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    ParticleSystem.Particle[] particleArray;
    ParticleSystem system;
    public GameObject systemHolder;
    
    //How many particles will initially be emitted
    public int emitCount = 4000;

    //The inverse factor for initial speed (ex. 0.5 will double init speed)
    public float speedFactor = 0.3f;

    //the percent of curl added to the particles' velocity each frame
    //0 would be no curl and 1 would make the particles follow the flow lines exactly
    [Range(0f,1f)]
    public float curlFactor = .07f;

    //defines how quickly the perlin field should step
    public float morphSpeed;

    //defines how strong the amplitude of the perlin field is 
    //(or actually is the amplitude if damping is false)
    [Range(0f, 1f)]
    public float strength = .215f;

    public bool damping;

    public float frequency = 1f;

    //define the octaves for the perlin noise which defines how many 'layers' of noise to overlay together
    //Higher values increase resolution/quality but is very very expensive
    [Range(1, 8)]
    public int octaves = 1;

    //how quickly the frequency increases in each successive octave
    [Range(1f, 4f)]
    public float lacunarity = 2f;

    //how quickly the amplitude decreases in each successive octave
    [Range(0f, 1f)]
    public float persistence = 0.5f;

    //will be used in advancing the perlin field
    private float morphOffset;

	void Start () 
    {

        //get the particle system from the GameObject containing it
        system = systemHolder.GetComponent<ParticleSystem>();

        //create the specified count of particles
        system.Emit(emitCount);
        
        //create an array to hold the particles
        particleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem>().particleCount];

        //fill particle array with system's particles so they can be manipulated
        system.GetParticles(particleArray);
        
        //the spacing between particles y value
        float offset = 2.0f / emitCount;

        //how much to increment the phi angle of the particles
        float increment = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));
        float x, y, z, r;

        //set the initial velocity and position of each of the particles in radial pattern using fibonnacci spiral
        //the fibonnacci spiral method prevents bunching of particles near the poles of the sphere (lat and long method)
        for (float i = 0; i < system.particleCount; i++)
        {
            //get the y value of current particle
            y = ((i * offset) - 1) + (offset / 2.0f);

            //find rotation along spiral based on y value
            r = Mathf.Sqrt(1 - Mathf.Pow(y, 2.0f));

            //calculate an approximation of phi for current particle
            float phi = ((i + 1) % emitCount) * increment;

            //use rotation along spiral r, and phi to calculate x and z coords
            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            //set particle position and velocity
            //initial postion of all particles is set to the center since they are meant to explode outwards
            particleArray[(int)i].position = new Vector3(0, 0, 0);
            //allow initial velocity to be tweaked with the speedFactor
            particleArray[(int)i].velocity = new Vector3(x / speedFactor, y / speedFactor, z / speedFactor);

        }
        //set the position and velocity of the particles in the system based on array
        system.SetParticles(particleArray, system.particleCount);
	}
	
	void LateUpdate () 
    {
        //the color and dissapation can be set with the particle system itself

	    //get all the particles
        system.GetParticles(particleArray);

        //calculate the particle's movement based on 3d perlin flow field
        PositionParticles();

        //set particles
        system.SetParticles(particleArray, system.particleCount);
        if (system.particleCount == 0)
        {
            Destroy(this.gameObject);
        }
	}

    /*This method handles the turbulence type effect that I want to induce on the initial
     * particle system using 3D perlin noise. Unfortunately, Unity does not have 3D perlin
     * noise built in (only the 2D mathf.PerlineNoise()) so I had to include the following
     * math created by Ken Perlin. It is based on pieces of an extensive tutorial on 
     * implementing noise in Unity3D that can be found here: 
     * http://catlikecoding.com/unity/tutorials/noise/
     * It relies on the NoiseSample struct defined in noise.cs as well 
     */
    private void PositionParticles()
    {
        //base the perlin on the location of the explosion gameObject
        Quaternion q = Quaternion.Euler(this.transform.position);
        Quaternion qInv = Quaternion.Inverse(q);

        //set the amplitude based on whether damping is on or not
        float amplitude = damping ? strength / frequency : strength;

        //increment the morph value based on time elapsed and the morph speed
        morphOffset += Time.deltaTime * morphSpeed;

        //limit morph to 256 since the hashmap is only 256 ints long
        if (morphOffset > 256f)
        {
            morphOffset -= 256f;
        }
        //looping through every particle
        for (int i = 0; i < particleArray.Length; i++)
        {
            //store the current particle's position
            Vector3 position = particleArray[i].position;
            //define a point based on current location
            Vector3 point = q * new Vector3(position.z, position.y, position.x + morphOffset);
            //lookup what the perlin value is at that point in the field
            NoiseSample sampleX = Noise.Sum(point, frequency, octaves, lacunarity, persistence);
            //scale the sample value by the amplitude (to more/less than the 0 to 1 range)
            sampleX *= amplitude;
            sampleX.derivative = qInv * sampleX.derivative;

            //repeat above with sampleY
            point = q * new Vector3(position.x + 100f, position.z, position.y + morphOffset);
            NoiseSample sampleY = Noise.Sum(point, frequency, octaves, lacunarity, persistence);
            sampleY *= amplitude;
            sampleY.derivative = qInv * sampleY.derivative;

            //repeat above with sampleZ
            point = q * new Vector3(position.y, position.x + 100f, position.z + morphOffset);
            NoiseSample sampleZ = Noise.Sum(point, frequency, octaves, lacunarity, persistence);
            sampleZ *= amplitude;
            sampleZ.derivative = qInv * sampleZ.derivative;

            //define the curl vector 
            Vector3 curl;
            //calculate the x, y, z components of the curl vector using the normal vectors at each
            //of the points in the field
            curl.x = sampleZ.derivative.x - sampleY.derivative.y;
            curl.y = sampleX.derivative.x - sampleZ.derivative.y + (1f / (1f + position.y));
            curl.z = sampleY.derivative.x - sampleX.derivative.y;

            //adjust the current velocity of the particle based on the curlFactor percent
            particleArray[i].velocity = particleArray[i].velocity * (1f - curlFactor) + curl * curlFactor;
        }
    }


}
