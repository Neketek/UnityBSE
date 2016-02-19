using System;
using System.Collections.Generic;
using DGPE.Math.FixedPoint;
using DGPE.Math.FixedPoint.Geometry2D;
using DGPE.Math.FixedPoint.Geometry3D;
namespace CFixedPoint
{
	class MainClass
	{

		public static void Main (string[] args)
		{
			FixedRectangle2D rect = new FixedRectangle2D((Fixed)(0),(Fixed)(0),(Fixed)1,(Fixed)2);
			Console.WriteLine(rect);
			rect.RotateZAxe(90,new FixedVector2(0,0));
			Console.WriteLine(rect);
		}
	}
}
