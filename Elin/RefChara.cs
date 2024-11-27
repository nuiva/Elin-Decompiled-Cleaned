using System;

public class RefChara : EClass
{
	public Chara GetAndCache(int uid)
	{
		if (this.chara != null && (this.chara.IsGlobal || this.chara.IsAliveInCurrentZone))
		{
			return this.chara;
		}
		if (uid == 0)
		{
			return null;
		}
		this.chara = Core.Instance.game.cards.globalCharas.TryGetValue(uid, null);
		if (this.chara == null)
		{
			foreach (Chara chara in EClass._map.charas)
			{
				if (chara.uid == uid)
				{
					this.chara = chara;
					break;
				}
			}
		}
		return this.chara;
	}

	public void Set(ref int val, Chara c)
	{
		this.chara = c;
		val = ((c != null) ? c.uid : 0);
	}

	public static Chara Get(int uid)
	{
		return Core.Instance.game.cards.globalCharas.TryGetValue(uid, null);
	}

	public Chara chara;
}
