using System;

public class AM_Blueprint : AM_Copy
{
	public override bool ShowBuildWidgets
	{
		get
		{
			return false;
		}
	}

	public override BuildMenu.Mode buildMenuMode
	{
		get
		{
			return BuildMenu.Mode.None;
		}
	}

	public override AM_Copy.Mode mode
	{
		get
		{
			if (this.bp == null || !this.bp.path.IsEmpty())
			{
				return AM_Copy.Mode.Place;
			}
			return AM_Copy.Mode.Create;
		}
	}

	public void SetBlueprint(TraitBlueprint _bp)
	{
		this.bp = _bp;
		if (!this.bp.path.IsEmpty())
		{
			base.Import(this.bp.path);
		}
	}

	public override void OnSave(PartialMap _partial)
	{
		Thing thing = this.bp.owner.Split(1);
		(thing.trait as TraitBlueprint).path = _partial.path;
		thing.c_idRefName = _partial.name;
		EClass.pc.Pick(thing, true, true);
		base.Deactivate();
	}

	public TraitBlueprint bp;
}
