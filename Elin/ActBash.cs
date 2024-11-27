using System;

public class ActBash : Act
{
	public override bool CanPerform()
	{
		return Act.TP.Distance(Act.CC.pos) <= 1 && Act.TP.HasObj && Act.TP.sourceObj.tileType.IsBlockPass;
	}

	public override bool Perform()
	{
		Act.CC.Say("bash", Act.CC, Act.TP.sourceObj.GetName(), null);
		Act.CC.PlaySound("kick", 1f, true);
		Act.CC.LookAt(Act.TP);
		Act.CC.renderer.PlayAnime(AnimeID.Attack, Act.TP);
		Act.TP.Animate(AnimeID.HitObj, true);
		return true;
	}
}
