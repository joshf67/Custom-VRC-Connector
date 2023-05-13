using System;
using UnityEngine;
using VRC.SDK3.Data;
using Joshf67.ServerConnector.Development;

namespace Joshf67.ServerConnector.Packing
{

    /// <summary>
    /// Used to determine what type of packing a DataStructure uses
    /// </summary>
    public enum PackingType
    {
        /// <summary>
        /// Used in functions where the PackingType should be inferred from the type
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the Endian of this DataStructure,
        /// doesn't matter and should be treated as sequential when packing
        /// </summary>
        Sequential,

        /// <summary>
        /// Indicates that the data uses Little Endian for its DataStructure
        /// </summary>
        LittleEndian,

        /// <summary>
        /// Indicates that the data uses Big Endian for its DataStructure
        /// </summary>
        BigEndian,
    }

    /// <summary>
    /// This class is a one-way converter to byte arrays, it doesn't not have a way to revert this
    /// This class is not optimized with generic objects so use DataTokens due to using typeof search which means we can't use switches because it's not const
    /// This is class is a combination of two different methods of converting to make generating server requests easier.
    /// <para>
    /// <see href="https://github.com/Xytabich/UNet/blob/master/UNet/ByteBufferWriter.cs"/>
    /// </para>
    /// <para>
    /// <see href="https://github.com/Miner28/NetworkedEventCaller"/>
    /// </para>
    /// </summary>
    public class ByteConverter
    {
        private const int BIT8 = 8;
        private const int BIT16 = 16;
        private const int BIT24 = 24;
        private const int BIT32 = 32;
        private const int BIT40 = 40;
        private const int BIT48 = 48;
        private const int BIT56 = 56;

        private const uint FLOAT_SIGN_BIT = 0x80000000;
        private const uint FLOAT_EXP_MASK = 0x7F800000;
        private const uint FLOAT_FRAC_MASK = 0x007FFFFF;

        /// <summary>
        /// convert any standard/unity type excluding UTF based on typeof into a DataList of bytes
        /// </summary>
        /// <param name="value"> The value to convert </param>
        /// <returns> A DataList Byte Array </returns>
        public static DataList ConvertObject(object value)
        {
            Type messageType = value.GetType();

            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Basic))
                Debug.Log("ByteConverter is converting an object with type: " + messageType);

	        //Check if the type is common or a non-array object
            if (messageType == typeof(DataToken))
                return ConvertDataToken((DataToken)value);

            if (messageType == typeof(DataList))
                return ConvertDataList((DataList)value);

            if (messageType == typeof(DataDictionary))
                return ConvertDataDictionary((DataDictionary)value);

            if (messageType == typeof(byte))
                return ConvertByte((byte)value);

            if (messageType == typeof(byte[]))
                return ConvertBytes((byte[])value);

            if (messageType == typeof(Boolean))
                return ConvertBool((Boolean)value);

            if (messageType == typeof(SByte))
	            return ConvertSByte((SByte)value);

            if (messageType == typeof(Char))
	            return ConvertChar((Char)value);

            if (messageType == typeof(Int16))
	            return ConvertInt16((Int16)value);

            if (messageType == typeof(UInt16))
	            return ConvertUInt16((UInt16)value);

            if (messageType == typeof(Int32))
	            return ConvertInt32((Int32)value);

            if (messageType == typeof(UInt32))
	            return ConvertUInt32((UInt32)value);

            if (messageType == typeof(Int64))
	            return ConvertInt64((Int64)value);

            if (messageType == typeof(UInt64))
	            return ConvertUInt64((UInt64)value);

            if (messageType == typeof(Single))
	            return ConvertSingle((Single)value);

            if (messageType == typeof(Decimal))
	            return ConvertDecimal((Decimal)value);

            if (messageType == typeof(DateTime))
	            return ConvertDateTime((DateTime)value);

            if (messageType == typeof(Guid))
	            return ConvertGUID((Guid)value);

            if (messageType == typeof(Vector2))
	            return ConvertVector2((Vector2)value);

            if (messageType == typeof(Vector3))
	            return ConvertVector3((Vector3)value);

            if (messageType == typeof(Vector4))
	            return ConvertVector4((Vector4)value);

            if (messageType == typeof(Quaternion))
	            return ConvertQuaternion((Quaternion)value);

