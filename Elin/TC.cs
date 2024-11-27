using System;
using UnityEngine;

public class TC : EMono
{
	public Card owner
	{
		get
		{
			return this.render.owner;
		}
	}

	public virtual bool isUI
	{
		get
		{
			return false;
		}
	}

	public virtual Vector3 FixPos
	{
		get
		{
			return Vector3.zero;
		}
	}

	public void SetOwner(CardRenderer r)
	{
		this.render = r;
		this.OnSetOwner();
	}

	public virtual void OnSetOwner()
	{
	}

	public virtual void OnDraw(ref Vector3 pos)
	{
		base.transform.position = pos + this.FixPos;
	}

	public void Kill()
	{
		this.OnKill();
		PoolManager.Despawn(this);
		this.render = null;
	}

	public virtual void OnKill()
	{
	}

	public static GameSetting.RenderSetting.TCSetting _setting;

	public CardRenderer render;
}
