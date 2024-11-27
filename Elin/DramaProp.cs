using System;
using UnityEngine;

public class DramaProp : EMono
{
	public void Enter()
	{
		this.aniEnter.Play(base.transform, null, -1f, 0f);
		this.OnEnter();
	}

	public virtual void OnEnter()
	{
	}

	public void Leave()
	{
		this.aniLeave.Play(base.transform, delegate
		{
			this.Kill();
		}, -1f, 0f);
		this.OnLeave();
	}

	public virtual void OnLeave()
	{
	}

	public void Kill()
	{
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public SpriteRenderer sr;

	public AnimeTween aniEnter;

	public AnimeTween aniLeave;
}
