public class TraitJukeBox : TraitEditPlaylist
{
	public override string IDActorEx
	{
		get
		{
			if (!owner.isOn || owner.refVal <= 1)
			{
				return null;
			}
			return "jukebox";
		}
	}

	public override bool OnUse(Chara c)
	{
		EClass.ui.AddLayer<LayerEditPlaylist>().Activate(this);
		return false;
	}

	public void OnSetBGM(BGMData d)
	{
		if (Electricity < 0 && !owner.isOn)
		{
			owner.PlaySound("electricity_insufficient");
			return;
		}
		owner.PlaySound("tape");
		owner.refVal = d.id;
		OnToggle();
	}

	public override void OnToggle()
	{
		EClass.scene.RemoveActorEx(owner);
		if (owner.isOn && owner.refVal > 1)
		{
			EClass.scene.AddActorEx(owner);
		}
	}
}
