#include "UnityCG.cginc"

//SINE
float easeInSine(float input)
{
    input = saturate(input);
    return 1 - cos(input * UNITY_PI / 2);
}
float easeOutSine(float input)
{
    input = saturate(input);
    return sin(input * UNITY_PI / 2);
}
float easeInOutSine(float input)
{
    input = saturate(input);
    return -(cos(UNITY_PI * input) - 1) / 2;
}

//QUAD
float easeInQuad(float input)
{
    input = saturate(input);
    return input * input;
}
float easeOutQuad(float input)
{
    input = saturate(input);
    return 1 - (1 - input) * (1 - input);
}
float easeInOutQuad(float input)
{
    input = saturate(input);
    if (input < 0.5) return 2 * input * input;
    return 1 - pow(-2 * input + 2, 2) / 2;
}

//CUBIC
float easeInCubic(float input)
{
    input = saturate(input);
    return input * input * input;
}
float easeOutCubic(float input)
{
    input = saturate(input);
    return 1 - pow(1 - input, 3);
}
float easeInOutCubic(float input)
{
    input = saturate(input);
    if (input < 0.5) return 4 * input * input * input;
    return 1 - pow(-2 * input + 2, 3) / 2;
}

//QUART
float easeInQuart(float input)
{
    input = saturate(input);
    return input * input * input * input;
}
float easeOutQuart(float input)
{
    input = saturate(input);
    return 1 - pow(1 - input, 4);
}
float easeInOutQuart(float input)
{
    input = saturate(input);
    if (input < 0.5) return 8 * input * input * input * input;
    return 1 - pow(-2 * input + 2, 4) / 2;
}

//QUINT
float easeInQuint(float input)
{
    input = saturate(input);
    return input * input * input * input * input;
}
float easeOutQuint(float input)
{
    input = saturate(input);
    return 1 - pow(1 - input, 5);
}
float easeInOutQuint(float input)
{
    input = saturate(input);
    if (input < 0.5) return 16 * input * input * input * input * input;
    return 1 - pow(-2 * input + 2, 5) / 2;
}

//EXPO
float easeInExpo(float input)
{
    input = saturate(input);
    if (input == 0) return 0;
    return pow(2, 10 * input - 10);
}
float easeOutExpo(float input)
{
    input = saturate(input);
    if (input == 1) return 1;
    return 1 - pow(2, -10 * input);
}
float easeInOutExpo(float input)
{
    input = saturate(input);
    if (input == 0.0)
        return 0.0;
    if (input == 1.0)
        return 1.0;
    if (input < 0.5)
        return pow(2.0, 20.0 * input - 10.0) * 0.5;
    return (2.0 - pow(2.0, -20.0 * input + 10.0)) * 0.5;
}

//CIRC
float easeInCirc(float input)
{
    input = saturate(input);
    return 1 - sqrt(1 - pow(input, 2));
}
float easeOutCirc(float input)
{
    input = saturate(input);
    return sqrt(1 - pow(input - 1, 2));
}
float easeInOutCirc(float input)
{
    input = saturate(input);
    
    if (input < 0.5) return (1 - sqrt(1 - pow(2 * input, 2))) / 2;
    return (sqrt(1.0 - pow(-2.0 * input + 2.0, 2.0)) + 1.0) / 2.0;
}