            //If you want to compress a ASCII string, then call the byte converter function manually
            if (messageType == typeof(String))
	            return ConvertUTF8String((string)value);
                
	        //If the type isn't a single object then check all array types
	        if (messageType == typeof(Boolean[]))
		        return ConvertBools((Boolean[])value);

	        if (messageType == typeof(SByte[]))
		        return ConvertSBytes((SByte[])value);

	        if (messageType == typeof(Char[]))
		        return ConvertChars((Char[])value);

	        if (messageType == typeof(Int16[]))
		        return ConvertInt16s((Int16[])value);

	        if (messageType == typeof(UInt16[]))
		        return ConvertUInt16s((UInt16[])value);

	        if (messageType == typeof(Int32[]))
		        return ConvertInt32s((Int32[])value);

	        if (messageType == typeof(UInt32[]))
		        return ConvertUInt32s((UInt32[])value);

	        if (messageType == typeof(Int64[]))
		        return ConvertInt64s((Int64[])value);

	        if (messageType == typeof(UInt64[]))
		        return ConvertUInt64s((UInt64[])value);

	        if (messageType == typeof(Single[]))
		        return ConvertSingles((Single[])value);

	        if (messageType == typeof(Decimal[]))
		        return ConvertDecimals((Decimal[])value);

	        if (messageType == typeof(DateTime[]))
		        return ConvertDateTimes((DateTime[])value);

	        if (messageType == typeof(Guid[]))
		        return ConvertGUIDs((Guid[])value);

	        if (messageType == typeof(Vector2[]))
		        return ConvertVector2s((Vector2[])value);

	        if (messageType == typeof(Vector3[]))
		        return ConvertVector3s((Vector3[])value);

	        if (messageType == typeof(Vector4[]))
		        return ConvertVector4s((Vector4[])value);

	        if (messageType == typeof(Quaternion[]))
		        return ConvertQuaternions((Quaternion[])value);

