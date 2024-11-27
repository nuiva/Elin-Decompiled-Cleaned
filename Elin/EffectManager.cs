using System;
using System.Collections.Generic;

public class EffectManager : EMono
{
	public void Add(Effect e)
	{
		this.list.Add(e);
	}

	public void Remove(Effect e)
	{
		this.list.Remove(e);
	}

	public void KillAll()
	{
		for (int i = this.list.Count - 1; i >= 0; i--)
		{
			this.list[i].Kill();
		}
	}

	public void UpdateEffects()
	{
		for (int i = this.list.Count - 1; i >= 0; i--)
		{
			this.list[i].OnUpdate();
		}
	}

	[NonSerialized]
	public List<Effect> list = new List<Effect>();

	public static EffectManager Instance;

	public EffectManager.EffectList effects;

	[Serializable]
	public class EffectList : DynamicAsset<Effect>
	{
		public Effect Get(string id)
		{
			return base.GetNew(id, null);
		}
	}
}
