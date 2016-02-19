using System.Collections.Generic;

namespace DGPE.Math.FixedPoint.Geometry2D{
	public interface Vertex2DChangeListener{
		void Vertex2DChangedEvent (FixedVertex2D vertex);
	}
	public interface FixedShape2D{
		bool Contains (FixedVector2 vertex);
		bool Contains(int x,int y);
		bool Contains(Fixed x,Fixed y);
		Fixed GetBoundingBoxMaxY();
		Fixed GetBoundingBoxMinY();
		Fixed GetBoundingBoxMinX();
		Fixed GetBoundingBoxMaxX();
		FixedShape2D RotateZAxe(int deg,FixedVector2 pivot);
	}
	public class FixedVertex2D{
		private List<Vertex2DChangeListener> listeners = null;
		private FixedVector2 coordinates;
		public Fixed X{
			get{
				return coordinates.x;
			}
			set{
				coordinates.x = value;
				InvokeOwnerChangeMethod ();
			}
		}
		public Fixed Y{
			get{
				return coordinates.y;
			}
			set{
				coordinates.y = value;
				InvokeOwnerChangeMethod ();
			}
		}
		public FixedVector2 Coordinates{
			get{
				return coordinates;
			}
			set{
				coordinates = value;
				InvokeOwnerChangeMethod ();
			}
		}
		public bool AddVertex2DChangeListener(Vertex2DChangeListener listener){
			if (listener == null)
				throw new System.ArgumentNullException ();
			if (listeners == null) {
				listeners = new List<Vertex2DChangeListener> (1);
				listeners.Add (listener);
			//	listener.Vertex2DChangedEvent (this);//initial
				return true;
			} else {
				if (listeners.Contains (listener))
					return false;
				else {
					listeners.Add (listener);
					//listener.Vertex2DChangedEvent (this);//initial
					return true;
				}
			}
		}
		public bool RemoveVertex2DChangeListener(Vertex2DChangeListener listener){//TODO may be i should remove list when it is empty
			if (listener != null) {
				return listeners.Remove (listener);
			}
			return false;
		}
		public FixedVector2 GetDirectionTo(FixedVertex2D v){
			return new FixedVector2(v.coordinates.x-coordinates.x,v.coordinates.y-coordinates.y);
		}
		public FixedVector2 GetDirectionTo(FixedVector2 vertexCoord){
			return new FixedVector2 (vertexCoord.x - coordinates.x, vertexCoord.y - coordinates.y);
		}
		public FixedVertex2D RotateZAxe(Fixed sin,Fixed cos,FixedVector2 pivotPoint){
			InvokeOwnerChangeMethod ();
			coordinates.RotateZAxe (sin,cos,pivotPoint);
			return this;
		}
		public FixedVertex2D RotateZAxe(int deg,FixedVector2 pivotPoint){
			InvokeOwnerChangeMethod ();
			coordinates.RotateZAxe (deg,pivotPoint);
			return this;
		}
		public FixedVertex2D(int x,int y){
			coordinates = new FixedVector2 ((Fixed)x, (Fixed)y);
		}
		public FixedVertex2D(Fixed x,Fixed y){
			coordinates = new FixedVector2 (x,y);
		}
		public FixedVertex2D(FixedVector2 coordinates){
			this.coordinates = coordinates;
		}
		public override string ToString ()
		{
			return string.Format ("{0}", coordinates);
		}
		public FixedVertex2D Translate(FixedVector2 translationVector){
			this.coordinates = coordinates + translationVector;
			InvokeOwnerChangeMethod ();
			return this;
		}
		public FixedVertex2D Translate(Fixed x,Fixed y){
			this.coordinates.x = coordinates.x + x;
			this.coordinates.y = coordinates.y + y;
			InvokeOwnerChangeMethod ();
			return this;
		}
		public FixedVertex2D Scale(FixedVector2 scale,FixedVector2 pivot){
			this.coordinates = coordinates - pivot;
			this.coordinates = new FixedVector2 (coordinates.x*scale.x,coordinates.y*scale.y);
			this.coordinates = coordinates + pivot;
			InvokeOwnerChangeMethod ();
			return this;
		}
		public FixedVertex2D Scale(FixedVector2 scale){
			this.coordinates = new FixedVector2 (coordinates.x*scale.x,coordinates.y*scale.y);
			InvokeOwnerChangeMethod ();
			return this;
		}
		private void InvokeOwnerChangeMethod(){
			if(listeners!=null){
				System.Console.WriteLine(this);
				System.Console.WriteLine("Invokes");
				foreach (Vertex2DChangeListener listener in listeners) {
					listener.Vertex2DChangedEvent (this);
					System.Console.WriteLine(listener);
				}
			}
		}

	}
	public class FixedSegment2D:Vertex2DChangeListener{
		private FixedVertex2D begin,end;
		private FixedVector2 direction;
		private FixedVector2 yFromXEqualationCoef;
		private FixedVector2 xFromYEqualationCoef;
		private bool directionRecalculationRequired = true;
		private bool equationCoefRecalculationRequired = true;
		public FixedSegment2D (FixedVertex2D begin,FixedVertex2D end){
			if (begin == null || end == null)
				throw new System.ArgumentNullException ();
			if (begin.Coordinates == end.Coordinates)
				throw new System.Exception ("End == Begin");
			this.begin = begin;
			this.end = end;
			this.begin.AddVertex2DChangeListener (this);
			this.end.AddVertex2DChangeListener (this);
		}
		public bool IsCrossingWith(FixedSegment2D s){
			RecalculateDirection ();
			FixedVector2 dirToBeginS = s.begin.Coordinates - this.begin.Coordinates;
			FixedVector2 dirToEndS = s.end.Coordinates - this.begin.Coordinates;
			Fixed angleToBeginS = this.direction.x * dirToBeginS.y - this.direction.y * dirToBeginS.x;
			Fixed angleToEndS = this.direction.x * dirToEndS.y - this.direction.y * dirToEndS.x;
			return angleToBeginS.IsPositiveOrZero() != angleToEndS.IsPositiveOrZero ();
		}
		public FixedSegment2D RotateZ(int deg,FixedVector2 pivotPoint){
			RotateZ (FixedTrig.Sin(deg),FixedTrig.Cos(deg),pivotPoint);
			return this;	
		}
		public FixedSegment2D RotateZ(Fixed sin,Fixed cos,FixedVector2 pivotPoint){
			begin.RotateZAxe (sin, cos, pivotPoint);
			end.RotateZAxe (sin, cos, pivotPoint);
			ActivateRecalculationFlags ();
			return this;
		}
		public FixedVertex2D Begin {
			get {
				return this.begin;
			}
			set {
				if (value == null)
					throw new System.ArgumentNullException ();
				CheckNotSegmentException (value, end);
				begin.RemoveVertex2DChangeListener (this);
				begin = value;
				begin.AddVertex2DChangeListener (this);
			}
		}

