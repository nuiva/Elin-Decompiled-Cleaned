using UnityEngine;

[ExecuteInEditMode]
public class LightControl2 : MonoBehaviour
{
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

	private void Start()
	{
	}

	private void OnEnable()
	{
		partcleTrail = base.gameObject.GetComponent<ParticleSystem>().trails;
		lightPart = particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
	}

	private void Update()
	{
		partcleTrail.enabled = enableHaze;
		lightPart.enabled = enableLight;
		particleLight.range = lightRange;
		particleLight.intensity = lightIntensity;
		Color color = partcleTrail.colorOverLifetime.color;
		color.a = hazeLevel / 255f;
		partcleTrail.colorOverLifetime = color;
	}
}
