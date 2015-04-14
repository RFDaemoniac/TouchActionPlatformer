using UnityEngine;
using System.Collections;
using System;

namespace NodeCanvas{

	public enum CompareMethod
	{
		EqualTo,
		GreaterThan,
		LessThan
	}

	public enum OperationMethod
	{
		Set,
		Add,
		Subtract,
		Multiply,
		Divide
	}

	public static class TaskTools {

		public static string GetOperationString(OperationMethod om){

			if (om == OperationMethod.Set)
				return " = ";

			if (om == OperationMethod.Add)
				return " += ";

			if (om == OperationMethod.Subtract)
				return " -= ";

			if (om == OperationMethod.Multiply)
				return " *= ";

			if (om == OperationMethod.Divide)
				return " /= ";

			return string.Empty;
		}

		public static object Operate(object a, object b, OperationMethod om){
			
			var type = a.GetType();
			if (type != b.GetType())
				return a;

			if (om == OperationMethod.Set)
				return a = b;

			if (type == typeof(float)){
				if (om == OperationMethod.Add)
					return (float)a + (float)b;
				if (om == OperationMethod.Subtract)
					return (float)a - (float)b;
				if (om == OperationMethod.Multiply)
					return (float)a * (float)b;
				if (om == OperationMethod.Divide)
					return (float)a / (float)b;
			}

			if (type == typeof(int)){
				if (om == OperationMethod.Add)
					return (int)a + (int)b;
				if (om == OperationMethod.Subtract)
					return (int)a - (int)b;
				if (om == OperationMethod.Multiply)
					return (int)a * (int)b;
				if (om == OperationMethod.Divide)
					return (int)a / (int)b;
			}

			if (type == typeof(Vector3)){
				if (om == OperationMethod.Add)
					return (Vector3)a + (Vector3)b;
				if (om == OperationMethod.Subtract)
					return (Vector3)a - (Vector3)b;
				if (om == OperationMethod.Multiply)
					return Vector3.Scale((Vector3)a, (Vector3)b);
				if (om == OperationMethod.Divide)
					return new Vector3( ((Vector3)a).x/((Vector3)b).x, ((Vector3)a).y/((Vector3)b).y, ((Vector3)a).z/((Vector3)b).z );
			}

			Debug.LogError("Requested Operation with non compatible types");
			return a;
		}

		public static string GetCompareString(CompareMethod cm){

			if (cm == CompareMethod.EqualTo)
				return " == ";

			if (cm == CompareMethod.GreaterThan)
				return " > ";

			if (cm == CompareMethod.LessThan)
				return " < ";

			return string.Empty;
		}

		public static bool Compare(IComparable a, IComparable b, CompareMethod cm, float floatingPoint = 0.05f){

			if (cm == CompareMethod.EqualTo){
				if (a.GetType() == typeof(float))
					return Mathf.Abs((float)a - (float)b) <= floatingPoint;
				return a.CompareTo(b) == 0;
			}

			if (cm == CompareMethod.GreaterThan)
				return a.CompareTo(b) > 0;

			if (cm == CompareMethod.LessThan)
				return a.CompareTo(b) < 0;

			return true;
		}
	}
}