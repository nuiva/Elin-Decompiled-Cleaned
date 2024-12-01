using System;
using System.Collections.Generic;

public class EffectManager : EMono
{
	[Serializable]
	public class EffectList : DynamicAsset<Effect>
	{
		public Effect Get(string id)
		{
			return GetNew(id);
		}
	}

	[NonSerialized]
	public List<Effect> list = new List<Effect>();

	public static EffectManager Instance;

	public EffectList effects;

	public void Add(Effect e)
	{
		list.Add(e);
	}

	public void Remove(Effect e)
	{
		list.Remove(e);
	}

	public void KillAll()
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num].Kill();
		}
	}

	public void UpdateEffects()
	{
		for (int num = list.Count - 1; num >= 0; num--)
		{
			list[num].OnUpdate();
		}
	}
}
