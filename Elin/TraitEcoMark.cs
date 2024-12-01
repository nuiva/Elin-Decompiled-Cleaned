public class TraitEcoMark : Trait
{
	public override bool IsTool => true;

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (t.IsInstalled && t.IsFurniture && t.Evalue(652) <= 0 && !t.IsUnique)
			{
				p.TrySetAct("actEco".lang(t.Name), delegate
				{
					Msg.Say("put_ecomark", t);
					SE.Play("click_paper");
					owner.ModNum(-1);
					t.elements.SetBase(652, 10);
					t.ChangeWeight(t.source.weight * 100 / 110);
					return false;
				});
			}
		});
	}
}
