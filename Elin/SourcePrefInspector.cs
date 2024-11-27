using System;
using System.Collections.Generic;

public class SourcePrefInspector : EMono
{
	public void ToggleUsePref()
	{
		if (!this.CanToggleUsePref)
		{
			return;
		}
		(this.target as CardRow).pref.flags |= PrefFlag.UsePref;
	}

	public bool CanToggleUsePref
	{
		get
		{
			CardRow cardRow = this.target as CardRow;
			return cardRow != null && cardRow.origin != null && !cardRow.pref.UsePref;
		}
	}

	private void Awake()
	{
		SourcePrefInspector.Instance = this;
		ShadowData.Instance = this.shadowData;
	}

	private void OnValidate()
	{
		if (this.card != null && this.card.ExistsOnMap)
		{
			this.card.isFloating = this.card.Pref.Float;
		}
	}

	public void ValidatePrefs()
	{
		EMono.debug.validatePref = false;
		EMono.sources.foods.ValidatePref();
		EMono.sources.thingV.ValidatePref();
		EMono.sources.things.ValidatePref();
		EMono.sources.charas.ValidatePref();
		EMono.sources.objs.ValidatePref();
		foreach (ShadowData.Item item in this.shadowData.items)
		{
			item.Validate();
		}
	}

	private void OnApplicationQuit()
	{
	}

	public static SourcePrefInspector Instance;

	public string id;

	[NonSerialized]
	public SourcePref pref;

	private Point lastPoint = new Point();

	private int index;

	private int lastShadowIndex = -1;

	private RenderRow target;

	private Card card;

	public ShadowData.Item shadow;

	public ShadowData shadowData;

	public static bool dirty;

	private List<RenderRow> list = new List<RenderRow>();
}