		public FixedVertex2D End {
			get {
				return this.end;
			}
			set {
				if (value == null)
					throw new System.ArgumentNullException ();
				CheckNotSegmentException (begin,end);
				end.RemoveVertex2DChangeListener (this);
				end = value;
				end.AddVertex2DChangeListener (this);
			}
		}

		public FixedVector2 Direction {
			get {
				RecalculateDirection ();
				return this.direction;
			}
		}
		public FixedVertex2D GetVertexOfCrossing(FixedSegment2D s){
			return null;
		}
		public override string ToString ()
		{
			RecalculateAllData ();
			return string.Format ("[{0},{1}|y = {2}x+({3});x = {4}y+({5});D = {6}]",
			                      begin, end,yFromXEqualationCoef.x,yFromXEqualationCoef.y,
			                      xFromYEqualationCoef.x,xFromYEqualationCoef.y,direction);
		}
		//TODO: think about using To Zero method thich makes values which are between (-EPS,EPS) equal to zero;

		public bool IsParallelToXAxe(){
			RecalculateDirection ();
			return direction.y.IsZero();
		}
		public bool IsParallelToYAxe(){
			RecalculateDirection ();
			return direction.x.IsZero();
		}
		public void Vertex2DChangedEvent (FixedVertex2D vertex)
		{
			ActivateRecalculationFlags ();
		}
		private void RecalculateDirection(){
			if(directionRecalculationRequired)
				this.direction = new FixedVector2 (end.X - begin.X, end.Y - begin.X);
		}
		private void CheckNotSegmentException(FixedVertex2D a,FixedVertex2D b){
			if (a.Coordinates == b.Coordinates)
				throw new System.Exception ("Begin == End");
		}
		private void RecalculateEqualationCoef(){
			if (!equationCoefRecalculationRequired)
				return;
			RecalculateDirection ();
			if (direction.x.IsZero ()) {
			
				xFromYEqualationCoef = new FixedVector2 (FixedConstants.FIXED_ZERO,begin.X);
				yFromXEqualationCoef = FixedVector2.CreateZeroVector ();
				return;
			}
			if (direction.y.IsZero ()) {
				yFromXEqualationCoef = new FixedVector2 (FixedConstants.FIXED_ZERO,begin.Y);
				xFromYEqualationCoef = FixedVector2.CreateZeroVector ();
				return;
			}
//			Fixed yc = begin.Y / direction.y;
//			Fixed xc = begin.X / direction.x;
			Fixed dx_div_dy = direction.x / direction.y;
			Fixed dy_div_dx = direction.y / direction.x;
			xFromYEqualationCoef = new FixedVector2(dy_div_dx,begin.Y-begin.X*dy_div_dx);
			yFromXEqualationCoef = new FixedVector2(dx_div_dy,begin.X-begin.Y*dx_div_dy);
			return;
		}
		private void RecalculateAllData(){
			RecalculateEqualationCoef (); // this method is calculating the direction vector too
		}
		private void ActivateRecalculationFlags(){
			equationCoefRecalculationRequired = true;
			directionRecalculationRequired = true;
		}
	}
	public class FixedTriangle2D:Vertex2DChangeListener,FixedShape2D{
		private FixedVertex2D a,b,c;
		private FixedVector2 ab,bc,ca;
		private bool directionalVectorsRecalculationRequired = true;
		public FixedTriangle2D(FixedVertex2D a,FixedVertex2D b,FixedVertex2D c){
			if(a==null||b==null||c==null){
				throw new System.ArgumentNullException("a==null||b==null||c==null");
			}
			//CheckNotTriangleException (a,b,c);
			this.a = a;
			this.b = b;
			this.c = c;
			this.a.AddVertex2DChangeListener (this);
			this.b.AddVertex2DChangeListener (this);
			this.c.AddVertex2DChangeListener (this);
		}
		public FixedTriangle2D(FixedVector2 va,FixedVector2 vb,FixedVector2 vc)
		:this(new FixedVertex2D(va),new FixedVertex2D(vb),new FixedVertex2D(vc)){

		}
		public bool IsInTriangle(FixedVector2 p){
			RecalculateDirectionalVectors ();
			FixedVector2 ap = a.GetDirectionTo (p);
			FixedVector2 bp = b.GetDirectionTo (p);
			FixedVector2 cp = b.GetDirectionTo (p);
			Fixed bpXab = FixedVector2.PseudoscalarMultiplication (bp,ab);
			Fixed cpXbc = FixedVector2.PseudoscalarMultiplication (cp,bc);
			Fixed apXca = FixedVector2.PseudoscalarMultiplication (ap,ca);
			//this return means that sign of the pseudoscalar multiplications is equal
			return 
				apXca.IsZero()||bpXab.IsZero()||cpXbc.IsZero()
				||
				(apXca.IsNegative()==bpXab.IsNegative()&&apXca.IsNegative()==cpXbc.IsNegative());
		}
		public void Vertex2DChangedEvent (FixedVertex2D vertex)
		{
			directionalVectorsRecalculationRequired = true;
		}
		public FixedVertex2D A {
			get {
				return this.a;
			}
			set {
				if (value == null)
					throw new System.ArgumentNullException ();
			//	CheckNotTriangleException (value,b,c);
				a.RemoveVertex2DChangeListener (this);
				a = value;
				a.AddVertex2DChangeListener (this);
			}
		}

