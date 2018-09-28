Shader "FX/Clouds Tess" {
	Properties{
		_Color("Tint", Color) = (1,1,1,1)
		_Noise("Noise (RGB)", 2D) = "gray" {}
		_Detail("Detail (RGB)", 2D) = "gray" {}
		_TexAdd("WorldSpace Noise Add", Range(0,1)) = 0.1
		_TColor("Cloud Top Color", Color) = (1,0.6,1,1)
		_CloudColor("Cloud Base Color", Color) = (0.6,1,1,1)
		_RimColor("Rim Color", Color) = (0.6,1,1,1)
		_RimPower("Rim Power", Range(0,40)) = 20
		_Scale("World Scale", Range(0,1)) = 0.004
		_SmallScale("Small Scale", Range(0,1)) = 0.004
		_AnimSpeedX("Animation Speed X", Range(-2,2)) = 1
		_AnimSpeedY("Animation Speed Y", Range(-2,2)) = 1
		_AnimSpeedZ("Animation Speed Z", Range(-2,2)) = 1
		_Height("Noise Height", Range(0,50)) = 0.8
		_HeightD("Noise Height D", Range(0,50)) = 0.8
		_Strength("Noise Emission Strength", Range(0,2)) = 0.3
		_Tess("Tessellation", Range(1,32)) = 4

	}

		SubShader{
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Lambert vertex:disp tessellate:tessDistance nolightmap
#pragma target 4.6
#include "Tessellation.cginc"

	sampler2D _Noise, _Detail, _SnowRamp;
	float4 _Color, _CloudColor, _TColor, _RimColor;
	float _Scale, _Strength, _RimPower, _Height, _SmallScale, _HeightD, _TexAdd;
	float _AnimSpeedX, _AnimSpeedY, _AnimSpeedZ;

	struct Input {
		float3 viewDir;
		float4 noiseComb;
		float4 col;
		float3 worldPos;
		float3 worldNormal;
	};

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	float _Tess;

	float4 tessDistance(appdata v0, appdata v1, appdata v2) {
		float minDist = 10.0;
		float maxDist = 30.0;
		return UnityDistanceBasedTess(v0.vertex, v1.vertex, v2.vertex, minDist, maxDist, _Tess);
	}

	

	void disp(inout appdata v)
	{
		float3 worldSpaceNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal.xyz));// worldspace normal for blending
		float3 worldNormalS = saturate(pow(worldSpaceNormal * 1.4, 4)); // corrected blend value
		float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;// world position for projecting

		float movementSpeedX = _Time.x * _AnimSpeedX; //movement over X
		float movementSpeedY = _Time.x * _AnimSpeedY; //movement over Y
		float movementSpeedZ = _Time.x * _AnimSpeedZ; //movement over Z
		
		float4 xy = float4((worldPos.x * _Scale) - movementSpeedX, (worldPos.y * _Scale)- movementSpeedY, 0, 0); // xy texture projection over worldpos * scale and movement
		float4 xz = float4((worldPos.x * _Scale) - movementSpeedX, (worldPos.z * _Scale) - movementSpeedZ, 0, 0); // same with xz
		float4 yz = float4((worldPos.y * _Scale) - movementSpeedY, (worldPos.z * _Scale) - movementSpeedZ, 0, 0); // same with yz

		float4 xyD = float4((worldPos.x * _SmallScale) - movementSpeedX, (worldPos.y * _SmallScale) - movementSpeedY, 0, 0); // xy texture projection over worldpos * scale and movement
		float4 xzD = float4((worldPos.x * _SmallScale) - movementSpeedX, (worldPos.z * _SmallScale) - movementSpeedZ, 0, 0); // same with xz
		float4 yzD = float4((worldPos.y * _SmallScale) - movementSpeedY, (worldPos.z * _SmallScale) - movementSpeedZ, 0, 0); // same with yz

		float4 noiseXY = tex2Dlod(_Noise, xy);// textures projected
		float4 noiseXZ = tex2Dlod(_Noise, xz );
		float4 noiseYZ = tex2Dlod(_Noise, yz);

		float4 noiseComb = noiseXY; // combining the texture sides
		noiseComb = lerp(noiseComb, noiseXZ, worldNormalS.y);
		noiseComb = lerp(noiseComb, noiseYZ, worldNormalS.x);
		
		float4 detailNoiseXY = tex2Dlod(_Detail, xyD);
		float4 detailNoiseXZ = tex2Dlod(_Detail, xzD);
		float4 detailNoiseYZ = tex2Dlod(_Detail, yzD);

		float4 detailTex = detailNoiseXY;
		detailTex = lerp(detailTex, detailNoiseXZ, worldNormalS.y);
		detailTex = lerp(detailTex, detailNoiseYZ, worldNormalS.x);

		v.vertex.xyz += (v.normal *(noiseComb * _Height)); // displacement 
		v.vertex.xyz += (v.normal * (detailTex*_HeightD));
	}
	



	void surf(Input IN, inout SurfaceOutput o) {

		float3 localPos = (IN.worldPos - mul(unity_ObjectToWorld, float4(0, 0, 0, 1)).xyz); // local position
		
		float3 worldNormalS = saturate(pow(IN.worldNormal * 1.4, 4)); // corrected blend value

		float movementSpeedX = _Time.x * _AnimSpeedX; //movement over X
		float movementSpeedY = _Time.x * _AnimSpeedY; //movement over Y
		float movementSpeedZ = _Time.x * _AnimSpeedZ; //movement over Z

		float2 xy = float2((IN.worldPos.x * _Scale) - movementSpeedX, (IN.worldPos.y * _Scale) - movementSpeedY); // xy texture projection over worldpos * scale and movement
		float2 xz = float2((IN.worldPos.x * _Scale) - movementSpeedX, (IN.worldPos.z * _Scale) - movementSpeedZ); // same with xz
		float2 yz = float2((IN.worldPos.y * _Scale) - movementSpeedY, (IN.worldPos.z * _Scale) - movementSpeedZ); // same with yz

		float2 xyD = float2((IN.worldPos.x * _SmallScale) - movementSpeedX, (IN.worldPos.y * _SmallScale) - movementSpeedY); // xy texture projection over worldpos * scale and movement
		float2 xzD = float2((IN.worldPos.x * _SmallScale) - movementSpeedX, (IN.worldPos.z * _SmallScale) - movementSpeedZ); // same with xz
		float2 yzD = float2((IN.worldPos.y * _SmallScale) - movementSpeedY, (IN.worldPos.z * _SmallScale) - movementSpeedZ); // same with yz

		float4 noiseXY = tex2D(_Noise, xy);// textures projected
		float4 noiseXZ = tex2D(_Noise, xz);
		float4 noiseYZ = tex2D(_Noise, yz);

		float4 noiseComb = noiseXY; // combining the texture sides
		noiseComb = lerp(noiseComb, noiseXZ, worldNormalS.y);
		noiseComb = lerp(noiseComb, noiseYZ, worldNormalS.x);
		

		float4 detailNoiseXY = tex2D(_Detail, xyD);
		float4 detailNoiseXZ = tex2D(_Detail, xzD);
		float4 detailNoiseYZ = tex2D(_Detail, yzD);

		float4 detailTex = detailNoiseXY;
		detailTex = lerp(detailTex, detailNoiseXZ, worldNormalS.y);
		detailTex = lerp(detailTex, detailNoiseYZ, worldNormalS.x);

		noiseComb += (detailTex * 0.2);
		half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal * (noiseComb))); // rimlight using normal and noise
		o.Emission = _RimColor.rgb *pow(rim, _RimPower); // add glow rimlight to the clouds 
		o.Albedo = (lerp(_CloudColor, _TColor , saturate(localPos.y + noiseComb))) * _Color ;
		o.Albedo += (noiseComb* _TexAdd);
	}
	ENDCG

	}

		Fallback "Diffuse"
}