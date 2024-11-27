using System;
using Newtonsoft.Json;

public class ZoneEventBeggerChest : ZoneEvent
{
	public Thing chest
	{
		get
		{
			return EClass._map.things.Find((Thing a) => a.uid == this.uidChest);
		}
	}

	public override int hoursToKill
	{
		get
		{
			return 96;
		}
	}

	public override void OnTickRound()
	{
		if (EClass.rnd(10) == 0 && this.chest != null)
		{
			if (this.chest.c_lockLv == 0)
			{
				base.Kill();
				return;
			}
			this.chest.PlayAnime(AnimeID.Shiver, false);
		}
	}

	[JsonProperty]
	public int uidChest;

	[JsonProperty]
	public int shiver;
}