		public FixedVertex2D B {
			get {
				return this.b;
			}
			set {
				if (value == null)
					throw new System.ArgumentNullException ();
		//		CheckNotTriangleException (a,value,b);
				b.RemoveVertex2DChangeListener (this);
				b = value;
				b.AddVertex2DChangeListener (this);
			}
		}

		public FixedVertex2D C {
			get {
				return this.c;
			}
			set {
				if (value == null)
					throw new System.ArgumentNullException ();
			//	CheckNotTriangleException (a, b, value);
				c.RemoveVertex2DChangeListener (this);
				c = value;
				c.AddVertex2DChangeListener (this);
			}
		}
		public FixedTriangle2D RotateZAxe(int deg,FixedVector2 pivotPoint){
			RotateZAxe (FixedTrig.Sin (deg),FixedTrig.Cos (deg),pivotPoint);
			return this;
		}
		public FixedTriangle2D RotateZAxe(Fixed sin,Fixed cos,FixedVector2 pivotPoint){
			a.RotateZAxe (sin, cos, pivotPoint);
			b.RotateZAxe (sin, cos, pivotPoint);
			c.RotateZAxe (sin, cos, pivotPoint);
			return this;
		}
		public override string ToString()
		{
			return string.Format ("[FixedTriangle2D: a={0}, b={1}, c={2}]", a, b, c);
		}
		//FIXME:Check this method by testing or logic
		public bool Contains (FixedVector2 vertex)
		{
			return IsInTriangle(vertex);
		}

