using System;
using BeautifyEffect;
using Colorful;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class CameraSupport : MonoBehaviour
{
	public void ResizeCameraToPixelPerfect()
	{
		this.cam.orthographicSize = this.GetOrthoSize() / this.Zoom;
	}

	public void OnChangeResolution()
	{
	}

	public float GetOrthoSize()
	{
		float result = 0f;
		switch (this.divier)
		{
		case CameraSupport.Divider.None:
			result = (float)Screen.height * 0.5f * 0.01f;
			break;
		case CameraSupport.Divider.Floor:
			result = (float)Mathf.FloorToInt((float)Screen.height * 0.5f) * 0.01f;
			break;
		case CameraSupport.Divider.Round:
			result = (float)Mathf.RoundToInt((float)Screen.height * 0.5f) * 0.01f;
			break;
		case CameraSupport.Divider.Ceil:
			result = (float)Mathf.CeilToInt((float)Screen.height * 0.5f) * 0.01f;
			break;
		case CameraSupport.Divider.Odd:
			result = (float)Screen.height * 0.5f * 0.01f + 0.005f;
			break;
		}
		return result;
	}

	public Camera cam;

	public Vector3 renderPos;

	public GameScreen screen;

	public ScreenGrading grading;

	public Upscaler upscaler;

	public CameraSupport.Divider divier;

	public bool snap;

	public float PixelsPerUnit = 100f;

	public TiltShift tiltShift;

	public BloomOptimized bloom;

	public Beautify beautify;

	public Kuwahara kuwahara;

	public GaussianBlur blur;

	[Range(0.1f, 2f)]
	public float Zoom = 1f;

	public enum Divider
	{
		None,
		Floor,
		Round,
		Ceil,
		Odd
	}
}
