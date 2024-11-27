using System;

public class LayerCover : ELayer
{
	public void SetDuration(float duration, Action onKill)
	{
		TweenUtil.Tween(duration, null, delegate()
		{
			ELayer.ui.RemoveLayer(this);
			onKill();
		});
	}

	public void SetCondition(Func<float, bool> func)
	{
		this.funcEnd = func;
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (this.funcEnd != null && this.funcEnd(Core.delta))
		{
			ELayer.ui.RemoveLayer(this);
		}
	}

	public Func<float, bool> funcEnd;
}