		public bool Contains (int x, int z)
		{
			return IsInTriangle(new FixedVector2(x,z));
		}

		public bool Contains (Fixed x, Fixed z)
		{
			return IsInTriangle(new FixedVector2(x,z));
		}

		public Fixed GetBoundingBoxMaxY ()
		{
			if(a.Y>=b.Y&&a.Y>=c.Y)
				return a.Y;
			else{
				if(b.Y>=c.Y&&b.Y>=a.Y)
					return b.Y;
				else 
					return c.Y;
			}
		}

		public Fixed GetBoundingBoxMinY ()
		{
			if(a.Y<=b.Y&&a.Y<=c.Y)
				return a.Y;
			else{
				if(b.Y<=c.Y&&b.Y<=a.Y)
					return b.Y;
				else 
					return c.Y;
			}
		}

		public Fixed GetBoundingBoxMinX ()
		{
			if(a.X<=b.Y&&a.X<=c.Y)
				return a.X;
			else{
				if(b.X<=c.Y&&b.X<=a.X)
					return b.X;
				else 
					return c.X;
			}
		}

		public Fixed GetBoundingBoxMaxX ()
		{
			if(a.X>=b.Y&&a.X>=c.Y)
				return a.X;
			else{
				if(b.X>=c.Y&&b.X>=a.X)
					return b.X;
				else 
					return c.X;
			}
		}
		FixedShape2D FixedShape2D.RotateZAxe (int deg, FixedVector2 pivot)
		{
			return RotateZAxe(deg,pivot);
		}
		private void RecalculateDirectionalVectors(){
			if (directionalVectorsRecalculationRequired) {
				ab = a.GetDirectionTo (b);
				bc = b.GetDirectionTo (c);
				ca = c.GetDirectionTo (a);
				directionalVectorsRecalculationRequired = false;
			}
		}
		//TODO: I can use CheckNotTriangleException but i don't know is it neccessary
		private void CheckNotTriangleException(FixedVertex2D a,FixedVertex2D b,FixedVertex2D c){
			FixedVector2 ab = a.GetDirectionTo(b);
			FixedVector2 ac = a.GetDirectionTo(c);
			if ((ab.x * ac.y - ab.y * ac.x).IsZero ())
				throw new System.ArgumentException ("This set of vertices can't form the triangle");
		}
	}
	public class FixedGrid2D{
		public readonly Fixed  cellWidth,cellHeight;
		public readonly int gridWidth,gridHeight;
		public readonly int maxIdAsOneDimensionalArray;
		public FixedGrid2D(int gridWidth,int gridHeight,Fixed cellWidth,Fixed cellHeight){
			CheckCellDimensionsException (cellWidth,cellHeight);
			CheckGridDimensionsException (gridWidth,gridHeight);
			this.cellWidth = cellWidth;
			this.cellHeight = cellHeight;
			this.gridWidth = gridWidth;
			this.gridHeight = gridHeight;
			this.maxIdAsOneDimensionalArray = gridWidth * gridHeight - 1;
		}
		public int GetCellIdAsOneDimensionalArray(Fixed x,Fixed y){
			x = x / cellWidth;
			y = y / cellHeight;
			int id = (int)x + (int)y * gridWidth;
			if (id < 0 || id > maxIdAsOneDimensionalArray)
				return -1;
			return id;
		}
		private void CheckGridDimensionsException(int width,int height){
			if (width <= 0)
				throw new System.ArgumentOutOfRangeException ("grid width  <= 0");
			if (height <= 0)
				throw new System.ArgumentOutOfRangeException ("grid height <= 0");
		}
		private void CheckCellDimensionsException(Fixed width,Fixed height){
			if(width.IsNegativeOrZero())
				throw new System.ArgumentOutOfRangeException ("cell width  <= 0");
			if(height.IsNegativeOrZero())
				throw new System.ArgumentOutOfRangeException ("cell height <= 0");
		}
	}
	public class FixedRectangle2D:FixedShape2D{
		private FixedTriangle2D abc,acd;
		private FixedVertex2D a, b, c, d;
		public FixedRectangle2D(Fixed leftX,Fixed bottomY,Fixed width,Fixed height){
			FixedVector2 a = new FixedVector2 (leftX,bottomY);
			FixedVector2 b = new FixedVector2 (leftX+width,bottomY);
			FixedVector2 c = new FixedVector2 (leftX + width, bottomY + height);
			FixedVector2 d = new FixedVector2 (leftX, bottomY + height);
			this.a = new FixedVertex2D (a);
			this.b = new FixedVertex2D (b);
			this.c = new FixedVertex2D (c);
			this.d = new FixedVertex2D (d);
			abc = new FixedTriangle2D (this.a,this.b,this.c);
			acd = new FixedTriangle2D (this.a,this.c,this.d);
		}
		public bool IsInRectangle(FixedVector2 coordinates){
			return abc.IsInTriangle (coordinates) || acd.IsInTriangle (coordinates);
		}
		public FixedRectangle2D RotateZAxe(int deg,FixedVector2 pivot){
			return this.RotateZAxe (FixedTrig.Sin (deg), FixedTrig.Cos (deg), pivot);
		}
		public FixedRectangle2D RotateZAxe(Fixed sin,Fixed cos,FixedVector2 pivot){
			a.RotateZAxe (sin,cos,pivot);
			b.RotateZAxe (sin,cos,pivot);
			c.RotateZAxe (sin,cos,pivot);
			d.RotateZAxe (sin,cos,pivot);
			return this;
		}
		public FixedRectangle2D Translate(FixedVector2 translation){
			a.Translate (translation);
			b.Translate (translation);
			c.Translate (translation);
			d.Translate (translation);
			return this;
		}
		public FixedRectangle2D Scale(FixedVector2 scale,FixedVector2 pivot){
			a.Scale (scale, pivot);
			b.Scale (scale, pivot);
			c.Scale (scale, pivot);
			d.Scale (scale, pivot);
			return this;
		}
		public FixedRectangle2D Scale(FixedVector2 scale){
			a.Scale (scale);
			b.Scale (scale);
			c.Scale (scale);
			d.Scale (scale);
			return this;
		}
		public FixedVector2 A {
			get {
				return this.a.Coordinates;
			}
		}

