Shader "Hidden/ShaderIndependentGlow_ortho" {
Properties {
 _SIG_GlowTint ("Color", Color) = (1,1,1,1)
 _SIG_GlowMask ("Glow mask(RGB)", 2D) = "white" { }
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  GpuProgramID 39895
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					attribute vec4 _glesMultiTexCoord0;
					uniform highp vec4 _ProjectionParams;
					uniform highp mat4 glstate_matrix_mvp;
					uniform highp mat4 glstate_matrix_modelview0;
					uniform highp vec4 _SIG_GlowMask_ST;
					varying highp vec4 xlv_TEXCOORD0;
					varying mediump vec2 xlv_TEXCOORD1;
					void main ()
					{
					  highp vec4 tmpvar_1;
					  highp vec4 tmpvar_2;
					  mediump vec2 tmpvar_3;
					  tmpvar_1 = (glstate_matrix_mvp * _glesVertex);
					  highp vec4 o_4;
					  highp vec4 tmpvar_5;
					  tmpvar_5 = (tmpvar_1 * 0.5);
					  highp vec2 tmpvar_6;
					  tmpvar_6.x = tmpvar_5.x;
					  tmpvar_6.y = (tmpvar_5.y * _ProjectionParams.x);
					  o_4.xy = (tmpvar_6 + tmpvar_5.w);
					  o_4.zw = tmpvar_1.zw;
					  tmpvar_2.xyw = o_4.xyw;
					  tmpvar_3 = ((_glesMultiTexCoord0.xy * _SIG_GlowMask_ST.xy) + _SIG_GlowMask_ST.zw);
					  tmpvar_2.z = -((glstate_matrix_modelview0 * _glesVertex).z);
					  gl_Position = tmpvar_1;
					  xlv_TEXCOORD0 = tmpvar_2;
					  xlv_TEXCOORD1 = tmpvar_3;
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform highp vec4 _ProjectionParams;
					uniform highp sampler2D _CameraDepthTexture;
					uniform sampler2D _SIG_GlowMask;
					uniform lowp vec4 _SIG_color;
					uniform highp float _SIG_ZShift;
					varying highp vec4 xlv_TEXCOORD0;
					varying mediump vec2 xlv_TEXCOORD1;
					void main ()
					{
					  lowp vec4 c_1;
					  highp float tmpvar_2;
					  tmpvar_2 = ((texture2DProj (_CameraDepthTexture, xlv_TEXCOORD0).x * _ProjectionParams.z) + _ProjectionParams.y);
					  c_1 = vec4(0.0, 0.0, 0.0, 0.0);
					  if (((xlv_TEXCOORD0.z * _SIG_ZShift) < tmpvar_2)) {
					    c_1 = (texture2D (_SIG_GlowMask, xlv_TEXCOORD1) * _SIG_color);
					  };
					  gl_FragData[0] = c_1;
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
					uniform 	vec4 _SIG_GlowMask_ST;
					uniform 	mediump vec4 _MainTex_TexelSize;
					uniform 	lowp vec4 _SIG_color;
					uniform 	float _SIG_ZShift;
					in highp vec4 in_POSITION0;
					in highp vec2 in_TEXCOORD0;
					out highp vec4 vs_TEXCOORD0;
					out mediump vec2 vs_TEXCOORD1;
					vec4 t0;
					vec4 t1;
					void main()
					{
					t0 = vec4(0.0);
					t1 = vec4(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    t0 = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
					    gl_Position = t0;
					    t0.y = t0.y * _ProjectionParams.x;
					    t1.xzw = t0.xwy * vec3(0.5, 0.5, 0.5);
					    vs_TEXCOORD0.w = t0.w;
					    vs_TEXCOORD0.xy = t1.zz + t1.xw;
					    t0.x = in_POSITION0.y * glstate_matrix_modelview0[1].z;
					    t0.x = glstate_matrix_modelview0[0].z * in_POSITION0.x + t0.x;
					    t0.x = glstate_matrix_modelview0[2].z * in_POSITION0.z + t0.x;
					    t0.x = glstate_matrix_modelview0[3].z * in_POSITION0.w + t0.x;
					    vs_TEXCOORD0.z = (-t0.x);
					    t0.xy = in_TEXCOORD0.xy * _SIG_GlowMask_ST.xy + _SIG_GlowMask_ST.zw;
					    vs_TEXCOORD1.xy = t0.xy;
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
					uniform 	vec4 _SIG_GlowMask_ST;
					uniform 	mediump vec4 _MainTex_TexelSize;
					uniform 	lowp vec4 _SIG_color;
					uniform 	float _SIG_ZShift;
					uniform highp sampler2D _CameraDepthTexture;
					uniform lowp sampler2D _SIG_GlowMask;
					in highp vec4 vs_TEXCOORD0;
					in mediump vec2 vs_TEXCOORD1;
					layout(location = 0) out lowp vec4 SV_Target0;
					vec2 t0;
					mediump vec4 t16_0;
					lowp vec4 t10_0;
					bool tb0;
					float t1;
					void main()
					{
					t0 = vec2(0.0);
					t16_0 = vec4(0.0);
					t10_0 = vec4(0.0);
					tb0 = bool(false);
					t1 = float(0.0);
					    t0.xy = vs_TEXCOORD0.xy / vs_TEXCOORD0.ww;
					    t0.x = texture(_CameraDepthTexture, t0.xy).x;
					    t0.x = t0.x * _ProjectionParams.z + _ProjectionParams.y;
					    t1 = vs_TEXCOORD0.z * _SIG_ZShift;
					#ifdef UNITY_ADRENO_ES3
					    tb0 = !!(t1<t0.x);
					#else
					    tb0 = t1<t0.x;
					#endif
					    if(tb0){
					        t10_0 = texture(_SIG_GlowMask, vs_TEXCOORD1.xy);
					        t16_0 = t10_0 * _SIG_color;
					        SV_Target0 = t16_0;
					    } else {
					        SV_Target0 = vec4(0.0, 0.0, 0.0, 0.0);
					    //ENDIF
					    }
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