using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace MoharGfx;

public class Graphic_CutoutCollection : Graphic
{
	protected Graphic[] subGraphics;

	public override void TryInsertIntoAtlas(TextureAtlasGroup groupKey)
	{
		Graphic[] array = subGraphics;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].TryInsertIntoAtlas(groupKey);
		}
	}

	public override void Init(GraphicRequest req)
	{
		data = req.graphicData;
		if (req.path.NullOrEmpty())
		{
			throw new ArgumentNullException("folderPath");
		}
		if (req.shader == null)
		{
			throw new ArgumentNullException("shader");
		}
		path = req.path;
		maskPath = req.maskPath;
		color = req.color;
		colorTwo = req.colorTwo;
		drawSize = req.drawSize;
		List<(Texture2D, string)> list = (from x in ContentFinder<Texture2D>.GetAllInFolder(req.path)
			where !x.name.EndsWith(Graphic_Single.MaskSuffix)
			orderby x.name
			select (x, x.name.Split('_')[0])).ToList();
		if (list.NullOrEmpty())
		{
			Log.Error("Graphic_CutoutCollection - Collection cannot init: No textures found at path " + req.path);
			subGraphics = new Graphic[1] { BaseContent.BadGraphic };
			return;
		}
		List<Graphic> list2 = new List<Graphic>();
		foreach (IGrouping<string, (Texture2D, string)> item in from s in list
			group s by s.Item2)
		{
			List<(Texture2D, string)> list3 = item.ToList();
			string text = req.path + "/" + item.Key;
			string itemPath = text + Graphic_Single.MaskSuffix;
			MaterialRequest materialRequest = default(MaterialRequest);
			materialRequest.mainTex = req.texture ?? ContentFinder<Texture2D>.Get(text);
			materialRequest.shader = req.shader;
			materialRequest.color = color;
			materialRequest.colorTwo = colorTwo;
			materialRequest.renderQueue = req.renderQueue;
			materialRequest.shaderParameters = req.shaderParameters;
			if (req.shader.SupportsMaskTex())
			{
				materialRequest.maskTex = ContentFinder<Texture2D>.Get(itemPath);
			}
			if (list3.Count <= 0)
			{
				continue;
			}
			foreach (var item2 in list3)
			{
				list2.Add(GraphicDatabase.Get(typeof(Graphic_Single), text, req.shader, drawSize, color, colorTwo, data, req.shaderParameters, itemPath));
			}
		}
		subGraphics = list2.ToArray();
	}
}
