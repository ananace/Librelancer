﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Numerics;

namespace LibreLancer
{
	public static class NetPacking
	{
        const int BITS_COMPONENT = 15;

        const float UNIT_MIN = -0.707107f;
        const float UNIT_MAX = 0.707107f;
		public static void Write(this Lidgren.Network.NetOutgoingMessage om, Quaternion q)
		{
			var maxIndex = 0;
			var maxValue = float.MinValue;
			var sign = 1f;

			maxValue = Math.Abs(q.X);
			sign = q.X < 0 ? -1 : 1;

			if (Math.Abs(q.Y) > maxValue)
			{
				maxValue = Math.Abs(q.Y);
				maxIndex = 1;
				sign = q.Y < 0 ? -1 : 1;
			}
			if (Math.Abs(q.Z) > maxValue)
			{
				maxValue = Math.Abs(q.Z);
				maxIndex = 2;
				sign = q.Z < 0 ? -1 : 1;
			}
			if (Math.Abs(q.W) > maxValue)
			{
				maxValue = Math.Abs(q.W);
				maxIndex = 3;
				sign = q.W < 0 ? -1 : 1;
			}
            om.WriteRangedInteger(0, 3, maxIndex);

  			if (maxIndex == 0)
			{
                om.WriteRangedSingle(q.Y * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.Z * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.W * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
			}
			else if (maxIndex == 1)
			{
                om.WriteRangedSingle(q.X * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.Z * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.W * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
			}
			else if (maxIndex == 2)
			{
                om.WriteRangedSingle(q.X * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.Y * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.W * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
			}
			else
			{
                om.WriteRangedSingle(q.X * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.Y * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
                om.WriteRangedSingle(q.Z * sign, UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
			}
            om.WritePadBits();
		}

		public static Quaternion ReadQuaternion(this Lidgren.Network.NetIncomingMessage im)
		{
            var maxIndex = im.ReadRangedInteger(0, 3);

            var a = im.ReadRangedSingle(UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
            var b = im.ReadRangedSingle(UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
            var c = im.ReadRangedSingle(UNIT_MIN, UNIT_MAX, BITS_COMPONENT);
			var d = (float)Math.Sqrt(1f - (a * a + b * b + c * c));
            im.ReadPadBits();
            Quaternion q;
			if (maxIndex == 0)
				return new Quaternion(d, a, b, c);
			if (maxIndex == 1)
				return new Quaternion(a, d, b, c);
			if (maxIndex == 2)
				return new Quaternion(a, b, d, c);
			return new Quaternion(a, b, c, d);
		}
            

		public static void Write(this Lidgren.Network.NetOutgoingMessage om, Vector3 vec)
		{
            om.Write(vec.X);
            om.Write(vec.Y);
            om.Write(vec.Z);
		}

        public static Vector3 ReadVector3(this Lidgren.Network.NetIncomingMessage im)
        {
            return new Vector3(im.ReadFloat(), im.ReadFloat(), im.ReadFloat());
        }

        static float WrapMinMax(float x, float min, float max)
        {
            var m = max - min;
            var y = (x - min);
            return min + (m + y % m) % m;
        }
        const float ANGLE_MIN = (float)(-2 * Math.PI);
        const float ANGLE_MAX = (float)(2 * Math.PI);
        public static void WriteRadiansQuantized(this Lidgren.Network.NetOutgoingMessage om, float angle)
        {
            om.WriteRangedSingle(WrapMinMax(angle, ANGLE_MIN, ANGLE_MAX), ANGLE_MIN, ANGLE_MAX, 16);
        }
        public static float ReadRadiansQuantized(this Lidgren.Network.NetIncomingMessage im)
        {
            return im.ReadRangedSingle(ANGLE_MIN, ANGLE_MAX, 16);
        }

        public static void Write(this Lidgren.Network.NetOutgoingMessage om, string[] array)
        {
            if (array == null)
            {
                om.WriteVariableUInt32(0);
            }
            else
            {
                om.WriteVariableUInt32((uint)array.Length);
                foreach(var s in array) om.Write(s);
            }
        }

        public static string[] ReadStringArray(this Lidgren.Network.NetIncomingMessage im)
        {
            var strs = new string[(int) im.ReadVariableUInt32()];
            for (int i = 0; i < strs.Length; i++)
                strs[i] = im.ReadString();
            return strs;
        }
    }
}
