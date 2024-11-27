using System;
using UnityEngine;
using UnityEngine.UI;

public class LayerTitle : ELayer, IChangeLanguage, IChangeResolution
{
	public LayerTitle.TitleType CurrentTitle
	{
		get
		{
			return this.types[this.altTitle ? 1 : 0];
		}
	}

	public override void OnInit()
	{
		LayerTitle.Instance = this;
		this.textVersion.text = (ELayer.core.version.GetText() ?? "");
		this.textIntro.text = IO.LoadText(CorePath.CorePackage.TextData + "intro.txt");
		ELayer.ui.SetLight(true);
		this.titleActor = Util.Instantiate<Transform>(this.titleActor, null);
		this.Refresh();
		SoundManager.current.PlayBGM(this.CurrentTitle.bgmTitle, 0f, 0f);
	}

	public void Refresh()
	{
		this.titleActor.GetComponentInChildren<SpriteRenderer>().sprite = this.CurrentTitle.bg;
		this.uiLight.color = this.CurrentTitle.light;
		this.imageFog.color = this.CurrentTitle.fog;
		this.OnChangeResolution();
	}

	public void OnClickStart()
	{
		if (!(ELayer.ui.GetTopLayer() is LayerTitle))
		{
			return;
		}
		this.embark = ELayer.ui.AddLayer<LayerEditBio>();
		SoundManager.current.PlayBGM(this.CurrentTitle.bgm, 0f, 0f);
	}

	public void OnChangeResolution()
	{
		this.imageBG.transform.localScale = new Vector3(Mathf.Max(1f, 1.7777778f / ((float)Screen.width / (float)Screen.height)), 1f, 1f);
	}

	public void OnClickContinue()
	{
		if (!(ELayer.ui.GetTopLayer() is LayerTitle))
		{
			return;
		}
		ELayer.ui.AddLayer<LayerLoadGame>().Init(false, "", "");
	}

	public void OnClickConfig()
	{
		ELayer.ui.AddLayer<LayerConfig>();
	}

	public void OnClickMod()
	{
		ELayer.ui.AddLayer<LayerMod>();
	}

	public void OnClickCredit()
	{
		ELayer.ui.AddLayer<LayerCredit>();
	}

	public void OnClickAnnounce()
	{
		ELayer.ui.AddLayer("LayerAnnounce");
	}

	public void OnClickAbout()
	{
		ELayer.ui.AddLayer("LayerAbout");
	}

	public void OnClickFeedback()
	{
		ELayer.ui.ToggleFeedback();
	}

	public void OnClickExit()
	{
		if (Application.isEditor)
		{
			ELayer.ui.RemoveLayer(this);
			if (LayerTitle.actor)
			{
				UnityEngine.Object.DestroyImmediate(LayerTitle.actor.gameObject);
			}
			ELayer.ui.AddLayer<LayerTitle>();
			return;
		}
		ELayer.core.Quit();
	}

	public override void OnKill()
	{
		UnityEngine.Object.DestroyImmediate(this.titleActor.gameObject);
		if (this.embark)
		{
			ELayer.ui.RemoveLayer(this.embark);
			UnityEngine.Object.DestroyImmediate(this.embark);
		}
		ELayer.ui.SetLight(false);
	}

	private void Update()
	{
		LayerEditBio layer = ELayer.ui.GetLayer<LayerEditBio>(false);
		if (!layer && SoundManager.current.currentBGM.id != this.CurrentTitle.bgmTitle.name)
		{
			if (this.toggle)
			{
				this.altTitle = !this.altTitle;
			}
			SoundManager.current.PlayBGM(this.CurrentTitle.bgmTitle, 0f, 0f);
			this.Refresh();
		}
		float num = Mathf.Clamp(this.cgBook.alpha + Time.smoothDeltaTime * this.speed * (float)(layer ? -1 : 1), 0f, 1f);
		if (this.cgBook.alpha != num)
		{
			this.cgBook.alpha = num;
		}
		if (this.cgBG.alpha != num)
		{
			this.cgBG.alpha = num;
		}
		this.ray.enabled = (num == 1f);
	}

	public void OnChangeLanguage()
	{
		this.textIntro.text = IO.LoadText(CorePath.CorePackage.TextData + "intro.txt");
	}

	public static void KillActor()
	{
		if (LayerTitle.actor)
		{
			UnityEngine.Object.DestroyImmediate(LayerTitle.actor.gameObject);
		}
	}

	public LayerTitle.TitleType[] types;

	public static LayerTitle Instance;

	public static EmbarkActor actor;

	public LayerEditBio embark;

	public UIText textVersion;

	public UIText textIntro;

	public Transform transBook;

	public CanvasGroup cgBook;

	public CanvasGroup cgBG;

	public float speed;

	public GraphicRaycaster ray;

	public Transform titleActor;

	public Image uiLight;

	public RawImage imageFog;

	public RawImage imageBG;

	public bool toggle;

	public bool altTitle;

	[Serializable]
	public class TitleType
	{
		public Color light;

		public Color fog;

		public Sprite bg;

		public BGMData bgm;

		public BGMData bgmTitle;
	}
}
