using System;
using UnityEngine;
using UnityEngine.UI;

public class UIMapSelector : EMono
{
	public void WriteNote(Zone z)
	{
		ZoneProfile.Load(z.source.idProfile);
		this.note.Clear();
		this.note.AddHeader(z.Name, null);
		this.note.AddText(z.source.GetDetail(), FontColor.DontChange).Hyphenate();
		this.note.Space(0, 1);
		this.note.AddTopic("climate".lang(), "climateTemp".lang());
		int cost = z.source.cost;
		this.note.AddTopic("ecosystem".lang(), "ecosystemTemp".lang());
		this.note.Build();
		this.imageZone.sprite = Resources.Load<Sprite>("Media/Graphics/Image/Zone/" + z.source.image.IsEmpty("default"));
	}

	public UINote note;

	public Image imageZone;
}
