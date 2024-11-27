using System;

public class TraitGuildDoorman : TraitUniqueGuildPersonnel
{
	public override AI_Idle.Behaviour IdleBehaviour
	{
		get
		{
			return AI_Idle.Behaviour.NoMove;
		}
	}

	public virtual bool IsGuildMember
	{
		get
		{
			return false;
		}
	}

	public override bool CanBePushed
	{
		get
		{
			return this.IsGuildMember;
		}
	}

	public virtual void GiveTrial()
	{
	}

	public virtual void OnJoinGuild()
	{
	}
}
