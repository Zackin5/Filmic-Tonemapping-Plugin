# Paint.NET Filmic Tonemapping Plugin
A filmic tonemapping plugin for Paint.NET. Includes variants of the Reinhard tonemap operator, as well as a simplified Haarm-Pieter Duiker curve and the Uncharted 2 operator.

## Building
FilmicTonemapping.cs can be loaded and built into a dll file using the CodeLab Paint.NET plugin: http://www.boltbait.com/pdn/codelab/

## Parameters
This plugin offers the following usage parameters:

* **Gamma Correction** - enables/disables gamma correction before and after tonemapping
* **Pre Gamma** - gamma value to account for before tonemapping, set to 0 to disable pre-processing
* **Post Gamma** - gamma value to adjust image to after tonemapping, set to 0 to disable post-processing
* **Tonemapping Model** - tonemapping model to use, offers the following options:
   * **Reinhard RGB Simple** - simple Reinhard model applied to RGB values  
   * **Reinhard RGB Full** - full Reinhard model applied to RGB values with support for white values  
   * **Reinhard Luminance Simple** - simple Reinhard model applied to Luminance values  
   * **Reinhard Luminance Full** - full Reinhard model applied to Luminance values with support for white values  
   * **Haarm-Peter Duiker Simple** - simple Haarm-Pieter Duiker curve  
   * **Uncharted 2 GDC** - Uncharted 2 model using parameter values from Hable's GDC talk, supports white values  
   * **Uncharted 2 Blog** - Uncharted 2 model using parameter values from Hable's blod post, supports white values  
* **White Value** - color value which will be mapped to full white in final results 
* **Pre Exposure** - Amount of image exposure to apply before tonemapping

## Credits
Inspiration and code came from the following works:

* [John Hable - Filmic Tonemapping Operators](http://filmicworlds.com/blog/filmic-tonemapping-operators/)
* [John Hable - Uncharted 2: HDR Lighting](http://www.gdcvault.com/play/1012351/Uncharted-2-HDR)
* [Tom Madams - Why Reinhard desaturates my blacks](https://imdoingitwrong.wordpress.com/2010/08/19/why-reinhard-desaturates-my-blacks-3/)
