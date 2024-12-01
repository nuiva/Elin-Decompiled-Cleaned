using Newtonsoft.Json;

public class ZoneInstanceRandomQuest : ZoneInstance
{
	[JsonProperty]
	public int uidClient;

	[JsonProperty]
	public int uidQuest;

	public override bool WarnExit => status != Status.Success;

	public override void OnLeaveZone()
	{
		foreach (Chara chara in EClass._map.charas)
		{
			if (chara.IsMinion && chara.homeZone == EClass._zone)
			{
				chara.c_uidMaster = 0;
				chara.master = null;
			}
		}
		base.ReturnZone.events.AddPreEnter(new ZonePreEnterOnCompleteQuestInstance
		{
			uidClient = uidClient,
			uidQuest = uidQuest,
			fail = (status != Status.Success)
		});
	}
}
