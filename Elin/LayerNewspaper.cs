using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class LayerNewspaper : ELayer
{
	public DayData data
	{
		get
		{
			return ELayer.world.dayData;
		}
	}

	public override void OnInit()
	{
		if (ELayer.debug.enable)
		{
			NewsList.dict = null;
			ELayer.world.CreateDayData();
		}
		this.seed = (ELayer.debug.enable ? -1 : ELayer.world.dayData.seed);
		Rand.UseSeed(this.seed, delegate
		{
			this.imageAdRight.sprite = this.spritesAd.RandomItem<Sprite>();
			this.imageAdRight.SetNativeSize();
		});
		this.textDate.text = "news_date".lang(Lang.GetList("monthEng")[ELayer.world.date.month - 1], ELayer.world.date.day.ToString() ?? "", ELayer.world.date.year.ToString() ?? "", null, null);
		int luck = (int)ELayer.world.dayData.luck;
		this.textFortune.text = "news_tellLuck".lang(Lang.GetList("dayLuck")[luck], Lang.GetList("dayLuck2")[luck], null, null, null);
		this.RefreshAD();
		this.RefreshChat();
		this.RefreshVote();
		this.RefreshWeather();
		this.RefreshNews();
		this.RefreshPage();
	}

	public void OnClickPage()
	{
		this.topPage = !this.topPage;
		this.buttonPage.mainText.SetText((this.topPage ? "news_firstPage" : "news_secondPage").lang());
		SE.Play("click_scroll");
		this.RefreshPage();
	}

	public void RefreshPage()
	{
		this.goTop.SetActive(this.topPage);
		this.goBack.SetActive(!this.topPage);
	}

	public void OnClickComic()
	{
		SE.Play("click_recipe");
		ELayer.ui.AddLayer<LayerImage>().SetImage(this.spriteComicBig);
	}

	public void RefreshNews()
	{
		LayerNewspaper.<RefreshNews>d__30 <RefreshNews>d__;
		<RefreshNews>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshNews>d__.<>4__this = this;
		<RefreshNews>d__.<>1__state = -1;
		<RefreshNews>d__.<>t__builder.Start<LayerNewspaper.<RefreshNews>d__30>(ref <RefreshNews>d__);
	}

	public void RefreshAD()
	{
		Rand.SetSeed(this.seed);
		this.moldAdBottom = this.layoutAdBottom.CreateMold(null);
		float num = this.layoutAdBottom.Rect().sizeDelta.x - 20f;
		IList<Sprite> list = this.spritesSmallAd.Copy<Sprite>().Shuffle<Sprite>();
		for (int i = 0; i < list.Count; i++)
		{
			Sprite sprite = list[i];
			if (num > sprite.textureRect.width)
			{
				Image image = Util.Instantiate<Image>(this.moldAdBottom, this.layoutAdBottom);
				num = num - sprite.textureRect.width - 5f;
				image.sprite = sprite;
				image.SetNativeSize();
			}
		}
		this.layoutAdBottom.RebuildLayout(false);
		Rand.SetSeed(-1);
	}

	public void RefreshChat()
	{
		LayerNewspaper.<RefreshChat>d__32 <RefreshChat>d__;
		<RefreshChat>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshChat>d__.<>4__this = this;
		<RefreshChat>d__.<>1__state = -1;
		<RefreshChat>d__.<>t__builder.Start<LayerNewspaper.<RefreshChat>d__32>(ref <RefreshChat>d__);
	}

	public void RefreshVote()
	{
		LayerNewspaper.<RefreshVote>d__33 <RefreshVote>d__;
		<RefreshVote>d__.<>t__builder = AsyncVoidMethodBuilder.Create();
		<RefreshVote>d__.<>4__this = this;
		<RefreshVote>d__.<>1__state = -1;
		<RefreshVote>d__.<>t__builder.Start<LayerNewspaper.<RefreshVote>d__33>(ref <RefreshVote>d__);
	}

	public void RefreshWeather()
	{
		List<Weather.WeatherForecast> list = ELayer.world.weather.GetWeatherForecast();
		BaseList baseList = this.listWeather;
		UIList.Callback<Weather.WeatherForecast, UIItem> callback = new UIList.Callback<Weather.WeatherForecast, UIItem>();
		callback.onInstantiate = delegate(Weather.WeatherForecast a, UIItem b)
		{
			string text = a.date.month.ToString() + "/" + a.date.day.ToString();
			foreach (KeyValuePair<Weather.Condition, int> keyValuePair in from a in a.cons
			orderby a.Value descending
			select a)
			{
				if (b.image1.sprite == null)
				{
					b.image1.sprite = ELayer.core.refs.icons.weather[keyValuePair.Key];
				}
				else if (b.image2.sprite == null)
				{
					b.image2.sprite = ELayer.core.refs.icons.weather[keyValuePair.Key];
					b.image2.SetActive(true);
				}
			}
			b.text1.text = text;
		};
		callback.onList = delegate(UIList.SortMode m)
		{
			for (int i = 0; i < 7; i++)
			{
				if (list.Count > i && list[i].cons.Count > 0)
				{
					this.listWeather.Add(list[i]);
				}
			}
		};
		baseList.callbacks = callback;
		this.listWeather.List(false);
	}

	public UIText textDate;

	public UIText textFortune;

	public UIText textVote;

	public UIText textVoteRemaining;

	public UIList listChat;

	public UIList listWeather;

	public UIList listVote;

	public UIList listNews;

	public Image imageHeadline;

	public Image imageAdRight;

	public Image moldAdBottom;

	public List<Sprite> spritesAd;

	public List<Sprite> spritesSmallAd;

	public List<Sprite> spritesHeadline;

	public LayoutGroup layoutAdBottom;

	public LayoutGroup layoutPortrait;

	public Portrait moldPortrait;

	public List<string> desiredNews;

	public UIButton buttonPage;

	public GameObject goTop;

	public GameObject goBack;

	public Sprite spriteComicBig;

	public bool topPage;

	private int seed;
}
