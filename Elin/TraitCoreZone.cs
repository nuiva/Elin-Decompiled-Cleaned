using UnityEngine;

public class TraitCoreZone : Trait
{
	public override bool CanBeDestroyed => false;

	public override bool CanOnlyCarry => true;

	public override bool CanPutAway => false;

	public override bool IsLightOn => true;

	public override void SetName(ref string s)
	{
		if (EClass.Branch != null)
		{
			s = s + " Lv " + EClass.Branch.TextLv;
		}
	}

	public override void TrySetAct(ActPlan p)
	{
		if (EClass._zone.IsUserZone)
		{
			p.TrySetAct("actNewZone", delegate
			{
				EClass.pc.MoveZone(EClass._zone.ParentZone);
				return false;
			}, owner, CursorSystem.MoveZone);
		}
		if (!EClass._zone.IsPCFaction || !owner.IsInstalled)
		{
			return;
		}
		p.TrySetAct("actCallReserve", () => LayerPeople.CreateReserve(), owner);
		p.TrySetAct("actNameZone", delegate
		{
			Dialog.InputName("dialogChangeName", EClass._zone.Name, delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					EClass._zone.name = text;
					EClass._zone.idPrefix = 0;
					WidgetDate.Refresh();
				}
			});
			return false;
		}, owner);
		if (EClass.player.spawnZone != EClass._zone)
		{
			p.TrySetAct("actSetSpawn", delegate
			{
				Effect.Get("aura_heaven").Play(EClass.pc.pos);
				EClass.Sound.Play("worship");
				EClass.player.spawnZone = EClass._zone;
				Msg.Say("setSpawn", owner);
				return true;
			}, owner);
		}
		if (EClass.pc.homeZone != EClass._zone)
		{
			p.TrySetAct("actSetHome", delegate
			{
				Dialog.YesNo("dialogSetHome", delegate
				{
					Effect.Get("aura_heaven").Play(EClass.pc.pos);
					EClass.Sound.Play("worship");
					EClass.pc.homeZone = EClass._zone;
					Msg.Say("setHome");
				});
				return false;
			}, owner);
		}
		if (Application.isEditor || (EClass.Branch.resources.worth.bestRank > 0 && EClass.Branch.resources.worth.bestRank <= 100 && !EClass._zone.name.IsEmpty()))
		{
			p.TrySetAct("actUploadMap", delegate
			{
				EClass.ui.AddLayer<LayerUploader>();
				return false;
			}, owner);
		}
		if (EClass._zone == EClass.game.StartZone || EClass._zone is Zone_Vernis)
		{
			return;
		}
		p.TrySetAct("actAbandonHome", delegate
		{
			Dialog.YesNo("dialogAbandonHome", delegate
			{
				if (!EClass.world.date.IsExpired(EClass._zone.GetInt(2) + 43200))
				{
					Msg.Say("claimCooldown");
				}
				else
				{
					owner.Die();
					EClass.player.DropReward(ThingGen.Create("deed"));
					EClass._zone.AbandonZone();
				}
			});
			return false;
		}, owner);
	}
}
