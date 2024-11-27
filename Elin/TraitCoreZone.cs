using System;
using UnityEngine;

public class TraitCoreZone : Trait
{
	public override bool CanBeDestroyed
	{
		get
		{
			return false;
		}
	}

	public override bool CanOnlyCarry
	{
		get
		{
			return true;
		}
	}

	public override bool CanPutAway
	{
		get
		{
			return false;
		}
	}

	public override bool IsLightOn
	{
		get
		{
			return true;
		}
	}

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
			p.TrySetAct("actNewZone", delegate()
			{
				EClass.pc.MoveZone(EClass._zone.ParentZone, ZoneTransition.EnterState.Auto);
				return false;
			}, this.owner, CursorSystem.MoveZone, 1, false, true, false);
		}
		if (!EClass._zone.IsPCFaction || !this.owner.IsInstalled)
		{
			return;
		}
		p.TrySetAct("actCallReserve", () => LayerPeople.CreateReserve(), this.owner, null, 1, false, true, false);
		p.TrySetAct("actNameZone", delegate()
		{
			Dialog.InputName("dialogChangeName", EClass._zone.Name, delegate(bool cancel, string text)
			{
				if (!cancel)
				{
					EClass._zone.name = text;
					EClass._zone.idPrefix = 0;
					WidgetDate.Refresh();
				}
			}, Dialog.InputType.Default);
			return false;
		}, this.owner, null, 1, false, true, false);
		if (EClass.player.spawnZone != EClass._zone)
		{
			p.TrySetAct("actSetSpawn", delegate()
			{
				Effect.Get("aura_heaven").Play(EClass.pc.pos, 0f, null, null);
				EClass.Sound.Play("worship");
				EClass.player.spawnZone = EClass._zone;
				Msg.Say("setSpawn", this.owner, null, null, null);
				return true;
			}, this.owner, null, 1, false, true, false);
		}
		if (EClass.pc.homeZone != EClass._zone)
		{
			p.TrySetAct("actSetHome", delegate()
			{
				Dialog.YesNo("dialogSetHome", delegate
				{
					Effect.Get("aura_heaven").Play(EClass.pc.pos, 0f, null, null);
					EClass.Sound.Play("worship");
					EClass.pc.homeZone = EClass._zone;
					Msg.Say("setHome");
				}, null, "yes", "no");
				return false;
			}, this.owner, null, 1, false, true, false);
		}
		if (Application.isEditor || (EClass.Branch.resources.worth.bestRank > 0 && EClass.Branch.resources.worth.bestRank <= 100 && !EClass._zone.name.IsEmpty()))
		{
			p.TrySetAct("actUploadMap", delegate()
			{
				EClass.ui.AddLayer<LayerUploader>();
				return false;
			}, this.owner, null, 1, false, true, false);
		}
		if (EClass._zone != EClass.game.StartZone && !(EClass._zone is Zone_Vernis))
		{
			p.TrySetAct("actAbandonHome", delegate()
			{
				Dialog.YesNo("dialogAbandonHome", delegate
				{
					if (!EClass.world.date.IsExpired(EClass._zone.GetInt(2, null) + 43200))
					{
						Msg.Say("claimCooldown");
						return;
					}
					this.owner.Die(null, null, AttackSource.None);
					EClass.player.DropReward(ThingGen.Create("deed", -1, -1), false);
					EClass._zone.AbandonZone();
				}, null, "yes", "no");
				return false;
			}, this.owner, null, 1, false, true, false);
		}
	}
}
