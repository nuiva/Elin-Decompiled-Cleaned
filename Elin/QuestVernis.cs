using System;

public class QuestVernis : QuestProgression
{
	public override string TitlePrefix
	{
		get
		{
			return "★";
		}
	}

	public override void OnEnterZone()
	{
		Zone_VernisMine zone_VernisMine = EClass._zone as Zone_VernisMine;
		if (zone_VernisMine != null && this.phase == 7 && zone_VernisMine.IsBossLv)
		{
			this.UpdateOnTalk();
		}
	}

	public override void OnChangePhase(int a)
	{
		int phase = this.phase;
		if (phase == 1)
		{
			EClass.game.cards.globalCharas.Find("loytel").MoveHome("vernis", -1, -1);
			return;
		}
		if (phase == 5)
		{
			Chara chara = EClass.game.cards.globalCharas.Find("quru");
			if (chara != null)
			{
				chara.MoveHome("vernis", -1, -1);
				EClass.Branch.AddMemeber(chara);
				EClass.game.cards.globalCharas.Find("kettle").MoveHome("vernis", -1, -1);
				EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("kettle"));
				EClass.game.cards.globalCharas.Find("farris").MoveHome("vernis", -1, -1);
				EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("farris"));
			}
			EClass.Branch.AddMemeber(EClass.game.cards.globalCharas.Find("loytel"));
			return;
		}
		if (phase != 7)
		{
			return;
		}
		base.DropReward(ThingGen.CreatePotion(8506, 1).SetNum(3));
		base.DropReward(ThingGen.Create("blanket_fire", -1, -1));
	}

	public override bool CanUpdateOnTalk(Chara c)
	{
		if (this.phase != 0 && EClass._zone.id != "vernis")
		{
			return false;
		}
		switch (this.phase)
		{
		case 0:
			return true;
		case 1:
			return true;
		case 2:
			return true;
		case 3:
		{
			bool valid = true;
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (c.sourceObj.id == 100)
				{
					valid = false;
				}
			});
			return valid;
		}
		case 4:
			return EClass._zone.IsPCFaction;
		case 5:
			return EClass.game.quests.IsCompleted("quru_past2") && EClass._zone.IsPCFaction && EClass.Branch.lv >= 2;
		case 6:
			return true;
		case 9:
			return EClass._zone.IsPCFaction;
		case 10:
			return true;
		}
		return false;
	}

	public override void OnComplete()
	{
		Chara chara = CharaGen.Create("corgon", -1);
		chara.SetInt(100, 1);
		EClass._zone.AddCard(chara, EClass.pc.pos.GetNearestPoint(false, true, true, false));
		EClass.Branch.AddMemeber(chara);
		EClass.game.quests.Add("mokyu", "corgon").startDate = EClass.world.date.GetRaw(0) + 14400;
		EClass.game.quests.Add("pre_debt", "farris").startDate = EClass.world.date.GetRaw(0) + 28800;
	}

	public override string GetTextProgress()
	{
		if (this.phase == 3 && EClass._zone is Zone_Vernis && EClass._zone.lv == 0)
		{
			int n = 0;
			EClass._map.bounds.ForeachCell(delegate(Cell c)
			{
				if (c.sourceObj.id == 100)
				{
					int n = n;
					n++;
				}
			});
			return "progressVernis".lang(n.ToString() ?? "", null, null, null, null);
		}
		return base.GetTextProgress();
	}
}
