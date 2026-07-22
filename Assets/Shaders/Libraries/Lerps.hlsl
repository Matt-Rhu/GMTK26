
#pragma region LerpClamped
float LerpClamped (float a, float b, float t)
{
    t = clamp(t, 0, 1);
    return (1 - t) * a + b * t;
}

float2 LerpClamped (float2 a, float2 b, float t)
{
    t = clamp(t, 0, 1);
    return (1 - t) * a + b * t;
}

float3 LerpClamped (float3 a, float3 b, float t)
{
    t = clamp(t, 0, 1);
    return (1 - t) * a + b * t;
}

float4 LerpClamped (float4 a, float4 b, float t)
{
    t = clamp(t, 0, 1);
    return (1 - t) * a + b * t;
}
#pragma endregion



#pragma region InvLerp
float InvLerp (float a, float b, float v)
{
    return (v - a) / (b - a);
}

float InvLerp (float2 a, float2 b, float2 v)
{
    float2 ba = b - a;
    return dot(v - a, ba) / dot(ba, ba);
}

float InvLerp (float3 a, float3 b, float3 v)
{
    float3 ba = b - a;
    return dot(v - a, ba) / dot(ba, ba);
}

float InvLerp (float4 a, float4 b, float4 v)
{
    float4 ba = b - a;
    return dot(v - a, ba) / dot(ba, ba);
}
#pragma endregion 



#pragma region InvLerpClamped
float InvLerpClamped (float a, float b, float v)
{
    return clamp((v - a) / (b - a), 0, 1);
}

float InvLerpClamped (float2 a, float2 b, float2 v)
{
    float2 ba = b - a;
    return clamp(dot(v - a, ba) / dot(ba, ba), 0, 1);
}

float InvLerpClamped (float3 a, float3 b, float3 v)
{
    float3 ba = b - a;
    return clamp(dot(v - a, ba) / dot(ba, ba), 0, 1);
}

float InvLerpClamped (float4 a, float4 b, float4 v)
{
    float4 ba = b - a;
    return clamp(dot(v - a, ba) / dot(ba, ba), 0, 1);
}
#pragma endregion 



#pragma region Remap
float Remap (float miO, float maO, float miN, float maN, float input)
{
    const float t = InvLerp(miO, maO, input);
    return lerp(miN, maN, t);
}


float2 Remap (float miO, float maO, float miN, float maN, float2 input)
{
    float2 o = 0;
    const float xT = InvLerp(miO, maO, input.x);
    const float yT = InvLerp(miO, maO, input.y);
    o.x = lerp(miN, maN, xT);
    o.y = lerp(miN, maN, yT);
    return o;
}

float2 Remap (float2 miO, float2 maO, float2 miN, float2 maN, float2 input)
{
    const float t = InvLerp(miO, maO, input);
    return lerp(miN, maN, t);
}


float3 Remap (float miO, float maO, float miN, float maN, float3 input)
{
    float3 o = 0;
    const float xT = InvLerp(miO, maO, input.x);
    const float yT = InvLerp(miO, maO, input.y);
    const float zT = InvLerp(miO, maO, input.z);
    o.x = lerp(miN, maN, xT);
    o.y = lerp(miN, maN, yT);
    o.z = lerp(miN, maN, zT);
    return o;
}

float3 Remap (float3 miO, float3 maO, float3 miN, float3 maN, float3 input)
{
    const float t = InvLerp(miO, maO, input);
    return lerp(miN, maN, t);
}


float4 Remap (float miO, float maO, float miN, float maN, float4 input)
{
    float4 o = 0;
    const float xT = InvLerp(miO, maO, input.x);
    const float yT = InvLerp(miO, maO, input.y);
    const float zT = InvLerp(miO, maO, input.z);
    const float wT = InvLerp(miO, maO, input.w);
    o.x = lerp(miN, maN, xT);
    o.y = lerp(miN, maN, yT);
    o.z = lerp(miN, maN, zT);
    o.w = lerp(miN, maN, wT);
    return o;
}

float4 Remap (float4 miO, float4 maO, float4 miN, float4 maN, float4 input)
{
    const float t = InvLerp(miO, maO, input);
    return lerp(miN, maN, t);
}
#pragma endregion



#pragma region RemapClamped
float RemapClamped (float miO, float maO, float miN, float maN, float input)
{
    const float t = InvLerpClamped(miO, maO, input);
    return LerpClamped(miN, maN, t);
}


float2 RemapClamped (float miO, float maO, float miN, float maN, float2 input)
{
    float2 o = 0;
    const float xT = InvLerpClamped(miO, maO, input.x);
    const float yT = InvLerpClamped(miO, maO, input.y);
    o.x = LerpClamped(miN, maN, xT);
    o.y = LerpClamped(miN, maN, yT);
    return o;
}

float2 RemapClamped (float2 miO, float2 maO, float2 miN, float2 maN, float2 input)
{
    const float t = InvLerpClamped(miO, maO, input);
    return LerpClamped(miN, maN, t);
}


float3 RemapClamped (float miO, float maO, float miN, float maN, float3 input)
{
    float3 o = 0;
    const float xT = InvLerpClamped(miO, maO, input.x);
    const float yT = InvLerpClamped(miO, maO, input.y);
    const float zT = InvLerpClamped(miO, maO, input.z);
    o.x = LerpClamped(miN, maN, xT);
    o.y = LerpClamped(miN, maN, yT);
    o.z = LerpClamped(miN, maN, zT);
    return o;
}

float3 RemapClamped (float3 miO, float3 maO, float3 miN, float3 maN, float3 input)
{
    const float t = InvLerpClamped(miO, maO, input);
    return LerpClamped(miN, maN, t);
}


float4 RemapClamped (float miO, float maO, float miN, float maN, float4 input)
{
    float4 o = 0;
    const float xT = InvLerpClamped(miO, maO, input.x);
    const float yT = InvLerpClamped(miO, maO, input.y);
    const float zT = InvLerpClamped(miO, maO, input.z);
    const float wT = InvLerpClamped(miO, maO, input.w);
    o.x = LerpClamped(miN, maN, xT);
    o.y = LerpClamped(miN, maN, yT);
    o.z = LerpClamped(miN, maN, zT);
    o.w = LerpClamped(miN, maN, wT);
    return o;
}

float4 RemapClamped (float4 miO, float4 maO, float4 miN, float4 maN, float4 input)
{
    const float t = InvLerpClamped(miO, maO, input);
    return LerpClamped(miN, maN, t);
}
#pragma endregion 