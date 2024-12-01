using UnityEngine;

public class TC : EMono
{
	public static GameSetting.RenderSetting.TCSetting _setting;

	public CardRenderer render;

	public Card owner => render.owner;

	public virtual bool isUI => false;

	public virtual Vector3 FixPos => Vector3.zero;

	public void SetOwner(CardRenderer r)
	{
		render = r;
		OnSetOwner();
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void OnDraw(ref Vector3 pos)
	{
		base.transform.position = pos + FixPos;
	}

	public void Kill()
	{
		OnKill();
		PoolManager.Despawn(this);
		render = null;
	}

	public virtual void OnKill()
	{
	}
}
