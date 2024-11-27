using System;
using Newtonsoft.Json;

public class ZoneInstanceRandomQuest : ZoneInstance
{
	public override bool WarnExit
	{
		get
		{
			return this.status != ZoneInstance.Status.Success;
		}
	}

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
			uidClient = this.uidClient,
			uidQuest = this.uidQuest,
			fail = (this.status != ZoneInstance.Status.Success)
		}, true);
	}

	[JsonProperty]
	public int uidClient;

	[JsonProperty]
	public int uidQuest;
}
