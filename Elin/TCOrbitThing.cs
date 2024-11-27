using System;
using UnityEngine;

public class TCOrbitThing : TCOrbit
{
	public override void OnSetOwner()
	{
		this.owner = (base.owner as Thing);
		RenderData data = this.owner.renderer.data;
		this.goIcon.transform.SetLocalPositionY(data.offset.y + data.size.y + 0.32f);
		this.RefreshAll();
	}

	public override void Refresh()
	{
		this.timer += Time.deltaTime;
		if (this.timer > 0.2f)
		{
			this.timer = 0f;
			this.RefreshAll();
		}
		if (!this.owner.isHidden || EMono.pc.CanSee(this.owner))
		{
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(true);
			}
			base.transform.position = this.owner.renderer.position;
			return;
		}
		if (base.gameObject.activeSelf)
		{
			base.gameObject.SetActive(false);
		}
	}

	public void RefreshAll()
	{
		CoreRef.OrbitIcons orbitIcons = EMono.core.refs.orbitIcons;
		Emo2 emo = Emo2.none;
		if (this.owner.IsInstalled)
		{
			if (this.owner.isRestocking && !this.owner.IsPCParty && this.owner.trait.GetRestockedIcon() != Emo2.none && !(EMono._zone is Zone_Dungeon))
			{
				emo = this.owner.trait.GetRestockedIcon();
			}
			else
			{
				Thing thing = EMono.player.currentHotItem.Thing;
				if (((thing != null) ? thing.trait : null) is TraitToolShears && this.owner.CanBeSheared())
				{
					emo = Emo2.fur;
				}
			}
		}
		bool flag = emo > Emo2.none;
		if (flag)
		{
			this.iconStatus.sprite = EMono.core.refs.spritesEmo[(int)emo];
		}
		this.iconStatus.SetActive(flag);
	}

	public GameObject goIcon;

	public SpriteRenderer iconStatus;

	public new Thing owner;

	private float timer;
}
