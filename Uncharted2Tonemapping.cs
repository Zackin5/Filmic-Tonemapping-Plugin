// Name: Uncharted 2 Tonemapping
// Submenu:
// Author: Jace Regenbrecht
// Title: Uncharted 2 Tonemapping
// Version: 1.0
// Desc:
// Keywords:
// URL: https://github.com/Zackin5/Filmic-Tonemapping-Plugin
// Help:
#region UICode
DoubleSliderControl Amount1 = 1; // [1,20] Pre Exposure
CheckboxControl Amount2 = true; // [0,1] Gamma Correction
DoubleSliderControl Amount3 = 2.2; // [1,5] Pre Gamma
DoubleSliderControl Amount4 = 2.2; // [1,5] Post Gamma
DoubleSliderControl Amount5 = 0.22; // [0,1] Shoulder strength
DoubleSliderControl Amount6 = 0.3; // [0,1] Linear strength
DoubleSliderControl Amount7 = 0.1; // [0,1] Linear angle
DoubleSliderControl Amount8 = 0.2; // [0,1] Toe strength
DoubleSliderControl Amount9 = 0.01; // [0,1] Toe numerator
DoubleSliderControl Amount10 = 0.22; // [0,1] Toe denominator
DoubleSliderControl Amount11 = 5; // [1,20] White Value
CheckboxControl Amount12 = false; // [0,1] Use Luminance
CheckboxControl Amount13 = false; // [0,1] Display Clipping
#endregion

// Misc functions
private double Clamp(double value, double min, double max)
{
    if(value > max)
        return max;
    else if(value < min)
        return min;
    else
        return value;
}

private ColorBgra CheckClipping(ColorBgra color)
{
    ColorBgra whiteClipColor, blackClipColor;
    double R, G, B;
    
    // Define clipping colours
    whiteClipColor = ColorBgra.Red;
    blackClipColor = ColorBgra.Blue;
    
    // Convert int values to doubles
    R = ((double)color.R);
    G = ((double)color.G);
    B = ((double)color.B);
    
    // Test for clipping
    if( R >= 254.0 & G >= 254.0 & B >= 254.0 )
        return whiteClipColor;
    else if( R <= 0.0 & G <= 0.0 & B <= 0.0 )
        return blackClipColor;
        
    return color;
}

// Tonemapping functions
private double TonemapUncharted2(double color, double W, double ShoulderStr, double LinStr, double LinAng, double ToeStr, double ToeNum, double ToeDenom)
{
    double ExposureBias = 2.0;
    
    double curr = TonemapUncharted2_(ExposureBias*color, ShoulderStr, LinStr, LinAng, ToeStr, ToeNum, ToeDenom);
    
    double whiteScale = 1.0f/TonemapUncharted2_(W, ShoulderStr, LinStr, LinAng, ToeStr, ToeNum, ToeDenom);
    
    return curr*whiteScale;
}

private double TonemapUncharted2_(double x, double A, double B, double C, double D, double E, double F)
{
    return ((x*(A*x+C*B)+D*E)/(x*(A*x+B)+D*F))-E/F;
}

// Main render function
void Render(Surface dst, Surface src, Rectangle rect)
{
    ColorBgra CurrentPixel;
    
    for (int y = rect.Top; y < rect.Bottom; y++)
    {
        if (IsCancelRequested) return;
        for (int x = rect.Left; x < rect.Right; x++)
        {
            CurrentPixel = src[x,y];
            double R, G, B, L, nL, scale;
            
            // Convert int values to doubles
            R = ((double)CurrentPixel.R)/255.0;
            G = ((double)CurrentPixel.G)/255.0;
            B = ((double)CurrentPixel.B)/255.0;
            
            // Exposure adjustment
            if(Amount1 > 1)
            {
                R *= Amount1;
                G *= Amount1;
                B *= Amount1;
            }
            
            // Pre de-gamma adjustment
            if(Amount2 && Amount3 >= 1)
            {
                R = Math.Pow(R,Amount3);
                G = Math.Pow(G,Amount3);
                B = Math.Pow(B,Amount3);
            }
            
            // Do tonemapping on RGB or luminance
            if(Amount12)
            {
                R = TonemapUncharted2(R, Amount11, Amount5, Amount6, Amount7, Amount8, Amount9, Amount10);
                G = TonemapUncharted2(G, Amount11, Amount5, Amount6, Amount7, Amount8, Amount9, Amount10);
                B = TonemapUncharted2(B, Amount11, Amount5, Amount6, Amount7, Amount8, Amount9, Amount10);
            }
            else
            {
                L = 0.2126 * R + 0.7152 * G + 0.0722 * B;
                nL = TonemapUncharted2(L, Amount11, Amount5, Amount6, Amount7, Amount8, Amount9, Amount10);
                scale = nL / L;
                R *= scale;
                G *= scale;
                B *= scale;
            }
            
            // Post gamma adjustment if enabled and not using the HPD algorithm
            if(Amount2 && Amount4 >= 1)
            {
                R = Math.Pow(R,1/Amount4);
                G = Math.Pow(G,1/Amount4);
                B = Math.Pow(B,1/Amount4);
            }
            
            // Resize doubles to ints and clamp to avoid color issues
            R = Clamp(R*255,0,255);
            G = Clamp(G*255,0,255);
            B = Clamp(B*255,0,255);
            
            // Convert back to Bgra int
            CurrentPixel = ColorBgra.FromBgra((byte)B, (byte)G, (byte)R, CurrentPixel.A);
            
            // Check for clipping if enabled
            if(Amount13)
                CurrentPixel = CheckClipping(CurrentPixel);
            
            // Render to image pixels
            dst[x,y] = CurrentPixel;
        }
    }
}
