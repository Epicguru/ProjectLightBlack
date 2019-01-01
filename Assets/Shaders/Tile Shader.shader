Shader "Tiles/Tile Shader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Back
		ZWrite Off
		Lighting Off
		Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            sampler2D _FillerTex;
			float4 _Regions[18]; // A vector4 for the texture region, another for the position within the tile.
			float4 _Filler; // The filler bounds.
			float2 _Offset; // Basically the position of the tile, used for scrolling the filler texture. It's a float4 but only x and y are used.

            fixed4 frag (v2f i) : SV_Target
            {
				const int TILE_SIZE = 32;
				const int EDGE = 5;

				fixed4 col;

				for(fixed j = 0; j < 9; j++)
				{
					if(_Regions[j * 2].x == -1)
					{
						continue;
					}

					float2 currentUV = i.uv;

					// Get the position within this tile that the region occupies. Expressed in the range 0-1.
					float4 pos = _Regions[j * 2 + 1];

					// Don't draw to current area if outside of it's bounds.
					if(currentUV.x <= pos.x + pos.z)
					{
						if(currentUV.x >= pos.x)
						{
							if(currentUV.y <= pos.y + pos.w)
							{
								if(currentUV.y >= pos.y)
								{
									float4 region = _Regions[j * 2];
									float w = pos.z; // Where 1 is a whole tile.
									float h = pos.w; // Where 1 is a whole tile.
									float ax = region.x + region.z * ((currentUV.x - pos.x) / w);
									float ay = region.y + region.w * ((currentUV.y - pos.y) / h);
									float2 adjustedUV = float2(ax, ay);
									fixed4 read = tex2D(_MainTex, adjustedUV);

									if(read.r == 1.0 && read.g == 0.0 && read.b == 1.0 && read.a == 1.0)
									{
										col = tex2D(_FillerTex, (currentUV + _Offset) * 0.25);
									}
									else
									{
										col = read;
									}

								}
							}
						}
					}					
				}
				col.rgb *= col.a;

                return col;
            }
            ENDCG
        }
    }
}
