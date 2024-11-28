using System;
using UnityEngine;

public class BaseGameScreen : EMono
{
	public bool fixFocus
	{
		get
		{
			ActionMode actionMode = EMono.scene.actionMode;
			return actionMode != null && actionMode.FixFocus;
		}
	}

	public virtual float TargetZoom
	{
		get
		{
			return EMono.scene.actionMode.TargetZoom;
		}
	}

	protected CameraSupport camSupport
	{
		get
		{
			return EMono.scene.camSupport;
		}
	}

	protected Transform transFocus
	{
		get
		{
			return EMono.scene.transFocus;
		}
	}

	public virtual float SkyRate
	{
		get
		{
			return Mathf.Clamp((1.2f - this.Zoom) * 2f - (EMono._zone.IsSnowCovered ? 0.38f : 0f), 0f, 2f);
		}
	}

	public virtual float ViewHeight
	{
		get
		{
			return this.SkyRate * 10f + 5f;
		}
	}

	public virtual bool IsGameScreen
	{
		get
		{
			return true;
		}
	}

	public virtual bool IsLocalMap
	{
		get
		{
			return false;
		}
	}

	public void Activate()
	{
		if (EMono.core.screen == this)
		{
			return;
		}
		if (EMono.core.screen != null)
		{
			EMono.core.screen.Deactivate();
		}
		this.SetUnitSize();
		EMono.core.screen = this;
		Point._screen = this;
		base.gameObject.SetActive(true);
		EMono.scene.flock.SetActive(this.IsLocalMap);
		EMono.scene.fireflyNight.SetActive(this.IsLocalMap);
		EMono.scene.firefly.SetActive(this.IsLocalMap);
		EMono.scene.star.SetActive(this.IsLocalMap);
		EMono.scene.sfxRain.SetActive(this.IsGameScreen);
		EMono.scene.sfxSea.SetActive(this.IsGameScreen);
		EMono.scene.sfxWind.SetActive(this.IsGameScreen);
		EMono.scene.sfxFire.SetActive(this.IsGameScreen);
		this.OnActivate();
		EMono.scene.rain.main.prewarm = (EMono.scene.snow.main.prewarm = (EMono.scene.ether.main.prewarm = true));
		ParticleSystem[] blossoms = EMono.scene.blossoms;
		for (int i = 0; i < blossoms.Length; i++)
		{
			blossoms[i].main.prewarm = true;
		}
		if (this.overlayShadow)
		{
			this.overlayShadow.SetActive(EMono.scene.profile.matOverlay);
			this.overlayShadow.sharedMaterial = EMono.scene.profile.matOverlay;
		}
		EMono.scene.camSupport.grading.material = EMono.scene.profile.matGrading;
		if (this.IsGameScreen)
		{
			EMono.scene.OnChangeHour();
			EMono.scene.ApplyZoneConfig();
			this.RefreshAll();
			this.tileMap.OnActivate(this);
			WidgetHotbar.RefreshHighlights();
			return;
		}
		if (EMono.core.config != null)
		{
			EMono.core.config.RefreshUIBrightness();
		}
	}

	public virtual void OnActivate()
	{
	}

	public void Deactivate()
	{
		this.OnDeactivate();
		base.gameObject.SetActive(false);
		EMono.core.screen = null;
	}

	public virtual void OnDeactivate()
	{
	}

	public virtual void SetUnitSize()
	{
	}

	public virtual void OnEndPlayerTurn()
	{
	}

