using System.Linq;
using UnityEngine;

public class LayerPolicy : ELayer
{
	public UIList listPolicyLaw;

	public UIList listPolicyEconomy;

	public UIList listPolicyLife;

	public Material matGrayscale;

	public override void OnInit()
	{
		RefreshPolicy();
	}

	public void RefreshPolicy()
	{
		RefreshPolicyList(listPolicyLaw, "law", "department_law");
		RefreshPolicyList(listPolicyEconomy, "economy", "department_economy");
		RefreshPolicyList(listPolicyLife, "life", "department_life");
		RefreshPolicyIcons();
	}

	public void RefreshPolicyList(UIList list, string cat, string plan)
	{
		list.callbacks = new UIList.Callback<Policy, UIButton>
		{
			onClick = delegate
			{
			},
			onInstantiate = delegate(Policy a, UIButton b)
			{
				b.mainText.SetText(a.Name);
				b.onClick.AddListener(delegate
				{
					if (!a.active && ELayer.Branch.policies.CurrentAP() + a.Cost > ELayer.Branch.MaxAP)
					{
						SE.Beep();
					}
					else
					{
						SE.ClickOk();
						a.active = !a.active;
						ELayer.Branch.policies.RefreshEffects();
						RefreshPolicyIcons();
					}
				});
				b.refObj = a;
				b.tooltip.onShowTooltip = delegate(UITooltip tp)
				{
					a.WriteNote(tp.note);
				};
			},
			onList = delegate
			{
				foreach (Policy item in ELayer.Branch.policies.list.Where((Policy p) => p.source.categorySub == cat))
				{
					list.Add(item);
				}
			},
			onSort = (Policy a, UIList.SortMode m) => a.GetSortVal(m),
			onRefresh = null
		};
		list.List();
		list.GetComponentInChildren<CanvasGroup>().alpha = ((list.items.Count > 0) ? 0.9f : 0.4f);
	}

	public void RefreshPolicyIcons()
	{
		int num = ELayer.Branch.policies.CurrentAP();
		UIList[] array = new UIList[3] { listPolicyLaw, listPolicyLife, listPolicyEconomy };
		for (int i = 0; i < array.Length; i++)
		{
			UIButton[] componentsInChildren = array[i].GetComponentsInChildren<UIButton>();
			foreach (UIButton obj in componentsInChildren)
			{
				Policy policy = obj.refObj as Policy;
				obj.icon.material = (policy.active ? null : matGrayscale);
				obj.imageCheck.SetActive(policy.active);
				obj.subText.SetText(policy.Cost.ToString() ?? "", (num + policy.Cost <= ELayer.Branch.MaxAP || policy.active) ? FontColor.Good : FontColor.Bad);
			}
		}
	}
}
