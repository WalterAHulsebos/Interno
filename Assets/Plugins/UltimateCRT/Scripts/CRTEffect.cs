﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Image Effects/CRT/Ultimate CRT")]
public class CRTEffect : BaseCRTEffect {
	void OnRenderImage(RenderTexture src, RenderTexture dest) {
		ProcessEffect(src, dest);
	}
}
