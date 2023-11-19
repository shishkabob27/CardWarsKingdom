Shader "Hidden/Unlit/Premultiplied Colored 3" {
Properties {
 _MainTex ("Base (RGB), Alpha (A)", 2D) = "black" { }
}
SubShader { 
 LOD 200
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 55924
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesColor;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _ClipRange0;
					uniform highp vec4 _ClipRange1;
					uniform highp vec4 _ClipArgs1;
					uniform highp vec4 _ClipRange2;
					uniform highp vec4 _ClipArgs2;
					varying mediump vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					varying highp vec4 xlv_TEXCOORD1;
					varying highp vec2 xlv_TEXCOORD2;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  tmpvar_1.xy = ((_glesVertex.xy * _ClipRange0.zw) + _ClipRange0.xy);
					  highp vec2 ret_2;
					  ret_2.x = ((_glesVertex.x * _ClipArgs1.w) - (_glesVertex.y * _ClipArgs1.z));
					  ret_2.y = ((_glesVertex.x * _ClipArgs1.z) + (_glesVertex.y * _ClipArgs1.w));
					  tmpvar_1.zw = ((ret_2 * _ClipRange1.zw) + _ClipRange1.xy);
					  highp vec2 ret_3;
					  ret_3.x = ((_glesVertex.x * _ClipArgs2.w) - (_glesVertex.y * _ClipArgs2.z));
					  ret_3.y = ((_glesVertex.x * _ClipArgs2.z) + (_glesVertex.y * _ClipArgs2.w));
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_COLOR = _glesColor;
					  xlv_TEXCOORD0 = _glesMultiTexCoord0.xy;
					  xlv_TEXCOORD1 = tmpvar_1;
					  xlv_TEXCOORD2 = ((ret_3 * _ClipRange2.zw) + _ClipRange2.xy);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					uniform highp vec4 _ClipArgs0;
					uniform highp vec4 _ClipArgs1;
					uniform highp vec4 _ClipArgs2;
					varying mediump vec4 xlv_COLOR;
					varying highp vec2 xlv_TEXCOORD0;
					varying highp vec4 xlv_TEXCOORD1;
					varying highp vec2 xlv_TEXCOORD2;
					void main ()
					{
					  mediump vec4 col_1;
					  highp vec2 factor_2;
					  highp vec2 tmpvar_3;
					  tmpvar_3 = ((vec2(1.0, 1.0) - abs(xlv_TEXCOORD1.xy)) * _ClipArgs0.xy);
					  factor_2 = ((vec2(1.0, 1.0) - abs(xlv_TEXCOORD1.zw)) * _ClipArgs1.xy);
					  highp float tmpvar_4;
					  tmpvar_4 = min (min (tmpvar_3.x, tmpvar_3.y), min (factor_2.x, factor_2.y));
					  factor_2 = ((vec2(1.0, 1.0) - abs(xlv_TEXCOORD2)) * _ClipArgs2.xy);
					  lowp vec4 tmpvar_5;
					  tmpvar_5 = texture2D (_MainTex, xlv_TEXCOORD0);
					  mediump vec4 tmpvar_6;
					  tmpvar_6 = (tmpvar_5 * xlv_COLOR);
					  highp float tmpvar_7;
					  tmpvar_7 = clamp (min (tmpvar_4, min (factor_2.x, factor_2.y)), 0.0, 1.0);
					  col_1.w = (tmpvar_6.w * tmpvar_7);
					  highp vec3 tmpvar_8;
					  tmpvar_8 = (tmpvar_6.xyz * vec3(tmpvar_7));
					  col_1.xyz = tmpvar_8;
					  gl_FragData[0] = col_1;
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
					uniform 	vec4 _ClipRange0;
					uniform 	vec4 _ClipArgs0;
					uniform 	vec4 _ClipRange1;
					uniform 	vec4 _ClipArgs1;
					uniform 	vec4 _ClipRange2;
					uniform 	vec4 _ClipArgs2;
					in highp vec4 in_POSITION0;
					in mediump vec4 in_COLOR0;
					in highp vec2 in_TEXCOORD0;
					out mediump vec4 vs_COLOR0;
					out highp vec2 vs_TEXCOORD0;
					out highp vec2 vs_TEXCOORD2;
					out highp vec4 vs_TEXCOORD1;
					vec4 t0;
					vec2 t2;
					void main()
					{
					t0 = vec4(0.0);
					t2 = vec2(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    vs_COLOR0 = in_COLOR0;
					    t0.x = in_POSITION0.y * _ClipArgs2.z;
					    t2.x = in_POSITION0.x * _ClipArgs2.w + (-t0.x);
					    t2.y = dot(in_POSITION0.xy, _ClipArgs2.zw);
					    vs_TEXCOORD2.xy = t2.xy * _ClipRange2.zw + _ClipRange2.xy;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy;
					    t0.x = in_POSITION0.y * _ClipArgs1.z;
					    t2.x = in_POSITION0.x * _ClipArgs1.w + (-t0.x);
					    t2.y = dot(in_POSITION0.xy, _ClipArgs1.zw);
					    vs_TEXCOORD1.zw = t2.xy * _ClipRange1.zw + _ClipRange1.xy;
					    vs_TEXCOORD1.xy = in_POSITION0.xy * _ClipRange0.zw + _ClipRange0.xy;
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
					uniform 	vec4 _ClipRange0;
					uniform 	vec4 _ClipArgs0;
					uniform 	vec4 _ClipRange1;
					uniform 	vec4 _ClipArgs1;
					uniform 	vec4 _ClipRange2;
					uniform 	vec4 _ClipArgs2;
					uniform lowp sampler2D _MainTex;
					in mediump vec4 vs_COLOR0;
					in highp vec2 vs_TEXCOORD0;
					in highp vec2 vs_TEXCOORD2;
					in highp vec4 vs_TEXCOORD1;
					layout(location = 0) out mediump vec4 SV_Target0;
					vec4 t0;
					mediump vec4 t16_1;
					lowp vec4 t10_1;
					vec2 t2;
					void main()
					{
					t0 = vec4(0.0);
					t16_1 = vec4(0.0);
					t10_1 = vec4(0.0);
					t2 = vec2(0.0);
					    t0 = -abs(vs_TEXCOORD1) + vec4(1.0, 1.0, 1.0, 1.0);
					    t0.xy = t0.xy * _ClipArgs0.xy;
					    t0.zw = vec2(t0.z * _ClipArgs1.x, t0.w * _ClipArgs1.y);
					    t0.xz = min(t0.yw, t0.xz);
					    t0.x = min(t0.z, t0.x);
					    t2.xy = vec2(-abs(vs_TEXCOORD2.x) + float(1.0), -abs(vs_TEXCOORD2.y) + float(1.0));
					    t2.xy = t2.xy * _ClipArgs2.xy;
					    t2.x = min(t2.y, t2.x);
					    t0.x = min(t2.x, t0.x);
					#ifdef UNITY_ADRENO_ES3
					    t0.x = min(max(t0.x, 0.0), 1.0);
					#else
					    t0.x = clamp(t0.x, 0.0, 1.0);
					#endif
					    t10_1 = texture(_MainTex, vs_TEXCOORD0.xy);
					    t16_1 = t10_1 * vs_COLOR0;
					    t0 = t0.xxxx * t16_1;
					    SV_Target0 = t0;
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
 LOD 100
 Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
 Pass {
  Tags { "QUEUE"="Transparent" "IGNOREPROJECTOR"="true" "RenderType"="Transparent" }
  ZWrite Off
  Cull Off
  Blend One OneMinusSrcAlpha
  ColorMask RGB
  Offset -1, -1
  GpuProgramID 71517
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesColor;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp vec4 _MainTex_ST;
					varying lowp vec4 xlv_COLOR0;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  mediump vec4 tmpvar_2;
					  tmpvar_2 = clamp (_glesColor, 0.0, 1.0);
					  tmpvar_1 = tmpvar_2;
					  highp vec4 tmpvar_3;
					  tmpvar_3.w = 1.0;
					  tmpvar_3.xyz = _glesVertex.xyz;
					  xlv_COLOR0 = tmpvar_1;
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  gl_Position = (glstate_matrix_mvp * tmpvar_3);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform sampler2D _MainTex;
					varying lowp vec4 xlv_COLOR0;
					varying highp vec2 xlv_TEXCOORD0;
					void main ()
					{
					  lowp vec4 tmpvar_1;
					  tmpvar_1 = (texture2D (_MainTex, xlv_TEXCOORD0) * xlv_COLOR0);
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
					uniform 	vec4 _MainTex_ST;
					in highp vec3 in_POSITION0;
					in mediump vec4 in_COLOR0;
					in highp vec3 in_TEXCOORD0;
					out lowp vec4 vs_COLOR0;
					out highp vec2 vs_TEXCOORD0;
					vec4 t0;
					mediump vec4 t16_0;
					void main()
					{
					t0 = vec4(0.0);
					t16_0 = vec4(0.0);
					    t16_0 = in_COLOR0;
					#ifdef UNITY_ADRENO_ES3
					    t16_0 = min(max(t16_0, 0.0), 1.0);
					#else
					    t16_0 = clamp(t16_0, 0.0, 1.0);
					#endif
					    vs_COLOR0 = t16_0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = t0 + glstate_matrix_mvp[3];
					    return;
					}
					#endif
					#ifdef FRAGMENT
					#version 300 es
					precision highp float;
					precision highp int;
					uniform lowp sampler2D _MainTex;
					in lowp vec4 vs_COLOR0;
					in highp vec2 vs_TEXCOORD0;
					layout(location = 0) out lowp vec4 SV_Target0;
					lowp vec4 t10_0;
					void main()
					{
					t10_0 = vec4(0.0);
					    t10_0 = texture(_MainTex, vs_TEXCOORD0.xy);
					    SV_Target0 = t10_0 * vs_COLOR0;
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