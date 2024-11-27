using System;
using UnityEngine;

[ExecuteInEditMode]
public class LightControl2 : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnEnable()
	{
		this.partcleTrail = base.gameObject.GetComponent<ParticleSystem>().trails;
		this.lightPart = this.particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
	}

	private void Update()
	{
		this.partcleTrail.enabled = this.enableHaze;
		this.lightPart.enabled = this.enableLight;
		this.particleLight.range = this.lightRange;
		this.particleLight.intensity = this.lightIntensity;
		Color color = this.partcleTrail.colorOverLifetime.color;
		color.a = this.hazeLevel / 255f;
		this.partcleTrail.colorOverLifetime = color;
	}

	public bool enableLight = true;

	public bool enableHaze = true;

	public float lightRange = 300f;

	public float lightIntensity = 1f;

	[Range(0f, 255f)]
	public float hazeLevel = 200f;

	[HideInInspector]
	public Light particleLight;

	private ParticleSystem.TrailModule partcleTrail;

	private ParticleSystem.LightsModule lightPart;
}