	public unsafe void Draw()
	{
		if (!this.IsGameScreen)
		{
			return;
		}
		if (this.Zoom != this.targetZoom)
		{
			bool flag = EMono.scene.actionMode == ActionMode.Bird;
			CoreConfig.CameraConfig camera = EMono.core.config.camera;
			float num = camera.zoomSpeed * (flag ? 0.05f : (camera.linearZoom ? 2f : 1f)) * Core.delta;
			this.zoomTimer += num * 0.3f;
			float t = (flag || !camera.linearZoom) ? num : this.zoomTimer;
			float num2 = Mathf.Lerp(EMono.screen.Zoom, this.targetZoom, t);
			if (Mathf.Abs(num2 - this.targetZoom) < (flag ? 0.0005f : 0.003f))
			{
				num2 = this.targetZoom;
			}
			if (camera.zoomToMouse && this.zoomPos != Vector3.zero)
			{
				this.position = Vector3.Lerp(this.position, this.zoomPos, t);
			}
			EMono.core.config.ApplyZoom(num2);
		}
		RenderObject.gameSpeed = EMono.scene.actionMode.gameSpeed;
		RenderObject.gameDelta = Core.gameDeltaNoPause;
		if (EMono.pc.currentZone == EMono._zone && !EMono._zone.IsRegion && this.focusOption == null && this.focusPos == null && !this.transFocus && this.fixFocus && (this.instantFocus || !EMono.core.config.camera.smoothFollow))
		{
			if (this.Zoom != this.targetZoom)
			{
				EMono.core.config.ApplyZoom(this.TargetZoom);
			}
			Vector3 vector = (EMono.pc.isSynced ? EMono.player.position : (*EMono.pc.pos.Position())) + this.focusFix;
			this.position = vector;
			this.scrollX = Mathf.FloorToInt(this.position.x / this.tileWorldSize.x) - this.width / 2 + (int)this.paddingOffset.x;
			this.scrollY = Mathf.FloorToInt(this.position.y / this.tileWorldSize.y) - this.height / 4 + (int)this.paddingOffset.y;
		}
		this.tileMap.Draw();
		if (this.instantFocus)
		{
			this.instantFocus = false;
		}
		this.RefreshPosition();
		if (EMono.game.activeZone != null)
		{
			for (int i = EMono._map.pointAnimes.Count - 1; i >= 0; i--)
			{
				if (EMono._map.pointAnimes[i].Update())
				{
					EMono._map.pointAnimes.RemoveAt(i);
				}
			}
		}
		this.UpdateShaders(EMono.scene.timeRatio);
		if (Scene.skipAnime)
		{
			EffectManager.Instance.KillAll();
			Scene.skipAnime = false;
		}
		EffectManager.Instance.UpdateEffects();
		FlockController flock = EMono.scene.flock;
		bool visible;
		if (this.targetZoom <= 1.25f)
		{
			Room room = EMono.pc.pos.cell.room;
			visible = (room == null || !room.HasRoof);
		}
		else
		{
			visible = false;
		}
		flock.UpdateVisible(visible);
		if (EMono.pc.isSynced && EMono.pc.renderer.orbit)
		{
			EMono.pc.renderer.orbit.Refresh();
		}
	}

	public void UpdateShaders(float time = 0f)
	{
		SceneProfile profile = EMono.scene.profile;
		SceneColorProfile color = profile.color;
		SceneLightProfile light = profile.light;
		Color color2 = color.fog.Evaluate(time);
		float value = light.nightRatioCurve.Evaluate(time);
		Shader.SetGlobalFloat(this._Snow, EMono._zone.IsSnowCovered ? 1f : 0f);
		Shader.SetGlobalFloat(this._BackDrawPower, (float)(ActionMode.Cinema.IsActive ? 0 : EMono.game.config.backDrawAlpha));
		Shader.SetGlobalFloat(this._SkyLevel, this.SkyRate);
		Shader.SetGlobalFloat(this._NightRate, value);
		Shader.SetGlobalFloat(this._ViewHeight, this.ViewHeight);
		Shader.SetGlobalFloat(this._FogStrength, color2.a * 0.1f);
		Shader.SetGlobalColor(this._FogColor, color2);
		FowProfile fowProfile = EMono._zone.biome.fowProfile;
		if (EMono._map.fowProfile)
		{
			fowProfile = EMono._map.fowProfile;
		}
		if (fowProfile)
		{
			FOWType type = fowProfile.type;
			if (type != FOWType.Color)
			{
				if (type != FOWType.ColorAdd)
				{
					Shader.SetGlobalColor(this._FowColor, color.fow.Evaluate(time));
				}
				else
				{
					Shader.SetGlobalColor(this._FowColor, fowProfile.color * color.fow.Evaluate(time));
				}
			}
			else
			{
				Shader.SetGlobalColor(this._FowColor, fowProfile.color);
			}
		}
		else
		{
			Shader.SetGlobalColor(this._FowColor, color.fow.Evaluate(time));
		}
		Shader.SetGlobalColor(this._SunColor, EMono._zone.IsSnowCovered ? color.sunSnow.Evaluate(time) : color.sun.Evaluate(time));
		Shader.SetGlobalColor(this._SeaColor, color.sea.Evaluate(time));
		Shader.SetGlobalColor(this._SkyColor, color.sky.Evaluate(time));
		Shader.SetGlobalColor(this._SkyBGColor, color.skyBG.Evaluate(time));
		Shader.SetGlobalVector(this._Offset, EMono.core.config.camera.extendZoomMin ? (this.position * 0.3f) : (this.position * 0.5f));
		Shader.SetGlobalFloat(this._Zoom, this.Zoom);
		Shader.SetGlobalFloat(this._LightPower, light.lightPower.Evaluate(time) * light.lightPowerMod);
		Shader.SetGlobalFloat(this._AnimeSpeedGlobal, (float)EMono.game.config.animeSpeed);
		Shader.SetGlobalColor(this._ScreenFlash, ScreenFlash.currentColor);
		EMono.scene.godray.main.startColor = (EMono._zone.IsSnowCovered ? color.godraySnow.Evaluate(time) : color.godray.Evaluate(time));
		this.camSupport.grading.vignettePower = light.vignetteCurve.Evaluate(time);
		this.camSupport.grading.UpdateVignette(0f);
		this.camSupport.bloom.threshold = light.bloomCurve.Evaluate(time);
		this.camSupport.beautify.bloomIntensity = light.bloomCurve2.Evaluate(time);
	}

