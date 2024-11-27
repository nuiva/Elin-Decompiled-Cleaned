using System;
using UnityEngine;
using UnityEngine.UI;

public class UISong : EMono
{
	private void Awake()
	{
		UISong.Instance = this;
	}

	public unsafe void SetSong(SoundSource _source, BGMData _bgm, BGMData.Part _part)
	{
		this.source = _source;
		this.part = _part;
		this.bgm = _bgm;
		Vector3 vector = Camera.main.WorldToScreenPoint(*EMono.pc.pos.Position());
		vector += this.posFix * EMono.screen.Zoom;
		vector.z = 0f;
		this.Rect().position = vector;
	}

	private void Update()
	{
		if (this.source.source == null)
		{
			this.Kill();
			return;
		}
		AudioSource audioSource = this.source.source;
		this.ratio = Mathf.Clamp((audioSource.time - this.part.start) / (this.part.duration + this.bgm.song.fadeIn), 0f, 1f);
		if (EMono.game == null || !EMono.core.IsGameStarted || this.source == null || !this.source.isPlaying || this.ratio >= 0.99f || (this.ratio <= 0f && !(EMono.pc.ai is AI_PlayMusic)))
		{
			this.timer += Core.delta;
			if (this.timer > 0.3f)
			{
				this.Kill();
			}
			return;
		}
		this.timer = 0f;
		RectTransform rectTransform = this.bar.rectTransform;
		rectTransform.sizeDelta = new Vector2(this.gauge.originalWidth * this.ratio, rectTransform.sizeDelta.y);
	}

	public void Kill()
	{
		UnityEngine.Object.DestroyImmediate(base.gameObject);
	}

	public static UISong Instance;

	public Gauge gauge;

	public Image bar;

	public SoundSource source;

	public BGMData.Part part;

	public BGMData bgm;

	public float ratio;

	public Vector3 posFix;

	private float timer;
}
