using System;

public class LayerCover : ELayer
{
	public Func<float, bool> funcEnd;

	public void SetDuration(float duration, Action onKill)
	{
		TweenUtil.Tween(duration, null, delegate
		{
			ELayer.ui.RemoveLayer(this);
			onKill();
		});
	}

	public void SetCondition(Func<float, bool> func)
	{
		funcEnd = func;
	}

	public override void OnUpdateInput()
	{
		base.OnUpdateInput();
		if (funcEnd != null && funcEnd(Core.delta))
		{
			ELayer.ui.RemoveLayer(this);
		}
	}
}
