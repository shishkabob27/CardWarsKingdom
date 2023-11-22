Shader "KFF/Alpha With Fade"
{
	Properties
	{
		_Color ("Color Tint", Color) = (1,1,1,1)
		_MainTex ("Base (RGB) Trans (A)", 2D) = "black" {}
	}
	
	SubShader
	{
		LOD 200

		Tags
		{
			"Queue" = "Transparent"
		}
		
		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			AlphaTest Greater .01
			Blend SrcAlpha OneMinusSrcAlpha

			ColorMaterial AmbientAndDiffuse
			
			SetTexture [_MainTex]
			{
					constantColor [_Color]
					Combine texture * constant, texture * constant
			}
		}
	}
}