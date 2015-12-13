using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

    ParticleSystem.Particle[] particleArray;
    ParticleSystem system;
    public GameObject systemHolder;

    //How many particles will initially be emitted
    public int emitCount = 500;

    //The inverse factor for initial speed (ex. 0.5 will double init speed)
    public float speedFactor = 0.1f;

    [Range(0f,1f)]
    public float curlFactor = .4f;

    public Vector3 offset;

    public Vector3 rotation;

    public float morphSpeed;

    [Range(0f, 1f)]
    public float strength = 1f;

    public bool damping;

    public float frequency = 1f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(1f, 4f)]
    public float lacunarity = 2f;

    [Range(0f, 1f)]
    public float persistence = 0.5f;

    [Range(1, 3)]
    public int dimensions = 3;

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
        

        float offset = 2.0f / emitCount;
        float increment = Mathf.PI * (3.0f - Mathf.Sqrt(5.0f));
        float x, y, z, r;

        //set the initial velocity and position of each of the particles in radial pattern using fibonnacci spiral
        //the fibonnacci spiral method prevents bunching of particles near the poles of the sphere (lat and long method)
        for (float i = 0; i < system.particleCount; i++)
        {
            y = ((i * offset) - 1) + (offset / 2.0f);
            r = Mathf.Sqrt(1 - Mathf.Pow(y, 2.0f));

            //calculate an approximation of phi
            float phi = ((i + 1) % emitCount) * increment;

            x = Mathf.Cos(phi) * r;
            z = Mathf.Sin(phi) * r;

            //set particle position and velocity
            particleArray[(int)i].position = new Vector3(0, 0, 0);

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
	}

    private void PositionParticles()
    {
        Quaternion q = Quaternion.Euler(rotation);
        Quaternion qInv = Quaternion.Inverse(q);
        NoiseMethod method = Noise.methods[0][0];
        float amplitude = damping ? strength / frequency : strength;
        morphOffset += Time.deltaTime * morphSpeed;
        if (morphOffset > 256f)
        {
            morphOffset -= 256f;
        }
        for (int i = 0; i < particleArray.Length; i++)
        {
            Vector3 position = particleArray[i].position;
            Vector3 point = q * new Vector3(position.z, position.y, position.x + morphOffset) + offset;
            NoiseSample sampleX = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
            sampleX *= amplitude;
            sampleX.derivative = qInv * sampleX.derivative;
            point = q * new Vector3(position.x + 100f, position.z, position.y + morphOffset) + offset;
            NoiseSample sampleY = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
            sampleY *= amplitude;
            sampleY.derivative = qInv * sampleY.derivative;
            point = q * new Vector3(position.y, position.x + 100f, position.z + morphOffset) + offset;
            NoiseSample sampleZ = Noise.Sum(method, point, frequency, octaves, lacunarity, persistence);
            sampleZ *= amplitude;
            sampleZ.derivative = qInv * sampleZ.derivative;
            Vector3 curl;
            curl.x = sampleZ.derivative.x - sampleY.derivative.y;
            curl.y = sampleX.derivative.x - sampleZ.derivative.y + (1f / (1f + position.y));
            curl.z = sampleY.derivative.x - sampleX.derivative.y;
            particleArray[i].velocity = particleArray[i].velocity * (1f - curlFactor) + curl * curlFactor;
        }
    }
}
