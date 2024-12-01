using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LayerProgress : ELayer
{
	public static bool completed;

	public static bool isActive;

	public static float progress;

	public float speed;

	public float minTime;

	public float endTime;

	public float minProgress;

	public Slider bar;

	public Text text;

	public Action onComplete;

	public Func<bool> funcComplete;

	public UniTask<bool> unitask;

	public Action onCancel;

	public bool useUnitask;

	public CanvasGroup cg;

	private float value;

	public void Init(string hint)
	{
		text.text = hint;
		completed = false;
		isActive = true;
		progress = (value = 0f);
		cg.alpha = 0f;
		cg.DOFade(1f, 0.1f).SetEase(Ease.InCubic).SetDelay(0.2f);
	}

	public void Update()
	{
		if (onCancel != null && Input.GetKeyDown(KeyCode.Escape))
		{
			isActive = false;
			Close();
			onCancel();
			return;
		}
		minTime -= Time.deltaTime;
		if (progress < 0.9f)
		{
			progress += minProgress * Time.deltaTime;
		}
		value = Mathf.Lerp(value, progress, Time.deltaTime * speed);
		if (useUnitask && unitask.Status == UniTaskStatus.Succeeded)
		{
			completed = true;
		}
		bar.value = Mathf.Clamp(value, 0f, 1f);
		if (!completed || !(minTime < 0f))
		{
			return;
		}
		progress = 1f;
		speed *= 5f;
		endTime -= Time.deltaTime;
		if (!(endTime > 0f))
		{
			isActive = false;
			Close();
			if (onComplete != null)
			{
				onComplete();
			}
		}
	}

	public static LayerProgress StartAsync(string text, UniTask<bool> task, Action onCancel = null)
	{
		LayerProgress layerProgress = ELayer.ui.AddLayer<LayerProgress>();
		layerProgress.Init(text);
		layerProgress.useUnitask = true;
		layerProgress.unitask = task;
		layerProgress.onCancel = onCancel;
		return layerProgress;
	}

	public static LayerProgress Start(string text, Action onComplete = null)
	{
		LayerProgress layerProgress = ELayer.ui.AddLayer<LayerProgress>();
		layerProgress.Init(text);
		layerProgress.onComplete = onComplete;
		return layerProgress;
	}

	public static void Start(string text, Action thread, Action onComplete)
	{
		if (ELayer.core.debug.dontUseThread)
		{
			thread();
			onComplete();
			return;
		}
		LayerProgress layerProgress = ELayer.ui.AddLayer<LayerProgress>();
		layerProgress.Init(text);
		layerProgress.onComplete = onComplete;
		ThreadPool.QueueUserWorkItem(delegate
		{
			thread();
			completed = true;
		});
	}
}
