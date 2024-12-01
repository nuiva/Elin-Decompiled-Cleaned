using UnityEngine;

[ExecuteInEditMode]
public class LightControl : MonoBehaviour
{
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

	private void Start()
	{
	}

	private void OnEnable()
	{
		partcleMain = Haze.GetComponent<ParticleSystem>().main;
		lightPart = particleLight.transform.parent.gameObject.GetComponent<ParticleSystem>().lights;
	}

	private void Update()
	{
		Haze.SetActive(enableHaze);
		lightPart.enabled = enableLight;
		particleLight.range = lightRange;
		particleLight.intensity = lightIntensity;
		Color color = partcleMain.startColor.color;
		color.a = hazeLevel / 255f;
		partcleMain.startColor = color;
	}
}