            Debug.LogError($"ByteConverter cannot convert object, does not accept type of object: {value.GetType()}" +
                            ", please adjust the type of convert it manually using the ByteConverter");
            return new DataList();
        }

        /// <summary>
        /// Convert any DataToken type into their byte array equivilent
        /// </summary>
        /// <param name="value"> The value to convert </param>
        /// <returns> A DataList Byte Array </returns>
        public static DataList ConvertDataToken(DataToken value)
        {
            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Basic))
                Debug.Log("ByteConverter is converting a DataToken with type: " + value.TokenType);

            switch (value.TokenType)
            {
                case TokenType.Null:
                    return new DataList();
                case TokenType.Byte:
                    return ConvertByte(value.Byte);
                case TokenType.SByte:
                    return ConvertSByte(value.SByte);
                case TokenType.Boolean:
                    return ConvertBool(value.Boolean);
                case TokenType.Short:
                    return ConvertInt16(value.Short);
                case TokenType.UShort:
                    return ConvertUInt16(value.UShort);
                case TokenType.Int:
                    return ConvertInt32(value.Int);
                case TokenType.UInt:
                    return ConvertUInt32(value.UInt);
                case TokenType.Long:
                    return ConvertInt64(value.Long);
                case TokenType.ULong:
                    return ConvertUInt64(value.ULong);
                case TokenType.Float:
                    return ConvertSingle(value.Float);
                case TokenType.Double:
                    return ConvertDecimal((decimal)value.Double);
                case TokenType.String:
                    return ConvertUTF8String(value.String);
                case TokenType.DataList:
                    return ConvertDataList(value.DataList);
                case TokenType.DataDictionary:
                    return ConvertDataDictionary(value.DataDictionary);
                case TokenType.Reference:
                    return ConvertObject(value.Reference);
                default:
                    Debug.LogError("ByteConverter cannot convert Token Type because it is unhandled, please use an alternative");
                    return new DataList();
            }
        }

        /// <summary>
        /// Returns the byte PackingType for a given type
        /// </summary>
        /// <param name="value"> The data to get the PackingType for </param>
        /// <returns></returns>
        public static PackingType GetPackingType(object value)
        {
            Type messageType = value.GetType();

            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                Debug.Log("ByteConverter is testing the packing type of an object with type: " + messageType);

            //If the object is a DataToken dynamically retrieve the data and then get the type for that
            if (messageType == typeof(DataToken))
                messageType = ReturnDataTokenValueAsObject((DataToken)value).GetType();

            //Check if the type is common or a non-array object and is a known value
            if (messageType == typeof(bool) || messageType == typeof(Byte) || messageType == typeof(SByte) ||
                messageType == typeof(Int16) || messageType == typeof(UInt16) ||
                messageType == typeof(Int32) || messageType == typeof(UInt32) ||
                messageType == typeof(Char) || (messageType == typeof(String) && ((string)value).Length == 1) ||
                messageType == typeof(Int64) || messageType == typeof(UInt64) ||
                messageType == typeof(Single) || messageType == typeof(Decimal) ||
                messageType == typeof(DateTime) || messageType == typeof(Guid))
                return PackingType.BigEndian;

            return PackingType.Sequential;

            /* These types are unhandled and are so treated as Sequential meaning optimization is not built-in
              || messageType == typeof(Vector2) ||
                messageType == typeof(Vector3) || messageType == typeof(Vector4) || messageType == typeof(Quaternion) ||
                messageType == typeof(String)
            */

        }

        /// <summary>
        /// Returns the byte PackingType for a given type
        /// </summary>
        /// <param name="value"> The data to get the PackingType for </param>
        /// <returns></returns>
        public static PackingType GetPackingType(DataToken value)
        {

            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                Debug.Log("ByteConverter is testing the packing type of an object with type: " + value.TokenType);

            switch (value.TokenType)
            {
                case TokenType.Reference:
                case TokenType.DataList:
                case TokenType.DataDictionary:
                    return PackingType.Sequential;
                default:
                    return PackingType.BigEndian;
            }

            /* These types are unhandled and are so treated as Sequential meaning optimization is not built-in
              || messageType == typeof(Vector2) ||
                messageType == typeof(Vector3) || messageType == typeof(Vector4) || messageType == typeof(Quaternion) ||
                messageType == typeof(String)
            */

        }

        #region basic types

        /// <summary>
        /// Convert a Byte to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The boolean to convert </param>
        /// <returns> A single DataList Byte Array </returns>
        public static DataList ConvertByte(byte value)
        {
            DataList ret = new DataList();
            ret.Add(value);
            return ret;
        }

        /// <summary>
        /// Convert a Boolean to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The boolean to convert </param>
        /// <returns> A single DataList Byte Array </returns>
        public static DataList ConvertBool(bool value)
        {
            DataList ret = new DataList();
            ret.Add(value == true ? (byte)1 : (byte)0);
            return ret;
        }

        /// <summary>
        /// Convert a Char to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The char to convert </param>
        /// <returns> A 2 DataList Byte Array </returns>
        public static DataList ConvertChar(char value)
        {
            DataList ret = new DataList();
            ret.Add((byte)value);
            ret.Add((byte)(value >> BIT8));
            return ret;
        }

        /// <summary>
        /// Convert a SByte to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The SByte to convert </param>
        /// <returns> A single DataList Byte Array </returns>
        public static DataList ConvertSByte(sbyte value)
        {
            DataList ret = new DataList();
            ret.Add((byte)(value < 0 ? value + 256 : value));
            return ret;
        }

        /// <summary>
        /// Convert a Int16 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Int16 to convert </param>
        /// <returns> A 2 DataList Byte Array </returns>
        public static DataList ConvertInt16(Int16 value)
        {
            int tmp = value < 0 ? value + 0xFFFF : value;
            DataList ret = new DataList();
            ret.Add((byte)(tmp >> BIT8));
            ret.Add((byte)(tmp & 0xFF));
            return ret;
        }

        /// <summary>
        /// Convert a UInt16 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The UInt16 to convert </param>
        /// <returns> A 2 DataList Byte Array </returns>
        public static DataList ConvertUInt16(UInt16 value)
        {
            int tmp = Convert.ToInt32(value);
            DataList ret = new DataList();
            ret.Add((byte)(tmp >> BIT8));
            ret.Add((byte)(tmp & 0xFF));
            return ret;
        }

        /// <summary>
        /// Convert an Integer to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Integer to convert </param>
        /// <returns> A 4 DataList Byte Array </returns>
        public static DataList ConvertInt32(int value)
        {
            DataList ret = new DataList();
            ret.Add((byte)((value >> BIT24) & 0xFF));
            ret.Add((byte)((value >> BIT16) & 0xFF));
            ret.Add((byte)((value >> BIT8) & 0xFF));
            ret.Add((byte)(value & 0xFF));
            return ret;
        }

        /// <summary>
        /// Convert an Unsigned Integer to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Unsigned Integer to convert </param>
        /// <returns> A 4 DataList Byte Array </returns>
        public static DataList ConvertUInt32(uint value)
        {
            DataList ret = new DataList();
            ret.Add((byte)((value >> BIT24) & 255u));
            ret.Add((byte)((value >> BIT16) & 255u));
            ret.Add((byte)((value >> BIT8) & 255u));
            ret.Add((byte)(value & 255u));
            return ret;
        }

        /// <summary>
        /// Convert a 64-bit Integer to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Integer to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertInt64(Int64 value)
        {
            DataList ret = new DataList();
            ret.Add((byte)((value >> BIT56) & 0xFF));
            ret.Add((byte)((value >> BIT48) & 0xFF));
            ret.Add((byte)((value >> BIT40) & 0xFF));
            ret.Add((byte)((value >> BIT32) & 0xFF));
            ret.Add((byte)((value >> BIT24) & 0xFF));
            ret.Add((byte)((value >> BIT16) & 0xFF));
            ret.Add((byte)((value >> BIT8) & 0xFF));
            ret.Add((byte)(value & 0xFF));
            return ret;
        }

        /// <summary>
        /// Convert a 64-bit Unsigned Integer to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Unsigned Integer to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertUInt64(UInt64 value)
        {
            DataList ret = new DataList();
            ret.Add((byte)((value >> BIT56) & 255ul));
            ret.Add((byte)((value >> BIT48) & 255ul));
            ret.Add((byte)((value >> BIT40) & 255ul));
            ret.Add((byte)((value >> BIT32) & 255ul));
            ret.Add((byte)((value >> BIT24) & 255ul));
            ret.Add((byte)((value >> BIT16) & 255ul));
            ret.Add((byte)((value >> BIT8) & 255ul));
            ret.Add((byte)(value & 255ul));
            return ret;
        }

        /// <summary>
        /// Convert a single-precision floating-point into a DataList Byte Array
        /// </summary>
        /// <param name="value"> The single-precision floating-point to convert </param>
        /// <returns> A 4 DataList Byte Array </returns>
        public static DataList ConvertSingle(float value)
        {
            uint tmp = 0;
            if (float.IsNaN(value))
            {
                tmp = FLOAT_EXP_MASK | FLOAT_FRAC_MASK;
            }
            else if (float.IsInfinity(value))
            {
                tmp = FLOAT_EXP_MASK;
                if (float.IsNegativeInfinity(value)) tmp |= FLOAT_SIGN_BIT;
            }
            else if (value != 0f)
            {
                if (value < 0f)
                {
                    value = -value;
                    tmp |= FLOAT_SIGN_BIT;
                }

                int exp = 0;
                bool normal = true;
                while (value >= 2f)
                {
                    value *= 0.5f;
                    exp++;
                }
                while (value < 1f)
                {
                    if (exp == -126)
                    {
                        normal = false;
                        break;
                    }
                    value *= 2f;
                    exp--;
                }

                if (normal)
                {
                    value -= 1f;
                    exp += 127;
                }
                else exp = 0;

                tmp |= Convert.ToUInt32(exp << 23) & FLOAT_EXP_MASK;
                tmp |= Convert.ToUInt32(value * (2 << 22)) & FLOAT_FRAC_MASK;
            }

            return ConvertUInt32(tmp);
        }

        /// <summary>
        /// Convert a half-precision floating-point to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The half-precision floating-point to convert </param>
        /// <returns> A 2 DataList Byte Array </returns>
        public static DataList ConvertHalf(float value)
        {
            return ConvertUInt16(Mathf.FloatToHalf(value));
        }

        /// <summary>
        /// Convert a DateTime to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The DateTime to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertDateTime(DateTime value)
        {
            return ConvertInt64(value.ToBinary());
        }

        /// <summary>
        /// Convert a TimeSpan to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The TimeSpan to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertTimeSpan(TimeSpan value)
        {
            return ConvertInt64(value.Ticks);
        }

        /// <summary>
        /// Convert a Decimal floating-point to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Decimal floating-point to convert </param>
        /// <returns> A 16 DataList Byte Array </returns>
        public static DataList ConvertDecimal(Decimal value)
        {
            var tmp = Decimal.GetBits(value);
            DataList ret = new DataList();
            ret.AddRange(ConvertInt32(tmp[0]));
            ret.AddRange(ConvertInt32(tmp[1]));
            ret.AddRange(ConvertInt32(tmp[2]));
            ret.AddRange(ConvertInt32(tmp[3]));
            return ret;
        }

        /// <summary>
        /// Convert a Guid to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Guid to convert </param>
        /// <returns> A 16 DataList Byte Array </returns>
        public static DataList ConvertGUID(Guid value)
        {
            DataList ret = new DataList();
            foreach (byte b in value.ToByteArray())
            {
                ret.Add(b);
            }
            return ret;
        }

        #endregion

        #region basic types array

        /// <summary>
        /// Convert bytes to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The byte array to convert </param>
        /// <returns> A DataList Byte Array </returns>
        public static DataList ConvertBytes(byte[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.Add(value[i]);
            }
            return ret;
        }

        /// <summary>
        /// Convert Booleans to a DataList Array
        /// </summary>
        /// <param name="value"> The boolean to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertBools(bool[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertBool(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Chars to a DataList Array
        /// </summary>
        /// <param name="value"> The chars to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertChars(char[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertChar(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert SBytes to a DataList Array
        /// </summary>
        /// <param name="value"> The SBytes to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertSBytes(sbyte[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertSByte(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Int16s to a DataList Array
        /// </summary>
        /// <param name="value"> The Int16s to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertInt16s(Int16[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertInt16(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert UInt16s to a DataList Array
        /// </summary>
        /// <param name="value"> The UInt16s to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertUInt16s(UInt16[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertUInt16(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert ints to a DataList Array
        /// </summary>
        /// <param name="value"> The ints to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertInt32s(int[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertInt32(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert uints to a DataList Array
        /// </summary>
        /// <param name="value"> The uints to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertUInt32s(uint[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertUInt32(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Int64s to a DataList Array
        /// </summary>
        /// <param name="value"> The Int64s to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertInt64s(Int64[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertInt64(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert UInt64 to a DataList Array
        /// </summary>
        /// <param name="value"> The UInt64 to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertUInt64s(UInt64[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertUInt64(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert floats to a DataList Array
        /// </summary>
        /// <param name="value"> The floats to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertSingles(float[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertSingle(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert half floats to a DataList Array
        /// </summary>
        /// <param name="value"> The half floats to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertHalf(float[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertHalf(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert DateTimes to a DataList Array
        /// </summary>
        /// <param name="value"> The DateTimes to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertDateTimes(DateTime[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertDateTime(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert TimeSpans to a DataList Array
        /// </summary>
        /// <param name="value"> The TimeSpans to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertTimeSpans(TimeSpan[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertTimeSpan(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Decimals to a DataList Array
        /// </summary>
        /// <param name="value"> The Decimals to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertDecimals(Decimal[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertDecimal(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Guids to a DataList Array
        /// </summary>
        /// <param name="value"> The Guids to convert </param>
        /// <returns> A DataList containing the individual converted results </returns>
        public static DataList ConvertGUIDs(Guid[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertGUID(value[i]));
            }
            return ret;
        }

        #endregion

        #region unity types

        /// <summary>
        /// Convert a Color to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Color to convert </param>
        /// <returns> A 4 DataList Byte Array </returns>
        public static DataList ConvertColor(Color value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.r));
            ret.AddRange(ConvertSingle(value.g));
            ret.AddRange(ConvertSingle(value.b));
            ret.AddRange(ConvertSingle(value.a));
            return ret;
        }

        /// <summary>
        /// Convert a Color32 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Color32 to convert </param>
        /// <returns> A 4 DataList Byte Array </returns>
        public static DataList ConvertColor32(Color32 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.r));
            ret.AddRange(ConvertSingle(value.g));
            ret.AddRange(ConvertSingle(value.b));
            ret.AddRange(ConvertSingle(value.a));
            return ret;
        }

        /// <summary>
        /// Convert a Vector2 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector2 to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertVector2(Vector2 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.x));
            ret.AddRange(ConvertSingle(value.y));
            return ret;
        }

        /// <summary>
        /// Convert a Vector3 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector3 to convert </param>
        /// <returns> A 12 DataList Byte Array </returns>
        public static DataList ConvertVector3(Vector3 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.x));
            ret.AddRange(ConvertSingle(value.y));
            ret.AddRange(ConvertSingle(value.z));
            return ret;
        }

        /// <summary>
        /// Convert a Vector4 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 16 DataList Byte Array </returns>
        public static DataList ConvertVector4(Vector4 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.x));
            ret.AddRange(ConvertSingle(value.y));
            ret.AddRange(ConvertSingle(value.z));
            ret.AddRange(ConvertSingle(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a Quaternion to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 16 DataList Byte Array </returns>
        public static DataList ConvertQuaternion(Quaternion value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertSingle(value.x));
            ret.AddRange(ConvertSingle(value.y));
            ret.AddRange(ConvertSingle(value.z));
            ret.AddRange(ConvertSingle(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector2 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector2 to convert </param>
        /// <returns> A 2 DataList Byte Array </returns>
        public static DataList ConvertHalfVector2(Vector2 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertHalf(value.x));
            ret.AddRange(ConvertHalf(value.y));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector3 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector3 to convert </param>
        /// <returns> A 6 DataList Byte Array </returns>
        public static DataList ConvertHalfVector3(Vector3 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertHalf(value.x));
            ret.AddRange(ConvertHalf(value.y));
            ret.AddRange(ConvertHalf(value.z));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector4 to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertHalfVector4(Vector4 value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertHalf(value.x));
            ret.AddRange(ConvertHalf(value.y));
            ret.AddRange(ConvertHalf(value.z));
            ret.AddRange(ConvertHalf(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a half Quaternion to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 8 DataList Byte Array </returns>
        public static DataList ConvertHalfQuaternion(Quaternion value)
        {
            DataList ret = new DataList();
            ret.AddRange(ConvertHalf(value.x));
            ret.AddRange(ConvertHalf(value.y));
            ret.AddRange(ConvertHalf(value.z));
            ret.AddRange(ConvertHalf(value.w));
            return ret;
        }

        #endregion

        #region unity types array

        /// <summary>
        /// Convert Colors to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Colors to convert </param>
        /// <returns> A DataList Byte Array </returns>
        public static DataList ConvertColors(Color[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertColor(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Color32s to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The Color32s to convert </param>
        /// <returns> A DataList Byte Array </returns>
        public static DataList ConvertColor32s(Color32[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertColor32(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Vector2s to a DataList Array
        /// </summary>
        /// <param name="value"> The Vector2s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertVector2s(Vector2[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertVector2(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Vector3s to a DataList Array
        /// </summary>
        /// <param name="value"> The Vector3s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertVector3s(Vector3[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertVector3(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Vector4s to a DataList Array
        /// </summary>
        /// <param name="value"> The Vector4s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertVector4s(Vector4[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertVector4(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert Quaternions to a DataList Array
        /// </summary>
        /// <param name="value"> The Quaternions to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertQuaternions(Quaternion[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertQuaternion(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert half Vector2s to a DataList Array
        /// </summary>
        /// <param name="value"> The half Vector2s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertHalfVector2s(Vector2[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertHalfVector2(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert half Vector3s to a DataList Array
        /// </summary>
        /// <param name="value"> The half Vector3s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertHalfVector3(Vector3[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertHalfVector3(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert half Vector4s to a DataList Array
        /// </summary>
        /// <param name="value"> The half Vector4s to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertHalfVector4(Vector4[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertHalfVector4(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert half Quaternions to a DataList Array
        /// </summary>
        /// <param name="value"> The half Quaternions to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertHalfQuaternion(Quaternion[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertHalfQuaternion(value[i]));
            }
            return ret;
        }

        #endregion

        #region string types

        /// <summary>
        /// Convert a ASCII string to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The ASCII String to convert </param>
        /// <returns> A DataList Byte Array with the same length as the string </returns>
        public static DataList ConvertASCIIString(string value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.Add((byte)(value[i] & 0x7F));
            }
            return ret;
        }

        /// <summary>
        /// Convert a UTF8 string to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The UTF8 String to convert </param>
        /// <returns> A DataList Byte Array with the same length as the string or compressed length + 1 </returns>
        public static DataList ConvertUTF8String(string value)
        {
            DataList ret = new DataList();

            for (int i = 0; i < value.Length; i++)
            {
                uint character = value[i];
                if (character < 0x80)
                {
                    ret.Add((byte)character);
                }
                else if (character < 0x800)
                {
                    ret.Add((byte)(0b11000000 | ((character >> 6) & 0b11111)));
                    ret.Add((byte)(0b10000000 | (character & 0b111111)));
                }
                else if (character < 0x10000)
                {
                    ret.Add((byte)(0b11100000 | ((character >> 12) & 0b1111)));
                    ret.Add((byte)(0b10000000 | ((character >> 6) & 0b111111)));
                    ret.Add((byte)(0b10000000 | (character & 0b111111)));
                }
                else
                {
                    ret.Add((byte)(0b11110000 | ((character >> 18) & 0b111)));
                    ret.Add((byte)(0b10000000 | ((character >> 12) & 0b111111)));
                    ret.Add((byte)(0b10000000 | ((character >> 6) & 0b111111)));
                    ret.Add((byte)(0b10000000 | (character & 0b111111)));
                }
            };

            return ret;
        }

        #endregion

        #region string types array

        /// <summary>
        /// Convert ASCII strings to a DataList Array
        /// </summary>
        /// <param name="value"> The ASCII strings to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertASCIIStrings(string[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertASCIIString(value[i]));
            }
            return ret;
        }

        /// <summary>
        /// Convert UTF8 strings to a DataList Array
        /// </summary>
        /// <param name="value"> The UTF8 strings to convert </param>
        /// <returns> A single DataList containing the individual converted results </returns>
        public static DataList ConvertUTF8String(string[] value)
        {
            DataList ret = new DataList();
            for (int i = 0; i < value.Length; i++)
            {
                ret.AddRange(ConvertUTF8String(value[i]));
            }
            return ret;
        }

        #endregion

        #region VRC types

        /// <summary>
        /// Returns the value for a DataToken regardless of type as an object
        /// </summary>
        /// <param name="value"> The DataToken to get the value for </param>
        /// <returns> object storing the DataToken value </returns>
        public static object ReturnDataTokenValueAsObject(DataToken value)
        {
            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                Debug.Log("ByteConverter is converting DataToken into its object with type: " + value.TokenType);

            switch (value.TokenType)
            {
                case TokenType.Null:
                    return new DataList();
                case TokenType.Byte:
                    return value.Byte;
                case TokenType.SByte:
                    return value.SByte;
                case TokenType.Boolean:
                    return value.Boolean;
                case TokenType.Short:
                    return value.Short;
                case TokenType.UShort:
                    return value.UShort;
                case TokenType.Int:
                    return value.Int;
                case TokenType.UInt:
                    return value.UInt;
                case TokenType.Long:
                    return value.Long;
                case TokenType.ULong:
                    return value.ULong;
                case TokenType.Float:
                    return value.Float;
                case TokenType.Double:
                    return (decimal)value.Double;
                case TokenType.String:
                    return value.String;
                case TokenType.DataList:
                    return value.DataList;
                case TokenType.DataDictionary:
                    return value.DataDictionary;
                case TokenType.Reference:
                    return value.Reference;
                default:
                    Debug.LogError("Token Type is unhandled, please use an alternative");
                    return new DataList();
            }
        }

        /// <summary>
        /// Convert a DataDictionary to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The DataDictionary to convert </param>
        /// <returns> A single DataList Byte Array </returns>
        public static DataList ConvertDataDictionary(DataDictionary value)
        {
            DataList ret = new DataList();
            DataList keys = value.GetKeys();

            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                Debug.Log("ByteConverter is converting an DataDictionary: " + value);

            //Loop through the key/value pairs and convert them one by one
            for (int i = 0; i < keys.Count; i++)
            {
                DataList keyBytes = ConvertDataToken(keys[i]);
                ret.Add(keyBytes.Count);
                ret.AddRange(keyBytes);

                DataList valuesBytes = ConvertDataToken(value[keys[i]]);
                ret.Add(valuesBytes.Count);
                ret.AddRange(valuesBytes);

                if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                    Debug.Log("ByteConverter is converting an DataDictionary key: " + 
                        ReturnDataTokenValueAsObject(keys[i]) +
                        ", Into the bytes: " + keyBytes.Count +
                        "And Value: " +
                        ReturnDataTokenValueAsObject(value[keys[i]]) +
                        ", Into the bytes: " + valuesBytes.Count);
            }
            return ret;
        }

        /// <summary>
        /// Convert a DataList to a DataList Byte Array
        /// </summary>
        /// <param name="value"> The DataList to convert </param>
        /// <returns> A single DataList Byte Array </returns>
        public static DataList ConvertDataList(DataList value)
        {
            if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                Debug.Log("ByteConverter is converting an DataList: " + value);

            DataList ret = new DataList();
            for (int i = 0; i < value.Count; i++)
            {
                if (DevelopmentManager.IsByteConverterEnabled(DevelopmentMode.Advanced))
                    Debug.Log("ByteConverter is converting DataList element of type: " + value[i].TokenType);

                switch (value[i].TokenType)
                {
                    case TokenType.Null:
                        //ret.Add((byte)0);
                        break;
                    case TokenType.Byte:
                        //ret.Add((byte)1);
                        ret.Add(value[i].Byte);
                        break;
                    case TokenType.SByte:
                        //ret.Add((byte)1);
                        ret.AddRange(ConvertSByte(value[i].SByte));
                        break;
                    case TokenType.Boolean:
                        //ret.Add((byte)1);
                        ret.AddRange(ConvertBool(value[i].Boolean));
                        break;
                    case TokenType.Short:
                        //ret.Add((byte)2);
                        ret.AddRange(ConvertInt16(value[i].Short));
                        break;
                    case TokenType.UShort:
                        //ret.Add((byte)2);
                        ret.AddRange(ConvertUInt16(value[i].UShort));
                        break;
                    case TokenType.Int:
                        //ret.Add((byte)4);
                        ret.AddRange(ConvertInt32(value[i].Int));
                        break;
                    case TokenType.UInt:
                        //ret.Add((byte)4);
                        ret.AddRange(ConvertUInt32(value[i].UInt));
                        break;
                    case TokenType.Long:
                        //ret.Add((byte)8);
                        ret.AddRange(ConvertInt64(value[i].Long));
                        break;
                    case TokenType.ULong:
                        //ret.Add((byte)8);
                        ret.AddRange(ConvertUInt64(value[i].ULong));
                        break;
                    case TokenType.Float:
                        //ret.Add((byte)4);
                        ret.AddRange(ConvertSingle(value[i].Float));
                        break;
                    case TokenType.Double:
                        //ret.Add((byte)16);
                        ret.AddRange(ConvertDecimal((decimal)value[i].Double));
                        break;
                    case TokenType.String:
                        DataList stringBytes = ConvertUTF8String(value[i].String);
                        ret.Add((byte)stringBytes.Count);
                        ret.AddRange(stringBytes);
                        break;
                    case TokenType.DataList:
                        DataList convertedDataListBytes = ConvertDataList(value[i].DataList);
                        ret.Add((byte)(convertedDataListBytes.Count & 0xFF));
                        ret.AddRange(convertedDataListBytes);
                        break;
                    case TokenType.DataDictionary:
                        DataList convertedDataDictionaryBytes = ConvertDataDictionary(value[i].DataDictionary);
                        ret.Add((byte)(convertedDataDictionaryBytes.Count & 0xFF));
                        ret.AddRange(convertedDataDictionaryBytes);
                        break;
                    case TokenType.Reference:
                        DataList convertedObjectBytes = ConvertObject(value[i].Reference);
                        ret.Add((byte)(convertedObjectBytes.Count & 0xFF));
                        ret.AddRange(convertedObjectBytes);
                        break;
                    default:
                        Debug.LogError("Token Type is unhandled, please use an alternative");
                        break;
                }
            }
            return ret;
        }

        #endregion
    }

}