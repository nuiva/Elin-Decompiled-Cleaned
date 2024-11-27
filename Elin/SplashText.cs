using System;
using TMPro;
using UnityEngine;

public class SplashText : MonoBehaviour
{
	private void Awake()
	{
		SplashText.Instance = this;
	}

	public TextMeshProUGUI textBig;

	public TextMeshProUGUI textSmall;

	public static SplashText Instance;
}
