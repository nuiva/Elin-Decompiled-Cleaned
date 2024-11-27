using System;

public class TraitTrolley : TraitFloorSwitch
{
	public virtual bool HideChara
	{
		get
		{
			return false;
		}
	}

	public override bool CanManucalActivate
	{
		get
		{
			return EClass.pc.pos.Equals(this.owner.pos) && this.CanActivate(EClass.pc);
		}
	}

	public bool CanActivate(Chara c)
	{
		if (c.host != null)
		{
			return false;
		}
		if (!this.owner.pos.HasRail || !this.owner.IsInstalled)
		{
			return false;
		}
		if (c.IsPCFaction && !c.IsPC && EClass._zone is Zone_Dungeon)
		{
			return false;
		}
		foreach (Chara chara in EClass._map.charas)
		{
			AI_Trolley ai_Trolley = chara.ai as AI_Trolley;
			if (ai_Trolley != null && ai_Trolley.IsRunning && ai_Trolley.trolley == this)
			{
				return false;
			}
		}
		return true;
	}

	public override void OnActivateTrap(Chara c)
	{
		TraitSwitch.haltMove = false;
		if (!this.CanActivate(c))
		{
			return;
		}
		c.SetAI(new AI_Trolley
		{
			trolley = this
		});
	}

	public virtual string GetIdSound()
	{
		if (this.owner.idSkin != 7)
		{
			return "ride_trolley";
		}
		return "ride_bike";
	}

	public virtual float FadeDuration
	{
		get
		{
			if (this.owner.idSkin != 7)
			{
				return 1f;
			}
			return 0.5f;
		}
	}
}
