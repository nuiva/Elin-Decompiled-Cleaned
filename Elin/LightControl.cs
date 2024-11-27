using System;
using UnityEngine;

[ExecuteInEditMode]
public class LightControl : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnEnable()
	{
		this.partcleMain = this.Haze.GetComponent<ParticleSystem>().main;
		this.lightPart = this.particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
	}

	private void Update()
	{
		this.Haze.SetActive(this.enableHaze);
		this.lightPart.enabled = this.enableLight;
		this.particleLight.range = this.lightRange;
		this.particleLight.intensity = this.lightIntensity;
		Color color = this.partcleMain.startColor.color;
		color.a = this.hazeLevel / 255f;
		this.partcleMain.startColor = color;
	}

	public bool enableLight = true;

	public bool enableHaze = true;

	public float lightRange = 300f;

	public float lightIntensity = 1f;

	[Range(0f, 255f)]
	public float hazeLevel = 120f;

	[HideInInspector]
	public Light particleLight;

	[HideInInspector]
	public GameObject Haze;

	private ParticleSystem.MainModule partcleMain;

	private ParticleSystem.LightsModule lightPart;
}
