using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HS_ParticleEndSound : MonoBehaviour
{
	private void Awake()
	{
		HS_ParticleEndSound.SharedInstance = this;
	}

	private void Start()
	{
		this.poolDictionary = new Dictionary<string, Queue<GameObject>>();
		foreach (HS_ParticleEndSound.Pool pool in this.pools)
		{
			Queue<GameObject> queue = new Queue<GameObject>();
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(pool.prefab);
			AudioSource component = gameObject.GetComponent<AudioSource>();
			if (pool.tag == "AudioExplosion")
			{
				component.clip = this.audioExplosion[UnityEngine.Random.Range(0, this.audioExplosion.Length)];
				component.volume = UnityEngine.Random.Range(this.explosionMinVolume, this.explosionMaxVolume);
				component.pitch = UnityEngine.Random.Range(this.explosionPitchMin, this.explosionPitchMax);
			}
			else if (pool.tag == "AudioShot")
			{
				component.clip = this.audioShot[UnityEngine.Random.Range(0, this.audioExplosion.Length)];
				component.volume = UnityEngine.Random.Range(this.shootMinVolume, this.shootMaxVolume);
				component.pitch = UnityEngine.Random.Range(this.shootPitchMin, this.shootPitchMax);
			}
			gameObject.transform.parent = base.gameObject.transform;
			gameObject.SetActive(false);
			queue.Enqueue(gameObject);
			this.poolDictionary.Add(pool.tag, queue);
		}
	}

	public GameObject SpawnFromPool(string tag, Vector3 position)
	{
		if (!this.poolDictionary.ContainsKey(tag))
		{
			Debug.LogWarning("Pool with tag" + tag + " does not excist.");
			return null;
		}
		GameObject gameObject = this.poolDictionary[tag].Dequeue();
		gameObject.SetActive(true);
		position.z = 0f;
		gameObject.transform.position = position;
		this.poolDictionary[tag].Enqueue(gameObject);
		return gameObject;
	}

	public void LateUpdate()
	{
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[base.GetComponent<ParticleSystem>().particleCount];
		int particles = base.GetComponent<ParticleSystem>().GetParticles(array);
		for (int i = 0; i < particles; i++)
		{
			if (this.audioExplosion.Length != 0 && array[i].remainingLifetime < Time.deltaTime)
			{
				GameObject gameObject = HS_ParticleEndSound.SharedInstance.SpawnFromPool("AudioExplosion", array[i].position);
				if (gameObject != null)
				{
					base.StartCoroutine(this.LateCall(gameObject));
				}
			}
			if (this.audioShot.Length != 0 && array[i].remainingLifetime >= array[i].startLifetime - Time.deltaTime)
			{
				GameObject gameObject2 = HS_ParticleEndSound.SharedInstance.SpawnFromPool("AudioShot", array[i].position);
				if (gameObject2 != null)
				{
					base.StartCoroutine(this.LateCall(gameObject2));
				}
			}
		}
	}

	private IEnumerator LateCall(GameObject soundInstance)
	{
		yield return new WaitForSeconds(this.poolReturnTimer);
		soundInstance.SetActive(false);
		yield break;
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

	public List<HS_ParticleEndSound.Pool> pools;

	public Dictionary<string, Queue<GameObject>> poolDictionary;

	[Serializable]
	public class Pool
	{
		public string tag;

		public GameObject prefab;

		public int size;
	}
}
