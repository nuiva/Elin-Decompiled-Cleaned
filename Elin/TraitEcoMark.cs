using System;

public class TraitEcoMark : Trait
{
	public override bool IsTool
	{
		get
		{
			return true;
		}
	}

	public override void TrySetHeldAct(ActPlan p)
	{
		p.pos.Things.ForEach(delegate(Thing t)
		{
			if (!t.IsInstalled || !t.IsFurniture || t.Evalue(652) > 0 || t.IsUnique)
			{
				return;
			}
			p.TrySetAct("actEco".lang(t.Name, null, null, null, null), delegate()
			{
				Msg.Say("put_ecomark", t, null, null, null);
				SE.Play("click_paper");
				this.owner.ModNum(-1, true);
				t.elements.SetBase(652, 10, 0);
				t.ChangeWeight(t.source.weight * 100 / 110);
				return false;
			}, null, 1);
		});
	}
}
