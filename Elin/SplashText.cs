using TMPro;
using UnityEngine;

public class SplashText : MonoBehaviour
{
	public TextMeshProUGUI textBig;

	public TextMeshProUGUI textSmall;

	public static SplashText Instance;

	private void Awake()
	{
		Instance = this;
	}
}
