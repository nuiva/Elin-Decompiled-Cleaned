public class ActBash : Act
{
	public override bool CanPerform()
	{
		if (Act.TP.Distance(Act.CC.pos) <= 1)
		{
			if (Act.TP.HasObj)
			{
				return Act.TP.sourceObj.tileType.IsBlockPass;
			}
			return false;
		}
		return false;
	}

	public override bool Perform()
	{
		Act.CC.Say("bash", Act.CC, Act.TP.sourceObj.GetName());
		Act.CC.PlaySound("kick");
		Act.CC.LookAt(Act.TP);
		Act.CC.renderer.PlayAnime(AnimeID.Attack, Act.TP);
		Act.TP.Animate(AnimeID.HitObj, animeBlock: true);
		return true;
	}
}
