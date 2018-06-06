﻿Shader "Custom/PlatformShader" {
	Properties{
		_DeleteHighlight("DeleteHighlight", Float) = 0
	}
	SubShader{
		
		Pass{
		//Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		//LOD 100
		//
		//ZWrite Off
		//Blend SrcAlpha OneMinusSrcAlpha

		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"

		float _DeleteHighlight;

		float3 mod(float3 x, float3 y)
	{
		return x - y * floor(x / y);
	}

	float3 mod289(float3 x)
	{
		return x - floor(x / 289.0) * 289.0;
	}

	float4 mod289(float4 x)
	{
		return x - floor(x / 289.0) * 289.0;
	}

	float4 permute(float4 x)
	{
		return mod289(((x*34.0) + 1.0)*x);
	}

	float4 taylorInvSqrt(float4 r)
	{
		return (float4)1.79284291400159 - r * 0.85373472095314;
	}

	float3 fade(float3 t) {
		return t * t*t*(t*(t*6.0 - 15.0) + 10.0);
	}

	// Classic Perlin noise
	float cnoise(float3 P)
	{
		float3 Pi0 = floor(P); // Integer part for indexing
		float3 Pi1 = Pi0 + (float3)1.0; // Integer part + 1
		Pi0 = mod289(Pi0);
		Pi1 = mod289(Pi1);
		float3 Pf0 = frac(P); // Fractional part for interpolation
		float3 Pf1 = Pf0 - (float3)1.0; // Fractional part - 1.0
		float4 ix = float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
		float4 iy = float4(Pi0.y, Pi0.y, Pi1.y, Pi1.y);
		float4 iz0 = (float4)Pi0.z;
		float4 iz1 = (float4)Pi1.z;

		float4 ixy = permute(permute(ix) + iy);
		float4 ixy0 = permute(ixy + iz0);
		float4 ixy1 = permute(ixy + iz1);

		float4 gx0 = ixy0 / 7.0;
		float4 gy0 = frac(floor(gx0) / 7.0) - 0.5;
		gx0 = frac(gx0);
		float4 gz0 = (float4)0.5 - abs(gx0) - abs(gy0);
		float4 sz0 = step(gz0, (float4)0.0);
		gx0 -= sz0 * (step((float4)0.0, gx0) - 0.5);
		gy0 -= sz0 * (step((float4)0.0, gy0) - 0.5);

		float4 gx1 = ixy1 / 7.0;
		float4 gy1 = frac(floor(gx1) / 7.0) - 0.5;
		gx1 = frac(gx1);
		float4 gz1 = (float4)0.5 - abs(gx1) - abs(gy1);
		float4 sz1 = step(gz1, (float4)0.0);
		gx1 -= sz1 * (step((float4)0.0, gx1) - 0.5);
		gy1 -= sz1 * (step((float4)0.0, gy1) - 0.5);

		float3 g000 = float3(gx0.x,gy0.x,gz0.x);
		float3 g100 = float3(gx0.y,gy0.y,gz0.y);
		float3 g010 = float3(gx0.z,gy0.z,gz0.z);
		float3 g110 = float3(gx0.w,gy0.w,gz0.w);
		float3 g001 = float3(gx1.x,gy1.x,gz1.x);
		float3 g101 = float3(gx1.y,gy1.y,gz1.y);
		float3 g011 = float3(gx1.z,gy1.z,gz1.z);
		float3 g111 = float3(gx1.w,gy1.w,gz1.w);

		float4 norm0 = taylorInvSqrt(float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
		g000 *= norm0.x;
		g010 *= norm0.y;
		g100 *= norm0.z;
		g110 *= norm0.w;

		float4 norm1 = taylorInvSqrt(float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
		g001 *= norm1.x;
		g011 *= norm1.y;
		g101 *= norm1.z;
		g111 *= norm1.w;

		float n000 = dot(g000, Pf0);
		float n100 = dot(g100, float3(Pf1.x, Pf0.y, Pf0.z));
		float n010 = dot(g010, float3(Pf0.x, Pf1.y, Pf0.z));
		float n110 = dot(g110, float3(Pf1.x, Pf1.y, Pf0.z));
		float n001 = dot(g001, float3(Pf0.x, Pf0.y, Pf1.z));
		float n101 = dot(g101, float3(Pf1.x, Pf0.y, Pf1.z));
		float n011 = dot(g011, float3(Pf0.x, Pf1.y, Pf1.z));
		float n111 = dot(g111, Pf1);

		float3 fade_xyz = fade(Pf0);
		float4 n_z = lerp(float4(n000, n100, n010, n110), float4(n001, n101, n011, n111), fade_xyz.z);
		float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
		float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x);
		return 2.2 * n_xyz;
	}

	// Classic Perlin noise, periodic variant
	float pnoise(float3 P, float3 rep)
	{
		float3 Pi0 = mod(floor(P), rep); // Integer part, modulo period
		float3 Pi1 = mod(Pi0 + (float3)1.0, rep); // Integer part + 1, mod period
		Pi0 = mod289(Pi0);
		Pi1 = mod289(Pi1);
		float3 Pf0 = frac(P); // Fractional part for interpolation
		float3 Pf1 = Pf0 - (float3)1.0; // Fractional part - 1.0
		float4 ix = float4(Pi0.x, Pi1.x, Pi0.x, Pi1.x);
		float4 iy = float4(Pi0.y, Pi0.y, Pi1.y, Pi1.y);
		float4 iz0 = (float4)Pi0.z;
		float4 iz1 = (float4)Pi1.z;

		float4 ixy = permute(permute(ix) + iy);
		float4 ixy0 = permute(ixy + iz0);
		float4 ixy1 = permute(ixy + iz1);

		float4 gx0 = ixy0 / 7.0;
		float4 gy0 = frac(floor(gx0) / 7.0) - 0.5;
		gx0 = frac(gx0);
		float4 gz0 = (float4)0.5 - abs(gx0) - abs(gy0);
		float4 sz0 = step(gz0, (float4)0.0);
		gx0 -= sz0 * (step((float4)0.0, gx0) - 0.5);
		gy0 -= sz0 * (step((float4)0.0, gy0) - 0.5);

		float4 gx1 = ixy1 / 7.0;
		float4 gy1 = frac(floor(gx1) / 7.0) - 0.5;
		gx1 = frac(gx1);
		float4 gz1 = (float4)0.5 - abs(gx1) - abs(gy1);
		float4 sz1 = step(gz1, (float4)0.0);
		gx1 -= sz1 * (step((float4)0.0, gx1) - 0.5);
		gy1 -= sz1 * (step((float4)0.0, gy1) - 0.5);

		float3 g000 = float3(gx0.x,gy0.x,gz0.x);
		float3 g100 = float3(gx0.y,gy0.y,gz0.y);
		float3 g010 = float3(gx0.z,gy0.z,gz0.z);
		float3 g110 = float3(gx0.w,gy0.w,gz0.w);
		float3 g001 = float3(gx1.x,gy1.x,gz1.x);
		float3 g101 = float3(gx1.y,gy1.y,gz1.y);
		float3 g011 = float3(gx1.z,gy1.z,gz1.z);
		float3 g111 = float3(gx1.w,gy1.w,gz1.w);

		float4 norm0 = taylorInvSqrt(float4(dot(g000, g000), dot(g010, g010), dot(g100, g100), dot(g110, g110)));
		g000 *= norm0.x;
		g010 *= norm0.y;
		g100 *= norm0.z;
		g110 *= norm0.w;
		float4 norm1 = taylorInvSqrt(float4(dot(g001, g001), dot(g011, g011), dot(g101, g101), dot(g111, g111)));
		g001 *= norm1.x;
		g011 *= norm1.y;
		g101 *= norm1.z;
		g111 *= norm1.w;

		float n000 = dot(g000, Pf0);
		float n100 = dot(g100, float3(Pf1.x, Pf0.y, Pf0.z));
		float n010 = dot(g010, float3(Pf0.x, Pf1.y, Pf0.z));
		float n110 = dot(g110, float3(Pf1.x, Pf1.y, Pf0.z));
		float n001 = dot(g001, float3(Pf0.x, Pf0.y, Pf1.z));
		float n101 = dot(g101, float3(Pf1.x, Pf0.y, Pf1.z));
		float n011 = dot(g011, float3(Pf0.x, Pf1.y, Pf1.z));
		float n111 = dot(g111, Pf1);

		float3 fade_xyz = fade(Pf0);
		float4 n_z = lerp(float4(n000, n100, n010, n110), float4(n001, n101, n011, n111), fade_xyz.z);
		float2 n_yz = lerp(n_z.xy, n_z.zw, fade_xyz.y);
		float n_xyz = lerp(n_yz.x, n_yz.y, fade_xyz.x);
		return 2.2 * n_xyz;
	}

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
			float3 objPos : TEXCOORD1;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.objPos = v.vertex.xyz + float3(1.0, 1.0, 1.0) / float3(2.0, 2.0, 2.0);
			o.uv = v.texcoord.xy;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float3 worldScale = float3(
				length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
				length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
				length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
				);

			

			float3 distances;
			distances.x = worldScale.x*((0.5 - abs(i.objPos.x - 0.5)) * 2);
			distances.y = worldScale.y*((0.5 - abs(i.objPos.y - 0.5)) * 2);
			distances.z = worldScale.z*((0.5 - abs(i.objPos.z - 0.5)) * 2);
			int2 importantAxis;
			if (i.objPos.z == 0 || i.objPos.z > 0.9999) {
				importantAxis.x = 0;
				importantAxis.y = 1;
			} else if (i.objPos.x == 0 || i.objPos.x > 0.9999) {
				importantAxis.x = 2;
				importantAxis.y = 1;
			} else {
				importantAxis.x = 0;
				importantAxis.y = 2;
			}
			float distToEdge = min(distances[importantAxis.x], distances[importantAxis.y]);
			//return fixed4((distances[importantAxis.y]-worldScale[importantAxis.y]/2)/length(worldScale)/10, 0, 0, 1);
			float3 p = float3(_SinTime.z * 5, i.objPos[importantAxis.x], i.objPos[importantAxis.y]);
			distToEdge *= pnoise(p, float3(1.5, 1.5, 1.5)) * 0.5 + pnoise(float3(p.x, p.y*2, p.z*2*worldScale[importantAxis.y]/worldScale[importantAxis.x]), float3(1.5, 1.5, 1.5)) * 0.2 + 1;
			distToEdge = pow(1 - distToEdge, 7);
			if (!_DeleteHighlight) {
				return fixed4(distToEdge, distToEdge, distToEdge, 1);
			}
			else {
				return fixed4(distToEdge, 0, 0, 1);
			}
			//return fixed4(worldScale.z/length(worldScale), 0, 0,1);
		}
		ENDCG

		}
	}
}
/*
Shader "Custom/PlatformShader" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader {

			Pass {
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		//#pragma surface surf Standard fullforwardshadows

#pragma vertex vert
#pragma fragment frag

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		// Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
		// See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
		// #pragma instancing_options assumeuniformscaling
		//UNITY_INSTANCING_BUFFER_START(Props)
			// put more per-instance properties here
		//UNITY_INSTANCING_BUFFER_END(Props)

		struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 texcol;
			texcol.rgb = uv.x;
			texcol.a = 1.0;
			return texcol;
		}
		ENDCG
			}
	}
	FallBack "Diffuse"
}
*/