using System;
using UnityEngine;
using UnityEngine.UI;

public class InteractionMenu : EMono
{
	private void Awake()
	{
		this.mold = this.layout.CreateMold(null);
	}

	public void Show()
	{
		this.layout.RebuildLayout(true);
		this.soundPop.Play();
	}

	public UIButton Add()
	{
		return Util.Instantiate<UIButton>(this.mold, this.layout);
	}

	public void Clear()
	{
		this.layout.DestroyChildren(false, true);
	}

	public LayoutGroup layout;

	public UIButton mold;

	public Vector3 offset;

	public Vector3 modPos;

	public SoundData soundPop;
}
