public class AM_Blueprint : AM_Copy
{
	public TraitBlueprint bp;

	public override bool ShowBuildWidgets => false;

	public override BuildMenu.Mode buildMenuMode => BuildMenu.Mode.None;

	public override Mode mode
	{
		get
		{
			if (bp == null || !bp.path.IsEmpty())
			{
				return Mode.Place;
			}
			return Mode.Create;
		}
	}

	public void SetBlueprint(TraitBlueprint _bp)
	{
		bp = _bp;
		if (!bp.path.IsEmpty())
		{
			Import(bp.path);
		}
	}

	public override void OnSave(PartialMap _partial)
	{
		Thing thing = bp.owner.Split(1);
		(thing.trait as TraitBlueprint).path = _partial.path;
		thing.c_idRefName = _partial.name;
		EClass.pc.Pick(thing);
		Deactivate();
	}
}
