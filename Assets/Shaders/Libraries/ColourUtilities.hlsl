float Luminance(float3 colour)
{
    return dot(colour, float3(0.299f, 0.587f, 0.114f));
}

float3 HueToRGB(float hue)
{
    hue = frac(hue);
    
    float r = abs(hue * 6 - 3) - 1;
    float g = 2 - abs(hue * 6 - 2);
    float b = 2 - abs(hue * 6 - 4);
    float3 rgb = float3(r, g, b);
    rgb = saturate(rgb);
    
    return rgb;
}

float3 HSVtoRGB(float3 hsv)
{
    float3 rgb = HueToRGB(hsv.x);
    rgb = lerp(1, rgb, hsv.y);
    rgb = rgb * hsv.z;

    return rgb;
}

float3 RGBtoHSV(float3 rgb)
{
    float maxComponent = max(rgb.r, max(rgb.g, rgb.b));
    float minComponent = min(rgb.r, min(rgb.g, rgb.b));
    float diff = maxComponent - minComponent;

    float hue = 0;

    if (maxComponent == rgb.r)
        hue = 0 + (rgb.g - rgb.b) / diff;
    else if (maxComponent == rgb.g)
        hue = 2 + (rgb.b - rgb.r) / diff;
    else if (maxComponent == rgb.b)
        hue = 4 + (rgb.r - rgb.g) / diff;

    hue = frac(hue / 6);
    float saturation = diff/maxComponent;
    float value = maxComponent;

    return float3(hue, saturation, value);
}