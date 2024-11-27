using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ContentPolicy : EContent
{
	public override void OnSwitchContent(int idTab)
	{
		EClass.Branch.policies.Validate();
		this.RefreshPolicy();
	}

	public void RefreshPolicy()
	{
		this.RefreshPolicyList(this.listPolicyLaw, "law", "department_law");
		this.RefreshPolicyList(this.listPolicyEconomy, "economy", "department_economy");
		this.RefreshPolicyList(this.listPolicyLife, "life", "department_life");
		this.RefreshPolicyList(this.listUtility, "utility", "department_utility");
		this.listUtility.SetActive(EClass.Branch.lv >= 2);
		this.RefreshPolicyIcons();
		base.GetComponentInParent<Layer>().RebuildLayout(true);
		this.RebuildLayout(true);
	}

	public void RefreshPolicyList(UIList list, string cat, string plan)
	{
		BaseList list2 = list;
		UIList.Callback<Policy, UIButton> callback = new UIList.Callback<Policy, UIButton>();
		callback.onClick = delegate(Policy a, UIButton b)
		{
		};
		callback.onInstantiate = delegate(Policy a, UIButton b)
		{
			b.mainText.SetText(a.Name);
			b.onClick.AddListener(delegate()
			{
				if (!a.active)
				{
					if ((a.Cost > 0 && EClass.Branch.policies.CurrentAP() + a.Cost > EClass.Branch.MaxAP) || !ContentPolicy.<RefreshPolicyList>g__CanActivate|7_0(a))
					{
						SE.Beep();
						return;
					}
					if (a.id == 2514 && EClass.pc.faction.CountTaxFreeLand() >= 3)
					{
						SE.Beep();
						return;
					}
				}
				SE.ClickOk();
				a.active = !a.active;
				if (a.id == 2705)
				{
					EClass.pc.faction.SetGlobalPolicyActive(2705, a.active);
				}
				if (a.id == 2708)
				{
					EClass.pc.faction.SetGlobalPolicyActive(2708, a.active);
				}
				if (a.id == 2710)
				{
					EClass.pc.faction.SetGlobalPolicyActive(2710, a.active);
				}
				if (a.id == 2711)
				{
					EClass.pc.faction.SetGlobalPolicyActive(2711, a.active);
				}
				EClass.Branch.policies.RefreshEffects();
				this.RefreshPolicyIcons();
			});
			b.refObj = a;
			b.tooltip.onShowTooltip = delegate(UITooltip tp)
			{
				a.WriteNote(tp.note);
			};
		};
		Func<Policy, bool> <>9__7;
		callback.onList = delegate(UIList.SortMode m)
		{
			IEnumerable<Policy> list3 = EClass.Branch.policies.list;
			Func<Policy, bool> predicate;
			if ((predicate = <>9__7) == null)
			{
				predicate = (<>9__7 = ((Policy p) => p.source.categorySub == cat));
			}
			foreach (Policy o in list3.Where(predicate))
			{
				list.Add(o);
			}
		};
		callback.onSort = ((Policy a, UIList.SortMode m) => a.GetSortVal(m));
		callback.onRefresh = null;
		list2.callbacks = callback;
		list.List(false);
		list.GetComponentInChildren<CanvasGroup>().alpha = ((list.items.Count > 0) ? 0.9f : 0.4f);
	}

	public void RefreshPolicyIcons()
	{
		int num = EClass.Branch.policies.CurrentAP();
		UIList[] array = new UIList[]
		{
			this.listPolicyLaw,
			this.listPolicyLife,
			this.listPolicyEconomy,
			this.listUtility
		};
		for (int i = 0; i < array.Length; i++)
		{
			foreach (UIButton uibutton in array[i].GetComponentsInChildren<UIButton>())
			{
				Policy policy = uibutton.refObj as Policy;
				uibutton.icon.material = (policy.active ? null : this.matGrayscale);
				uibutton.imageCheck.SetActive(policy.active);
				uibutton.subText.SetText(policy.Cost.ToString() ?? "", (num + policy.Cost <= EClass.Branch.MaxAP || policy.active) ? FontColor.Good : FontColor.Bad);
			}
		}
	}

	[CompilerGenerated]
	internal static bool <RefreshPolicyList>g__CanActivate|7_0(Policy p)
	{
		return (p.id != 2515 || !EClass.Branch.policies.IsActive(2516, -1)) && (p.id != 2516 || !EClass.Branch.policies.IsActive(2515, -1)) && (p.id != 2814 || !EClass.Branch.policies.IsActive(2823, -1)) && (p.id != 2823 || !EClass.Branch.policies.IsActive(2814, -1));
	}

	public UIList listPolicyLaw;

	public UIList listPolicyEconomy;

	public UIList listPolicyLife;

	public UIList listUtility;

	public Material matGrayscale;
}
