using UnityEngine;

public class ScreenEffect : EMono
{
	public bool worldParticle;

	public float killDuration;

	public float stopParticleTime;

	private GameObject goParent;

	private float time;

	private bool particleStopped;

	public static void Play(string id)
	{
		Util.Instantiate<ScreenEffect>("Media/Effect/ScreenEffect/" + id, Camera.main.transform);
	}

	private void Awake()
	{
		if ((bool)base.transform.parent)
		{
			goParent = base.transform.parent.gameObject;
		}
		base.transform.SetParent(Camera.main.transform, worldPositionStays: false);
		if (worldParticle)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				ParticleSystem.MainModule main = componentsInChildren[i].main;
				main.simulationSpace = ParticleSystemSimulationSpace.World;
			}
		}
	}

	private void Update()
	{
		if ((bool)goParent)
		{
			return;
		}
		time += Time.unscaledDeltaTime;
		if (!particleStopped && stopParticleTime != 0f && time > stopParticleTime)
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				if (particleSystem.transform.tag.Contains("ParticleEmitter"))
				{
					ParticleSystem.MainModule main = particleSystem.main;
					main.loop = false;
					ParticleSystem.EmissionModule emission = particleSystem.emission;
					emission.enabled = false;
				}
			}
			SoundEmitter[] componentsInChildren2 = GetComponentsInChildren<SoundEmitter>();
			foreach (SoundEmitter soundEmitter in componentsInChildren2)
			{
				if ((bool)soundEmitter.source)
				{
					soundEmitter.source.Stop(2f);
					soundEmitter.source = null;
				}
			}
			particleStopped = true;
		}
		if (time > killDuration)
		{
			Object.Destroy(base.gameObject);
		}
	}
}
