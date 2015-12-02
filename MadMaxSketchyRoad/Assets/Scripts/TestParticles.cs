using UnityEngine;
using System.Collections;

public class TestParticles : MonoBehaviour {

	ParticleSystem.Particle []ParticleArray;
	ParticleSystem system;
	public GameObject systemHolder;
	int i;
	void Start () {
		//this.gameObject.AddComponent (ParticleSystem);

		//GameObject test = new GameObject ("test");
		//test.AddComponent (ParticleSystem);

		//There are no particles at this point in time
		//ParticleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem> ().particleCount];
		system = systemHolder.GetComponent<ParticleSystem> ();
	}
	

	void Update () {
		ParticleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem> ().particleCount];
		system.GetParticles (ParticleArray);
		for(i = 0; i < system.particleCount; ++i){
			//ParticleArray[i].velocity.Set(0, Mathf.PerlinNoise(Random.Range(0,10), Random.Range(0,10)), 0);
			ParticleArray[i].position = new Vector3(0, Mathf.Sin(Time.time /5 ) * 10 - i, 0);
		}
		system.SetParticles (ParticleArray, system.particleCount);
	}
}
