using System;
using UnityEngine;

public class ScreenEffect : EMono
{
	public static void Play(string id)
	{
		Util.Instantiate<ScreenEffect>("Media/Effect/ScreenEffect/" + id, Camera.main.transform);
	}

	private void Awake()
	{
		if (base.transform.parent)
		{
			this.goParent = base.transform.parent.gameObject;
		}
		base.transform.SetParent(Camera.main.transform, false);
		if (this.worldParticle)
		{
			ParticleSystem[] componentsInChildren = base.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				componentsInChildren[i].main.simulationSpace = ParticleSystemSimulationSpace.World;
			}
		}
	}

	private void Update()
	{
		if (!this.goParent)
		{
			this.time += Time.unscaledDeltaTime;
			if (!this.particleStopped && this.stopParticleTime != 0f && this.time > this.stopParticleTime)
			{
				foreach (ParticleSystem particleSystem in base.GetComponentsInChildren<ParticleSystem>())
				{
					if (particleSystem.transform.tag.Contains("ParticleEmitter"))
					{
						particleSystem.main.loop = false;
						particleSystem.emission.enabled = false;
					}
				}
				foreach (SoundEmitter soundEmitter in base.GetComponentsInChildren<SoundEmitter>())
				{
					if (soundEmitter.source)
					{
						soundEmitter.source.Stop(2f);
						soundEmitter.source = null;
					}
				}
				this.particleStopped = true;
			}
			if (this.time > this.killDuration)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}

	public bool worldParticle;

	public float killDuration;

	public float stopParticleTime;

	private GameObject goParent;

	private float time;

	private bool particleStopped;
}