	public virtual void OnChangeHour()
	{
	}

	public unsafe virtual void RefreshPosition()
	{
		if (EMono.pc.currentZone != EMono._zone)
		{
			return;
		}
		if (this.focusOption != null)
		{
			Vector3 vector = this.focusOption.player ? (*EMono.pc.pos.Position() + this.focusFix) : (*this.focusOption.pos.PositionCenter());
			vector.z = this.position.z;
			if (this.focusOption.linear)
			{
				this.position = Vector3.MoveTowards(this.position, vector, this.focusOption.speed * Core.delta);
			}
			else
			{
				this.position = Vector3.Lerp(this.position, vector, this.focusOption.speed * Core.delta);
			}
		}
		else if (this.focusPos != null)
		{
			this.focusPos = new Vector3?(new Vector3((this.focusPos ?? this.position).x, (this.focusPos ?? this.position).y, this.position.z));
			this.position = Vector3.Lerp(this.position, this.focusPos ?? this.position, this.focusSpeedSlow * Core.delta);
			if (Vector3.Distance(this.position, this.focusPos ?? this.position) < 0.1f)
			{
				this.focusPos = null;
			}
		}
		else if (this.transFocus)
		{
			this.pushBack = Vector3.zero;
			this.position.x = this.transFocus.position.x;
			this.position.y = this.transFocus.position.y;
		}
		else if (this.fixFocus)
		{
			float z = this.position.z;
			this.targetZoom = this.TargetZoom;
			for (int i = 0; i < EMono.core.config.camera.zooms.Length; i++)
			{
				if (this.targetZoom == EMono.core.config.camera.zooms[i])
				{
					this.targetZoomIndex = i;
				}
			}
			Vector3 b = (EMono.pc.isSynced ? EMono.player.position : (*EMono.pc.pos.Position())) + this.focusFix;
			if (this.instantFocus)
			{
				this.position = b;
			}
			else if (EMono.core.config.camera.smoothFollow)
			{
				this.position = Vector3.Lerp(this.position, b, this.focusSpeed * Core.delta * Mathf.Lerp(0.2f, 1f, 1f - Mathf.Clamp(ActionMode.Adv.rightMouseTimer, 0f, 1f)));
			}
			else
			{
				this.position = b;
			}
			this.position.z = z;
			this.pcOrbit.transform.position = EMono.pc.renderer.position;
		}
		else
		{
			if (ActionMode.Adv.IsActive)
			{
				this.targetZoom = ActionMode.Adv.TargetZoom;
			}
			this.pushBack = Vector3.zero;
			int num = this.scrollX - this.scrollY;
			int num2 = this.scrollY + this.scrollX;
			if ((float)num <= this.mapLimit.x)
			{
				this.pushBack.x = this.mapLimit.x - (float)num;
			}
			if ((float)num2 <= this.mapLimit.y)
			{
				this.pushBack.y = this.mapLimit.y - (float)num2;
			}
			if ((float)num >= (float)EMono._map.Size + this.mapLimit.width - (float)(this.width / 2))
			{
				this.pushBack.x = (float)EMono._map.Size + this.mapLimit.width - (float)(this.width / 2) - (float)num;
			}
			if ((float)num2 >= (float)EMono._map.Size + this.mapLimit.height - (float)(this.height / 2))
			{
				this.pushBack.y = (float)EMono._map.Size + this.mapLimit.height - (float)(this.height / 2) - (float)num2;
			}
			this.position += Quaternion.Euler(this.planeAngle) * this.pushBack * Core.delta * this.pushbackSpeed;
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
		this.SnapScreen(ref this.camPos, this.Zoom);
		this.camPos.z = -500f;
		this._camPos.x = this.camPos.x + this.screenFixX2 + this.screenFixX4;
		this._camPos.y = this.camPos.y + this.screenFixY2 + this.screenFixY4;
		this._camPos.z = this.camPos.z;
		EMono.scene.cam.transform.localPosition = this._camPos;
		this.camPos.z = 0f;
		EMono.scene.transAudio.position = this.camPos + EMono.scene.posAudioListener;
		if (WidgetMinimap.Instance)
		{
			WidgetMinimap.Instance.OnUpdate();
		}
	}

	public void RefreshAll()
	{
		this.RefreshScreenSize();
		bool indoor = EMono._map.config.indoor;
		ScreenGrading grading = this.camSupport.grading;
		EMono.scene.flock.SetActive(!indoor);
		if (indoor)
		{
			grading.material.DisableKeyword("CLOUD_ON");
		}
		else
		{
			grading.material.EnableKeyword("CLOUD_ON");
		}
		grading.profile.vignette.enable = EMono.scene.profile.light.vignette;
		grading.profile.vignette.vignetteColor = EMono.scene.profile.light.vignetteColor;
		this.RefreshSky();
		this.RefreshTilt();
	}

	public void RefreshScreenSize()
	{
		this.Zoom = EMono.scene.camSupport.Zoom;
		this.width = (int)((float)Screen.width / this.tileSize.x / this.Zoom) + (int)this.paddings.x + 2;
		this.height = (int)((float)Screen.height / (this.tileSize.y / 2f) / this.Zoom) + (int)this.paddings.y + 4;
		this.camSupport.divier = EMono.core.config.fix.divider;
		this.camSupport.ResizeCameraToPixelPerfect();
		Vector3 localScale = new Vector3(EMono.scene.cam.orthographicSize * ((float)Screen.width / (float)Screen.height) * 2.5f, EMono.scene.cam.orthographicSize * 2.5f, 1f);
		if (EMono.core.IsGameStarted)
		{
			EMono.scene.skyBG.transform.localScale = localScale;
			float num = (ActionMode.IsAdv || this.Zoom >= 1f || EMono._map.IsIndoor) ? 0f : (0.4f / this.Zoom);
			if (EMono.world.weather.CurrentCondition == Weather.Condition.SnowHeavy)
			{
				num += 0.8f;
			}
			if (!EMono.player.simulatingZone)
			{
				EMono.scene.sfxWind.SetVolume(Mathf.Min(num, 1f));
			}
		}
		Vector3 vector = EMono.scene.cam.transform.position;
		EMono.scene.cam.transform.position = Vector3.zero;
		Vector3 vector2 = EMono.scene.cam.WorldToScreenPoint(Vector3.zero);
		this.screenFixX2 = vector2.x % this.screenFixX * this.screenFixX3;
		this.screenFixY2 = vector2.y % this.screenFixY * this.screenFixY3;
		EMono.scene.cam.transform.position = vector;
	}

	public Point GetRandomPoint()
	{
		int num = EMono.screen.scrollX - EMono.screen.scrollY + this.width / 2 - (this.height / 2 + 1) / 2;
		int num2 = EMono.screen.scrollY + EMono.screen.scrollX + this.width / 2 + this.height / 2 / 2;
		num = num - 20 + EMono.rnd(40);
		num2 = num2 - 20 + EMono.rnd(40);
		return this._randomPoint.Set(num, num2);
	}

	public void RefreshSky()
	{
		CoreConfig.GraphicSetting graphic = EMono.core.config.graphic;
		bool indoor = EMono._map.config.indoor;
		EMono.scene.firefly.baseParticleCount = graphic.fireflyCount;
		EMono.scene.fireflyNight.baseParticleCount = graphic.fireflyCount * 10;
		EMono.scene.star.baseParticleCount = graphic.starCount;
		EMono.scene.star.gameObject.GetComponent<ParticleSystemRenderer>().enabled = EMono.world.date.IsNight;
		EMono.scene.star.SetActive(this.IsLocalMap && !indoor);
		EMono.scene.firefly.SetActive(this.IsLocalMap && graphic.firefly && !indoor);
		EMono.scene.fireflyNight.SetActive(this.IsLocalMap && graphic.firefly && !indoor);
		if (this.overlayShadow && EMono.scene.profile.matOverlay && this is GameScreen)
		{
			this.overlayShadow.sharedMaterial.SetFloat("_ShadowStrength", EMono._map.config.shadowStrength * (EMono._zone.IsSnowCovered ? 0.5f : 1f) * EMono.setting.render.shadowStrength);
		}
		this.RefreshWeather();
		this.RefreshGrading();
	}

	public virtual void RefreshWeather()
	{
		bool flag;
		if (!EMono._map.config.indoor)
		{
			if (EMono.pc.IsInActiveZone)
			{
				Room room = EMono.pc.pos.cell.room;
				flag = (room != null && room.HasRoof);
			}
			else
			{
				flag = false;
			}
		}
		else
		{
			flag = true;
		}
		bool flag2 = flag;
		Room room2 = EMono.pc.pos.cell.room;
		bool flag3 = room2 != null && room2.data.atrium;
		Weather weather = EMono.world.weather;
		Weather.Condition currentCondition = weather.CurrentCondition;
		EMono.scene.filterRain.enabled = (!flag2 && weather.IsRaining && EMono.core.config.graphic.enhanceRain);
		EMono.scene.filterRain.Fade = ((currentCondition == Weather.Condition.RainHeavy) ? 0.4f : 0.3f);
		EMono.scene.rain.enableEmission = (!flag2 && weather.IsRaining);
		EMono.scene.rain.emission.rateOverTime = (float)((currentCondition == Weather.Condition.RainHeavy) ? 750 : 200);
		EMono.scene.snow.enableEmission = (!flag2 && currentCondition == Weather.Condition.Snow);
		EMono.scene.ether.enableEmission = (!flag2 && weather.IsEther);
		bool enableEmission = !flag2 && weather.IsBlossom;
		ParticleSystem[] array = EMono.scene.blossoms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = enableEmission;
		}
		enableEmission = (!flag2 && currentCondition == Weather.Condition.SnowHeavy && EMono.core.config.graphic.blizzard);
		array = EMono.scene.blizzards;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].enableEmission = enableEmission;
		}
		EMono.scene.transBlizzard.localScale = new Vector3(1f, 1f, 1f);
		bool flag4 = (EMono._map.config.forceGodRay || (EMono.core.config.graphic.godray && !flag2 && (currentCondition == Weather.Condition.Fine || currentCondition == Weather.Condition.Snow))) && !BuildMenu.Instance;
		EMono.scene.godray.SetActive(flag4, delegate(bool enabled)
		{
			if (!enabled)
			{
				EMono.scene.godray.Clear();
			}
		});
		EMono.scene.godrayDust.SetActive(flag4 && EMono.world.date.IsNight);
		EMono.scene.snow.SetActive(true);
		EMono.scene.rain.SetActive(true);
		EMono.scene.ether.SetActive(true);
		EMono.scene.blossom.SetActive(true);
		float num = (EMono._zone.lv <= -2) ? 0f : ((EMono._zone.lv <= -1) ? 0.3f : ((flag2 && !flag3) ? 0.6f : 1f));
		EMono.scene.sfxRain.SetVolume(weather.IsRaining ? num : 0f);
		EMono.scene.sfxSea.SetVolume(EMono._zone.VolumeSea * num);
		EMono.scene.camSupport.grading.profile.fog = EMono.setting.render.fogs[EMono._map.config.fog];
		EMono.scene.camSupport.grading.userFog = ((!EMono._map.config.indoor && flag2) ? 0f : 1f);
		float num2 = (!EMono._map.IsIndoor && EMono.world.weather.IsRaining) ? ((currentCondition == Weather.Condition.RainHeavy) ? -0.08f : -0.04f) : 0f;
		if (EMono._zone.IsSnowCovered)
		{
			num2 += EMono.scene.profile.global.snowBrightness;
		}
		this.camSupport.grading.sceneBrightness = num2;
		EMono.scene.rain.main.prewarm = (EMono.scene.snow.main.prewarm = (EMono.scene.ether.main.prewarm = false));
		array = EMono.scene.blossoms;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].main.prewarm = false;
		}
		this.camSupport.grading.SetGrading();
	}

	public void RefreshGrading()
	{
		float timeRatio = EMono.core.IsGameStarted ? EMono.scene.timeRatio : 0f;
		if (this.camSupport.grading.profile.overlay)
		{
			this.camSupport.grading.profile.overlay.Refresh(timeRatio, EMono._zone.IsSnowCovered);
		}
		ScreenGrading.blind = EMono.pc.isBlind;
		this.camSupport.grading.profile.sharpen.enable = (EMono.core.config.graphic.sharpen > 0);
		this.camSupport.grading.profile.sharpen.Strength = Mathf.Clamp(0.1f * (float)EMono.core.config.graphic.sharpen, 0f, (this is GameScreen) ? 10f : 1f);
		this.camSupport.beautify.sharpen = Mathf.Clamp(0.01f * (float)EMono.core.config.graphic.sharpen2, 0f, (this is GameScreen) ? 10f : 0.5f);
		this.camSupport.grading.SetGrading();
		SceneTemplate sceneTemplate = SceneTemplate.Load(EMono._map.config.idSceneTemplate.IsEmpty(EMono._zone.IDSceneTemplate));
		Color color = EMono._map.config.colorScreen.Get();
		if (color.a == 0f)
		{
			color = sceneTemplate.colorScreen;
		}
		this.camSupport.beautify.tintColor = color;
		color = EMono._map.config.colorSea.Get();
		MATERIAL.sourceWaterSea.matColor = ((color.a == 0f) ? MATERIAL.sourceWaterSea.GetColor() : color);
		EMono.core.config.RefreshUIBrightness();
		float num = 0f;
		if (!EMono._map.IsIndoor)
		{
			num += 0.01f * EMono.core.config.test.brightnessNight * EMono.scene.profile.light.nightRatioCurve.Evaluate(EMono.scene.timeRatio);
		}
		this.camSupport.grading.nightBrightness = num;
	}

	public void RefreshTilt()
	{
		this.camSupport.tiltShift.enabled = ((ActionMode.Bird.IsActive || EMono._zone.IsRegion) ? EMono.game.config.tiltRegion : EMono.game.config.tilt);
		this.camSupport.tiltShift.blurArea = (ActionMode.Bird.IsActive ? 12f : (0.1f * (float)(EMono._zone.IsRegion ? EMono.game.config.tiltPowerRegion : EMono.game.config.tiltPower)));
	}

	public void ScrollMouse(float x, float y)
	{
		this.position.x = this.position.x + x / this.Zoom;
		this.position.y = this.position.y + y / this.Zoom;
		if (x > 0.1f || x < -0.1f || y > 0.1f || y < 0.1f)
		{
			this.RefreshPosition();
		}
	}

	public void ScrollAxis(Vector3 axis, bool direct = false)
	{
		if (direct)
		{
			this.position += axis;
			return;
		}
		this.position += axis * Core.delta * this.camSpeed2 * EMono.core.config.camera.senseKeyboard / this.Zoom;
	}

	public void Focus(Int3 ints)
	{
		this.position = new Vector3((float)ints.x, (float)ints.y, (float)ints.z);
	}

	public void Focus(int x, int y)
	{
		if (EMono.AdvMode)
		{
			SE.Beep();
			return;
		}
		this.scrollX = x;
		this.position.x = (float)x;
		this.scrollY = y;
		this.position.y = (float)y;
	}

	public void Focus(Card c)
	{
		if (c == null)
		{
			return;
		}
		this.Focus(c.GetRootCard().pos);
	}

	public void FocusCenter()
	{
		Point pos = new Point(EMono._map.Size / 2, EMono._map.Size / 2);
		this.Focus(pos);
	}

	public unsafe void Focus(Point pos)
	{
		this.position = *pos.PositionCenter();
	}

	public void FocusPC()
	{
		this.focusPos = null;
		this.Focus((EMono.pc.currentZone == EMono.game.activeZone) ? EMono.pc.pos : EMono._map.bounds.GetCenterPos());
		this.instantFocus = true;
		this.RefreshPosition();
	}

	public void FocusImmediate(Point pos)
	{
		this.focusPos = null;
		this.Focus(pos);
		this.instantFocus = true;
		this.RefreshPosition();
		this.RefreshPosition();
	}

	public void SnapScreen(ref Vector3 v, float zoom)
	{
		float num = this.snapSize.x / zoom;
		float num2 = this.snapSize.y / zoom;
		switch (EMono.core.config.fix.snapType)
		{
		case CoreConfig.ScreenSnapType.Default:
			v.x = num * (float)Mathf.RoundToInt(v.x / num);
			v.y = num2 * (float)Mathf.RoundToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Floor:
			v.x = num * (float)Mathf.FloorToInt(v.x / num);
			v.y = num2 * (float)Mathf.FloorToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Ceil:
			v.x = num * (float)Mathf.CeilToInt(v.x / num);
			v.y = num2 * (float)Mathf.CeilToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Hack3:
			num = this.snapSize.x;
			num2 = this.snapSize.y;
			v.x = num * (float)Mathf.RoundToInt(v.x / num);
			v.y = num2 * (float)Mathf.RoundToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Hack4:
			num = this.snapSize.x;
			num2 = this.snapSize.y;
			v.x = num * (float)Mathf.FloorToInt(v.x / num);
			v.y = num2 * (float)Mathf.FloorToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Hack5:
			num = this.snapSize.x;
			num2 = this.snapSize.y;
			v.x = num * (float)Mathf.CeilToInt(v.x / num);
			v.y = num2 * (float)Mathf.CeilToInt(v.y / num2);
			return;
		case CoreConfig.ScreenSnapType.Hack6:
			v.x = 0.01f * (float)Mathf.FloorToInt(v.x * 100f) + 0.001f;
			v.y = 0.01f * (float)Mathf.FloorToInt(v.y * 100f) + 0.001f;
			return;
		case CoreConfig.ScreenSnapType.Hack7:
			v.x = 0.01f * (float)Mathf.FloorToInt(v.x * 100f) - 0.001f;
			v.y = 0.01f * (float)Mathf.FloorToInt(v.y * 100f) - 0.001f;
			return;
		case CoreConfig.ScreenSnapType.Hack8:
			v.x = 0.01f * (float)Mathf.RoundToInt(v.x * 100f) + 0.005f;
			v.y = 0.01f * (float)Mathf.RoundToInt(v.y * 100f) + 0.005f;
			return;
		case CoreConfig.ScreenSnapType.Hack9:
			v.x = 0.01f * (float)Mathf.RoundToInt(v.x * 100f) + 0.025f;
			v.y = 0.01f * (float)Mathf.RoundToInt(v.y * 100f) + 0.025f;
			return;
		default:
			return;
		}
	}

	public void SetZoom(float z)
	{
		this.Zoom = z;
		this.targetZoom = z;
		EMono.core.config.ApplyZoom(this.targetZoom);
		this.zoomTimer = 0f;
	}

	public void SetTargetZoomIndex(int index)
	{
		this.targetZoomIndex = index;
		float num = this.targetZoom;
		this.targetZoom = EMono.core.config.camera.zooms[this.targetZoomIndex];
		if (num != this.targetZoom)
		{
			this.zoomTimer = 0f;
		}
	}

	public void ModTargetZoomIndex(int a)
	{
		if (EMono.scene.elomapActor.IsActive)
		{
			return;
		}
		int num = this.targetZoomIndex;
		this.targetZoomIndex -= a;
		int num2 = EMono.core.config.camera.extendZoomMin ? 0 : 1;
		int num3 = EMono.core.config.camera.extendZoomMax ? 4 : 3;
		if (this.targetZoomIndex < num2)
		{
			this.targetZoomIndex = num2;
		}
		else if (this.targetZoomIndex >= num3)
		{
			this.targetZoomIndex = num3;
		}
		if (this.targetZoomIndex > num && EInput.buttonScroll != null && !EInput.buttonScroll.pressing)
		{
			this.zoomPos = EInput.mposWorld;
		}
		this.SetTargetZoomIndex(this.targetZoomIndex);
	}

	private readonly int _BackDrawPower = Shader.PropertyToID("_BackDrawPower");

	private readonly int _Snow = Shader.PropertyToID("_Snow");

	private readonly int _SkyLevel = Shader.PropertyToID("_SkyLevel");

	private readonly int _NightRate = Shader.PropertyToID("_NightRate");

	private readonly int _ViewHeight = Shader.PropertyToID("_ViewHeight");

	private readonly int _FogStrength = Shader.PropertyToID("_FogStrength");

	private readonly int _FogColor = Shader.PropertyToID("_FogColor");

	private readonly int _FowColor = Shader.PropertyToID("_FowColor");

	private readonly int _SunColor = Shader.PropertyToID("_SunColor");

	private readonly int _SeaColor = Shader.PropertyToID("_SeaColor");

	private readonly int _SkyColor = Shader.PropertyToID("_SkyColor");

	private readonly int _SkyBGColor = Shader.PropertyToID("_SkyBGColor");

	private readonly int _Offset = Shader.PropertyToID("_Offset");

	private readonly int _Zoom = Shader.PropertyToID("_Zoom");

	private readonly int _LightPower = Shader.PropertyToID("_LightPower");

	private readonly int _AnimeSpeedGlobal = Shader.PropertyToID("_AnimeSpeedGlobal");

	private readonly int _ScreenFlash = Shader.PropertyToID("_ScreenFlash");

	public float SharpenAmount;

	public float camSpeed2;

	public float heightLight;

	public float pushbackSpeed;

	public Vector2 snapSize;

	public Vector2 tileSize;

	public Vector2 tileAlign;

	public Vector2 tileWorldSize;

	public Vector2 tileViewSize;

	public Vector3 tileWeight;

	public Vector3 focusFix;

	public Vector3 tileOffsetHeight;

	public Vector3 tileWeightHeight;

	public Vector3 planeAngle;

	public Vector3 planeSpeed;

	public Vector2 paddings;

	public Vector2 paddingOffset;

	public Vector3 lastPos;

	public Rect mapLimit;

	[Space]
	public float Zoom;

	[Space]
	public float nextMove;

	[Space]
	public float focusSpeed;

	[Space]
	public float focusSpeedSlow;

	[Space]
	public float focusSpeedSlow2;

	public int width;

	public int height;

	public int scrollX;

	public int scrollY;

	public int moonLevel;

	public BaseTileMap tileMap;

	public BaseTileSelector tileSelector;

	public ScreenGuide guide;

	public Grid grid;

	public MeshRenderer overlayShadow;

	public PCOrbit pcOrbit;

	[NonSerialized]
	public float forcePrecision;

	[NonSerialized]
	public bool isCameraMoving;

	[NonSerialized]
	public BaseGameScreen.FocusOption focusOption;

	[NonSerialized]
	public Vector3? focusPos;

	[NonSerialized]
	public Vector3 position;

	protected Vector3 camPos;

	protected Vector3 pushBack;

	public float screenFixX;

	public float screenFixX2;

	public float screenFixX3;

	public float screenFixX4;

	public float screenFixY;

	public float screenFixY2;

	public float screenFixY3;

	public float screenFixY4;

	private Vector3 _camPos;

	private Point _randomPoint = new Point();

	[NonSerialized]
	public bool instantFocus;

	public int targetZoomIndex;

	public float targetZoom = 0.5f;

	public float zoomTimer;

	public Vector3 zoomPos;

	public class FocusOption
	{
		public Point pos;

		public float speed = 2f;

		public bool linear = true;

		public bool player;
	}
}
