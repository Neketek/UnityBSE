using System;
using DGPE.Math.FixedPoint;
using UnityEngine;
namespace DEngine.Utils{
	//FIXME: System.Random cant be used for fixed point random generation because of the fact that this class is
	//floating point operations in generation proccess , but additional investigation is required
	// i have done addional investigations and found that in the System.Random constuction float*int exists that 
	// means that this class is not suitable for deterministic calculations
	public class RandomFixedGenerator{
		private System.Random integerRandom;
		public RandomFixedGenerator(int seed){
			integerRandom = new System.Random(seed);
		}
		public Fixed Next(){
			return Fixed.CreateFixedByFixedValue(integerRandom.Next(FixedConstants.FIXED_ONE_VALUE));
		}
		public Fixed Next(Fixed max){
			return Fixed.CreateFixedByFixedValue(integerRandom.Next(max.GetFixedValue()));
		}
		public Fixed Next(Fixed min,Fixed max){
			return Fixed.CreateFixedByFixedValue(integerRandom.Next(min.GetFixedValue(),max.GetFixedValue()));
		}
		public int NextInt(int min,int max){
			return integerRandom.Next(min,max);
		}
		public void SetRandomSeed(int seed){
			this.integerRandom = new System.Random(seed);
		}
	}
	public class RandomColorGenerator{
		private System.Random integerRandom;
		public RandomColorGenerator(int seed){
			integerRandom = new System.Random(seed);
		}
		public Color Next(){
			return new Color((float)integerRandom.NextDouble(),
			                 (float)integerRandom.NextDouble(),
			                 (float)integerRandom.NextDouble());
		}
		public Color Next(Color min,Color max){
			if(min.r>max.r||min.g>max.g||min.b>max.b)
				throw new System.ArgumentException("Some min.r>max.r||min.g>max.g||min.b>max.b");
			return new Color(min.r + (float)integerRandom.NextDouble()*(max.r-min.r),
			                 min.g + (float)integerRandom.NextDouble()*(max.r-min.r),
			                 min.b + (float)integerRandom.NextDouble()*(max.r-min.r));
		}
	}
}
