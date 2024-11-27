using System;
using UnityEngine;

public class TCState : TC
{
	public override Vector3 FixPos
	{
		get
		{
			return TC._setting.statePos;
		}
	}

	public void SetSprite(MultiSprite _sprites)
	{
		if (this.sprites == _sprites)
		{
			return;
		}
		base.CancelInvoke();
		this.sprites = _sprites;
		this.sr.sprite = this.sprites.frames[0];
		if (this.sprites.frames.Length > 1)
		{
			this.index = 0;
			base.InvokeRepeating("UpdateFrame", this.sprites.interval, this.sprites.interval);
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke();
		this.sprites = null;
	}

	public void UpdateFrame()
	{
		this.index++;
		if (this.index >= this.sprites.frames.Length)
		{
			this.index = 0;
		}
		this.sr.sprite = this.sprites.frames[this.index];
	}

	public SpriteRenderer sr;

	public MultiSprite sprites;

	private int index;
}
