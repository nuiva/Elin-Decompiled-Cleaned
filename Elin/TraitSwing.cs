using DG.Tweening;

public class TraitSwing : Trait
{
	public bool swinging;

	private Tween tween;

	public override TileMode tileMode
	{
		get
		{
			if (!UseAltTiles)
			{
				return TileMode.Default;
			}
			return TileMode.DefaultNoAnime;
		}
	}

	public override bool UseAltTiles => !swinging;

	public override void OnStepped(Chara c)
	{
		owner.isOn = true;
		swinging = false;
	}

	public override void OnSteppedOut(Chara c)
	{
		bool isOn = owner.isOn;
		owner.isOn = owner.pos.HasChara;
		if (isOn && !owner.isOn)
		{
			if (tween != null)
			{
				tween.Kill();
			}
			swinging = true;
			owner.PlaySound("swing");
			tween = TweenUtil.Tween(5f).OnComplete(delegate
			{
				swinging = false;
			}).OnKill(delegate
			{
				swinging = false;
			});
		}
	}
}
