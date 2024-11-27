using System;

public class ConBlind : BadCondition
{
	public override Emo2 EmoIcon
	{
		get
		{
			return Emo2.blind;
		}
	}

	public override int GetPhase()
	{
		return 0;
	}

	public override void SetOwner(Chara _owner, bool onDeserialize = false)
	{
		base.SetOwner(_owner, false);
		this.owner.isBlind = true;
	}

	public override void OnStart()
	{
		if (this.owner.IsPC)
		{
			this.owner.RecalculateFOV();
			ScreenGrading.blind = true;
			EClass.scene.camSupport.grading.SetGrading();
		}
	}

	public override void OnRemoved()
	{
		this.owner.isBlind = false;
		if (this.owner.IsPC)
		{
			this.owner.RecalculateFOV();
			ScreenGrading.blind = false;
			EClass.scene.camSupport.grading.SetGrading();
		}
	}
}
