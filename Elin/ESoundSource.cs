using System;
using UnityEngine;

public class ESoundSource : EMono
{
	private void OnEnable()
	{
		this.Play();
		base.InvokeRepeating("Refresh", 0f, 0.1f);
	}

	private void OnDisable()
	{
		base.CancelInvoke();
	}

	public void Play()
	{
		this.source.clip = this.data.clip;
		this.source.loop = (this.data.loop != 0);
		this.source.volume = this.data.volume;
		this.source.pitch = this.data.pitch * (1f + ((this.data.randomPitch == 0f) ? 0f : Rand.Range(-this.data.randomPitch, this.data.randomPitch)));
		this.source.time = this.data.startAt;
		this.source.name = this.data.name;
		this.source.spatialBlend = 0f;
		this.source.Play();
	}

	public void Refresh()
	{
		Vector3 vector = base.transform.position - EMono.scene.transAudio.position;
		vector.z = 0f;
		float magnitude = vector.magnitude;
		float num = Mathf.Clamp01(1f - magnitude / this.maxDistance);
		this.source.volume = this.data.volume * num * this.data.spatial + this.data.volume * (1f - this.data.spatial);
	}

	public TC owner;

	public AudioSource source;

	public SoundData data;

	public float maxDistance;
}
