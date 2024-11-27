using System;

public class QuestDialog : QuestProgression
{
	public override bool RequireClientInSameZone
	{
		get
		{
			return false;
		}
	}

	public override void ShowCompleteText()
	{
		if (this.id == "pre_debt_runaway")
		{
			SE.WriteJournal();
			Msg.Say("completeQuest", this.GetTitle(), null, null, null);
			return;
		}
		base.ShowCompleteText();
	}

	public override bool CanStartQuest()
	{
		return !(base.source.id == "farris_tulip") || EClass.pc.faction.HasMember("farris", true);
	}

	public override void OnDropReward()
	{
		string id = base.source.id;
		uint num = <PrivateImplementationDetails>.ComputeStringHash(id);
		if (num <= 1725189290U)
		{
			if (num > 362372447U)
			{
				if (num != 724892422U)
				{
					if (num != 1165613602U)
					{
						if (num != 1725189290U)
						{
							return;
						}
						if (!(id == "exile_quru"))
						{
							return;
						}
						EClass.game.quests.Add("exile_kettle", "kettle").startDate = EClass.world.date.GetRaw(0) + 1440;
						if (!EClass.game.quests.IsCompleted("into_darkness"))
						{
							EClass.game.quests.Add("into_darkness", "kettle").startDate = EClass.world.date.GetRaw(0) + 7200;
							return;
						}
					}
					else
					{
						if (!(id == "exile_kettle"))
						{
							return;
						}
						EClass.game.quests.Add("exile_whisper", null).startDate = EClass.world.date.GetRaw(0) + 1440;
						return;
					}
				}
				else
				{
					if (!(id == "quru_past1"))
					{
						return;
					}
					EClass.game.quests.Add("quru_past2", "farris").startDate = EClass.world.date.GetRaw(0) + 1440;
				}
				return;
			}
			if (num != 246178350U)
			{
				if (num != 291779300U)
				{
					if (num != 362372447U)
					{
						return;
					}
					if (!(id == "farris_tulip"))
					{
						return;
					}
					EClass.game.quests.Add("kettle_join", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
					return;
				}
				else
				{
					if (!(id == "quru_sing"))
					{
						return;
					}
					EClass.game.quests.Add("quru_past1", "kettle").startDate = EClass.world.date.GetRaw(0) + 1440;
					return;
				}
			}
			else
			{
				if (!(id == "pre_debt"))
				{
					return;
				}
				Chara chara = EClass.game.cards.globalCharas.Find("loytel");
				if (chara.currentZone == null)
				{
					EClass.pc.homeBranch.AddMemeber(chara);
				}
				chara.MoveHome("olvina", 59, 66);
				chara.noMove = true;
				chara.AddEditorTag(EditorTag.Invulnerable);
				EClass.player.flags.loytelEscaped = true;
				EClass.game.quests.Start("pre_debt_runaway", "farris");
				return;
			}
		}
		else if (num <= 3250077815U)
		{
			if (num != 2107315830U)
			{
				if (num != 2673677365U)
				{
					if (num != 3250077815U)
					{
						return;
					}
					if (!(id == "greatDebt"))
					{
						return;
					}
					EClass.game.quests.Add("farris_tulip", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
					return;
				}
				else
				{
					if (!(id == "fiama_reward"))
					{
						return;
					}
					base.DropReward(ThingGen.CreateRecipe("workbench2"));
					base.DropReward(ThingGen.CreateRecipe("factory_stone"));
					base.DropReward(ThingGen.CreateRecipe("stonecutter"));
					EClass.game.quests.globalList.Add(Quest.Create("fiama_lock", null, null).SetClient(EClass.game.cards.globalCharas.Find("fiama"), false));
					return;
				}
			}
			else
			{
				if (!(id == "exile_meet"))
				{
					return;
				}
				EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("demitas", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
				EClass.game.quests.Add("exile_quru", "quru").startDate = EClass.world.date.GetRaw(0) + 1440;
				return;
			}
		}
		else if (num <= 3922658727U)
		{
			if (num != 3764520845U)
			{
				if (num != 3922658727U)
				{
					return;
				}
				if (!(id == "kettle_join"))
				{
					return;
				}
				EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("kettle", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
				EClass.Branch.AddMemeber(EClass._zone.AddCard(CharaGen.Create("quru", -1), EClass.pc.pos.GetNearestPoint(false, false, true, false)) as Chara);
				EClass.game.quests.Add("quru_morning", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
				return;
			}
			else
			{
				if (!(id == "exile_whisper"))
				{
					return;
				}
				EClass.game.quests.Add("exile_voice", null).startDate = EClass.world.date.GetRaw(0) + 1440;
				return;
			}
		}
		else if (num != 4049106153U)
		{
			if (num != 4120258342U)
			{
				return;
			}
			if (!(id == "pre_debt_runaway"))
			{
				return;
			}
			EClass.game.quests.Add("exile_meet", "quru").startDate = EClass.world.date.GetRaw(0) + 43200;
			return;
		}
		else
		{
			if (!(id == "quru_morning"))
			{
				return;
			}
			EClass.game.quests.Add("quru_sing", "quru").startDate = EClass.world.date.GetRaw(0) + 2880;
			EClass.game.quests.Add("vernis_gold", "loytel").startDate = EClass.world.date.GetRaw(0) + 1440;
			return;
		}
	}
}
