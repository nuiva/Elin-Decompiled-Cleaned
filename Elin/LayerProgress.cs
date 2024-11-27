using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.UI;

public class LayerProgress : ELayer
{
	public void Init(string hint)
	{
		this.text.text = hint;
		LayerProgress.completed = false;
		LayerProgress.isActive = true;
		LayerProgress.progress = (this.value = 0f);
		this.cg.alpha = 0f;
		this.cg.DOFade(1f, 0.1f).SetEase(Ease.InCubic).SetDelay(0.2f);
	}

	public void Update()
	{
		if (this.onCancel != null && Input.GetKeyDown(KeyCode.Escape))
		{
			LayerProgress.isActive = false;
			this.Close();
			this.onCancel();
			return;
		}
		this.minTime -= Time.deltaTime;
		if (LayerProgress.progress < 0.9f)
		{
			LayerProgress.progress += this.minProgress * Time.deltaTime;
		}
		this.value = Mathf.Lerp(this.value, LayerProgress.progress, Time.deltaTime * this.speed);
		if (this.useUnitask && this.unitask.Status == UniTaskStatus.Succeeded)
		{
			LayerProgress.completed = true;
		}
		this.bar.value = Mathf.Clamp(this.value, 0f, 1f);
		if (LayerProgress.completed && this.minTime < 0f)
		{
			LayerProgress.progress = 1f;
			this.speed *= 5f;
			this.endTime -= Time.deltaTime;
			if (this.endTime > 0f)
			{
				return;
			}
			LayerProgress.isActive = false;
			this.Close();
			if (this.onComplete != null)
			{
				this.onComplete();
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
		ThreadPool.QueueUserWorkItem(delegate(object a)
		{
			thread();
			LayerProgress.completed = true;
		});
	}

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
}
