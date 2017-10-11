// Name: Filmic Tonemapping Operators
// Submenu:
// Author: Jace Regenbrecht
// Title: Filmic Tonemapping Operators
// Version: 1.1
// Desc:
// Keywords:
// URL: https://github.com/Zackin5/Filmic-Tonemapping-Plugin
// Help:
#region UICode
CheckboxControl Amount1 = true; // [0,1] Gamma Correction
DoubleSliderControl Amount2 = 2.2; // [1,5] Pre Gamma
DoubleSliderControl Amount3 = 2.2; // [1,5] Post Gamma
ListBoxControl Amount4 = 4; // Tonemapping Model|Reinhard RGB Simple|Reinhard RGB Full|Reinhard Luminance Simple|Reinhard Luminance Full|Uncharted 2 GDC|Uncharted 2 Blog|Haarm-Pieter Duiker Simple|ACES Narkowicz|ACES Baking Lab
DoubleSliderControl Amount5 = 5; // [1,20] White Value
DoubleSliderControl Amount6 = 1; // [1,20] Pre Exposure
CheckboxControl Amount7 = false; // [0,1] Display Clipping
#endregion

// equation sources: 
// https://imdoingitwrong.wordpress.com/2010/08/19/why-reinhard-desaturates-my-blacks-3/
// http://filmicworlds.com/blog/filmic-tonemapping-operators/

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

private double[,] matrixMult(double[,] a, double[,] b)
{
    double[,] product = new double[a.GetLength(0), b.GetLength(1)];
    
    for (int x = 0; x < a.GetLength(0); x++)
        for (int y = 0; y < b.GetLength(1); y++)
            for (int i = 0; i < a.GetLength(1) || i < a.GetLength(0); i++)
                product[x,y] += a[x,i] * b[i,y];
                
    return product;
}

// Tonemapping functions
private double TonemapReinhard(double color)
{
    return color/(color+1.0);
}

private double TonemapReinhardWhite(double color, double colorWhite)
{
    return (color*(1.0+color/Math.Pow(colorWhite,2.0)))/(color+1.0);
}

private double TonemapHPD(double color)
{
    double x = Math.Max(0,color-0.004);
    return (x*(6.2*x+.5))/(x*(6.2*x+1.7)+0.06);
}

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

private double TonemapACESNark(double x, double A, double B, double C, double D, double E)
{
    return Clamp((x*(A*x+B))/(x*(C*x+D)+E),0.0,1.0);
} 

