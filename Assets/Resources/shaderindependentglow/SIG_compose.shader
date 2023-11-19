Shader "Hidden/SIG_Compose" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _GlowMask ("Glow mask", 2D) = "black" { }
 _BlurMask ("Blurred glow mask", 2D) = "black" { }
}
SubShader { 
 Pass {
  GpuProgramID 45416
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					varying highp vec2 xlv_TEXCOORD0;
					varying highp vec2 xlv_TEXCOORD1;
					void main ()
					{
					  mediump vec2 tmpvar_1;
					  tmpvar_1 = _glesMultiTexCoord0.xy;
					  highp vec2 tmpvar_2;
					  highp vec2 inUV_3;
					  inUV_3 = tmpvar_1;
					  highp vec4 tmpvar_4;
					  tmpvar_4.zw = vec2(0.0, 0.0);
					  tmpvar_4.xy = inUV_3;
					  tmpvar_2 = (mat4(1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0, 1.0) * tmpvar_4).xy;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = tmpvar_2;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					uniform sampler2D _GlowMask;
					uniform sampler2D _BlurMask;
					uniform lowp vec4 _GlobalTint;
					uniform lowp float _GlowPower;
					uniform lowp float _BlurPower;
					varying highp vec2 xlv_TEXCOORD0;
					varying highp vec2 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = clamp (((texture2D (_MainTex, xlv_TEXCOORD0) + 
					    ((texture2D (_GlowMask, xlv_TEXCOORD1) * _GlobalTint) * _GlowPower)
					  ) + (
					    (texture2D (_BlurMask, xlv_TEXCOORD1) * _GlobalTint)
					   * _BlurPower)), 0.0, 1.0);
					  gl_FragData[0] = tmpvar_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
"!!GLES3
					#ifdef VERTEX
					#version 300 es
					uniform 	vec4 _Time;
					uniform 	vec4 _SinTime;
					uniform 	vec4 _CosTime;
					uniform 	vec4 unity_DeltaTime;
					uniform 	vec3 _WorldSpaceCameraPos;
					uniform 	vec4 _ProjectionParams;
					uniform 	vec4 _ScreenParams;
					uniform 	vec4 _ZBufferParams;
					uniform 	vec4 unity_OrthoParams;
					uniform 	vec4 unity_CameraWorldClipPlanes[6];
					uniform 	mat4x4 unity_CameraProjection;
					uniform 	mat4x4 unity_CameraInvProjection;
					uniform 	vec4 _WorldSpaceLightPos0;
					uniform 	vec4 _LightPositionRange;
					uniform 	vec4 unity_4LightPosX0;
					uniform 	vec4 unity_4LightPosY0;
					uniform 	vec4 unity_4LightPosZ0;
					uniform 	mediump vec4 unity_4LightAtten0;
					uniform 	mediump vec4 unity_LightColor[8];
					uniform 	vec4 unity_LightPosition[8];
					uniform 	mediump vec4 unity_LightAtten[8];
					uniform 	vec4 unity_SpotDirection[8];
					uniform 	mediump vec4 unity_SHAr;
					uniform 	mediump vec4 unity_SHAg;
					uniform 	mediump vec4 unity_SHAb;
					uniform 	mediump vec4 unity_SHBr;
					uniform 	mediump vec4 unity_SHBg;
					uniform 	mediump vec4 unity_SHBb;
					uniform 	mediump vec4 unity_SHC;
					uniform 	mediump vec3 unity_LightColor0;
					uniform 	mediump vec3 unity_LightColor1;
					uniform 	mediump vec3 unity_LightColor2;
					uniform 	mediump vec3 unity_LightColor3;
					uniform 	vec4 unity_ShadowSplitSpheres[4];
					uniform 	vec4 unity_ShadowSplitSqRadii;
					uniform 	vec4 unity_LightShadowBias;
					uniform 	vec4 _LightSplitsNear;
					uniform 	vec4 _LightSplitsFar;
					uniform 	mat4x4 unity_World2Shadow[4];
					uniform 	mediump vec4 _LightShadowData;
					uniform 	vec4 unity_ShadowFadeCenterAndType;
					uniform 	mat4x4 glstate_matrix_mvp;
					uniform 	mat4x4 glstate_matrix_modelview0;
					uniform 	mat4x4 glstate_matrix_invtrans_modelview0;
					uniform 	mat4x4 _Object2World;
					uniform 	mat4x4 _World2Object;
					uniform 	vec4 unity_LODFade;
					uniform 	vec4 unity_WorldTransformParams;
					uniform 	mat4x4 glstate_matrix_transpose_modelview0;
					uniform 	mat4x4 glstate_matrix_projection;
					uniform 	lowp vec4 glstate_lightmodel_ambient;
					uniform 	mat4x4 unity_MatrixV;
					uniform 	mat4x4 unity_MatrixVP;
					uniform 	lowp vec4 unity_AmbientSky;
					uniform 	lowp vec4 unity_AmbientEquator;
					uniform 	lowp vec4 unity_AmbientGround;
					uniform 	lowp vec4 unity_FogColor;
					uniform 	vec4 unity_FogParams;
					uniform 	vec4 unity_LightmapST;
					uniform 	vec4 unity_DynamicLightmapST;
					uniform 	vec4 unity_SpecCube0_BoxMax;
					uniform 	vec4 unity_SpecCube0_BoxMin;
					uniform 	vec4 unity_SpecCube0_ProbePosition;
					uniform 	mediump vec4 unity_SpecCube0_HDR;
					uniform 	vec4 unity_SpecCube1_BoxMax;
					uniform 	vec4 unity_SpecCube1_BoxMin;
					uniform 	vec4 unity_SpecCube1_ProbePosition;
					uniform 	mediump vec4 unity_SpecCube1_HDR;
					uniform 	lowp vec4 unity_ColorSpaceGrey;
					uniform 	lowp vec4 unity_ColorSpaceDouble;
					uniform 	mediump vec4 unity_ColorSpaceDielectricSpec;
					uniform 	mediump vec4 unity_ColorSpaceLuminance;
					uniform 	mediump vec4 unity_Lightmap_HDR;
					uniform 	mediump vec4 unity_DynamicLightmap_HDR;
					uniform 	mediump vec4 _MainTex_TexelSize;
					uniform 	lowp vec4 _GlobalTint;
					uniform 	lowp float _GlowPower;
					uniform 	lowp float _BlurPower;
					in highp vec4 in_POSITION0;
					in mediump vec2 in_TEXCOORD0;
					out highp vec2 vs_TEXCOORD0;
					highp  vec4 phase0_Output0_1;
					out highp vec2 vs_TEXCOORD1;
					vec4 t0;
					void main()
					{
					t0 = vec4(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    phase0_Output0_1 = in_TEXCOORD0.xyxy;
					vs_TEXCOORD0 = phase0_Output0_1.xy;
					vs_TEXCOORD1 = phase0_Output0_1.zw;
					    return;
					}
					#endif
					#ifdef FRAGMENT
					#version 300 es
					precision highp float;
					precision highp int;
					uniform 	vec4 _Time;
					uniform 	vec4 _SinTime;
					uniform 	vec4 _CosTime;
					uniform 	vec4 unity_DeltaTime;
					uniform 	vec3 _WorldSpaceCameraPos;
					uniform 	vec4 _ProjectionParams;
					uniform 	vec4 _ScreenParams;
					uniform 	vec4 _ZBufferParams;
					uniform 	vec4 unity_OrthoParams;
					uniform 	vec4 unity_CameraWorldClipPlanes[6];
					uniform 	mat4x4 unity_CameraProjection;
					uniform 	mat4x4 unity_CameraInvProjection;
					uniform 	vec4 _WorldSpaceLightPos0;
					uniform 	vec4 _LightPositionRange;
					uniform 	vec4 unity_4LightPosX0;
					uniform 	vec4 unity_4LightPosY0;
					uniform 	vec4 unity_4LightPosZ0;
					uniform 	mediump vec4 unity_4LightAtten0;
					uniform 	mediump vec4 unity_LightColor[8];
					uniform 	vec4 unity_LightPosition[8];
					uniform 	mediump vec4 unity_LightAtten[8];
					uniform 	vec4 unity_SpotDirection[8];
					uniform 	mediump vec4 unity_SHAr;
					uniform 	mediump vec4 unity_SHAg;
					uniform 	mediump vec4 unity_SHAb;
					uniform 	mediump vec4 unity_SHBr;
					uniform 	mediump vec4 unity_SHBg;
					uniform 	mediump vec4 unity_SHBb;
					uniform 	mediump vec4 unity_SHC;
					uniform 	mediump vec3 unity_LightColor0;
					uniform 	mediump vec3 unity_LightColor1;
					uniform 	mediump vec3 unity_LightColor2;
					uniform 	mediump vec3 unity_LightColor3;
					uniform 	vec4 unity_ShadowSplitSpheres[4];
					uniform 	vec4 unity_ShadowSplitSqRadii;
					uniform 	vec4 unity_LightShadowBias;
					uniform 	vec4 _LightSplitsNear;
					uniform 	vec4 _LightSplitsFar;
					uniform 	mat4x4 unity_World2Shadow[4];
					uniform 	mediump vec4 _LightShadowData;
					uniform 	vec4 unity_ShadowFadeCenterAndType;
					uniform 	mat4x4 glstate_matrix_mvp;
					uniform 	mat4x4 glstate_matrix_modelview0;
					uniform 	mat4x4 glstate_matrix_invtrans_modelview0;
					uniform 	mat4x4 _Object2World;
					uniform 	mat4x4 _World2Object;
					uniform 	vec4 unity_LODFade;
					uniform 	vec4 unity_WorldTransformParams;
					uniform 	mat4x4 glstate_matrix_transpose_modelview0;
					uniform 	mat4x4 glstate_matrix_projection;
					uniform 	lowp vec4 glstate_lightmodel_ambient;
					uniform 	mat4x4 unity_MatrixV;
					uniform 	mat4x4 unity_MatrixVP;
					uniform 	lowp vec4 unity_AmbientSky;
					uniform 	lowp vec4 unity_AmbientEquator;
					uniform 	lowp vec4 unity_AmbientGround;
					uniform 	lowp vec4 unity_FogColor;
					uniform 	vec4 unity_FogParams;
					uniform 	vec4 unity_LightmapST;
					uniform 	vec4 unity_DynamicLightmapST;
					uniform 	vec4 unity_SpecCube0_BoxMax;
					uniform 	vec4 unity_SpecCube0_BoxMin;
					uniform 	vec4 unity_SpecCube0_ProbePosition;
					uniform 	mediump vec4 unity_SpecCube0_HDR;
					uniform 	vec4 unity_SpecCube1_BoxMax;
					uniform 	vec4 unity_SpecCube1_BoxMin;
					uniform 	vec4 unity_SpecCube1_ProbePosition;
					uniform 	mediump vec4 unity_SpecCube1_HDR;
					uniform 	lowp vec4 unity_ColorSpaceGrey;
					uniform 	lowp vec4 unity_ColorSpaceDouble;
					uniform 	mediump vec4 unity_ColorSpaceDielectricSpec;
					uniform 	mediump vec4 unity_ColorSpaceLuminance;
					uniform 	mediump vec4 unity_Lightmap_HDR;
					uniform 	mediump vec4 unity_DynamicLightmap_HDR;
					uniform 	mediump vec4 _MainTex_TexelSize;
					uniform 	lowp vec4 _GlobalTint;
					uniform 	lowp float _GlowPower;
					uniform 	lowp float _BlurPower;
					uniform lowp sampler2D _MainTex;
					uniform lowp sampler2D _GlowMask;
					uniform lowp sampler2D _BlurMask;
					in highp vec2 vs_TEXCOORD0;
					in highp vec2 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					lowp vec4 t10_0;
					lowp vec4 t10_1;
					void main()
					{
					t10_0 = vec4(0.0);
					t10_1 = vec4(0.0);
					    t10_0 = texture(_GlowMask, vs_TEXCOORD1.xy);
					    t10_0 = t10_0 * _GlobalTint;
					    t10_1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    t10_0 = t10_0 * vec4(_GlowPower) + t10_1;
					    t10_1 = texture(_BlurMask, vs_TEXCOORD1.xy);
					    t10_1 = t10_1 * _GlobalTint;
					    SV_Target0 = t10_1 * vec4(vec4(_BlurPower, _BlurPower, _BlurPower, _BlurPower)) + t10_0;
					#ifdef UNITY_ADRENO_ES3
					    SV_Target0 = min(max(SV_Target0, 0.0), 1.0);
					#else
					    SV_Target0 = clamp(SV_Target0, 0.0, 1.0);
					#endif
					    return;
					}
					#endif"
}
}
Program "fp" {
SubProgram "gles " {
"!!GLES"
}
SubProgram "gles3 " {
"!!GLES3"
}
}
 }
}
}