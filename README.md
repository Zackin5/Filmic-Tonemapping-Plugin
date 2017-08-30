# Paint.NET Filmic Tonemapping Plugin
A filmic tonemapping plugin for Paint.NET. Includes variants of the Reinhard tonemap operator, as well as a simplified Haarm-Peter Duiker curve and the Uncharted 2 operator.

## Building
FilmicTonemapping.cs can be loaded and built into a dll file using the CodeLab Paint.NET plugin: http://www.boltbait.com/pdn/codelab/

## Parameters
This plugin offers the following useage parameters:

* **Gamma Correction** - enables/disables gamma correction before and after tonemapping
* **Pre Gamma** - gamma value to account for before tonemapping, set to 0 to disable pre-processing
* **Post Gamma** - gamma value to adjust image to after tonemapping, set to 0 to disable post-processing
* **Tonemapping Model** - tonemapping model to use, offers the following options:
   * **Reinhard RGB Simple** - simple Reinhard model applied to RGB values  
   * **Reinhard RGB Full** - full Reinhard model applied to RGB values with support for white values  
   * **Reinhard Lumianance Simple** - simple Reinhard model applied to Lumianance values  
   * **Reinhard Lumianance Full** - full Reinhard model applied to Lumianance values with support for white values  
   * **Haarm-Peter Duiker Simple** - simple Haarm-Peter Duiker curve  
   * **Uncharted 2** - Uncharted 2 model using default values, supports white values  
* **White Value** - Lumianance value which will be mapped to full white in final results 
* **Pre Exposure** - Amount of image exposure to apply before tonemapping

## Credits
Inspiration and code came from the following works:

* [John Hable - Filmic Tonemapping Operators](http://filmicworlds.com/blog/filmic-tonemapping-operators/)
* [Tom Madams - Why Reinhard desaturates my blacks](https://imdoingitwrong.wordpress.com/2010/08/19/why-reinhard-desaturates-my-blacks-3/)
* [John Hable - Uncharted 2: HDR Lighting](http://www.gdcvault.com/play/1012351/Uncharted-2-HDR)