private ColorBgra TonemapACESBakingLab(double R, double G, double B)
{
    // TEMP disable while I work on actually maxing this work
    /*double[,] color = new double[3,1] {{R}, {G}, {B}};
    double[] result = new double[] {0,0,0};
    
    double[,] ACESInput = new double[3,3] {
        {0.59719f, 0.35458f, 0.04823f},
        {0.07600f, 0.90834f, 0.01566f},
        {0.02840f, 0.13383f, 0.83777f}
    };
    
    double[,] ACESOutput = new double[3,3] {
        { 1.60475f, -0.53108f, -0.07367f},
        {-0.10208f,  1.10813f, -0.00605f},
        {-0.00327f, -0.07276f,  1.07302f}
    };
    
    color = matrixMult(ACESInput, color);
    
    // RRTAndODFit
    double[,] a, b;
    a = new double[3,1];
    b = new double[3,1];
    
    a = color * (color + 0.0245786f) - 0.000090537f;
    
    color = a / b;
    
    color = matrixMult(ACESOutput, color);
    
    result[0] = color[0,0];
    result[1] = color[1,0];
    result[2] = color[2,0];
    
    return result;*/
    
    return ColorBgra.FromBgra((byte)B, (byte)G, (byte)R, 1);;
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
            if(Amount6 > 1)
            {
                R *= Amount6;
                G *= Amount6;
                B *= Amount6;
            }
            
            // Pre de-gamma adjustment
            if(Amount1 && Amount2 >= 1)
            {
                R = Math.Pow(R,Amount2);
                G = Math.Pow(G,Amount2);
                B = Math.Pow(B,Amount2);
            }
            
            // Do tonemap
            switch(Amount4)
            {
                // Reinhard RGB
                case 0:
                    R = TonemapReinhard(R);
                    G = TonemapReinhard(G);
                    B = TonemapReinhard(B);
                    break;
                // Reinhard RGB with white value
                case 1:
                    R = TonemapReinhardWhite(R, Amount5);
                    G = TonemapReinhardWhite(G, Amount5);
                    B = TonemapReinhardWhite(B, Amount5);
                    break;
                // Reinhard Luminance
                case 2:
                    L = 0.2126 * R + 0.7152 * G + 0.0722 * B;
                    nL = TonemapReinhard(L);
                    scale = nL / L;
                    R *= scale;
                    G *= scale;
                    B *= scale;
                    break;
                // Reinhard Luminance with white value
                case 3:
                    L = 0.2126 * R + 0.7152 * G + 0.0722 * B;
                    nL = TonemapReinhardWhite(L, Amount5);
                    scale = nL / L;
                    R *= scale;
                    G *= scale;
                    B *= scale;
                    break;
                // Uncharted 2 GDC Values
                case 4:
                {
                    double uA = 0.20; // Shoulder strength
                    double uB = 0.30; // Linear strength
                    double uC = 0.10; // Linear angle
                    double uD = 0.20; // Toe strength
                    double uE = 0.01; // Toe numerator
                    double uF = 0.30; // Toe denominator
                    R = TonemapUncharted2(R, Amount5, uA, uB, uC, uD, uE, uF);
                    G = TonemapUncharted2(G, Amount5, uA, uB, uC, uD, uE, uF);
                    B = TonemapUncharted2(B, Amount5, uA, uB, uC, uD, uE, uF);
                }
                    break;
                // Uncharted 2 Blog Values
                case 5:
                {
                    double uA = 0.15; // Shoulder strength
                    double uB = 0.50; // Linear strength
                    double uC = 0.10; // Linear angle
                    double uD = 0.20; // Toe strength
                    double uE = 0.02; // Toe numerator
                    double uF = 0.30; // Toe denominator
                    R = TonemapUncharted2(R, Amount5, uA, uB, uC, uD, uE, uF);
                    G = TonemapUncharted2(G, Amount5, uA, uB, uC, uD, uE, uF);
                    B = TonemapUncharted2(B, Amount5, uA, uB, uC, uD, uE, uF);
                }
                    break;
                // Haarm-Pieter Duiker (optimized)
                case 6:
                    R = TonemapHPD(R);
                    G = TonemapHPD(G);
                    B = TonemapHPD(B);
                    break;
                // ACES Knarkowicz implementation
                case 7:
                {
                    double anA = 2.51;
                    double anB = 0.03;
                    double anC = 2.43;
                    double anD = 0.59;
                    double anE = 0.14;
                    R = TonemapACESNark(R, anA, anB, anC, anD, anE);
                    G = TonemapACESNark(G, anA, anB, anC, anD, anE);
                    B = TonemapACESNark(B, anA, anB, anC, anD, anE);
                }
                    break;
                // ACES Baking Lab implementation
                case 8:
                {
                    ColorBgra ACESColors = TonemapACESBakingLab(R, G, B);
                    R = ACESColors.R;
                    G = ACESColors.G;
                    B = ACESColors.B;
                }
                    break;
                default:
                    break;
            }
            
            // Post gamma adjustment if enabled and not using the HPD algorithm
            if(Amount1 && Amount3 >= 1 && Amount4 != 6)
            {
                R = Math.Pow(R,1/Amount3);
                G = Math.Pow(G,1/Amount3);
                B = Math.Pow(B,1/Amount3);
            }
            
            // Resize doubles to ints and clamp to avoid color issues
            R = Clamp(R*255,0,255);
            G = Clamp(G*255,0,255);
            B = Clamp(B*255,0,255);
            
            // Convert back to Bgra int
            CurrentPixel = ColorBgra.FromBgra((byte)B, (byte)G, (byte)R, CurrentPixel.A);
            
            // Check for clipping if enabled
            if(Amount7)
                CurrentPixel = CheckClipping(CurrentPixel);
            
            // Render to image pixels
            dst[x,y] = CurrentPixel;
        }
    }
}
