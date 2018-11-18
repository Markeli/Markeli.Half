# Markeli.Half
Implementation of a half-precision floating point number in .net standard

|    	|       master     	|  develop 	|
|----------	|:-------------:	|:-------------:|
| build 	|  [![Build Status](https://travis-ci.org/Markeli/Markeli.Half.svg?branch=master)](https://travis-ci.org/Markeli/Markeli.Half) | [![Build Status](https://travis-ci.org/Markeli/Markeli.Half.svg?branch=develop)](https://travis-ci.org/Markeli/Markeli.Half)|
| nuget 	|  ![NuGet](https://img.shields.io/nuget/v/Markeli.Half.svg?style=plastic)  |   N/A 	|



Half is not fast enough and precision is also very bad, so is should not be used for mathematical computation (use `Single` instead). The main advantage of Half type is lower memory cost: two bytes per number. Half is typically used in graphical applications.

It is ported to `.net standard` project from [source forge repository](https://sourceforge.net/projects/csharp-half/) owned by [ladislavlang](https://sourceforge.net/u/ladislavlang/profile/). 

## References
- Fast Half Float Conversions, Jeroen van der Zijp, link: http://www.fox-toolkit.org/ftp/fasthalffloatconversion.pdf
- IEEE 754 revision, link: http://grouper.ieee.org/groups/754/