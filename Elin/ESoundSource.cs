using UnityEngine;

public class ESoundSource : EMono
{
	public TC owner;

	public AudioSource source;

	public SoundData data;

	public float maxDistance;

	private void OnEnable()
	{
		Play();
		InvokeRepeating("Refresh", 0f, 0.1f);
	}

	private void OnDisable()
	{
		CancelInvoke();
	}

	public void Play()
	{
		source.clip = data.clip;
		source.loop = data.loop != 0;
		source.volume = data.volume;
		source.pitch = data.pitch * (1f + ((data.randomPitch == 0f) ? 0f : Rand.Range(0f - data.randomPitch, data.randomPitch)));
		source.time = data.startAt;
		source.name = data.name;
		source.spatialBlend = 0f;
		source.Play();
	}

	public void Refresh()
	{
		Vector3 vector = base.transform.position - EMono.scene.transAudio.position;
		vector.z = 0f;
		float magnitude = vector.magnitude;
		float num = Mathf.Clamp01(1f - magnitude / maxDistance);
		source.volume = data.volume * num * data.spatial + data.volume * (1f - data.spatial);
	}
}
