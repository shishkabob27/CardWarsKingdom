Shader "Toon/Basic Outline" {
Properties {
 _Color ("Main Color", Color) = (0.5,0.5,0.5,1)
 _OutlineColor ("Outline Color", Color) = (0,0,0,1)
 _Outline ("Outline width", Range(0.002,0.03)) = 0.005
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _ToonShade ("ToonShader Cubemap(RGB)", CUBE) = "" { }
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 UsePass "Toon/Basic/BASE"
 Pass {
  Name "OUTLINE"
  Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
  Cull Front
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 9625
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 glstate_matrix_invtrans_modelview0;
					uniform highp mat4 glstate_matrix_projection;
					uniform highp float _Outline;
					uniform highp vec4 _OutlineColor;
					varying highp vec4 xlv_COLOR;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  highp mat3 tmpvar_2;
					  tmpvar_2[0] = glstate_matrix_invtrans_modelview0[0].xyz;
					  tmpvar_2[1] = glstate_matrix_invtrans_modelview0[1].xyz;
					  tmpvar_2[2] = glstate_matrix_invtrans_modelview0[2].xyz;
					  highp mat2 tmpvar_3;
					  tmpvar_3[0] = glstate_matrix_projection[0].xy;
					  tmpvar_3[1] = glstate_matrix_projection[1].xy;
					  tmpvar_1.xy = (tmpvar_1.xy + ((
					    (tmpvar_3 * (tmpvar_2 * _glesNormal).xy)
					   * tmpvar_1.z) * _Outline));
					  gl_Position = tmpvar_1;
					  xlv_COLOR = _OutlineColor;
					}
					
					
					#endif
					#ifdef FRAGMENT
					varying highp vec4 xlv_COLOR;
					void main ()
					{
					  mediump vec4 tmpvar_1;
					  tmpvar_1 = xlv_COLOR;
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
					uniform 	float _Outline;
					uniform 	vec4 _OutlineColor;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out highp vec4 vs_COLOR0;
					vec2 t0;
					vec4 t1;
					vec2 t2;
					void main()
					{
					t0 = vec2(0.0);
					t1 = vec4(0.0);
					t2 = vec2(0.0);
					    t0.xy = in_NORMAL0.yy * glstate_matrix_invtrans_modelview0[1].xy;
					    t0.xy = glstate_matrix_invtrans_modelview0[0].xy * in_NORMAL0.xx + t0.xy;
					    t0.xy = glstate_matrix_invtrans_modelview0[2].xy * in_NORMAL0.zz + t0.xy;
					    t2.xy = t0.yy * glstate_matrix_projection[1].xy;
					    t0.xy = glstate_matrix_projection[0].xy * t0.xx + t2.xy;
					    t1 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t1 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t1;
					    t1 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t1;
					    t1 = glstate_matrix_mvp[3] * in_POSITION0.wwww + t1;
					    t0.xy = t0.xy * t1.zz;
					    gl_Position.xy = t0.xy * vec2(_Outline) + t1.xy;
					    gl_Position.zw = t1.zw;
					    vs_COLOR0 = _OutlineColor;
					    return;
					}
					#endif
					#ifdef FRAGMENT
					#version 300 es
					precision highp float;
					precision highp int;
					in highp vec4 vs_COLOR0;
					layout(location = 0) out mediump vec4 SV_Target0;
					void main()
					{
					    SV_Target0 = vs_COLOR0;
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
SubShader { 
 Tags { "RenderType"="Opaque" }
 UsePass "Toon/Basic/BASE"
 Pass {
  Name "OUTLINE"
  Tags { "LIGHTMODE"="Always" "RenderType"="Opaque" }
  Cull Front
  Blend SrcAlpha OneMinusSrcAlpha
  ColorMask RGB
  GpuProgramID 588559
Program "vp" {
}
 }
}
Fallback "Toon/Basic"
}