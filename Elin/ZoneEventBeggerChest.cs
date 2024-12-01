using Newtonsoft.Json;

public class ZoneEventBeggerChest : ZoneEvent
{
	[JsonProperty]
	public int uidChest;

	[JsonProperty]
	public int shiver;

	public Thing chest => EClass._map.things.Find((Thing a) => a.uid == uidChest);

	public override int hoursToKill => 96;

	public override void OnTickRound()
	{
		if (EClass.rnd(10) == 0 && chest != null)
		{
			if (chest.c_lockLv == 0)
			{
				Kill();
			}
			else
			{
				chest.PlayAnime(AnimeID.Shiver);
			}
		}
	}
}
