
struct BATCH {
	float4x4	Projection		;
	float4x4	View			;
	float4x4	World			;
	
	float4		ViewPos			;
	
	float4		SkyLightDir		;
	float4		SkyLightColor	;
	
	float4		LightPos0		;
	float4		LightPos1		;
	float4		LightPos2		;
	float4		LightColor0		;
	float4		LightColor1		;
	float4		LightColor2		;
};

struct VS_IN {
	float3 Position : POSITION;
	float3 Normal 	: NORMAL;
	float4 Color 	: COLOR;
	float2 TexCoord : TEXCOORD0;
};

struct PS_IN {
	float4 	Position 	: SV_POSITION;
	float4 	Color 		: COLOR;
	float2 	TexCoord 	: TEXCOORD0;
	float3	WNormal		: TEXCOORD1;
};


cbuffer 		CBBatch 	: 	register(b0) { BATCH Batch : packoffset( c0 ); }	
SamplerState	Sampler		: 	register(s0);
Texture2D		Texture 	: 	register(t0);
//Texture2D		Texture2 	: 	register(t1);

#if 0
$pixel +NEGATIVE
$vertex 
#endif
 
/*-----------------------------------------------------------------------------
	Shader functions :
-----------------------------------------------------------------------------*/
PS_IN VSMain( VS_IN input )
{
	PS_IN output 	= (PS_IN)0;
	
	float4 	pos		=	float4( input.Position, 1 ) - float4(Batch.ViewPos);
	float4	wPos	=	mul( pos,  Batch.World 		);
	float4	vPos	=	mul( wPos, Batch.View 		);
	float4	pPos	=	mul( vPos, Batch.Projection );
	float4	normal	=	mul( float4(input.Normal,0),  Batch.World 		);
	
	output.Position = pPos;
	output.Color 	= 1;
	output.TexCoord	= input.TexCoord;
	output.WNormal	= normal;
	
	return output;
}


float4 PSMain( PS_IN input ) : SV_Target
{
	//return dot( input.WNormal, normalize(float3(1,1,1)));
	//return Texture.Sample(Sampler, input.TexCoord);
	float4 color = float4(1.0f, 0.0f, 0.0f, 1.0f);
	return  color;
}







