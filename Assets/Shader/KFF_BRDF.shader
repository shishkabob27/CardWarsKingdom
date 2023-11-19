Shader "KFF/BRDF" {
Properties {
 _MainTex ("Base (RGB)", 2D) = "white" { }
 _Ramp2D ("BRDF Ramp", 2D) = "gray" { }
}
SubShader { 
 LOD 200
 Tags { "RenderType"="Opaque" }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardBase" "SHADOWSUPPORT"="true" "RenderType"="Opaque" }
  GpuProgramID 26331
Program "vp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].x;
					  v_5.y = _World2Object[1].x;
					  v_5.z = _World2Object[2].x;
					  v_5.w = _World2Object[3].x;
					  highp vec4 v_6;
					  v_6.x = _World2Object[0].y;
					  v_6.y = _World2Object[1].y;
					  v_6.z = _World2Object[2].y;
					  v_6.w = _World2Object[3].y;
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].z;
					  v_7.y = _World2Object[1].z;
					  v_7.z = _World2Object[2].z;
					  v_7.w = _World2Object[3].z;
					  highp vec3 tmpvar_8;
					  tmpvar_8 = normalize(((
					    (v_5.xyz * _glesNormal.x)
					   + 
					    (v_6.xyz * _glesNormal.y)
					  ) + (v_7.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_8;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_9;
					  tmpvar_9.w = 1.0;
					  tmpvar_9.xyz = worldNormal_2;
					  mediump vec4 normal_10;
					  normal_10 = tmpvar_9;
					  mediump vec3 res_11;
					  mediump vec3 x_12;
					  x_12.x = dot (unity_SHAr, normal_10);
					  x_12.y = dot (unity_SHAg, normal_10);
					  x_12.z = dot (unity_SHAb, normal_10);
					  mediump vec3 x1_13;
					  mediump vec4 tmpvar_14;
					  tmpvar_14 = (normal_10.xyzz * normal_10.yzzx);
					  x1_13.x = dot (unity_SHBr, tmpvar_14);
					  x1_13.y = dot (unity_SHBg, tmpvar_14);
					  x1_13.z = dot (unity_SHBb, tmpvar_14);
					  res_11 = (x_12 + (x1_13 + (unity_SHC.xyz * 
					    ((normal_10.x * normal_10.x) - (normal_10.y * normal_10.y))
					  )));
					  res_11 = max (((1.055 * 
					    pow (max (res_11, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_11;
					  tmpvar_4 = shlight_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = (_Object2World * _glesVertex).xyz;
					  xlv_TEXCOORD3 = tmpvar_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_11;
					  mediump vec3 LightDir_12;
					  LightDir_12 = lightDir_4;
					  mediump vec3 viewDir_13;
					  viewDir_13 = worldViewDir_3;
					  highp vec4 c_14;
					  highp vec3 BRDF_15;
					  highp float NdotE_16;
					  highp float NdotL_17;
					  mediump float tmpvar_18;
					  tmpvar_18 = dot (tmpvar_2, LightDir_12);
					  NdotL_17 = tmpvar_18;
					  mediump float tmpvar_19;
					  tmpvar_19 = dot (tmpvar_2, viewDir_13);
					  NdotE_16 = tmpvar_19;
					  highp vec2 tmpvar_20;
					  tmpvar_20.x = (NdotE_16 * 0.8);
					  tmpvar_20.y = ((NdotL_17 * 0.5) + 0.5);
					  lowp vec3 tmpvar_21;
					  tmpvar_21 = texture2D (_Ramp2D, tmpvar_20).xyz;
					  BRDF_15 = tmpvar_21;
					  c_14.xyz = (BRDF_15 * 0.5);
					  c_14.w = tmpvar_8;
					  tmpvar_11 = c_14;
					  c_1.xyz = (c_1 + tmpvar_11).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_TEXCOORD0;
					out highp vec2 vs_TEXCOORD0;
					out mediump vec3 vs_TEXCOORD1;
					out highp vec3 vs_TEXCOORD2;
					out lowp vec3 vs_TEXCOORD3;
					vec4 t0;
					vec3 t1;
					mediump vec4 t16_1;
					mediump vec3 t16_2;
					mediump vec3 t16_3;
					float t12;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t16_1 = vec4(0.0);
					t16_2 = vec3(0.0);
					t16_3 = vec3(0.0);
					t12 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t12 = dot(t0.xyz, t0.xyz);
					    t12 = inversesqrt(t12);
					    t0.xyz = vec3(t12) * t0.xyz;
					    vs_TEXCOORD1.xyz = t0.xyz;
					    t1.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t1.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t1.xyz;
					    t1.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t1.xyz;
					    vs_TEXCOORD2.xyz = _Object2World[3].xyz * in_POSITION0.www + t1.xyz;
					    t16_2.x = t0.y * t0.y;
					    t16_2.x = t0.x * t0.x + (-t16_2.x);
					    t16_1 = t0.yzzx * t0.xyzz;
					    t16_3.x = dot(unity_SHBr, t16_1);
					    t16_3.y = dot(unity_SHBg, t16_1);
					    t16_3.z = dot(unity_SHBb, t16_1);
					    t16_2.xyz = unity_SHC.xyz * t16_2.xxx + t16_3.xyz;
					    t0.w = 1.0;
					    t16_3.x = dot(unity_SHAr, t0);
					    t16_3.y = dot(unity_SHAg, t0);
					    t16_3.z = dot(unity_SHAb, t0);
					    t16_2.xyz = t16_2.xyz + t16_3.xyz;
					    t16_2.xyz = max(t16_2.xyz, vec3(0.0, 0.0, 0.0));
					    t0.xyz = log2(t16_2.xyz);
					    t0.xyz = t0.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    t0.xyz = exp2(t0.xyz);
					    t0.xyz = t0.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    t0.xyz = max(t0.xyz, vec3(0.0, 0.0, 0.0));
					    vs_TEXCOORD3.xyz = t0.xyz;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					uniform lowp sampler2D _MainTex;
					uniform lowp sampler2D _Ramp2D;
					in highp vec2 vs_TEXCOORD0;
					in mediump vec3 vs_TEXCOORD1;
					in highp vec3 vs_TEXCOORD2;
					in lowp vec3 vs_TEXCOORD3;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					lowp vec3 t10_2;
					float t9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t10_2 = vec3(0.0);
					t9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD2.xyz) + _WorldSpaceCameraPos.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, t0.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_0.xyz = t10_0.xyz * vec3(0.5, 0.5, 0.5);
					    t10_2.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
					    t16_1.xyz = t10_2.xyz * vs_TEXCOORD3.xyz + t16_0.xyz;
					    SV_Target0.xyz = t16_1.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 unity_World2Shadow[4];
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  mediump vec4 tmpvar_5;
					  highp vec4 v_6;
					  v_6.x = _World2Object[0].x;
					  v_6.y = _World2Object[1].x;
					  v_6.z = _World2Object[2].x;
					  v_6.w = _World2Object[3].x;
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].y;
					  v_7.y = _World2Object[1].y;
					  v_7.z = _World2Object[2].y;
					  v_7.w = _World2Object[3].y;
					  highp vec4 v_8;
					  v_8.x = _World2Object[0].z;
					  v_8.y = _World2Object[1].z;
					  v_8.z = _World2Object[2].z;
					  v_8.w = _World2Object[3].z;
					  highp vec3 tmpvar_9;
					  tmpvar_9 = normalize(((
					    (v_6.xyz * _glesNormal.x)
					   + 
					    (v_7.xyz * _glesNormal.y)
					  ) + (v_8.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_9;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_10;
					  tmpvar_10.w = 1.0;
					  tmpvar_10.xyz = worldNormal_2;
					  mediump vec4 normal_11;
					  normal_11 = tmpvar_10;
					  mediump vec3 res_12;
					  mediump vec3 x_13;
					  x_13.x = dot (unity_SHAr, normal_11);
					  x_13.y = dot (unity_SHAg, normal_11);
					  x_13.z = dot (unity_SHAb, normal_11);
					  mediump vec3 x1_14;
					  mediump vec4 tmpvar_15;
					  tmpvar_15 = (normal_11.xyzz * normal_11.yzzx);
					  x1_14.x = dot (unity_SHBr, tmpvar_15);
					  x1_14.y = dot (unity_SHBg, tmpvar_15);
					  x1_14.z = dot (unity_SHBb, tmpvar_15);
					  res_12 = (x_13 + (x1_14 + (unity_SHC.xyz * 
					    ((normal_11.x * normal_11.x) - (normal_11.y * normal_11.y))
					  )));
					  res_12 = max (((1.055 * 
					    pow (max (res_12, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_12;
					  tmpvar_4 = shlight_1;
					  highp vec4 tmpvar_16;
					  tmpvar_16 = (_Object2World * _glesVertex);
					  tmpvar_5 = (unity_World2Shadow[0] * tmpvar_16);
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = tmpvar_16.xyz;
					  xlv_TEXCOORD3 = tmpvar_4;
					  xlv_TEXCOORD4 = tmpvar_5;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform mediump vec4 _LightShadowData;
					uniform highp sampler2D _ShadowMapTexture;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  lowp float tmpvar_11;
					  highp float tmpvar_12;
					  tmpvar_12 = max (float((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD4).x > 
					    (xlv_TEXCOORD4.z / xlv_TEXCOORD4.w)
					  )), _LightShadowData.x);
					  tmpvar_11 = tmpvar_12;
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_13;
					  mediump vec3 LightDir_14;
					  LightDir_14 = lightDir_4;
					  mediump vec3 viewDir_15;
					  viewDir_15 = worldViewDir_3;
					  mediump float atten_16;
					  atten_16 = tmpvar_11;
					  highp vec4 c_17;
					  highp vec3 BRDF_18;
					  highp float NdotE_19;
					  highp float NdotL_20;
					  mediump float tmpvar_21;
					  tmpvar_21 = dot (tmpvar_2, LightDir_14);
					  NdotL_20 = tmpvar_21;
					  mediump float tmpvar_22;
					  tmpvar_22 = dot (tmpvar_2, viewDir_15);
					  NdotE_19 = tmpvar_22;
					  highp vec2 tmpvar_23;
					  tmpvar_23.x = (NdotE_19 * 0.8);
					  tmpvar_23.y = ((NdotL_20 * 0.5) + 0.5);
					  lowp vec3 tmpvar_24;
					  tmpvar_24 = texture2D (_Ramp2D, tmpvar_23).xyz;
					  BRDF_18 = tmpvar_24;
					  c_17.xyz = ((BRDF_18 * atten_16) * 0.5);
					  c_17.w = tmpvar_8;
					  tmpvar_13 = c_17;
					  c_1.xyz = (c_1 + tmpvar_13).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec4 unity_4LightPosX0;
					uniform highp vec4 unity_4LightPosY0;
					uniform highp vec4 unity_4LightPosZ0;
					uniform mediump vec4 unity_4LightAtten0;
					uniform mediump vec4 unity_LightColor[8];
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  highp vec3 tmpvar_5;
					  tmpvar_5 = (_Object2World * _glesVertex).xyz;
					  highp vec4 v_6;
					  v_6.x = _World2Object[0].x;
					  v_6.y = _World2Object[1].x;
					  v_6.z = _World2Object[2].x;
					  v_6.w = _World2Object[3].x;
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].y;
					  v_7.y = _World2Object[1].y;
					  v_7.z = _World2Object[2].y;
					  v_7.w = _World2Object[3].y;
					  highp vec4 v_8;
					  v_8.x = _World2Object[0].z;
					  v_8.y = _World2Object[1].z;
					  v_8.z = _World2Object[2].z;
					  v_8.w = _World2Object[3].z;
					  highp vec3 tmpvar_9;
					  tmpvar_9 = normalize(((
					    (v_6.xyz * _glesNormal.x)
					   + 
					    (v_7.xyz * _glesNormal.y)
					  ) + (v_8.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_9;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_10;
					  tmpvar_10.w = 1.0;
					  tmpvar_10.xyz = worldNormal_2;
					  mediump vec4 normal_11;
					  normal_11 = tmpvar_10;
					  mediump vec3 res_12;
					  mediump vec3 x_13;
					  x_13.x = dot (unity_SHAr, normal_11);
					  x_13.y = dot (unity_SHAg, normal_11);
					  x_13.z = dot (unity_SHAb, normal_11);
					  mediump vec3 x1_14;
					  mediump vec4 tmpvar_15;
					  tmpvar_15 = (normal_11.xyzz * normal_11.yzzx);
					  x1_14.x = dot (unity_SHBr, tmpvar_15);
					  x1_14.y = dot (unity_SHBg, tmpvar_15);
					  x1_14.z = dot (unity_SHBb, tmpvar_15);
					  res_12 = (x_13 + (x1_14 + (unity_SHC.xyz * 
					    ((normal_11.x * normal_11.x) - (normal_11.y * normal_11.y))
					  )));
					  res_12 = max (((1.055 * 
					    pow (max (res_12, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_12;
					  tmpvar_4 = shlight_1;
					  highp vec3 lightColor0_16;
					  lightColor0_16 = unity_LightColor[0].xyz;
					  highp vec3 lightColor1_17;
					  lightColor1_17 = unity_LightColor[1].xyz;
					  highp vec3 lightColor2_18;
					  lightColor2_18 = unity_LightColor[2].xyz;
					  highp vec3 lightColor3_19;
					  lightColor3_19 = unity_LightColor[3].xyz;
					  highp vec4 lightAttenSq_20;
					  lightAttenSq_20 = unity_4LightAtten0;
					  highp vec3 normal_21;
					  normal_21 = worldNormal_2;
					  highp vec3 col_22;
					  highp vec4 ndotl_23;
					  highp vec4 lengthSq_24;
					  highp vec4 tmpvar_25;
					  tmpvar_25 = (unity_4LightPosX0 - tmpvar_5.x);
					  highp vec4 tmpvar_26;
					  tmpvar_26 = (unity_4LightPosY0 - tmpvar_5.y);
					  highp vec4 tmpvar_27;
					  tmpvar_27 = (unity_4LightPosZ0 - tmpvar_5.z);
					  lengthSq_24 = (tmpvar_25 * tmpvar_25);
					  lengthSq_24 = (lengthSq_24 + (tmpvar_26 * tmpvar_26));
					  lengthSq_24 = (lengthSq_24 + (tmpvar_27 * tmpvar_27));
					  ndotl_23 = (tmpvar_25 * normal_21.x);
					  ndotl_23 = (ndotl_23 + (tmpvar_26 * normal_21.y));
					  ndotl_23 = (ndotl_23 + (tmpvar_27 * normal_21.z));
					  highp vec4 tmpvar_28;
					  tmpvar_28 = max (vec4(0.0, 0.0, 0.0, 0.0), (ndotl_23 * inversesqrt(lengthSq_24)));
					  ndotl_23 = tmpvar_28;
					  highp vec4 tmpvar_29;
					  tmpvar_29 = (tmpvar_28 * (1.0/((1.0 + 
					    (lengthSq_24 * lightAttenSq_20)
					  ))));
					  col_22 = (lightColor0_16 * tmpvar_29.x);
					  col_22 = (col_22 + (lightColor1_17 * tmpvar_29.y));
					  col_22 = (col_22 + (lightColor2_18 * tmpvar_29.z));
					  col_22 = (col_22 + (lightColor3_19 * tmpvar_29.w));
					  tmpvar_4 = (tmpvar_4 + col_22);
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = tmpvar_5;
					  xlv_TEXCOORD3 = tmpvar_4;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_11;
					  mediump vec3 LightDir_12;
					  LightDir_12 = lightDir_4;
					  mediump vec3 viewDir_13;
					  viewDir_13 = worldViewDir_3;
					  highp vec4 c_14;
					  highp vec3 BRDF_15;
					  highp float NdotE_16;
					  highp float NdotL_17;
					  mediump float tmpvar_18;
					  tmpvar_18 = dot (tmpvar_2, LightDir_12);
					  NdotL_17 = tmpvar_18;
					  mediump float tmpvar_19;
					  tmpvar_19 = dot (tmpvar_2, viewDir_13);
					  NdotE_16 = tmpvar_19;
					  highp vec2 tmpvar_20;
					  tmpvar_20.x = (NdotE_16 * 0.8);
					  tmpvar_20.y = ((NdotL_17 * 0.5) + 0.5);
					  lowp vec3 tmpvar_21;
					  tmpvar_21 = texture2D (_Ramp2D, tmpvar_20).xyz;
					  BRDF_15 = tmpvar_21;
					  c_14.xyz = (BRDF_15 * 0.5);
					  c_14.w = tmpvar_8;
					  tmpvar_11 = c_14;
					  c_1.xyz = (c_1 + tmpvar_11).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_TEXCOORD0;
					out highp vec2 vs_TEXCOORD0;
					out mediump vec3 vs_TEXCOORD1;
					out highp vec3 vs_TEXCOORD2;
					out lowp vec3 vs_TEXCOORD3;
					vec4 t0;
					vec4 t1;
					vec4 t2;
					mediump vec3 t16_2;
					vec4 t3;
					mediump vec4 t16_3;
					vec4 t4;
					mediump vec3 t16_4;
					vec3 t5;
					float t18;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec4(0.0);
					t2 = vec4(0.0);
					t16_2 = vec3(0.0);
					t3 = vec4(0.0);
					t16_3 = vec4(0.0);
					t4 = vec4(0.0);
					t16_4 = vec3(0.0);
					t5 = vec3(0.0);
					t18 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t18 = dot(t0.xyz, t0.xyz);
					    t18 = inversesqrt(t18);
					    t0.xyz = vec3(t18) * t0.xyz;
					    vs_TEXCOORD1.xyz = t0.xyz;
					    t1.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t1.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t1.xyz;
					    t1.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t1.xyz;
					    t1.xyz = _Object2World[3].xyz * in_POSITION0.www + t1.xyz;
					    vs_TEXCOORD2.xyz = t1.xyz;
					    t16_2.x = t0.y * t0.y;
					    t16_2.x = t0.x * t0.x + (-t16_2.x);
					    t16_3 = t0.yzzx * t0.xyzz;
					    t16_4.x = dot(unity_SHBr, t16_3);
					    t16_4.y = dot(unity_SHBg, t16_3);
					    t16_4.z = dot(unity_SHBb, t16_3);
					    t16_2.xyz = unity_SHC.xyz * t16_2.xxx + t16_4.xyz;
					    t0.w = 1.0;
					    t16_3.x = dot(unity_SHAr, t0);
					    t16_3.y = dot(unity_SHAg, t0);
					    t16_3.z = dot(unity_SHAb, t0);
					    t16_2.xyz = t16_2.xyz + t16_3.xyz;
					    t16_2.xyz = max(t16_2.xyz, vec3(0.0, 0.0, 0.0));
					    t5.xyz = log2(t16_2.xyz);
					    t5.xyz = t5.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    t5.xyz = exp2(t5.xyz);
					    t5.xyz = t5.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    t5.xyz = max(t5.xyz, vec3(0.0, 0.0, 0.0));
					    t2 = (-t1.yyyy) + unity_4LightPosY0;
					    t3 = t0.yyyy * t2;
					    t2 = t2 * t2;
					    t4 = (-t1.xxxx) + unity_4LightPosX0;
					    t1 = (-t1.zzzz) + unity_4LightPosZ0;
					    t3 = t4 * t0.xxxx + t3;
					    t0 = t1 * t0.zzzz + t3;
					    t2 = t4 * t4 + t2;
					    t1 = t1 * t1 + t2;
					    t2 = inversesqrt(t1);
					    t1 = t1 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    t1 = vec4(1.0, 1.0, 1.0, 1.0) / t1;
					    t0 = t0 * t2;
					    t0 = max(t0, vec4(0.0, 0.0, 0.0, 0.0));
					    t0 = t1 * t0;
					    t1.xyz = t0.yyy * unity_LightColor[1].xyz;
					    t1.xyz = unity_LightColor[0].xyz * t0.xxx + t1.xyz;
					    t0.xyz = unity_LightColor[2].xyz * t0.zzz + t1.xyz;
					    t0.xyz = unity_LightColor[3].xyz * t0.www + t0.xyz;
					    t0.xyz = t0.xyz + t5.xyz;
					    vs_TEXCOORD3.xyz = t0.xyz;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					uniform lowp sampler2D _MainTex;
					uniform lowp sampler2D _Ramp2D;
					in highp vec2 vs_TEXCOORD0;
					in mediump vec3 vs_TEXCOORD1;
					in highp vec3 vs_TEXCOORD2;
					in lowp vec3 vs_TEXCOORD3;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					lowp vec3 t10_2;
					float t9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t10_2 = vec3(0.0);
					t9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD2.xyz) + _WorldSpaceCameraPos.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, t0.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_0.xyz = t10_0.xyz * vec3(0.5, 0.5, 0.5);
					    t10_2.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
					    t16_1.xyz = t10_2.xyz * vs_TEXCOORD3.xyz + t16_0.xyz;
					    SV_Target0.xyz = t16_1.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec4 unity_4LightPosX0;
					uniform highp vec4 unity_4LightPosY0;
					uniform highp vec4 unity_4LightPosZ0;
					uniform mediump vec4 unity_4LightAtten0;
					uniform mediump vec4 unity_LightColor[8];
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 unity_World2Shadow[4];
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  mediump vec4 tmpvar_5;
					  highp vec4 tmpvar_6;
					  tmpvar_6 = (_Object2World * _glesVertex);
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].x;
					  v_7.y = _World2Object[1].x;
					  v_7.z = _World2Object[2].x;
					  v_7.w = _World2Object[3].x;
					  highp vec4 v_8;
					  v_8.x = _World2Object[0].y;
					  v_8.y = _World2Object[1].y;
					  v_8.z = _World2Object[2].y;
					  v_8.w = _World2Object[3].y;
					  highp vec4 v_9;
					  v_9.x = _World2Object[0].z;
					  v_9.y = _World2Object[1].z;
					  v_9.z = _World2Object[2].z;
					  v_9.w = _World2Object[3].z;
					  highp vec3 tmpvar_10;
					  tmpvar_10 = normalize(((
					    (v_7.xyz * _glesNormal.x)
					   + 
					    (v_8.xyz * _glesNormal.y)
					  ) + (v_9.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_10;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_11;
					  tmpvar_11.w = 1.0;
					  tmpvar_11.xyz = worldNormal_2;
					  mediump vec4 normal_12;
					  normal_12 = tmpvar_11;
					  mediump vec3 res_13;
					  mediump vec3 x_14;
					  x_14.x = dot (unity_SHAr, normal_12);
					  x_14.y = dot (unity_SHAg, normal_12);
					  x_14.z = dot (unity_SHAb, normal_12);
					  mediump vec3 x1_15;
					  mediump vec4 tmpvar_16;
					  tmpvar_16 = (normal_12.xyzz * normal_12.yzzx);
					  x1_15.x = dot (unity_SHBr, tmpvar_16);
					  x1_15.y = dot (unity_SHBg, tmpvar_16);
					  x1_15.z = dot (unity_SHBb, tmpvar_16);
					  res_13 = (x_14 + (x1_15 + (unity_SHC.xyz * 
					    ((normal_12.x * normal_12.x) - (normal_12.y * normal_12.y))
					  )));
					  res_13 = max (((1.055 * 
					    pow (max (res_13, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_13;
					  tmpvar_4 = shlight_1;
					  highp vec3 lightColor0_17;
					  lightColor0_17 = unity_LightColor[0].xyz;
					  highp vec3 lightColor1_18;
					  lightColor1_18 = unity_LightColor[1].xyz;
					  highp vec3 lightColor2_19;
					  lightColor2_19 = unity_LightColor[2].xyz;
					  highp vec3 lightColor3_20;
					  lightColor3_20 = unity_LightColor[3].xyz;
					  highp vec4 lightAttenSq_21;
					  lightAttenSq_21 = unity_4LightAtten0;
					  highp vec3 normal_22;
					  normal_22 = worldNormal_2;
					  highp vec3 col_23;
					  highp vec4 ndotl_24;
					  highp vec4 lengthSq_25;
					  highp vec4 tmpvar_26;
					  tmpvar_26 = (unity_4LightPosX0 - tmpvar_6.x);
					  highp vec4 tmpvar_27;
					  tmpvar_27 = (unity_4LightPosY0 - tmpvar_6.y);
					  highp vec4 tmpvar_28;
					  tmpvar_28 = (unity_4LightPosZ0 - tmpvar_6.z);
					  lengthSq_25 = (tmpvar_26 * tmpvar_26);
					  lengthSq_25 = (lengthSq_25 + (tmpvar_27 * tmpvar_27));
					  lengthSq_25 = (lengthSq_25 + (tmpvar_28 * tmpvar_28));
					  ndotl_24 = (tmpvar_26 * normal_22.x);
					  ndotl_24 = (ndotl_24 + (tmpvar_27 * normal_22.y));
					  ndotl_24 = (ndotl_24 + (tmpvar_28 * normal_22.z));
					  highp vec4 tmpvar_29;
					  tmpvar_29 = max (vec4(0.0, 0.0, 0.0, 0.0), (ndotl_24 * inversesqrt(lengthSq_25)));
					  ndotl_24 = tmpvar_29;
					  highp vec4 tmpvar_30;
					  tmpvar_30 = (tmpvar_29 * (1.0/((1.0 + 
					    (lengthSq_25 * lightAttenSq_21)
					  ))));
					  col_23 = (lightColor0_17 * tmpvar_30.x);
					  col_23 = (col_23 + (lightColor1_18 * tmpvar_30.y));
					  col_23 = (col_23 + (lightColor2_19 * tmpvar_30.z));
					  col_23 = (col_23 + (lightColor3_20 * tmpvar_30.w));
					  tmpvar_4 = (tmpvar_4 + col_23);
					  tmpvar_5 = (unity_World2Shadow[0] * tmpvar_6);
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = tmpvar_6.xyz;
					  xlv_TEXCOORD3 = tmpvar_4;
					  xlv_TEXCOORD4 = tmpvar_5;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform mediump vec4 _LightShadowData;
					uniform highp sampler2D _ShadowMapTexture;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  lowp float tmpvar_11;
					  highp float tmpvar_12;
					  tmpvar_12 = max (float((texture2DProj (_ShadowMapTexture, xlv_TEXCOORD4).x > 
					    (xlv_TEXCOORD4.z / xlv_TEXCOORD4.w)
					  )), _LightShadowData.x);
					  tmpvar_11 = tmpvar_12;
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_13;
					  mediump vec3 LightDir_14;
					  LightDir_14 = lightDir_4;
					  mediump vec3 viewDir_15;
					  viewDir_15 = worldViewDir_3;
					  mediump float atten_16;
					  atten_16 = tmpvar_11;
					  highp vec4 c_17;
					  highp vec3 BRDF_18;
					  highp float NdotE_19;
					  highp float NdotL_20;
					  mediump float tmpvar_21;
					  tmpvar_21 = dot (tmpvar_2, LightDir_14);
					  NdotL_20 = tmpvar_21;
					  mediump float tmpvar_22;
					  tmpvar_22 = dot (tmpvar_2, viewDir_15);
					  NdotE_19 = tmpvar_22;
					  highp vec2 tmpvar_23;
					  tmpvar_23.x = (NdotE_19 * 0.8);
					  tmpvar_23.y = ((NdotL_20 * 0.5) + 0.5);
					  lowp vec3 tmpvar_24;
					  tmpvar_24 = texture2D (_Ramp2D, tmpvar_23).xyz;
					  BRDF_18 = tmpvar_24;
					  c_17.xyz = ((BRDF_18 * atten_16) * 0.5);
					  c_17.w = tmpvar_8;
					  tmpvar_13 = c_17;
					  c_1.xyz = (c_1 + tmpvar_13).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					#extension GL_EXT_shadow_samplers : enable
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 unity_World2Shadow[4];
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  mediump vec4 tmpvar_5;
					  highp vec4 v_6;
					  v_6.x = _World2Object[0].x;
					  v_6.y = _World2Object[1].x;
					  v_6.z = _World2Object[2].x;
					  v_6.w = _World2Object[3].x;
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].y;
					  v_7.y = _World2Object[1].y;
					  v_7.z = _World2Object[2].y;
					  v_7.w = _World2Object[3].y;
					  highp vec4 v_8;
					  v_8.x = _World2Object[0].z;
					  v_8.y = _World2Object[1].z;
					  v_8.z = _World2Object[2].z;
					  v_8.w = _World2Object[3].z;
					  highp vec3 tmpvar_9;
					  tmpvar_9 = normalize(((
					    (v_6.xyz * _glesNormal.x)
					   + 
					    (v_7.xyz * _glesNormal.y)
					  ) + (v_8.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_9;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_10;
					  tmpvar_10.w = 1.0;
					  tmpvar_10.xyz = worldNormal_2;
					  mediump vec4 normal_11;
					  normal_11 = tmpvar_10;
					  mediump vec3 res_12;
					  mediump vec3 x_13;
					  x_13.x = dot (unity_SHAr, normal_11);
					  x_13.y = dot (unity_SHAg, normal_11);
					  x_13.z = dot (unity_SHAb, normal_11);
					  mediump vec3 x1_14;
					  mediump vec4 tmpvar_15;
					  tmpvar_15 = (normal_11.xyzz * normal_11.yzzx);
					  x1_14.x = dot (unity_SHBr, tmpvar_15);
					  x1_14.y = dot (unity_SHBg, tmpvar_15);
					  x1_14.z = dot (unity_SHBb, tmpvar_15);
					  res_12 = (x_13 + (x1_14 + (unity_SHC.xyz * 
					    ((normal_11.x * normal_11.x) - (normal_11.y * normal_11.y))
					  )));
					  res_12 = max (((1.055 * 
					    pow (max (res_12, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_12;
					  tmpvar_4 = shlight_1;
					  highp vec4 tmpvar_16;
					  tmpvar_16 = (_Object2World * _glesVertex);
					  tmpvar_5 = (unity_World2Shadow[0] * tmpvar_16);
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = tmpvar_16.xyz;
					  xlv_TEXCOORD3 = tmpvar_4;
					  xlv_TEXCOORD4 = tmpvar_5;
					}
					
					
					#endif
					#ifdef FRAGMENT
					#extension GL_EXT_shadow_samplers : enable
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform mediump vec4 _LightShadowData;
					uniform lowp sampler2DShadow _ShadowMapTexture;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  lowp float shadow_11;
					  shadow_11 = (_LightShadowData.x + (shadow2DEXT (_ShadowMapTexture, xlv_TEXCOORD4.xyz) * (1.0 - _LightShadowData.x)));
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_12;
					  mediump vec3 LightDir_13;
					  LightDir_13 = lightDir_4;
					  mediump vec3 viewDir_14;
					  viewDir_14 = worldViewDir_3;
					  mediump float atten_15;
					  atten_15 = shadow_11;
					  highp vec4 c_16;
					  highp vec3 BRDF_17;
					  highp float NdotE_18;
					  highp float NdotL_19;
					  mediump float tmpvar_20;
					  tmpvar_20 = dot (tmpvar_2, LightDir_13);
					  NdotL_19 = tmpvar_20;
					  mediump float tmpvar_21;
					  tmpvar_21 = dot (tmpvar_2, viewDir_14);
					  NdotE_18 = tmpvar_21;
					  highp vec2 tmpvar_22;
					  tmpvar_22.x = (NdotE_18 * 0.8);
					  tmpvar_22.y = ((NdotL_19 * 0.5) + 0.5);
					  lowp vec3 tmpvar_23;
					  tmpvar_23 = texture2D (_Ramp2D, tmpvar_22).xyz;
					  BRDF_17 = tmpvar_23;
					  c_16.xyz = ((BRDF_17 * atten_15) * 0.5);
					  c_16.w = tmpvar_8;
					  tmpvar_12 = c_16;
					  c_1.xyz = (c_1 + tmpvar_12).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_TEXCOORD0;
					out highp vec2 vs_TEXCOORD0;
					out mediump vec3 vs_TEXCOORD1;
					out highp vec3 vs_TEXCOORD2;
					out lowp vec3 vs_TEXCOORD3;
					out mediump vec4 vs_TEXCOORD4;
					vec4 t0;
					vec4 t1;
					mediump vec4 t16_1;
					mediump vec3 t16_2;
					mediump vec3 t16_3;
					float t12;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec4(0.0);
					t16_1 = vec4(0.0);
					t16_2 = vec3(0.0);
					t16_3 = vec3(0.0);
					t12 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t12 = dot(t0.xyz, t0.xyz);
					    t12 = inversesqrt(t12);
					    t0.xyz = vec3(t12) * t0.xyz;
					    vs_TEXCOORD1.xyz = t0.xyz;
					    t1.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t1.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t1.xyz;
					    t1.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t1.xyz;
					    vs_TEXCOORD2.xyz = _Object2World[3].xyz * in_POSITION0.www + t1.xyz;
					    t16_2.x = t0.y * t0.y;
					    t16_2.x = t0.x * t0.x + (-t16_2.x);
					    t16_1 = t0.yzzx * t0.xyzz;
					    t16_3.x = dot(unity_SHBr, t16_1);
					    t16_3.y = dot(unity_SHBg, t16_1);
					    t16_3.z = dot(unity_SHBb, t16_1);
					    t16_2.xyz = unity_SHC.xyz * t16_2.xxx + t16_3.xyz;
					    t0.w = 1.0;
					    t16_3.x = dot(unity_SHAr, t0);
					    t16_3.y = dot(unity_SHAg, t0);
					    t16_3.z = dot(unity_SHAb, t0);
					    t16_2.xyz = t16_2.xyz + t16_3.xyz;
					    t16_2.xyz = max(t16_2.xyz, vec3(0.0, 0.0, 0.0));
					    t0.xyz = log2(t16_2.xyz);
					    t0.xyz = t0.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    t0.xyz = exp2(t0.xyz);
					    t0.xyz = t0.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    t0.xyz = max(t0.xyz, vec3(0.0, 0.0, 0.0));
					    vs_TEXCOORD3.xyz = t0.xyz;
					    t0 = in_POSITION0.yyyy * _Object2World[1];
					    t0 = _Object2World[0] * in_POSITION0.xxxx + t0;
					    t0 = _Object2World[2] * in_POSITION0.zzzz + t0;
					    t0 = _Object2World[3] * in_POSITION0.wwww + t0;
					    t1 = t0.yyyy * unity_World2Shadow[0][1];
					    t1 = unity_World2Shadow[0][0] * t0.xxxx + t1;
					    t1 = unity_World2Shadow[0][2] * t0.zzzz + t1;
					    t0 = unity_World2Shadow[0][3] * t0.wwww + t1;
					    vs_TEXCOORD4 = t0;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					uniform lowp sampler2D _MainTex;
					uniform lowp sampler2D _Ramp2D;
					uniform lowp sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					uniform lowp sampler2D _ShadowMapTexture;
					in highp vec2 vs_TEXCOORD0;
					in mediump vec3 vs_TEXCOORD1;
					in highp vec3 vs_TEXCOORD2;
					in lowp vec3 vs_TEXCOORD3;
					in mediump vec4 vs_TEXCOORD4;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					lowp vec3 t10_2;
					mediump float t16_4;
					float t9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t10_2 = vec3(0.0);
					t16_4 = float(0.0);
					t9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD2.xyz) + _WorldSpaceCameraPos.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, t0.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    vec3 txVec6 = vec3(vs_TEXCOORD4.xy,vs_TEXCOORD4.z);
					    t16_1.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec6, 0.0);
					    t16_4 = (-_LightShadowData.x) + 1.0;
					    t16_1.x = t16_1.x * t16_4 + _LightShadowData.x;
					    t16_0.xyz = t10_0.xyz * t16_1.xxx;
					    t16_0.xyz = t16_0.xyz * vec3(0.5, 0.5, 0.5);
					    t10_2.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
					    t16_1.xyz = t10_2.xyz * vs_TEXCOORD3.xyz + t16_0.xyz;
					    SV_Target0.xyz = t16_1.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					#extension GL_EXT_shadow_samplers : enable
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec4 unity_4LightPosX0;
					uniform highp vec4 unity_4LightPosY0;
					uniform highp vec4 unity_4LightPosZ0;
					uniform mediump vec4 unity_4LightAtten0;
					uniform mediump vec4 unity_LightColor[8];
					uniform mediump vec4 unity_SHAr;
					uniform mediump vec4 unity_SHAg;
					uniform mediump vec4 unity_SHAb;
					uniform mediump vec4 unity_SHBr;
					uniform mediump vec4 unity_SHBg;
					uniform mediump vec4 unity_SHBb;
					uniform mediump vec4 unity_SHC;
					uniform highp mat4 unity_World2Shadow[4];
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					uniform highp vec4 _MainTex_ST;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  highp vec3 shlight_1;
					  lowp vec3 worldNormal_2;
					  mediump vec3 tmpvar_3;
					  lowp vec3 tmpvar_4;
					  mediump vec4 tmpvar_5;
					  highp vec4 tmpvar_6;
					  tmpvar_6 = (_Object2World * _glesVertex);
					  highp vec4 v_7;
					  v_7.x = _World2Object[0].x;
					  v_7.y = _World2Object[1].x;
					  v_7.z = _World2Object[2].x;
					  v_7.w = _World2Object[3].x;
					  highp vec4 v_8;
					  v_8.x = _World2Object[0].y;
					  v_8.y = _World2Object[1].y;
					  v_8.z = _World2Object[2].y;
					  v_8.w = _World2Object[3].y;
					  highp vec4 v_9;
					  v_9.x = _World2Object[0].z;
					  v_9.y = _World2Object[1].z;
					  v_9.z = _World2Object[2].z;
					  v_9.w = _World2Object[3].z;
					  highp vec3 tmpvar_10;
					  tmpvar_10 = normalize(((
					    (v_7.xyz * _glesNormal.x)
					   + 
					    (v_8.xyz * _glesNormal.y)
					  ) + (v_9.xyz * _glesNormal.z)));
					  worldNormal_2 = tmpvar_10;
					  tmpvar_3 = worldNormal_2;
					  lowp vec4 tmpvar_11;
					  tmpvar_11.w = 1.0;
					  tmpvar_11.xyz = worldNormal_2;
					  mediump vec4 normal_12;
					  normal_12 = tmpvar_11;
					  mediump vec3 res_13;
					  mediump vec3 x_14;
					  x_14.x = dot (unity_SHAr, normal_12);
					  x_14.y = dot (unity_SHAg, normal_12);
					  x_14.z = dot (unity_SHAb, normal_12);
					  mediump vec3 x1_15;
					  mediump vec4 tmpvar_16;
					  tmpvar_16 = (normal_12.xyzz * normal_12.yzzx);
					  x1_15.x = dot (unity_SHBr, tmpvar_16);
					  x1_15.y = dot (unity_SHBg, tmpvar_16);
					  x1_15.z = dot (unity_SHBb, tmpvar_16);
					  res_13 = (x_14 + (x1_15 + (unity_SHC.xyz * 
					    ((normal_12.x * normal_12.x) - (normal_12.y * normal_12.y))
					  )));
					  res_13 = max (((1.055 * 
					    pow (max (res_13, vec3(0.0, 0.0, 0.0)), vec3(0.4166667, 0.4166667, 0.4166667))
					  ) - 0.055), vec3(0.0, 0.0, 0.0));
					  shlight_1 = res_13;
					  tmpvar_4 = shlight_1;
					  highp vec3 lightColor0_17;
					  lightColor0_17 = unity_LightColor[0].xyz;
					  highp vec3 lightColor1_18;
					  lightColor1_18 = unity_LightColor[1].xyz;
					  highp vec3 lightColor2_19;
					  lightColor2_19 = unity_LightColor[2].xyz;
					  highp vec3 lightColor3_20;
					  lightColor3_20 = unity_LightColor[3].xyz;
					  highp vec4 lightAttenSq_21;
					  lightAttenSq_21 = unity_4LightAtten0;
					  highp vec3 normal_22;
					  normal_22 = worldNormal_2;
					  highp vec3 col_23;
					  highp vec4 ndotl_24;
					  highp vec4 lengthSq_25;
					  highp vec4 tmpvar_26;
					  tmpvar_26 = (unity_4LightPosX0 - tmpvar_6.x);
					  highp vec4 tmpvar_27;
					  tmpvar_27 = (unity_4LightPosY0 - tmpvar_6.y);
					  highp vec4 tmpvar_28;
					  tmpvar_28 = (unity_4LightPosZ0 - tmpvar_6.z);
					  lengthSq_25 = (tmpvar_26 * tmpvar_26);
					  lengthSq_25 = (lengthSq_25 + (tmpvar_27 * tmpvar_27));
					  lengthSq_25 = (lengthSq_25 + (tmpvar_28 * tmpvar_28));
					  ndotl_24 = (tmpvar_26 * normal_22.x);
					  ndotl_24 = (ndotl_24 + (tmpvar_27 * normal_22.y));
					  ndotl_24 = (ndotl_24 + (tmpvar_28 * normal_22.z));
					  highp vec4 tmpvar_29;
					  tmpvar_29 = max (vec4(0.0, 0.0, 0.0, 0.0), (ndotl_24 * inversesqrt(lengthSq_25)));
					  ndotl_24 = tmpvar_29;
					  highp vec4 tmpvar_30;
					  tmpvar_30 = (tmpvar_29 * (1.0/((1.0 + 
					    (lengthSq_25 * lightAttenSq_21)
					  ))));
					  col_23 = (lightColor0_17 * tmpvar_30.x);
					  col_23 = (col_23 + (lightColor1_18 * tmpvar_30.y));
					  col_23 = (col_23 + (lightColor2_19 * tmpvar_30.z));
					  col_23 = (col_23 + (lightColor3_20 * tmpvar_30.w));
					  tmpvar_4 = (tmpvar_4 + col_23);
					  tmpvar_5 = (unity_World2Shadow[0] * tmpvar_6);
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = ((_glesMultiTexCoord0.xy * _MainTex_ST.xy) + _MainTex_ST.zw);
					  xlv_TEXCOORD1 = tmpvar_3;
					  xlv_TEXCOORD2 = tmpvar_6.xyz;
					  xlv_TEXCOORD3 = tmpvar_4;
					  xlv_TEXCOORD4 = tmpvar_5;
					}
					
					
					#endif
					#ifdef FRAGMENT
					#extension GL_EXT_shadow_samplers : enable
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform mediump vec4 _LightShadowData;
					uniform lowp sampler2DShadow _ShadowMapTexture;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying highp vec2 xlv_TEXCOORD0;
					varying mediump vec3 xlv_TEXCOORD1;
					varying highp vec3 xlv_TEXCOORD2;
					varying lowp vec3 xlv_TEXCOORD3;
					varying mediump vec4 xlv_TEXCOORD4;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  mediump vec3 tmpvar_5;
					  tmpvar_5 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_5;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD2));
					  worldViewDir_3 = tmpvar_6;
					  tmpvar_2 = xlv_TEXCOORD1;
					  lowp vec3 tmpvar_7;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, xlv_TEXCOORD0);
					  c_9 = tmpvar_10;
					  tmpvar_7 = c_9.xyz;
					  tmpvar_8 = c_9.w;
					  lowp float shadow_11;
					  shadow_11 = (_LightShadowData.x + (shadow2DEXT (_ShadowMapTexture, xlv_TEXCOORD4.xyz) * (1.0 - _LightShadowData.x)));
					  c_1.w = 0.0;
					  c_1.xyz = (tmpvar_7 * xlv_TEXCOORD3);
					  mediump vec4 tmpvar_12;
					  mediump vec3 LightDir_13;
					  LightDir_13 = lightDir_4;
					  mediump vec3 viewDir_14;
					  viewDir_14 = worldViewDir_3;
					  mediump float atten_15;
					  atten_15 = shadow_11;
					  highp vec4 c_16;
					  highp vec3 BRDF_17;
					  highp float NdotE_18;
					  highp float NdotL_19;
					  mediump float tmpvar_20;
					  tmpvar_20 = dot (tmpvar_2, LightDir_13);
					  NdotL_19 = tmpvar_20;
					  mediump float tmpvar_21;
					  tmpvar_21 = dot (tmpvar_2, viewDir_14);
					  NdotE_18 = tmpvar_21;
					  highp vec2 tmpvar_22;
					  tmpvar_22.x = (NdotE_18 * 0.8);
					  tmpvar_22.y = ((NdotL_19 * 0.5) + 0.5);
					  lowp vec3 tmpvar_23;
					  tmpvar_23 = texture2D (_Ramp2D, tmpvar_22).xyz;
					  BRDF_17 = tmpvar_23;
					  c_16.xyz = ((BRDF_17 * atten_15) * 0.5);
					  c_16.w = tmpvar_8;
					  tmpvar_12 = c_16;
					  c_1.xyz = (c_1 + tmpvar_12).xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" "VERTEXLIGHT_ON" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					in highp vec4 in_TEXCOORD0;
					out highp vec2 vs_TEXCOORD0;
					out mediump vec3 vs_TEXCOORD1;
					out highp vec3 vs_TEXCOORD2;
					out lowp vec3 vs_TEXCOORD3;
					out mediump vec4 vs_TEXCOORD4;
					vec4 t0;
					vec4 t1;
					vec4 t2;
					mediump vec3 t16_2;
					vec4 t3;
					mediump vec4 t16_3;
					vec4 t4;
					mediump vec3 t16_4;
					vec3 t5;
					float t18;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec4(0.0);
					t2 = vec4(0.0);
					t16_2 = vec3(0.0);
					t3 = vec4(0.0);
					t16_3 = vec4(0.0);
					t4 = vec4(0.0);
					t16_4 = vec3(0.0);
					t5 = vec3(0.0);
					t18 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    vs_TEXCOORD0.xy = in_TEXCOORD0.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t18 = dot(t0.xyz, t0.xyz);
					    t18 = inversesqrt(t18);
					    t0.xyz = vec3(t18) * t0.xyz;
					    vs_TEXCOORD1.xyz = t0.xyz;
					    t1.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t1.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t1.xyz;
					    t1.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t1.xyz;
					    t1.xyz = _Object2World[3].xyz * in_POSITION0.www + t1.xyz;
					    vs_TEXCOORD2.xyz = t1.xyz;
					    t16_2.x = t0.y * t0.y;
					    t16_2.x = t0.x * t0.x + (-t16_2.x);
					    t16_3 = t0.yzzx * t0.xyzz;
					    t16_4.x = dot(unity_SHBr, t16_3);
					    t16_4.y = dot(unity_SHBg, t16_3);
					    t16_4.z = dot(unity_SHBb, t16_3);
					    t16_2.xyz = unity_SHC.xyz * t16_2.xxx + t16_4.xyz;
					    t0.w = 1.0;
					    t16_3.x = dot(unity_SHAr, t0);
					    t16_3.y = dot(unity_SHAg, t0);
					    t16_3.z = dot(unity_SHAb, t0);
					    t16_2.xyz = t16_2.xyz + t16_3.xyz;
					    t16_2.xyz = max(t16_2.xyz, vec3(0.0, 0.0, 0.0));
					    t5.xyz = log2(t16_2.xyz);
					    t5.xyz = t5.xyz * vec3(0.416666657, 0.416666657, 0.416666657);
					    t5.xyz = exp2(t5.xyz);
					    t5.xyz = t5.xyz * vec3(1.05499995, 1.05499995, 1.05499995) + vec3(-0.0549999997, -0.0549999997, -0.0549999997);
					    t5.xyz = max(t5.xyz, vec3(0.0, 0.0, 0.0));
					    t2 = (-t1.yyyy) + unity_4LightPosY0;
					    t3 = t0.yyyy * t2;
					    t2 = t2 * t2;
					    t4 = (-t1.xxxx) + unity_4LightPosX0;
					    t1 = (-t1.zzzz) + unity_4LightPosZ0;
					    t3 = t4 * t0.xxxx + t3;
					    t0 = t1 * t0.zzzz + t3;
					    t2 = t4 * t4 + t2;
					    t1 = t1 * t1 + t2;
					    t2 = inversesqrt(t1);
					    t1 = t1 * unity_4LightAtten0 + vec4(1.0, 1.0, 1.0, 1.0);
					    t1 = vec4(1.0, 1.0, 1.0, 1.0) / t1;
					    t0 = t0 * t2;
					    t0 = max(t0, vec4(0.0, 0.0, 0.0, 0.0));
					    t0 = t1 * t0;
					    t1.xyz = t0.yyy * unity_LightColor[1].xyz;
					    t1.xyz = unity_LightColor[0].xyz * t0.xxx + t1.xyz;
					    t0.xyz = unity_LightColor[2].xyz * t0.zzz + t1.xyz;
					    t0.xyz = unity_LightColor[3].xyz * t0.www + t0.xyz;
					    t0.xyz = t0.xyz + t5.xyz;
					    vs_TEXCOORD3.xyz = t0.xyz;
					    t0 = in_POSITION0.yyyy * _Object2World[1];
					    t0 = _Object2World[0] * in_POSITION0.xxxx + t0;
					    t0 = _Object2World[2] * in_POSITION0.zzzz + t0;
					    t0 = _Object2World[3] * in_POSITION0.wwww + t0;
					    t1 = t0.yyyy * unity_World2Shadow[0][1];
					    t1 = unity_World2Shadow[0][0] * t0.xxxx + t1;
					    t1 = unity_World2Shadow[0][2] * t0.zzzz + t1;
					    t0 = unity_World2Shadow[0][3] * t0.wwww + t1;
					    vs_TEXCOORD4 = t0;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	vec4 _MainTex_ST;
					uniform lowp sampler2D _MainTex;
					uniform lowp sampler2D _Ramp2D;
					uniform lowp sampler2DShadow hlslcc_zcmp_ShadowMapTexture;
					uniform lowp sampler2D _ShadowMapTexture;
					in highp vec2 vs_TEXCOORD0;
					in mediump vec3 vs_TEXCOORD1;
					in highp vec3 vs_TEXCOORD2;
					in lowp vec3 vs_TEXCOORD3;
					in mediump vec4 vs_TEXCOORD4;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					lowp vec3 t10_2;
					mediump float t16_4;
					float t9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t10_2 = vec3(0.0);
					t16_4 = float(0.0);
					t9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD2.xyz) + _WorldSpaceCameraPos.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, t0.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t16_1.x = dot(vs_TEXCOORD1.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    vec3 txVec5 = vec3(vs_TEXCOORD4.xy,vs_TEXCOORD4.z);
					    t16_1.x = textureLod(hlslcc_zcmp_ShadowMapTexture, txVec5, 0.0);
					    t16_4 = (-_LightShadowData.x) + 1.0;
					    t16_1.x = t16_1.x * t16_4 + _LightShadowData.x;
					    t16_0.xyz = t10_0.xyz * t16_1.xxx;
					    t16_0.xyz = t16_0.xyz * vec3(0.5, 0.5, 0.5);
					    t10_2.xyz = texture(_MainTex, vs_TEXCOORD0.xy).xyz;
					    t16_1.xyz = t10_2.xyz * vs_TEXCOORD3.xyz + t16_0.xyz;
					    SV_Target0.xyz = t16_1.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_OFF" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" "SHADOWS_SCREEN" "SHADOWS_NATIVE" "LIGHTMAP_OFF" "DIRLIGHTMAP_OFF" "DYNAMICLIGHTMAP_OFF" }
					"!!GLES3"
}
}
 }
 Pass {
  Name "FORWARD"
  Tags { "LIGHTMODE"="ForwardAdd" "RenderType"="Opaque" }
  ZWrite Off
  Blend One One
  GpuProgramID 79641
Program "vp" {
SubProgram "gles " {
Keywords { "POINT" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec3 worldNormal_1;
					  mediump vec3 tmpvar_2;
					  highp vec4 v_3;
					  v_3.x = _World2Object[0].x;
					  v_3.y = _World2Object[1].x;
					  v_3.z = _World2Object[2].x;
					  v_3.w = _World2Object[3].x;
					  highp vec4 v_4;
					  v_4.x = _World2Object[0].y;
					  v_4.y = _World2Object[1].y;
					  v_4.z = _World2Object[2].y;
					  v_4.w = _World2Object[3].y;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].z;
					  v_5.y = _World2Object[1].z;
					  v_5.z = _World2Object[2].z;
					  v_5.w = _World2Object[3].z;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize(((
					    (v_3.xyz * _glesNormal.x)
					   + 
					    (v_4.xyz * _glesNormal.y)
					  ) + (v_5.xyz * _glesNormal.z)));
					  worldNormal_1 = tmpvar_6;
					  tmpvar_2 = worldNormal_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = (_Object2World * _glesVertex).xyz;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform highp vec4 _WorldSpaceLightPos0;
					uniform sampler2D _LightTexture0;
					uniform mediump mat4 _LightMatrix0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  highp vec2 tmpvar_5;
					  tmpvar_5.x = 1.0;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceLightPos0.xyz - xlv_TEXCOORD1));
					  lightDir_4 = tmpvar_6;
					  highp vec3 tmpvar_7;
					  tmpvar_7 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD1));
					  worldViewDir_3 = tmpvar_7;
					  tmpvar_2 = xlv_TEXCOORD0;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, tmpvar_5);
					  c_9 = tmpvar_10;
					  tmpvar_8 = c_9.w;
					  highp vec4 tmpvar_11;
					  tmpvar_11.w = 1.0;
					  tmpvar_11.xyz = xlv_TEXCOORD1;
					  highp vec3 tmpvar_12;
					  tmpvar_12 = (_LightMatrix0 * tmpvar_11).xyz;
					  highp float tmpvar_13;
					  tmpvar_13 = dot (tmpvar_12, tmpvar_12);
					  lowp float tmpvar_14;
					  tmpvar_14 = texture2D (_LightTexture0, vec2(tmpvar_13)).w;
					  mediump vec4 tmpvar_15;
					  mediump vec3 LightDir_16;
					  LightDir_16 = lightDir_4;
					  mediump vec3 viewDir_17;
					  viewDir_17 = worldViewDir_3;
					  mediump float atten_18;
					  atten_18 = tmpvar_14;
					  highp vec4 c_19;
					  highp vec3 BRDF_20;
					  highp float NdotE_21;
					  highp float NdotL_22;
					  mediump float tmpvar_23;
					  tmpvar_23 = dot (tmpvar_2, LightDir_16);
					  NdotL_22 = tmpvar_23;
					  mediump float tmpvar_24;
					  tmpvar_24 = dot (tmpvar_2, viewDir_17);
					  NdotE_21 = tmpvar_24;
					  highp vec2 tmpvar_25;
					  tmpvar_25.x = (NdotE_21 * 0.8);
					  tmpvar_25.y = ((NdotL_22 * 0.5) + 0.5);
					  lowp vec3 tmpvar_26;
					  tmpvar_26 = texture2D (_Ramp2D, tmpvar_25).xyz;
					  BRDF_20 = tmpvar_26;
					  c_19.xyz = ((BRDF_20 * atten_18) * 0.5);
					  c_19.w = tmpvar_8;
					  tmpvar_15 = c_19;
					  c_1.xyz = tmpvar_15.xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "POINT" }
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out mediump vec3 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					vec4 t0;
					vec3 t1;
					float t6;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t6 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    vs_TEXCOORD0.xyz = t0.xyz;
					    t0.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t0.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t0.xyz;
					    t0.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t0.xyz;
					    vs_TEXCOORD1.xyz = _Object2World[3].xyz * in_POSITION0.www + t0.xyz;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					uniform lowp sampler2D _LightTexture0;
					uniform lowp sampler2D _Ramp2D;
					in mediump vec3 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					vec3 t2;
					float t6;
					float t9;
					lowp float t10_9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t2 = vec3(0.0);
					t6 = float(0.0);
					t9 = float(0.0);
					t10_9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, t0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    t6 = dot(t2.xyz, t2.xyz);
					    t6 = inversesqrt(t6);
					    t2.xyz = vec3(t6) * t2.xyz;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, t2.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_1.xyz = vs_TEXCOORD1.yyy * _LightMatrix0[1].xyz;
					    t16_1.xyz = _LightMatrix0[0].xyz * vs_TEXCOORD1.xxx + t16_1.xyz;
					    t16_1.xyz = _LightMatrix0[2].xyz * vs_TEXCOORD1.zzz + t16_1.xyz;
					    t16_1.xyz = t16_1.xyz + _LightMatrix0[3].xyz;
					    t9 = dot(t16_1.xyz, t16_1.xyz);
					    t10_9 = texture(_LightTexture0, vec2(t9)).w;
					    t16_0.xyz = vec3(t10_9) * t10_0.xyz;
					    t16_0.xyz = t16_0.xyz * vec3(0.5, 0.5, 0.5);
					    SV_Target0.xyz = t16_0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec3 worldNormal_1;
					  mediump vec3 tmpvar_2;
					  highp vec4 v_3;
					  v_3.x = _World2Object[0].x;
					  v_3.y = _World2Object[1].x;
					  v_3.z = _World2Object[2].x;
					  v_3.w = _World2Object[3].x;
					  highp vec4 v_4;
					  v_4.x = _World2Object[0].y;
					  v_4.y = _World2Object[1].y;
					  v_4.z = _World2Object[2].y;
					  v_4.w = _World2Object[3].y;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].z;
					  v_5.y = _World2Object[1].z;
					  v_5.z = _World2Object[2].z;
					  v_5.w = _World2Object[3].z;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize(((
					    (v_3.xyz * _glesNormal.x)
					   + 
					    (v_4.xyz * _glesNormal.y)
					  ) + (v_5.xyz * _glesNormal.z)));
					  worldNormal_1 = tmpvar_6;
					  tmpvar_2 = worldNormal_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = (_Object2World * _glesVertex).xyz;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  highp vec2 tmpvar_5;
					  tmpvar_5.x = 1.0;
					  mediump vec3 tmpvar_6;
					  tmpvar_6 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_6;
					  highp vec3 tmpvar_7;
					  tmpvar_7 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD1));
					  worldViewDir_3 = tmpvar_7;
					  tmpvar_2 = xlv_TEXCOORD0;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, tmpvar_5);
					  c_9 = tmpvar_10;
					  tmpvar_8 = c_9.w;
					  mediump vec4 tmpvar_11;
					  mediump vec3 LightDir_12;
					  LightDir_12 = lightDir_4;
					  mediump vec3 viewDir_13;
					  viewDir_13 = worldViewDir_3;
					  highp vec4 c_14;
					  highp vec3 BRDF_15;
					  highp float NdotE_16;
					  highp float NdotL_17;
					  mediump float tmpvar_18;
					  tmpvar_18 = dot (tmpvar_2, LightDir_12);
					  NdotL_17 = tmpvar_18;
					  mediump float tmpvar_19;
					  tmpvar_19 = dot (tmpvar_2, viewDir_13);
					  NdotE_16 = tmpvar_19;
					  highp vec2 tmpvar_20;
					  tmpvar_20.x = (NdotE_16 * 0.8);
					  tmpvar_20.y = ((NdotL_17 * 0.5) + 0.5);
					  lowp vec3 tmpvar_21;
					  tmpvar_21 = texture2D (_Ramp2D, tmpvar_20).xyz;
					  BRDF_15 = tmpvar_21;
					  c_14.xyz = (BRDF_15 * 0.5);
					  c_14.w = tmpvar_8;
					  tmpvar_11 = c_14;
					  c_1.xyz = tmpvar_11.xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out mediump vec3 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					vec4 t0;
					vec3 t1;
					float t6;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t6 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    vs_TEXCOORD0.xyz = t0.xyz;
					    t0.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t0.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t0.xyz;
					    t0.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t0.xyz;
					    vs_TEXCOORD1.xyz = _Object2World[3].xyz * in_POSITION0.www + t0.xyz;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform lowp sampler2D _Ramp2D;
					in mediump vec3 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump float t16_1;
					float t6;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = float(0.0);
					t6 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    t16_1 = dot(vs_TEXCOORD0.xyz, t0.xyz);
					    t0.x = t16_1 * 0.800000012;
					    t16_1 = dot(vs_TEXCOORD0.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1 * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_0.xyz = t10_0.xyz * vec3(0.5, 0.5, 0.5);
					    SV_Target0.xyz = t16_0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "SPOT" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec3 worldNormal_1;
					  mediump vec3 tmpvar_2;
					  highp vec4 v_3;
					  v_3.x = _World2Object[0].x;
					  v_3.y = _World2Object[1].x;
					  v_3.z = _World2Object[2].x;
					  v_3.w = _World2Object[3].x;
					  highp vec4 v_4;
					  v_4.x = _World2Object[0].y;
					  v_4.y = _World2Object[1].y;
					  v_4.z = _World2Object[2].y;
					  v_4.w = _World2Object[3].y;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].z;
					  v_5.y = _World2Object[1].z;
					  v_5.z = _World2Object[2].z;
					  v_5.w = _World2Object[3].z;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize(((
					    (v_3.xyz * _glesNormal.x)
					   + 
					    (v_4.xyz * _glesNormal.y)
					  ) + (v_5.xyz * _glesNormal.z)));
					  worldNormal_1 = tmpvar_6;
					  tmpvar_2 = worldNormal_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = (_Object2World * _glesVertex).xyz;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform highp vec4 _WorldSpaceLightPos0;
					uniform sampler2D _LightTexture0;
					uniform mediump mat4 _LightMatrix0;
					uniform sampler2D _LightTextureB0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp float atten_2;
					  mediump vec4 lightCoord_3;
					  lowp vec3 tmpvar_4;
					  lowp vec3 worldViewDir_5;
					  lowp vec3 lightDir_6;
					  highp vec2 tmpvar_7;
					  tmpvar_7.x = 1.0;
					  highp vec3 tmpvar_8;
					  tmpvar_8 = normalize((_WorldSpaceLightPos0.xyz - xlv_TEXCOORD1));
					  lightDir_6 = tmpvar_8;
					  highp vec3 tmpvar_9;
					  tmpvar_9 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD1));
					  worldViewDir_5 = tmpvar_9;
					  tmpvar_4 = xlv_TEXCOORD0;
					  lowp float tmpvar_10;
					  mediump vec4 c_11;
					  lowp vec4 tmpvar_12;
					  tmpvar_12 = texture2D (_MainTex, tmpvar_7);
					  c_11 = tmpvar_12;
					  tmpvar_10 = c_11.w;
					  highp vec4 tmpvar_13;
					  tmpvar_13.w = 1.0;
					  tmpvar_13.xyz = xlv_TEXCOORD1;
					  highp vec4 tmpvar_14;
					  tmpvar_14 = (_LightMatrix0 * tmpvar_13);
					  lightCoord_3 = tmpvar_14;
					  lowp vec4 tmpvar_15;
					  mediump vec2 P_16;
					  P_16 = ((lightCoord_3.xy / lightCoord_3.w) + 0.5);
					  tmpvar_15 = texture2D (_LightTexture0, P_16);
					  highp vec3 LightCoord_17;
					  LightCoord_17 = lightCoord_3.xyz;
					  highp float tmpvar_18;
					  tmpvar_18 = dot (LightCoord_17, LightCoord_17);
					  lowp vec4 tmpvar_19;
					  tmpvar_19 = texture2D (_LightTextureB0, vec2(tmpvar_18));
					  mediump float tmpvar_20;
					  tmpvar_20 = ((float(
					    (lightCoord_3.z > 0.0)
					  ) * tmpvar_15.w) * tmpvar_19.w);
					  atten_2 = tmpvar_20;
					  mediump vec4 tmpvar_21;
					  mediump vec3 LightDir_22;
					  LightDir_22 = lightDir_6;
					  mediump vec3 viewDir_23;
					  viewDir_23 = worldViewDir_5;
					  mediump float atten_24;
					  atten_24 = atten_2;
					  highp vec4 c_25;
					  highp vec3 BRDF_26;
					  highp float NdotE_27;
					  highp float NdotL_28;
					  mediump float tmpvar_29;
					  tmpvar_29 = dot (tmpvar_4, LightDir_22);
					  NdotL_28 = tmpvar_29;
					  mediump float tmpvar_30;
					  tmpvar_30 = dot (tmpvar_4, viewDir_23);
					  NdotE_27 = tmpvar_30;
					  highp vec2 tmpvar_31;
					  tmpvar_31.x = (NdotE_27 * 0.8);
					  tmpvar_31.y = ((NdotL_28 * 0.5) + 0.5);
					  lowp vec3 tmpvar_32;
					  tmpvar_32 = texture2D (_Ramp2D, tmpvar_31).xyz;
					  BRDF_26 = tmpvar_32;
					  c_25.xyz = ((BRDF_26 * atten_24) * 0.5);
					  c_25.w = tmpvar_10;
					  tmpvar_21 = c_25;
					  c_1.xyz = tmpvar_21.xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out mediump vec3 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					vec4 t0;
					vec3 t1;
					float t6;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t6 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    vs_TEXCOORD0.xyz = t0.xyz;
					    t0.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t0.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t0.xyz;
					    t0.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t0.xyz;
					    vs_TEXCOORD1.xyz = _Object2World[3].xyz * in_POSITION0.www + t0.xyz;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					uniform lowp sampler2D _LightTexture0;
					uniform lowp sampler2D _LightTextureB0;
					uniform lowp sampler2D _Ramp2D;
					in mediump vec3 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					mediump vec4 t16_0;
					mediump vec2 t16_1;
					vec3 t2;
					mediump vec3 t16_2;
					lowp vec3 t10_2;
					lowp float t10_3;
					vec3 t4;
					bool tb7;
					float t12;
					lowp float t10_12;
					float t17;
					void main()
					{
					t16_0 = vec4(0.0);
					t16_1 = vec2(0.0);
					t2 = vec3(0.0);
					t16_2 = vec3(0.0);
					t10_2 = vec3(0.0);
					t10_3 = float(0.0);
					t4 = vec3(0.0);
					tb7 = bool(false);
					t12 = float(0.0);
					t10_12 = float(0.0);
					t17 = float(0.0);
					    t16_0 = vs_TEXCOORD1.yyyy * _LightMatrix0[1];
					    t16_0 = _LightMatrix0[0] * vs_TEXCOORD1.xxxx + t16_0;
					    t16_0 = _LightMatrix0[2] * vs_TEXCOORD1.zzzz + t16_0;
					    t16_0 = t16_0 + _LightMatrix0[3];
					    t16_1.xy = t16_0.xy / t16_0.ww;
					    t16_1.xy = t16_1.xy + vec2(0.5, 0.5);
					    t10_2.x = texture(_LightTexture0, t16_1.xy).w;
					#ifdef UNITY_ADRENO_ES3
					    tb7 = !!(0.0<t16_0.z);
					#else
					    tb7 = 0.0<t16_0.z;
					#endif
					    t12 = dot(t16_0.xyz, t16_0.xyz);
					    t10_12 = texture(_LightTextureB0, vec2(t12)).w;
					    t10_3 = (tb7) ? 1.0 : 0.0;
					    t10_3 = t10_2.x * t10_3;
					    t10_3 = t10_12 * t10_3;
					    t2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    t17 = dot(t2.xyz, t2.xyz);
					    t17 = inversesqrt(t17);
					    t2.xyz = vec3(t17) * t2.xyz;
					    t16_0.x = dot(vs_TEXCOORD0.xyz, t2.xyz);
					    t2.y = t16_0.x * 0.5 + 0.5;
					    t4.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    t12 = dot(t4.xyz, t4.xyz);
					    t12 = inversesqrt(t12);
					    t4.xyz = vec3(t12) * t4.xyz;
					    t16_0.x = dot(vs_TEXCOORD0.xyz, t4.xyz);
					    t2.x = t16_0.x * 0.800000012;
					    t10_2.xyz = texture(_Ramp2D, t2.xy).xyz;
					    t16_2.xyz = vec3(t10_3) * t10_2.xyz;
					    t16_2.xyz = t16_2.xyz * vec3(0.5, 0.5, 0.5);
					    SV_Target0.xyz = t16_2.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec3 worldNormal_1;
					  mediump vec3 tmpvar_2;
					  highp vec4 v_3;
					  v_3.x = _World2Object[0].x;
					  v_3.y = _World2Object[1].x;
					  v_3.z = _World2Object[2].x;
					  v_3.w = _World2Object[3].x;
					  highp vec4 v_4;
					  v_4.x = _World2Object[0].y;
					  v_4.y = _World2Object[1].y;
					  v_4.z = _World2Object[2].y;
					  v_4.w = _World2Object[3].y;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].z;
					  v_5.y = _World2Object[1].z;
					  v_5.z = _World2Object[2].z;
					  v_5.w = _World2Object[3].z;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize(((
					    (v_3.xyz * _glesNormal.x)
					   + 
					    (v_4.xyz * _glesNormal.y)
					  ) + (v_5.xyz * _glesNormal.z)));
					  worldNormal_1 = tmpvar_6;
					  tmpvar_2 = worldNormal_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = (_Object2World * _glesVertex).xyz;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform highp vec4 _WorldSpaceLightPos0;
					uniform lowp samplerCube _LightTexture0;
					uniform mediump mat4 _LightMatrix0;
					uniform sampler2D _LightTextureB0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  highp vec2 tmpvar_5;
					  tmpvar_5.x = 1.0;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize((_WorldSpaceLightPos0.xyz - xlv_TEXCOORD1));
					  lightDir_4 = tmpvar_6;
					  highp vec3 tmpvar_7;
					  tmpvar_7 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD1));
					  worldViewDir_3 = tmpvar_7;
					  tmpvar_2 = xlv_TEXCOORD0;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, tmpvar_5);
					  c_9 = tmpvar_10;
					  tmpvar_8 = c_9.w;
					  highp vec4 tmpvar_11;
					  tmpvar_11.w = 1.0;
					  tmpvar_11.xyz = xlv_TEXCOORD1;
					  highp vec3 tmpvar_12;
					  tmpvar_12 = (_LightMatrix0 * tmpvar_11).xyz;
					  highp float tmpvar_13;
					  tmpvar_13 = dot (tmpvar_12, tmpvar_12);
					  lowp float tmpvar_14;
					  tmpvar_14 = (texture2D (_LightTextureB0, vec2(tmpvar_13)).w * textureCube (_LightTexture0, tmpvar_12).w);
					  mediump vec4 tmpvar_15;
					  mediump vec3 LightDir_16;
					  LightDir_16 = lightDir_4;
					  mediump vec3 viewDir_17;
					  viewDir_17 = worldViewDir_3;
					  mediump float atten_18;
					  atten_18 = tmpvar_14;
					  highp vec4 c_19;
					  highp vec3 BRDF_20;
					  highp float NdotE_21;
					  highp float NdotL_22;
					  mediump float tmpvar_23;
					  tmpvar_23 = dot (tmpvar_2, LightDir_16);
					  NdotL_22 = tmpvar_23;
					  mediump float tmpvar_24;
					  tmpvar_24 = dot (tmpvar_2, viewDir_17);
					  NdotE_21 = tmpvar_24;
					  highp vec2 tmpvar_25;
					  tmpvar_25.x = (NdotE_21 * 0.8);
					  tmpvar_25.y = ((NdotL_22 * 0.5) + 0.5);
					  lowp vec3 tmpvar_26;
					  tmpvar_26 = texture2D (_Ramp2D, tmpvar_25).xyz;
					  BRDF_20 = tmpvar_26;
					  c_19.xyz = ((BRDF_20 * atten_18) * 0.5);
					  c_19.w = tmpvar_8;
					  tmpvar_15 = c_19;
					  c_1.xyz = tmpvar_15.xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out mediump vec3 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					vec4 t0;
					vec3 t1;
					float t6;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t6 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    vs_TEXCOORD0.xyz = t0.xyz;
					    t0.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t0.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t0.xyz;
					    t0.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t0.xyz;
					    vs_TEXCOORD1.xyz = _Object2World[3].xyz * in_POSITION0.www + t0.xyz;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					uniform lowp sampler2D _LightTextureB0;
					uniform lowp samplerCube _LightTexture0;
					uniform lowp sampler2D _Ramp2D;
					in mediump vec3 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec3 t16_1;
					vec3 t2;
					lowp float t10_2;
					float t6;
					float t9;
					mediump float t16_9;
					lowp float t10_9;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec3(0.0);
					t2 = vec3(0.0);
					t10_2 = float(0.0);
					t6 = float(0.0);
					t9 = float(0.0);
					t16_9 = float(0.0);
					t10_9 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceLightPos0.xyz;
					    t9 = dot(t0.xyz, t0.xyz);
					    t9 = inversesqrt(t9);
					    t0.xyz = vec3(t9) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, t0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t2.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    t6 = dot(t2.xyz, t2.xyz);
					    t6 = inversesqrt(t6);
					    t2.xyz = vec3(t6) * t2.xyz;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, t2.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_1.xyz = vs_TEXCOORD1.yyy * _LightMatrix0[1].xyz;
					    t16_1.xyz = _LightMatrix0[0].xyz * vs_TEXCOORD1.xxx + t16_1.xyz;
					    t16_1.xyz = _LightMatrix0[2].xyz * vs_TEXCOORD1.zzz + t16_1.xyz;
					    t16_1.xyz = t16_1.xyz + _LightMatrix0[3].xyz;
					    t9 = dot(t16_1.xyz, t16_1.xyz);
					    t10_2 = texture(_LightTexture0, t16_1.xyz).w;
					    t10_9 = texture(_LightTextureB0, vec2(t9)).w;
					    t16_9 = t10_2 * t10_9;
					    t16_0.xyz = vec3(t16_9) * t10_0.xyz;
					    t16_0.xyz = t16_0.xyz * vec3(0.5, 0.5, 0.5);
					    SV_Target0.xyz = t16_0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
					"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec3 _glesNormal;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 _Object2World;
					uniform highp mat4 _World2Object;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec3 worldNormal_1;
					  mediump vec3 tmpvar_2;
					  highp vec4 v_3;
					  v_3.x = _World2Object[0].x;
					  v_3.y = _World2Object[1].x;
					  v_3.z = _World2Object[2].x;
					  v_3.w = _World2Object[3].x;
					  highp vec4 v_4;
					  v_4.x = _World2Object[0].y;
					  v_4.y = _World2Object[1].y;
					  v_4.z = _World2Object[2].y;
					  v_4.w = _World2Object[3].y;
					  highp vec4 v_5;
					  v_5.x = _World2Object[0].z;
					  v_5.y = _World2Object[1].z;
					  v_5.z = _World2Object[2].z;
					  v_5.w = _World2Object[3].z;
					  highp vec3 tmpvar_6;
					  tmpvar_6 = normalize(((
					    (v_3.xyz * _glesNormal.x)
					   + 
					    (v_4.xyz * _glesNormal.y)
					  ) + (v_5.xyz * _glesNormal.z)));
					  worldNormal_1 = tmpvar_6;
					  tmpvar_2 = worldNormal_1;
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = (_Object2World * _glesVertex).xyz;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec3 _WorldSpaceCameraPos;
					uniform mediump vec4 _WorldSpaceLightPos0;
					uniform sampler2D _LightTexture0;
					uniform mediump mat4 _LightMatrix0;
					uniform sampler2D _MainTex;
					uniform sampler2D _Ramp2D;
					varying mediump vec3 xlv_TEXCOORD0;
					varying highp vec3 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  lowp vec3 tmpvar_2;
					  lowp vec3 worldViewDir_3;
					  lowp vec3 lightDir_4;
					  highp vec2 tmpvar_5;
					  tmpvar_5.x = 1.0;
					  mediump vec3 tmpvar_6;
					  tmpvar_6 = _WorldSpaceLightPos0.xyz;
					  lightDir_4 = tmpvar_6;
					  highp vec3 tmpvar_7;
					  tmpvar_7 = normalize((_WorldSpaceCameraPos - xlv_TEXCOORD1));
					  worldViewDir_3 = tmpvar_7;
					  tmpvar_2 = xlv_TEXCOORD0;
					  lowp float tmpvar_8;
					  mediump vec4 c_9;
					  lowp vec4 tmpvar_10;
					  tmpvar_10 = texture2D (_MainTex, tmpvar_5);
					  c_9 = tmpvar_10;
					  tmpvar_8 = c_9.w;
					  highp vec4 tmpvar_11;
					  tmpvar_11.w = 1.0;
					  tmpvar_11.xyz = xlv_TEXCOORD1;
					  highp vec2 tmpvar_12;
					  tmpvar_12 = (_LightMatrix0 * tmpvar_11).xy;
					  lowp float tmpvar_13;
					  tmpvar_13 = texture2D (_LightTexture0, tmpvar_12).w;
					  mediump vec4 tmpvar_14;
					  mediump vec3 LightDir_15;
					  LightDir_15 = lightDir_4;
					  mediump vec3 viewDir_16;
					  viewDir_16 = worldViewDir_3;
					  mediump float atten_17;
					  atten_17 = tmpvar_13;
					  highp vec4 c_18;
					  highp vec3 BRDF_19;
					  highp float NdotE_20;
					  highp float NdotL_21;
					  mediump float tmpvar_22;
					  tmpvar_22 = dot (tmpvar_2, LightDir_15);
					  NdotL_21 = tmpvar_22;
					  mediump float tmpvar_23;
					  tmpvar_23 = dot (tmpvar_2, viewDir_16);
					  NdotE_20 = tmpvar_23;
					  highp vec2 tmpvar_24;
					  tmpvar_24.x = (NdotE_20 * 0.8);
					  tmpvar_24.y = ((NdotL_21 * 0.5) + 0.5);
					  lowp vec3 tmpvar_25;
					  tmpvar_25 = texture2D (_Ramp2D, tmpvar_24).xyz;
					  BRDF_19 = tmpvar_25;
					  c_18.xyz = ((BRDF_19 * atten_17) * 0.5);
					  c_18.w = tmpvar_8;
					  tmpvar_14 = c_18;
					  c_1.xyz = tmpvar_14.xyz;
					  c_1.w = 1.0;
					  gl_FragData[0] = c_1;
					}
					
					
					#endif"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					in highp vec4 in_POSITION0;
					in highp vec3 in_NORMAL0;
					out mediump vec3 vs_TEXCOORD0;
					out highp vec3 vs_TEXCOORD1;
					vec4 t0;
					vec3 t1;
					float t6;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec3(0.0);
					t6 = float(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    t0.x = in_NORMAL0.x * _World2Object[0].x;
					    t0.y = in_NORMAL0.x * _World2Object[1].x;
					    t0.z = in_NORMAL0.x * _World2Object[2].x;
					    t1.x = in_NORMAL0.y * _World2Object[0].y;
					    t1.y = in_NORMAL0.y * _World2Object[1].y;
					    t1.z = in_NORMAL0.y * _World2Object[2].y;
					    t0.xyz = t0.xyz + t1.xyz;
					    t1.x = in_NORMAL0.z * _World2Object[0].z;
					    t1.y = in_NORMAL0.z * _World2Object[1].z;
					    t1.z = in_NORMAL0.z * _World2Object[2].z;
					    t0.xyz = t0.xyz + t1.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    vs_TEXCOORD0.xyz = t0.xyz;
					    t0.xyz = in_POSITION0.yyy * _Object2World[1].xyz;
					    t0.xyz = _Object2World[0].xyz * in_POSITION0.xxx + t0.xyz;
					    t0.xyz = _Object2World[2].xyz * in_POSITION0.zzz + t0.xyz;
					    vs_TEXCOORD1.xyz = _Object2World[3].xyz * in_POSITION0.www + t0.xyz;
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
					uniform 	mediump vec4 _WorldSpaceLightPos0;
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
					uniform 	lowp vec4 _LightColor0;
					uniform 	lowp vec4 _SpecColor;
					uniform 	mediump mat4x4 _LightMatrix0;
					uniform lowp sampler2D _LightTexture0;
					uniform lowp sampler2D _Ramp2D;
					in mediump vec3 vs_TEXCOORD0;
					in highp vec3 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec3 t0;
					mediump vec3 t16_0;
					lowp vec3 t10_0;
					mediump vec2 t16_1;
					float t6;
					lowp float t10_6;
					void main()
					{
					t0 = vec3(0.0);
					t16_0 = vec3(0.0);
					t10_0 = vec3(0.0);
					t16_1 = vec2(0.0);
					t6 = float(0.0);
					t10_6 = float(0.0);
					    t0.xyz = (-vs_TEXCOORD1.xyz) + _WorldSpaceCameraPos.xyz;
					    t6 = dot(t0.xyz, t0.xyz);
					    t6 = inversesqrt(t6);
					    t0.xyz = vec3(t6) * t0.xyz;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, t0.xyz);
					    t0.x = t16_1.x * 0.800000012;
					    t16_1.x = dot(vs_TEXCOORD0.xyz, _WorldSpaceLightPos0.xyz);
					    t0.y = t16_1.x * 0.5 + 0.5;
					    t10_0.xyz = texture(_Ramp2D, t0.xy).xyz;
					    t16_1.xy = vs_TEXCOORD1.yy * _LightMatrix0[1].xy;
					    t16_1.xy = _LightMatrix0[0].xy * vs_TEXCOORD1.xx + t16_1.xy;
					    t16_1.xy = _LightMatrix0[2].xy * vs_TEXCOORD1.zz + t16_1.xy;
					    t16_1.xy = t16_1.xy + _LightMatrix0[3].xy;
					    t10_6 = texture(_LightTexture0, t16_1.xy).w;
					    t16_0.xyz = vec3(t10_6) * t10_0.xyz;
					    t16_0.xyz = t16_0.xyz * vec3(0.5, 0.5, 0.5);
					    SV_Target0.xyz = t16_0.xyz;
					    SV_Target0.w = 1.0;
					    return;
					}
					#endif"
}
}
Program "fp" {
SubProgram "gles " {
Keywords { "POINT" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT" }
					"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL" }
					"!!GLES3"
}
SubProgram "gles " {
Keywords { "SPOT" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "SPOT" }
					"!!GLES3"
}
SubProgram "gles " {
Keywords { "POINT_COOKIE" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "POINT_COOKIE" }
					"!!GLES3"
}
SubProgram "gles " {
Keywords { "DIRECTIONAL_COOKIE" }
					"!!GLES"
}
SubProgram "gles3 " {
Keywords { "DIRECTIONAL_COOKIE" }
					"!!GLES3"
}
}
 }
}
Fallback "Diffuse"
}