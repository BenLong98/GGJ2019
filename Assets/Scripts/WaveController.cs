using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WaveController : MonoBehaviour {

	float minSpawnDelay = 1f;
	float maxSpawnDelay = 3f;

	GameObject[] zombieSpawns;

	[SerializeField] GameObject zombiePrefab;
	GameObject zombieHolder;

	bool waveInProgress = false;

	// how many zombies are still alive this round
	int zombiesInWave = 0;
	// how many zombies will be spawned this wave, this changes only on nextwave
	int zombiesToSpawnThisWave = 20;
	// zombies left to spawn this wave, this is decreased each time a zombie is spawned
	int zombiesLeftToSpawn = 20;
	// zombies added per wave, this is how much zombiesToSpawnThisWave will go up each wave
	int zombiesAddedPerWave = 20;


	int timeBetweenWaves = 15;


	void Awake() {
		zombieHolder = GameObject.FindWithTag("zombieHolder");
		zombieSpawns = GameObject.FindGameObjectsWithTag("zombieSpawn");
	}

	void Start() {
		StartGame();
	}

	void StartGame() {
		waveInProgress = true;
		Invoke("runNextSpawnZombies",15);
		print("Starting game, 15 seconds to build!");
	}


	IEnumerator thinkSpawnZombies() {
		GameObject freeSpawner = null;
		while (freeSpawner == null) {
			freeSpawner = zombieSpawns[Random.Range(0,zombieSpawns.Length)];
		}

		spawnZombieAt(freeSpawner.transform.position);
		yield return new WaitForEndOfFrame();
	}

	void spawnZombieAt(Vector3 spawnPos) {
		zombiesLeftToSpawn--;
		zombiesInWave++;
		GameObject serverObject = Instantiate(zombiePrefab,spawnPos,Quaternion.identity);
		if (NetworkServer.active) {
			NetworkServer.Spawn(serverObject);

			//print("spawning " +serverObject + " on the network");
		}
	}

	void runNextSpawnZombies() {
		if (waveInProgress && zombiesLeftToSpawn>0) {
			StartCoroutine(thinkSpawnZombies());
		}
		Invoke("runNextSpawnZombies",Random.Range(minSpawnDelay,maxSpawnDelay));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	bool IsSpawnFree(Vector3 pos) {
		foreach (Collider col in Physics.OverlapSphere(pos,2f)) {
			if (col.transform.GetComponent<ZombieController>()) {
				return false;
			}
		}
		return true;
	}

	void nextWave() {
		print("starting next wave.");
		zombiesToSpawnThisWave += zombiesAddedPerWave;
		zombiesLeftToSpawn = zombiesToSpawnThisWave;
		waveInProgress = true;
	}

	public void zombieDied(ZombieController zombie) {
		print("zombies left: "+zombiesLeftToSpawn);
		zombiesInWave--;
		if (zombiesInWave<1) {
			waveInProgress = false;
			Invoke("nextWave",timeBetweenWaves);
			print("waiting for next wave, "+timeBetweenWaves+" second delay.");
		}
	}
}
