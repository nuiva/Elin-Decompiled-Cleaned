using System;
using UnityEngine;

public class ZonePreEnterOnCompleteQuestInstance : ZonePreEnterEvent
{
	public override void Execute()
	{
		EClass.player.returnInfo = null;
		if (this.uidQuest == 0)
		{
			return;
		}
		Quest quest = EClass.game.quests.Get(this.uidQuest);
		Chara chara = EClass._map.FindChara(this.uidClient);
		if (chara == null)
		{
			string[] array = new string[7];
			array[0] = this.uidClient.ToString();
			array[1] = "/";
			array[2] = this.uidQuest.ToString();
			array[3] = "/";
			array[4] = this.fail.ToString();
			array[5] = "/";
			int num = 6;
			Quest quest2 = quest;
			array[num] = ((quest2 != null) ? quest2.ToString() : null);
			Debug.Log(string.Concat(array));
			string str = "exception: quest not found:";
			Chara chara2 = chara;
			string str2 = (chara2 != null) ? chara2.ToString() : null;
			string str3 = "/";
			Quest quest3 = (chara != null) ? chara.quest : null;
			Debug.LogError(str + str2 + str3 + ((quest3 != null) ? quest3.ToString() : null));
			return;
		}
		if (chara.quest == null)
		{
			if (quest != null)
			{
				chara.quest = quest;
			}
			string[] array2 = new string[7];
			array2[0] = this.uidClient.ToString();
			array2[1] = "/";
			array2[2] = this.uidQuest.ToString();
			array2[3] = "/";
			array2[4] = this.fail.ToString();
			array2[5] = "/";
			int num2 = 6;
			Quest quest4 = quest;
			array2[num2] = ((quest4 != null) ? quest4.ToString() : null);
			Debug.Log(string.Concat(array2));
			string str4 = "exception: assigned quest to:";
			Chara chara3 = chara;
			Debug.LogWarning(str4 + ((chara3 != null) ? chara3.ToString() : null));
		}
		if (quest == null && chara.quest.uid == this.uidQuest)
		{
			quest = chara.quest;
		}
		if (quest == null)
		{
			return;
		}
		if (this.fail)
		{
			quest.Fail();
		}
		else
		{
			quest.Complete();
		}
		if (EClass.pc.IsAliveInCurrentZone)
		{
			chara.ShowDialog("_chara", this.fail ? "quest_fail" : "quest_success", "");
		}
	}

	public int uidClient;

	public int uidQuest;

	public bool fail;
}
