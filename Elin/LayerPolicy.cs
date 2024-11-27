using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LayerPolicy : ELayer
{
	public override void OnInit()
	{
		this.RefreshPolicy();
	}

	public void RefreshPolicy()
	{
		this.RefreshPolicyList(this.listPolicyLaw, "law", "department_law");
		this.RefreshPolicyList(this.listPolicyEconomy, "economy", "department_economy");
		this.RefreshPolicyList(this.listPolicyLife, "life", "department_life");
		this.RefreshPolicyIcons();
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
				if (!a.active && ELayer.Branch.policies.CurrentAP() + a.Cost > ELayer.Branch.MaxAP)
				{
					SE.Beep();
					return;
				}
				SE.ClickOk();
				a.active = !a.active;
				ELayer.Branch.policies.RefreshEffects();
				this.RefreshPolicyIcons();
			});
			b.refObj = a;
			b.tooltip.onShowTooltip = delegate(UITooltip tp)
			{
				a.WriteNote(tp.note);
			};
		};
		Func<Policy, bool> <>9__6;
		callback.onList = delegate(UIList.SortMode m)
		{
			IEnumerable<Policy> list3 = ELayer.Branch.policies.list;
			Func<Policy, bool> predicate;
			if ((predicate = <>9__6) == null)
			{
				predicate = (<>9__6 = ((Policy p) => p.source.categorySub == cat));
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
		int num = ELayer.Branch.policies.CurrentAP();
		UIList[] array = new UIList[]
		{
			this.listPolicyLaw,
			this.listPolicyLife,
			this.listPolicyEconomy
		};
		for (int i = 0; i < array.Length; i++)
		{
			foreach (UIButton uibutton in array[i].GetComponentsInChildren<UIButton>())
			{
				Policy policy = uibutton.refObj as Policy;
				uibutton.icon.material = (policy.active ? null : this.matGrayscale);
				uibutton.imageCheck.SetActive(policy.active);
				uibutton.subText.SetText(policy.Cost.ToString() ?? "", (num + policy.Cost <= ELayer.Branch.MaxAP || policy.active) ? FontColor.Good : FontColor.Bad);
			}
		}
	}

	public UIList listPolicyLaw;

	public UIList listPolicyEconomy;

	public UIList listPolicyLife;

	public Material matGrayscale;
}