		public FixedVector2 B {
			get {
				return this.b.Coordinates;
			}
		}

		public FixedVector2 C {
			get {
				return this.c.Coordinates;
			}
		}

		public FixedVector2 D {
			get {
				return this.d.Coordinates;
			}
		}
		public override string ToString ()
		{
			return string.Format ("[FixedRectangle2D: a={0}, b={1}, c={2}, d={3}]", a, b, c, d);
		}
		public bool Contains (FixedVector2 vertex)
		{
			return IsInRectangle(vertex);
		}

		public bool Contains (int x, int y)
		{
			return IsInRectangle(new FixedVector2(x,y));
		}

		public bool Contains (Fixed x, Fixed y)
		{
			return IsInRectangle(new FixedVector2(x,y));
		}

		public Fixed GetBoundingBoxMaxY ()
		{
			if(a.Y>=b.Y&&a.Y>=c.Y&&a.Y>=d.Y){
				return a.Y;
			}else{
				if(b.Y>=a.Y&&b.Y>=c.Y&&b.Y>=d.Y){
					return b.Y;
				}else{
					if(c.Y>=a.Y&&c.Y>=b.Y&&c.Y>=d.Y)
						return c.Y;
					else
						return d.Y;
				}
			}
		}

		public Fixed GetBoundingBoxMinY ()
		{
			if(a.Y<=b.Y&&a.Y<=c.Y&&a.Y<=d.Y){
				return a.Y;
			}else{
				if(b.Y<=a.Y&&b.Y<=c.Y&&b.Y<=d.Y){
					return b.Y;
				}else{
					if(c.Y<=a.Y&&c.Y<=b.Y&&c.Y<=d.Y)
						return c.Y;
					else
						return d.Y;
				}
			}
		}

