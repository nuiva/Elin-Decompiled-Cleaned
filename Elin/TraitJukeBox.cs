using System;

public class TraitJukeBox : TraitEditPlaylist
{
	public override string IDActorEx
	{
		get
		{
			if (!this.owner.isOn || this.owner.refVal <= 1)
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
		if (this.Electricity < 0 && !this.owner.isOn)
		{
			this.owner.PlaySound("electricity_insufficient", 1f, true);
			return;
		}
		this.owner.PlaySound("tape", 1f, true);
		this.owner.refVal = d.id;
		this.OnToggle();
	}

	public override void OnToggle()
	{
		EClass.scene.RemoveActorEx(this.owner);
		if (this.owner.isOn && this.owner.refVal > 1)
		{
			EClass.scene.AddActorEx(this.owner, null);
		}
	}
}
