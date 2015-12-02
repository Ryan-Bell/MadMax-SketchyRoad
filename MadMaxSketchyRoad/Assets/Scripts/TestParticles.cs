using UnityEngine;
using System.Collections;

public class TestParticles : MonoBehaviour {

	ParticleSystem.Particle []ParticleArray;
	ParticleSystem system;
	public GameObject systemHolder;
	public int delay = 10;
	int i, j;
	float animationTime = 4;
	float elapsedTime = 0;
	void Start () {
		//this.gameObject.AddComponent (ParticleSystem);

		//GameObject test = new GameObject ("test");
		//test.AddComponent (ParticleSystem);

		//There are no particles at this point in time
		//ParticleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem> ().particleCount];
		system = systemHolder.GetComponent<ParticleSystem> ();
		j = 0;
	}
	
	//This update method steps through assigning particles to points according to random and Perlin noise
	/*
	void Update () {
		if (j < delay) {
			j++;
			return;
		} else {
			j = 0;
		}
		ParticleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem> ().particleCount];
		system.GetParticles (ParticleArray);
		for(i = 0; i < system.particleCount; ++i){
			ParticleArray[i].position = new Vector3(Random.Range(-10,10), Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10, Random.Range(-10,10));
			ParticleArray[i].velocity = Vector3.zero;
		}
		system.SetParticles (ParticleArray, system.particleCount);
	}
	*/
	void Update(){
		ParticleArray = new ParticleSystem.Particle[systemHolder.GetComponent<ParticleSystem> ().particleCount];
		system.GetParticles (ParticleArray);
		for(i = 0; i < system.particleCount; i++){

			//particles spawned that lerp based on perlin. They essentially stay in a box
			/*ParticleArray[i].velocity = new Vector3(Mathf.Lerp(ParticleArray[i].velocity.x, Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10 - 5, elapsedTime / animationTime), 
			                                        Mathf.Lerp(ParticleArray[i].velocity.y, Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10 - 5, elapsedTime / animationTime),
			                                        Mathf.Lerp(ParticleArray[i].velocity.z, Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10 - 5, elapsedTime / animationTime));*/


			//vertically ascending particles shifted by perlin
			ParticleArray[i].velocity = new Vector3(Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10 - 5, 
			                                        4,
			                                        Mathf.PerlinNoise(ParticleArray[i].position.x, ParticleArray[i].position.y) * 10 - 5);
		}
		system.SetParticles (ParticleArray, system.particleCount);
		elapsedTime += Time.deltaTime;
		if (animationTime - elapsedTime < .15) {
			elapsedTime = 0;
		}
	}

}