		public Fixed GetBoundingBoxMinX ()
		{
			if(a.X<=b.X&&a.X<=c.X&&a.X<=d.X){
				return a.X;
			}else{
				if(b.X<=a.X&&b.X<=c.X&&b.X<=d.X){
					return b.X;
				}else{
					if(c.X<=a.X&&c.X<=b.X&&c.X<=d.X)
						return c.X;
					else
						return d.X;
				}
			}
		}

		public Fixed GetBoundingBoxMaxX ()
		{
			if(a.X>=b.X&&a.X>=c.X&&a.X>=d.X){
				return a.X;
			}else{
				if(b.X>=a.X&&b.X>=c.X&&b.X>=d.X){
					return b.X;
				}else{
					if(c.X>=a.X&&c.X>=b.X&&c.X>=d.X)
						return c.X;
					else
						return d.X;
				}
			}
		}
		FixedShape2D FixedShape2D.RotateZAxe (int deg, FixedVector2 pivot)
		{
			return RotateZAxe(deg,pivot);
		}
	}
	public class FixedCircle2D:FixedShape2D{
		private FixedVertex2D center;
		private Fixed radius;
		private Fixed squareRadius;
		public bool IsInCircle(FixedVector2 coordinates){
			coordinates = coordinates - center.Coordinates;
			return Fixed.Square(coordinates.x)+Fixed.Square(coordinates.y)<=squareRadius;
		}
		public FixedCircle2D(FixedVertex2D center,Fixed radius){
			this.Radius = radius;
			this.Center = center;
		}
		public FixedVertex2D Center {
			get {
				return this.center;
			}
			set{
				if (value == null)
					throw new System.ArgumentNullException("Center == null");
				this.center = value;
			}
		}

		public Fixed Radius {
			get {
				return this.radius;
			}
			set{
				if (value.IsNegativeOrZero())
					throw new System.ArgumentOutOfRangeException ("ERROR: radius.IsNegativeOrZero ()");
				radius = value;
				squareRadius = Fixed.Square (radius);
			}
		}
		public Fixed SquareRadius {
			get {
				return this.squareRadius;
			}
		}
		public bool Contains (FixedVector2 vertex)
		{
			return IsInCircle(vertex);
		}

		public bool Contains (int x, int y)
		{
			return IsInCircle(new FixedVector2(x,y));
		}

		public bool Contains (Fixed x, Fixed y)
		{
			return IsInCircle(new FixedVector2(x,y));
		}

		public Fixed GetBoundingBoxMaxY ()
		{
			return center.Y+radius;
		}

		public Fixed GetBoundingBoxMinY ()
		{
			return center.Y-radius;
		}

		public Fixed GetBoundingBoxMinX ()
		{
			return center.X-radius;
		}

		public Fixed GetBoundingBoxMaxX ()
		{
			return center.X+radius;
		}
		public FixedShape2D RotateZAxe (int deg, FixedVector2 pivot)
		{
			center.RotateZAxe(deg,pivot);
			return this;
		}
	}
}
