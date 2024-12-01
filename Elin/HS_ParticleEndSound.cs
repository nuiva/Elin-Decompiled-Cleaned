using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_ParticleEndSound : MonoBehaviour
{
	[Serializable]
	public class Pool
	{
		public string tag;

		public GameObject prefab;

		public int size;
	}

	public float poolReturnTimer = 1.5f;

	public float explosionMinVolume = 0.3f;

	public float explosionMaxVolume = 0.7f;

	public float explosionPitchMin = 0.75f;

	public float explosionPitchMax = 1.25f;

	public float shootMinVolume = 0.05f;

	public float shootMaxVolume = 0.1f;

	public float shootPitchMin = 0.75f;

	public float shootPitchMax = 1.25f;

	public AudioClip[] audioExplosion;

	public AudioClip[] audioShot;

	public static HS_ParticleEndSound SharedInstance;

	public List<Pool> pools;

	public Dictionary<string, Queue<GameObject>> poolDictionary;

	private void Awake()
	{
		SharedInstance = this;
	}

	private void Start()
	{
		poolDictionary = new Dictionary<string, Queue<GameObject>>();
		foreach (Pool pool in pools)
		{
			Queue<GameObject> queue = new Queue<GameObject>();
			GameObject gameObject = UnityEngine.Object.Instantiate(pool.prefab);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (pool.tag == "AudioExplosion")
			{
				component.clip = audioExplosion[UnityEngine.Random.Range(0, audioExplosion.Length)];
				component.volume = UnityEngine.Random.Range(explosionMinVolume, explosionMaxVolume);
				component.pitch = UnityEngine.Random.Range(explosionPitchMin, explosionPitchMax);
			}
			else if (pool.tag == "AudioShot")
			{
				component.clip = audioShot[UnityEngine.Random.Range(0, audioExplosion.Length)];
				component.volume = UnityEngine.Random.Range(shootMinVolume, shootMaxVolume);
				component.pitch = UnityEngine.Random.Range(shootPitchMin, shootPitchMax);
			}
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.SetActive(value: false);
			queue.Enqueue(gameObject);
			poolDictionary.Add(pool.tag, queue);
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position)
	{
		if (!poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + " does not excist.");
			return null;
		}
		GameObject gameObject = poolDictionary[tag].Dequeue();
		gameObject.SetActive(value: true);
		position.z = 0f;
		gameObject.transform.position = position;
		poolDictionary[tag].Enqueue(gameObject);
		return gameObject;
	}

	public void LateUpdate()
	{
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[GetComponent<ParticleSystem>().particleCount];
		int particles = GetComponent<ParticleSystem>().GetParticles(array);
		for (int i = 0; i < particles; i++)
		{
			if (audioExplosion.Length != 0 && array[i].remainingLifetime < Time.deltaTime)
			{
				GameObject gameObject = SharedInstance.SpawnFromPool("AudioExplosion", array[i].position);
				if (gameObject != null)
				{
					StartCoroutine(LateCall(gameObject));
				}
			}
			if (audioShot.Length != 0 && array[i].remainingLifetime >= array[i].startLifetime - Time.deltaTime)
			{
				GameObject gameObject2 = SharedInstance.SpawnFromPool("AudioShot", array[i].position);
				if (gameObject2 != null)
				{
					StartCoroutine(LateCall(gameObject2));
				}
			}
		}
	}

	private IEnumerator LateCall(GameObject soundInstance)
	{
		yield return new WaitForSeconds(poolReturnTimer);
		soundInstance.SetActive(value: false);
	}
}
