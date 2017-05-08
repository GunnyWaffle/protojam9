
//Adapted from https://github.com/libretro/common-shaders/blob/master/crt/shaders/crt-geom.cg 
//for use with Unity's flavor of the Cg language
/*
CRT-interlaced

Copyright (C) 2010-2012 cgwg, Themaister and DOLLS

This program is free software; you can redistribute it and/or modify it
under the terms of the GNU General Public License as published by the Free
Software Foundation; either version 2 of the License, or (at your option)
any later version.

(cgwg gave their consent to have the original version of this shader
distributed under the GPL in this message:

http://board.byuu.org/viewtopic.php?p=26075#p26075

"Feel free to distribute my shaders under the GPL. After all, the
barrel distortion code was taken from the Curvature shader, which is
under the GPL."
)
This shader variant is pre-configured with screen curvature
*/

Shader "Custom/CRTShader"
{
	Properties
	{
		[PerRenderer] _MainTex("Texture", 2D) = "white" {}
		[PerRenderer] _crtGamma("Target Gamma", Range(0.1, 5.0)) = 2.4
		[PerRenderer] _monitorGamma("Monitor Gamma", Range(0.1, 5.0)) = 2.2
		[PerRenderer] _distance("Distance", Range(0.1, 3.0)) = 1.5
		[Toggle(CURVATURE)] _curvature("Curvature Toggle", Float) = 1.0
		[PerRenderer] _radius("Curvature Radius", Range(0.1, 10.0)) = 2.0
		[PerRenderer] _cornerSize("Corner Size", Range(0.001, 1.0)) = 0.03
		[PerRenderer] _cornerSmooth("Corner Smoothness", Range(80.0, 2000.0)) = 1000.0
		[PerRenderer] _xTilt("Horizontal Tilt", Range(-0.5, 0.5)) = 0.0
		[PerRenderer] _yTilt("Vertical Tilt", Range(-0.5, 0.5)) = 0.0
		[PerRenderer] _overscanX("Horizontal Overscan %", Range(-125.0, 125.0)) = 100.0
		[PerRenderer] _overscanY("Vertical Overscan %", Range(-125.0, 125.0)) = 100.0
		[Toggle(DOTMASK)] _dotmask("Dot Mask Toggle", Float) = 1.0 //Should be 0 - 0.3
		[PerRenderer] _sharper("Sharpness", Range(1.0, 3.0)) = 1.0
		[PerRenderer] _scanlineWeight("Scanline Weight", Range(0.1, 0.5)) = 0.3
		[PerRenderer] _lum("Luminance Boost", Range(0.0, 1.0)) = 0.0
		[Toggle(INTERLACED)] _interlace("Interlace Toggle", Float) = 1.0 //Should be 1.0 - 5.0
		[Toggle(USE_GAUSSIAN)] _gaussian("Use Gaussian beam profile", Float) = 0.0
		[Toggle(OVERSAMPLE)] _oversample("Oversampled beam profile", Float) = 1.0
		[Toggle(LINEAR_PROCESSING)] _linearProcessing("Process in linear gamma space", Float) = 1.0
		[PerRenderer] _frameCount("Frame Count", Int) = 0
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			#define FIX(c) max(abs(c), 1e-5);
			#define PI 3.141592653589

			#ifdef LINEAR_PROCESSING
			#	define TEX2D(c) pow(tex2D(_MainTex, (c)), float4(_crtGamma))
			#else
			#	define TEX2D(c) tex2D(_MainTex, (c))
			#endif


			uniform sampler2D _MainTex;
			float4 _MainTex_TexelSize;
			uniform float _crtGamma;
			uniform float _monitorGamma;
			uniform float _distance;
			uniform float _curvature;
			uniform float _radius;
			uniform float _cornerSize;
			uniform float _cornerSmooth;
			uniform float _xTilt;
			uniform float _yTilt;
			uniform float _overscanX;
			uniform float _overscanY;
			uniform float _dotmask;
			uniform float _sharper;
			uniform float _scanlineWeight;
			uniform float _lum;
			uniform float _interlace;
			uniform float _gaussian;
			uniform float _oversample;
			uniform float _linearProcessing;
			uniform int _frameCount;

			static float2 aspect = float2(1.0, 0.75);

			float intersect(float2 xy, float2 sinangle, float2 cosangle)
			{
				float A = dot(xy, xy) + _distance*_distance;
				float B = 2.0*(_radius*(dot(xy, sinangle) - _distance*cosangle.x*cosangle.y) - _distance*_distance);
				float C = _distance*_distance + 2.0*_radius*_distance*cosangle.x*cosangle.y;
				return (-B - sqrt(B*B - 4.0*A*C)) / (2.0*A);
			}

			float2 bkwtrans(float2 xy, float2 sinangle, float2 cosangle)
			{
				float c = intersect(xy, sinangle, cosangle);
				float2 p = float2(c, c)*xy;
				p -= float2(-_radius, -_radius)*sinangle;
				p /= float2(_radius, _radius);
				float2 tang = sinangle / cosangle;
				float2 poc = p / cosangle;
				float A = dot(tang, tang) + 1.0;
				float B = -2.0*dot(poc, tang);
				float C = dot(poc, poc) - 1.0;
				float a = (-B + sqrt(B*B - 4.0*A*C)) / (2.0*A);
				float2 uv = (p - a*sinangle) / cosangle;
				float r = FIX(_radius*acos(a));
				return uv*r / sin(r / _radius);
			}

			float2 fwtrans(float2 uv, float2 sinangle, float2 cosangle)
			{
				float r = FIX(sqrt(dot(uv, uv)));
				uv *= sin(r / _radius) / r;
				float x = 1.0 - cos(r / _radius);
				float D = _distance / _radius + x*cosangle.x*cosangle.y + dot(uv, sinangle);
				return _distance*(uv*cosangle - x*sinangle) / D;
			}

			float3 maxscale(float2 sinangle, float2 cosangle)
			{
				float2 c = bkwtrans(-_radius * sinangle / (1.0 + _radius / _distance*cosangle.x*cosangle.y), sinangle, cosangle);
				float2 a = float2(0.5, 0.5)*aspect;
				float2 lo = float2(fwtrans(float2(-a.x, c.y), sinangle, cosangle).x,
					fwtrans(float2(c.x, -a.y), sinangle, cosangle).y) / aspect;
				float2 hi = float2(fwtrans(float2(+a.x, c.y), sinangle, cosangle).x,
					fwtrans(float2(c.x, +a.y), sinangle, cosangle).y) / aspect;
				return float3((hi + lo)*aspect*0.5, max(hi.x - lo.x, hi.y - lo.y));
			}

			// Calculate the influence of a scanline on the current pixel.
			//
			// 'distance' is the distance in texture coordinates from the current
			// pixel to the scanline in question.
			// 'color' is the colour of the scanline at the horizontal location of
			// the current pixel.
			float4 scanlineWeights(float distance, float4 color)
			{
				// "wid" controls the width of the scanline beam, for each RGB
				// channel The "weights" lines basically specify the formula
				// that gives you the profile of the beam, i.e. the intensity as
				// a function of distance from the vertical center of the
				// scanline. In this case, it is gaussian if width=2, and
				// becomes nongaussian for larger widths. Ideally this should
				// be normalized so that the integral across the beam is
				// independent of its width. That is, for a narrower beam
				// "weights" should have a higher peak at the center of the
				// scanline than for a wider beam.
	#ifdef USE_GAUSSIAN
				float4 wid = 0.3 + 0.1 * pow(color, float4(3.0, 3.0, 3.0, 3.0));
				float4 weights = float4(distance / (wid * _scanlineWeight / 0.3));
				return (_lum + 0.4) * exp(-weights * weights) / wid;
	#else
				float4 wid = 2.0 + 2.0 * pow(color, float4(4.0, 4.0, 4.0, 4.0));
				float w = distance / _scanlineWeight;
				float4 weights = float4(w, w, w, w);
				return (_lum + 1.4) * exp(-pow(weights * rsqrt(0.5 * wid), wid)) / (0.6 + 0.2 * wid);
	#endif
			}

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float2 one : TEXCOORD1;
				float mod_factor : TEXCOORD2;
				float2 ilFac : TEXCOORD3;
				float3 stretch : TEXCOORD4;
				float2 sinAngle : TEXCOORD5;
				float2 cosAngle : TEXCOORD6;
				float2 texSize : TEXCOORD7;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.sinAngle = sin(float2(_xTilt, _yTilt));
				o.cosAngle = cos(float2(_xTilt, _yTilt));
				o.stretch = maxscale(o.sinAngle, o.cosAngle);
				o.texSize = float2(_sharper * _MainTex_TexelSize.z, _MainTex_TexelSize.w);
				o.ilFac = float2(1.0, clamp(floor(_MainTex_TexelSize.w / (200.0 * (_interlace * 4.0 + 1.0))), 1.0, 2.0));
				o.one = o.ilFac / o.texSize;
				o.mod_factor = o.uv.x * _MainTex_TexelSize.z;
				return o;
			}


			/*fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				// just invert the colors
				col = 1 - col;
				return col;
			}*/

			fixed4 frag(v2f input) : SV_Target
			{
				fixed4 color;

				float2 xy = float2(0.0, 0.0);
				if (_curvature > 0.5)
				{
					float2 cd = input.uv;
					//cd *= input.texSize / input.vidSize;
					cd = (cd - float2(0.5, 0.5)) * aspect  * input.stretch.z + input.stretch.xy;
					xy = (bkwtrans(cd, input.sinAngle, input.cosAngle) / float2(_overscanX / 100.0, _overscanY / 100.0) / aspect + float2(0.5, 0.5)); //*input.video_size / input.texture_size
				}
				else
				{
					xy = input.uv;
				}

				float2 cd = xy;
				//cd *= input.texture_size/ input.video_size;
				cd = (cd - float2(0.5, 0.5)) * float2(_overscanX / 100.0, _overscanY / 100.0) + float2(0.5, 0.5);
				cd = min(cd, float2(1.0, 1.0) - cd) * aspect;
				float2 cDist = float2(_cornerSize, _cornerSize);
				cd = (cDist - min(cd, cDist));
				float dist = sqrt(dot(cd, cd));
				float cVal = clamp((cDist.x - dist)*_cornerSmooth, 0.0, 1.0);

				float2 xy2 = ((xy - float2(0.5, 0.5)) * float2(1.0, 1.0) + float2(0.5, 0.5));
				float2 ilFloat = float2(0.0, input.ilFac.y > 1.5 ? fmod(float(_frameCount), 2.0) : 0.0);

				float2 ratioScale = (xy * input.texSize - float2(0.5, 0.5) + ilFloat) / input.ilFac;

				float2 uvRatio = frac(ratioScale);
				xy = (floor(ratioScale) * input.ilFac + float2(0.5, 0.5) - ilFloat) / input.texSize;

				float4 coeffs = PI * float4(1.0 + uvRatio.x, uvRatio.x, 1.0 - uvRatio.x, 2.0 - uvRatio.x);
				coeffs = FIX(coeffs);
				coeffs = 2.0 * sin(coeffs) * sin(coeffs / 2.0) / (coeffs * coeffs);
				coeffs /= dot(coeffs, (1.0).rrrr);

				float4 col = clamp(mul(coeffs, float4x4(
					TEX2D(xy + float2(-input.one.x, 0.0)),
					TEX2D(xy),
					TEX2D(xy + float2(input.one.x, 0.0)),
					TEX2D(xy + float2(2.0 * input.one.x, 0.0))
				)), 0.0, 1.0);

				float4 col2 = clamp(mul(coeffs, float4x4(
					TEX2D(xy + float2(-input.one.x, input.one.y)),
					TEX2D(xy + float2(0.0, input.one.y)),
					TEX2D(xy + input.one),
					TEX2D(xy + float2(2.0 * input.one.x, input.one.y))
				)), 0.0, 1.0);

				#ifndef LINEAR_PROCESSING
				col = pow(col, _crtGamma.rrrr);
				col2 = pow(col2, _crtGamma.rrrr);
				#endif

				float4 weights = scanlineWeights(uvRatio.y, col);
				float4 weights2 = scanlineWeights(1.0 - uvRatio.y, col2);

				#ifdef OVERSAMPLE
				uvRatio.y = uvRatio.y + 1.0 / 3.0;
				weights = (weights + scanlineWeights(uvRatio.y, col)) / 3.0;
				weights2 = (weights2 + scanlineWeights(abs(1.0 - uvRatio.y), col2)) / 3.0;
				uvRatio.y = uvRatio.y - 2 / 3.0;
				weights = weights + scanlineWeights(abs(uvRatio.y), col) / 3.0;
				weights2 = weights2 + scanlineWeights(abs(1.0 - uvRatio.y), col2) / 3.0;
				#endif

				float3 mulRes = (col * weights + col2 * weights2).rgb;
				mulRes *= cVal.rrr;

				float3 dotMaskWeights = lerp(
					float3(1.0, 1.0 - _dotmask * 0.3, 1.0),
					float3(1.0 - _dotmask * 0.3, 1.0, 1.0 - _dotmask * 0.3),
					floor(fmod(input.mod_factor, 2.0)).rrr
				);

				mulRes = pow(mulRes, 1.0 / _monitorGamma);

				return fixed4(mulRes, 1.0);
			}
			ENDCG
		}
	}

	Fallback off
}
