using System;
using DG.Tweening;

public class TraitSwing : Trait
{
	public override Trait.TileMode tileMode
	{
		get
		{
			if (!this.UseAltTiles)
			{
				return Trait.TileMode.Default;
			}
			return Trait.TileMode.DefaultNoAnime;
		}
	}

	public override bool UseAltTiles
	{
		get
		{
			return !this.swinging;
		}
	}

	public override void OnStepped(Chara c)
	{
		this.owner.isOn = true;
		this.swinging = false;
	}

	public override void OnSteppedOut(Chara c)
	{
		bool isOn = this.owner.isOn;
		this.owner.isOn = this.owner.pos.HasChara;
		if (isOn && !this.owner.isOn)
		{
			if (this.tween != null)
			{
				this.tween.Kill(false);
			}
			this.swinging = true;
			this.owner.PlaySound("swing", 1f, true);
			this.tween = TweenUtil.Tween(5f, null, null).OnComplete(delegate
			{
				this.swinging = false;
			}).OnKill(delegate
			{
				this.swinging = false;
			});
		}
	}

	public bool swinging;

	private Tween tween;
}
