using System;
using UnityEngine;

public class LayerSleep : ELayer
{
	public override void OnAfterInit()
	{
		ELayer.ui.ShowCover(this.coverShow, this.coverAlpha, null, default(Color));
	}

	public void Wait(float duration = 5f)
	{
		ELayer.Sound.Play("jingle_sleep");
		TweenUtil.Tween(duration, null, delegate()
		{
			ELayer.ui.HideCover(this.coverHide, delegate
			{
				this.Close();
			});
		});
	}

	public void Sleep(int _hours, Thing _bed, Thing _pillow = null)
	{
		ELayer.Sound.Play("jingle_sleep");
		this.hours = _hours;
		this.bed = _bed;
		this.pillow = _pillow;
		this.maxMin = this.hours * 60;
		base.InvokeRepeating("Advance", this.repeatRate, this.repeatRate);
		ELayer.pc.faith.Revelation("sleep", 100);
		ELayer.player.stats.slept++;
	}

	public void Advance()
	{
		if (this.min > this.maxMin)
		{
			base.CancelInvoke();
			if (!ELayer._zone.IsInstance && (ELayer._zone.IsPCFaction || ELayer._zone is Zone_Field || ELayer._zone.IsRegion))
			{
				ELayer.player.SimulateFaction();
			}
			foreach (Chara chara in ELayer.pc.party.members)
			{
				chara.OnSleep(this.bed, 1);
			}
			Msg.Say("slept", this.hours.ToString() ?? "", null, null, null);
			if (this.pillow != null && this.pillow.trait is TraitPillowOpatos)
			{
				ELayer.world.region.RenewRandomSites();
			}
			if (!ELayer.debug.ignoreAutoSave)
			{
				ELayer.game.Save(false, null, false);
			}
			if (ELayer._zone.IsRegion)
			{
				ELayer._zone.Region.UpdateRandomSites();
			}
			ELayer.ui.ShowCover(0f, 1f, null, default(Color));
			TweenUtil.Delay(this.hideDelay, delegate
			{
				ELayer.ui.HideCover(this.coverHide, delegate
				{
					this.Close();
				});
			});
			return;
		}
		this.min += 10;
		ELayer.world.date.AdvanceMin(10);
	}

	public float repeatRate;

	public float coverAlpha;

	public float coverShow;

	public float coverHide;

	public float hideDelay;

	private int hours;

	private int min;

	private int maxMin;

	private Thing bed;

	private Thing pillow;
}
