using System;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using UnityEngine;
using UnityEngine.UI;

public class LayerUploader : ELayer
{
	private string savePath
	{
		get
		{
			return CorePath.ZoneSaveUser + this.inputId.text + ".z";
		}
	}

	public override void OnInit()
	{
		this.ini = ELayer.core.mods.GetElinIni();
		string text = this.ini.GetKey("pass") ?? "password";
		InputField inputField = this.inputId;
		CustomData custom = ELayer._map.custom;
		inputField.text = (((custom != null) ? custom.id : null) ?? "new_zone");
		this.inputPassword.text = text;
	}

	private void Update()
	{
		string text = this.inputId.text;
		this.validId = (text.Length >= 3 && text.IndexOfAny(LayerUploader.InvalidChars) == -1 && !this.invalidIds.Contains(text));
		text = this.inputPassword.text;
		this.validPass = (text.Length >= 3 && text.IndexOfAny(LayerUploader.InvalidChars) == -1);
		this.textInvalidId.SetActive(!this.validId);
		this.textInvalidPass.SetActive(!this.validPass);
		bool interactableWithAlpha = this.validId && this.validPass;
		this.buttonSave.SetInteractableWithAlpha(interactableWithAlpha);
		int num = LayerUploader.nextUpload - (int)Time.realtimeSinceStartup;
		this.textNextUpload.SetActive(num > 0);
		if (num > 0)
		{
			this.textNextUpload.text = "net_nextUpload".lang(num.ToString() ?? "", null, null, null, null);
			if (!ELayer.debug.enable)
			{
				interactableWithAlpha = false;
			}
		}
		this.buttonUpload.SetInteractableWithAlpha(interactableWithAlpha);
	}

	public override void OnKill()
	{
		if (this.validId && this.validPass)
		{
			this.SaveID();
		}
	}

	public void SaveID()
	{
		if (ELayer._map.custom != null)
		{
			ELayer._map.custom.id = this.inputId.text;
		}
		this.ini.Global["pass"] = this.inputPassword.text;
		new FileIniDataParser().WriteFile(ModManager.PathIni, this.ini, null);
	}

	public void ExportMap()
	{
		ELayer._zone.Export(this.savePath, null, true);
		Msg.Say("net_mapSaved".lang(this.savePath, null, null, null, null));
		SE.WriteJournal();
	}

	public void Upload()
	{
		Debug.Log("aaaa");
		if (this.ini.Global["agreed"].IsEmpty())
		{
			string[] items = new string[]
			{
				"readTerms",
				"agree",
				"disagree"
			};
			Dialog.List<string>("dialogTermsOfUse".lang(), items, (string j) => j, delegate(int c, string d)
			{
				if (c == 0)
				{
					LayerHelp.Toggle("custom", "terms");
					return false;
				}
				if (c == 1)
				{
					this.ini.Global["agreed"] = "yes";
					this.Upload();
				}
				return true;
			}, true);
			return;
		}
		Debug.Log("Uploading map");
		string text = this.inputId.text;
		string text2 = this.inputPassword.text;
		this.SaveID();
		this.ExportMap();
		Net.UploadFile(text, text2, ELayer.pc.NameBraced, ELayer._zone.Name, this.savePath, Lang.langCode);
		LayerUploader.nextUpload = (int)Time.realtimeSinceStartup + this.limitSec;
	}

	public static int nextUpload;

	public static char[] InvalidChars = new char[]
	{
		'*',
		'&',
		'|',
		'#',
		'\\',
		'/',
		'?',
		'!',
		'"',
		'>',
		'<',
		':',
		';',
		'.',
		',',
		'~',
		'@',
		'^',
		'$',
		'%',
		' '
	};

	public InputField inputId;

	public InputField inputPassword;

	public IniData ini;

	public UIText textInvalidId;

	public UIText textInvalidPass;

	public UIText textNextUpload;

	public UIButton buttonUpload;

	public UIButton buttonSave;

	public int limitSec;

	public HashSet<string> invalidIds = new HashSet<string>();

	[NonSerialized]
	public bool validId;

	[NonSerialized]
	public bool validPass;
}
