Shader "KFF/Unlit Opaque White" {
Properties {
 _Color ("Color", Color) = (1,1,1,1)
}
SubShader { 
 Tags { "RenderType"="Opaque" }
 Pass {
  Tags { "RenderType"="Opaque" }
  GpuProgramID 28924
Program "vp" {
SubProgram "gles " {
"!!GLES
					#version 100
					
					#ifdef VERTEX
					attribute vec4 _glesVertex;
					uniform highp mat4 glstate_matrix_mvp;
					void main ()
					{
					  gl_Position = (glstate_matrix_mvp * _glesVertex);
					}
					
					
					#endif
					#ifdef FRAGMENT
					uniform mediump vec4 _Color;
					void main ()
					{
					  gl_FragData[0] = _Color;
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
					uniform 	mediump vec4 _Color;
					in highp vec4 in_POSITION0;
					vec4 t0;
					void main()
					{
					t0 = vec4(0.0);
					    t0 = in_POSITION0.yyyy * glstate_matrix_mvp[1];
					    t0 = glstate_matrix_mvp[0] * in_POSITION0.xxxx + t0;
					    t0 = glstate_matrix_mvp[2] * in_POSITION0.zzzz + t0;
					    gl_Position = glstate_matrix_mvp[3] * in_POSITION0.wwww + t0;
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
					uniform 	mediump vec4 _Color;
					layout(location = 0) out mediump vec4 SV_Target0;
					void main()
					{
					    SV_Target0 = _Color;
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
Fallback "Diffuse"
}