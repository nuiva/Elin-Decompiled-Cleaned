using System;
using System.Collections.Generic;

public class SourcePrefInspector : EMono
{
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

	public bool CanToggleUsePref
	{
		get
		{
			if (target is CardRow { origin: not null } cardRow)
			{
				return !cardRow.pref.UsePref;
			}
			return false;
		}
	}

	public void ToggleUsePref()
	{
		if (CanToggleUsePref)
		{
			(target as CardRow).pref.flags |= PrefFlag.UsePref;
		}
	}

	private void Awake()
	{
		Instance = this;
		ShadowData.Instance = shadowData;
	}

	private void OnValidate()
	{
		if (card != null && card.ExistsOnMap)
		{
			card.isFloating = card.Pref.Float;
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
		foreach (ShadowData.Item item in shadowData.items)
		{
			item.Validate();
		}
	}

	private void OnApplicationQuit()
	{
	}
}
