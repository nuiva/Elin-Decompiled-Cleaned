using System;

public class ActRestrain : Act
{
	public override TargetType TargetType
	{
		get
		{
			return TargetType.Chara;
		}
	}

	public override int MaxRadius
	{
		get
		{
			return 2;
		}
	}

	public override int PerformDistance
	{
		get
		{
			return 2;
		}
	}

	public override bool IsHostileAct
	{
		get
		{
			return true;
		}
	}

	public override bool CanPerform()
	{
		Act.TC = Act.TP.FirstChara;
		return Act.TC != null && !Act.TC.isRestrained && Act.TC.IsAliveInCurrentZone && !Act.TC.IsMultisize && Act.TC.Chara.host == null && Act.TC.IsPCFaction && EClass._zone.IsPCFaction;
	}

	public override bool Perform()
	{
		Act.TC = Act.TP.FirstChara;
		if (Act.TC.Chara.IsHostile() || Act.TC.IsMultisize || Act.TC.Chara.host != null)
		{
			Msg.Say("resist", Act.TC, null, null, null);
			return true;
		}
		SE.Change();
		this.shackle.Restrain(Act.TC, true);
		EClass._zone.AddCard(this.shackle.owner, Act.TP);
		this.shackle.owner.SetPlaceState(PlaceState.installed, false);
		if (!Act.TC.IsPCFaction)
		{
			EClass.player.ModKarma(-1);
		}
		if (Act.TC.IsPCFaction && EClass._zone.IsPCFaction)
		{
			Act.CC.SetAI(new AI_Torture
			{
				shackle = this.shackle
			});
		}
		return true;
	}

	public TraitShackle shackle;
}
