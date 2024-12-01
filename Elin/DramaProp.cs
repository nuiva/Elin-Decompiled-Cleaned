using UnityEngine;

public class DramaProp : EMono
{
	public SpriteRenderer sr;

	public AnimeTween aniEnter;

	public AnimeTween aniLeave;

	public void Enter()
	{
		aniEnter.Play(base.transform);
		OnEnter();
	}

	public virtual void OnEnter()
	{
	}

	public void Leave()
	{
		aniLeave.Play(base.transform, delegate
		{
			Kill();
		});
		OnLeave();
	}

	public virtual void OnLeave()
	{
	}

	public void Kill()
	{
		Object.Destroy(base.gameObject);
	}
}
