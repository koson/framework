// Accord Statistics Library
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2017
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//


using System;
using System.Collections.Generic;
using System.Text;
using Accord.Math    

public static double[] ACF(double[] vector, int nlag)
{
	
    int nTime = vector.Length;   
    if (nlag<1)
    {
       nlag = nTime;
    }
    if  (nTime<=1)
    {
       //throw new System.ArgumentException("Vector length should be >=2", "vector");
       double[] acf1 = new double[] { 1.0 };
       return acf1;
    }
    // padding the length to be the power of 2 to facilitate FFT speed.
    int newLength = Convert.ToInt32(Math.Pow(2, Math.Ceiling( Math.Log(nTime, 2) ) )) ;
  
    // define and calculate FFT
    double VecMean = vector.Mean();
    Complex[] Frf = new Complex[newLength];
    for (int k = 0; k< newLength; k++)
    {
         if (k < nTime)
         {
            Frf[k] = new Complex(Convert.ToDouble(vector[k] - VecMean), 0);
         }
         else
         {  
            Frf[k]=0;
         }
     }
     Accord.Math.Transforms.FourierTransform2.FFT(Frf, Accord.Math.FourierTransform.Direction.Forward);
           
     // calculate inverse(backward) FFT of ( Frf*Conjugate(Frf) )
     Complex[] iFTFTj = new Complex[Frf.Length];
     for (int k=0; k < Frf.Length; k++)
     {
        Complex FrfConj = Complex.Conjugate(Frf[k]);
        double RealPart = Frf[k].Real*FrfConj.Real - Frf[k].Imaginary*FrfConj.Imaginary;
        double ImaginaryPart = Frf[k].Real*FrfConj.Imaginary + Frf[k].Imaginary*FrfConj.Real;
        iFTFTj[k] = new Complex(RealPart, ImaginaryPart);
     }                    
     Accord.Math.Transforms.FourierTransform2.FFT(iFTFTj, Accord.Math.FourierTransform.Direction.Backward);
  
     // calculate ACF, normalized against the first item
     double[] acf = new double[nTime];
     double normalizer = 1.0;        
     int newlag = nTime<nlag ? nTime : nlag;
     for (int k=0; k < newlag; k++)
     {
         acf[k] = iFTFTj[k].Real / (nTime * normalizer);
         if (k==0)
         {
             normalizer = acf[0];
             acf[0]=1.0;
         }              
     }          
     return acf;
}
