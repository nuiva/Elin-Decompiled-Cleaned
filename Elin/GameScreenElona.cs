using System;
using CreativeSpore.SuperTilemapEditor;
using UnityEngine;

public class GameScreenElona : BaseGameScreen
{
	public override float SkyRate
	{
		get
		{
			return this.skyRate;
		}
	}

	public override float TargetZoom
	{
		get
		{
			return this.zoom + 0.01f * (float)EMono.game.config.regionZoom - 1f;
		}
	}

	public override void OnActivate()
	{
		this.actor.transMap.SetActive(true);
		this.actor.selector.hasTargetChanged = true;
		EMono.scene.skyBG.SetActive(false);
		EMono.scene.cam.clearFlags = CameraClearFlags.Color;
		Tutorial.Play("region");
		this.actor.OnActivate();
	}

	public override void OnDeactivate()
	{
		this.actor.OnDeactivate();
	}

	public override void OnChangeHour()
	{
		this.actor.OnChangeHour();
	}

	public override void SetUnitSize()
	{
		this.tileAlign = new Vector2(this.tileSize.x * 0.01f, this.tileSize.y * 0.01f);
		this.tileWorldSize = new Vector2(this.tileSize.x * 0.01f, this.tileSize.y * 0.01f);
		float x = this.tileSize.x;
		float num = 100f / x;
		float num2 = num * 100f;
		float num3 = num * 100f;
		this.tileViewSize = new Vector2(num2 * 0.01f, num3 * 0.01f);
	}

	public unsafe override void RefreshPosition()
	{
		STETilemap fogmap = this.actor.elomap.fogmap;
		this.actor.transMap.position = new Vector3((float)(-(float)fogmap.MinGridX) * this.tileAlign.x + this.actorPos.x, (float)(-(float)fogmap.MinGridY) * this.tileAlign.y + this.actorPos.y, this.actorPos.z);
		if (base.fixFocus && EMono.pc.currentZone == EMono._zone)
		{
			float z = this.position.z;
			this.targetZoom = this.TargetZoom;
			Vector3 vector = (EMono.pc.isSynced ? EMono.pc.renderer.position : (*EMono.pc.pos.PositionTopdown())) + this.focusFix;
			if (this.instantFocus)
			{
				this.position = vector;
			}
			else
			{
				this.position = Vector3.Lerp(this.position, vector, this.focusSpeed * Core.delta);
			}
			this.position.z = z;
			this.pcOrbit.transform.position = EMono.pc.renderer.position;
		}
		else
		{
			if (ActionMode.Adv.IsActive)
			{
				this.targetZoom = 0.01f * (float)EMono.game.config.defaultZoom;
			}
			this.pushBack = Vector3.zero;
			int scrollX = this.scrollX;
			int scrollY = this.scrollY;
			if ((float)scrollX <= this.mapLimit.x)
			{
				this.pushBack.x = this.mapLimit.x - (float)scrollX;
			}
			if ((float)scrollY <= this.mapLimit.y)
			{
				this.pushBack.y = this.mapLimit.y - (float)scrollY;
			}
			if ((float)scrollX >= (float)EMono._map.Size + this.mapLimit.width - (float)(this.width / 2))
			{
				this.pushBack.x = (float)EMono._map.Size + this.mapLimit.width - (float)(this.width / 2) - (float)scrollX;
			}
			if ((float)scrollY >= (float)EMono._map.Size + this.mapLimit.height - (float)(this.height / 2))
			{
				this.pushBack.y = (float)EMono._map.Size + this.mapLimit.height - (float)(this.height / 2) - (float)scrollY;
			}
			this.position += this.pushBack * Core.delta * this.pushbackSpeed;
		}
		this.scrollX = Mathf.FloorToInt(this.position.x / this.tileWorldSize.x) - this.width / 2 + (int)this.paddingOffset.x;
		this.scrollY = Mathf.FloorToInt(this.position.y / this.tileWorldSize.y) - this.height / 4 + (int)this.paddingOffset.y;
		if (this.lastPos.x != (float)this.scrollX || this.lastPos.y != (float)this.scrollY)
		{
			this.lastPos.x = (float)this.scrollX;
			this.lastPos.y = (float)this.scrollY;
		}
		this.camPos.x = this.position.x;
		this.camPos.y = this.position.y;
		base.SnapScreen(ref this.camPos, this.Zoom);
		this.camPos.z = -500f;
		EMono.scene.cam.transform.localPosition = this.camPos;
		this.camPos.z = 0f;
		EMono.scene.transAudio.position = this.camPos;
		if (WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnUpdate();
		}
		this.RefreshWeather();
	}

	public override void RefreshWeather()
	{
		EMono.world.weather.RefreshWeather();
		Point pos = EMono.pc.pos;
		Weather weather = EMono.world.weather;
		Weather.Condition currentCondition = weather.CurrentCondition;
		bool isRaining = weather.IsRaining;
		bool isEther = weather.IsEther;
		bool flag = currentCondition == Weather.Condition.Snow;
		EMono.scene.filterRain.enabled = (isRaining && !flag && EMono.core.config.graphic.enhanceRain);
		EMono.scene.filterRain.Fade = ((currentCondition == Weather.Condition.RainHeavy) ? 0.4f : 0.3f);
		EMono.scene.rain.enableEmission = (isRaining && !flag);
		ParticleSystem.EmissionModule emission = EMono.scene.rain.emission;
		emission.rateOverTime = (float)((currentCondition == Weather.Condition.RainHeavy) ? 700 : 200);
		EMono.scene.snow.enableEmission = flag;
		emission = EMono.scene.snow.emission;
		emission.rateOverTime = (float)((currentCondition == Weather.Condition.SnowHeavy) ? 120 : 35);
		EMono.scene.ether.enableEmission = isEther;
		EMono.scene.snow.SetActive(true);
		EMono.scene.rain.SetActive(true);
		EMono.scene.ether.SetActive(true);
		EMono.scene.blossom.SetActive(false);
		EMono.scene.godray.SetActive(false);
		EMono.scene.godrayDust.SetActive(false);
		EMono.scene.sfxRain._Volume += Core.delta * ((isRaining && !flag) ? 0.5f : -0.5f);
		EMono.scene.sfxRain.SetVolume(EMono.scene.sfxRain._Volume);
		EMono.scene.sfxWind.SetVolume(1f);
		EMono.scene.sfxSea.SetVolume(Mathf.Lerp(0f, 1f, -0.05f * (float)(pos.x - 20)));
		EMono.scene.rain.main.prewarm = (EMono.scene.snow.main.prewarm = (EMono.scene.ether.main.prewarm = false));
		ParticleSystem[] array = EMono.scene.blossoms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].main.prewarm = false;
		}
		bool enableEmission = currentCondition == Weather.Condition.SnowHeavy && EMono.core.config.graphic.blizzard;
		array = EMono.scene.blizzards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = enableEmission;
		}
		EMono.scene.transBlizzard.localScale = new Vector3(2f, 2f, 2f);
		EMono.scene.camSupport.grading.profile.fog = EMono.setting.render.fogs[EMono._map.config.fog];
		float sceneBrightness = (!EMono._map.IsIndoor && EMono.world.weather.IsRaining) ? ((currentCondition == Weather.Condition.RainHeavy) ? -0.06f : -0.03f) : 0f;
		base.camSupport.grading.sceneBrightness = sceneBrightness;
	}

	public EloMapActor actor;

	public float skyRate;

	public float zoom;

	public Vector3 actorPos;
}
