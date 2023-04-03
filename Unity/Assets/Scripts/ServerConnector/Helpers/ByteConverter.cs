using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ServerConnector
{

    /// <summary>
    /// This is class is a combination of two different methods of converting to make generating server requests easier.
    /// <see cref="https://github.com/Xytabich/UNet/blob/master/UNet/ByteBufferWriter.cs"/>
    /// <see cref="https://github.com/Miner28/NetworkedEventCaller"/>
    /// </summary>
    public static class ByteConverter
    {
        //private static Type[] _types =
        //{
        //    typeof(bool),
        //    typeof(byte),
        //    typeof(sbyte),
        //    typeof(short),
        //    typeof(ushort),
        //    typeof(int),
        //    typeof(uint),
        //    typeof(long),
        //    typeof(ulong),
        //    typeof(float),
        //    typeof(double),
        //    typeof(decimal),
        //    typeof(string),
        //    typeof(Color),
        //    typeof(Color32),
        //    typeof(Vector2),
        //    typeof(Vector2Int),
        //    typeof(Vector3),
        //    typeof(Vector3Int),
        //    typeof(Vector4),
        //    typeof(Quaternion),
        //    typeof(DateTime),
        //    //Arrays
        //    typeof(bool[]),
        //    typeof(byte[]),
        //    typeof(sbyte[]),
        //    typeof(short[]),
        //    typeof(ushort[]),
        //    typeof(int[]),
        //    typeof(uint[]),
        //    typeof(long[]),
        //    typeof(ulong[]),
        //    typeof(float[]),
        //    typeof(double[]),
        //    typeof(decimal[]),
        //    typeof(string[]),
        //    typeof(Color[]),
        //    typeof(Color32[]),
        //    typeof(Vector2[]),
        //    typeof(Vector2Int[]),
        //    typeof(Vector3[]),
        //    typeof(Vector3Int[]),
        //    typeof(Vector4[]),
        //    typeof(Quaternion[]),
        //};

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

        #region basic types

        /// <summary>
        /// Convert a Boolean to a byte array
        /// </summary>
        /// <param name="value"> The boolean to convert </param>
        /// <returns> A single byte array</returns>
        public static byte[] ConvertBool(bool value)
        {
            if (value == true) return new byte[] { 1 };
            return new byte[] { 0 };
        }

        /// <summary>
        /// Convert a Char to a byte array
        /// </summary>
        /// <param name="value"> The char to convert </param>
        /// <returns> A 2 byte array </returns>
        public static byte[] ConvertChar(char value)
        {
            return new byte[]
            {
                (byte)value,
                (byte)(value >> BIT8)
            };
        }

        /// <summary>
        /// Convert a SByte to a byte array
        /// </summary>
        /// <param name="value"> The SByte to convert </param>
        /// <returns> A single byte array </returns>
        public static byte[] ConvertSByte(sbyte value)
        {
            return new byte[]
            {
                (byte)(value < 0 ? value + 256 : value)
            };
        }

        /// <summary>
        /// Convert a Int16 to a byte array
        /// </summary>
        /// <param name="value"> The Int16 to convert </param>
        /// <returns> A 2 byte array </returns>
        public static byte[] ConvertInt16(Int16 value)
        {
            int tmp = value < 0 ? value + 0xFFFF : value;
            return new byte[]
            {
                (byte)(tmp >> BIT8),
                (byte)(tmp & 0xFF),
            };
        }

        /// <summary>
        /// Convert a UInt16 to a byte array
        /// </summary>
        /// <param name="value"> The UInt16 to convert </param>
        /// <returns> A 2 byte array </returns>
        public static byte[] ConvertUInt16(UInt16 value)
        {
            int tmp = Convert.ToInt32(value);
            return new byte[]
            {
                (byte)(tmp >> BIT8),
                (byte)(tmp & 0xFF),
            };
        }

        /// <summary>
        /// Convert an Integer to a byte array
        /// </summary>
        /// <param name="value"> The Integer to convert </param>
        /// <returns> A 4 byte array </returns>
        public static byte[] ConvertInt32(int value)
        {
            return new byte[]
            {
                (byte)((value >> BIT24) & 0xFF),
                (byte)((value >> BIT16) & 0xFF),
                (byte)((value >> BIT8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        /// <summary>
        /// Convert an Unsigned Integer to a byte array
        /// </summary>
        /// <param name="value"> The Unsigned Integer to convert </param>
        /// <returns> A 4 byte array </returns>
        public static byte[] ConvertUInt32(uint value)
        {
            return new byte[]
            {
                (byte)((value >> BIT24) & 255u),
                (byte)((value >> BIT16) & 255u),
                (byte)((value >> BIT8) & 255u),
                (byte)(value & 255u)
            };
        }

        /// <summary>
        /// Convert a 64-bit Integer to a byte array
        /// </summary>
        /// <param name="value"> The Integer to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertInt64(Int64 value)
        {
            return new byte[]
            {
                (byte)((value >> BIT56) & 0xFF),
                (byte)((value >> BIT48) & 0xFF),
                (byte)((value >> BIT40) & 0xFF),
                (byte)((value >> BIT32) & 0xFF),
                (byte)((value >> BIT24) & 0xFF),
                (byte)((value >> BIT16) & 0xFF),
                (byte)((value >> BIT8) & 0xFF),
                (byte)(value & 0xFF)
            };
        }

        /// <summary>
        /// Convert a 64-bit Unsigned Integer to a byte array
        /// </summary>
        /// <param name="value"> The Unsigned Integer to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertUInt64(UInt64 value)
        {
            return new byte[]
            {
                (byte)((value >> BIT56) & 255ul),
                (byte)((value >> BIT48) & 255ul),
                (byte)((value >> BIT40) & 255ul),
                (byte)((value >> BIT32) & 255ul),
                (byte)((value >> BIT24) & 255ul),
                (byte)((value >> BIT16) & 255ul),
                (byte)((value >> BIT8) & 255ul),
                (byte)(value & 255ul)
            };
        }

        /// <summary>
        /// Convert a single-precision floating-point into a byte array
        /// </summary>
        /// <param name="value"> The single-precision floating-point to convert </param>
        /// <returns> A 4 byte array </returns>
        public static byte[] ConvertSingle(float value)
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
        /// Convert a half-precision floating-point to a byte array
        /// </summary>
        /// <param name="value"> The half-precision floating-point to convert </param>
        /// <returns> A 2 byte array </returns>
        public static byte[] ConvertHalf(float value)
        {
            return ConvertUInt16(Mathf.FloatToHalf(value));
        }

        /// <summary>
        /// Convert a DateTime to a byte array
        /// </summary>
        /// <param name="value"> The DateTime to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertDateTime(DateTime value)
        {
            return ConvertInt64(value.ToBinary());
        }

        /// <summary>
        /// Convert a TimeSpan to a byte array
        /// </summary>
        /// <param name="value"> The TimeSpan to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertTimeSpan(TimeSpan value)
        {
            return ConvertInt64(value.Ticks);
        }

        /// <summary>
        /// Convert a Decimal floating-point to a byte array
        /// </summary>
        /// <param name="value"> The Decimal floating-point to convert </param>
        /// <returns> A 16 byte array </returns>
        public static byte[] ConvertDecimal(Decimal value)
        {
            var tmp = Decimal.GetBits(value);
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertInt32(tmp[0]));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertInt32(tmp[1]));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertInt32(tmp[2]));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertInt32(tmp[3]));
            return ret;
        }

        /// <summary>
        /// Convert a Guid to a byte array
        /// </summary>
        /// <param name="value"> The Guid to convert </param>
        /// <returns> A 16 byte array </returns>
        public static byte[] ConvertGUID(Guid value)
        {
            return value.ToByteArray();
        }
        #endregion



        #region unity types

        /// <summary>
        /// Convert a Vector2 to a byte array
        /// </summary>
        /// <param name="value"> The Vector2 to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertVector2(Vector2 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.y));
            return ret;
        }

        /// <summary>
        /// Convert a Vector3 to a byte array
        /// </summary>
        /// <param name="value"> The Vector3 to convert </param>
        /// <returns> A 12 byte array </returns>
        public static byte[] ConvertVector3(Vector3 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.z));
            return ret;
        }

        /// <summary>
        /// Convert a Vector4 to a byte array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 16 byte array </returns>
        public static byte[] ConvertVector4(Vector4 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.z));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a Quaternion to a byte array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 16 byte array </returns>
        public static byte[] ConvertQuaternion(Quaternion value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.z));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertSingle(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector2 to a byte array
        /// </summary>
        /// <param name="value"> The Vector2 to convert </param>
        /// <returns> A 2 byte array </returns>
        public static byte[] ConvertHalfVector2(Vector2 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.y));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector3 to a byte array
        /// </summary>
        /// <param name="value"> The Vector3 to convert </param>
        /// <returns> A 6 byte array </returns>
        public static byte[] ConvertHalfVector3(Vector3 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.z));
            return ret;
        }

        /// <summary>
        /// Convert a half Vector4 to a byte array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertHalfVector4(Vector4 value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.z));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.w));
            return ret;
        }

        /// <summary>
        /// Convert a half Quaternion to a byte array
        /// </summary>
        /// <param name="value"> The Vector4 to convert </param>
        /// <returns> A 8 byte array </returns>
        public static byte[] ConvertHalfQuaternion(Quaternion value)
        {
            byte[] ret = new byte[0];
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.x));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.y));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.z));
            ArrayUtilities.AddRangeToArray(ref ret, ConvertHalf(value.w));
            return ret;
        }
        #endregion

        #region strings

        /// <summary>
        /// Convert a ASCII string to a byte array
        /// </summary>
        /// <param name="value"> The ASCII String to convert </param>
        /// <returns> A byte array with the same length as the string </returns>
        public static byte[] ConvertASCIIString(string value)
        {
            byte[] ret = new byte[value.Length];
            for (int i = 0; i < value.Length; i++)
            {
                ret[i] = (byte)(value[i] & 0x7F);
            }
            return ret;
        }

        /// <summary>
        /// Convert a UTF8 string to a byte array
        /// </summary>
        /// <param name="value"> The UTF8 String to convert </param>
        /// <param name="compress"> Controls if any unused bytes are removed </param>
        /// <returns> A byte array with the same length as the string or compressed length + 1 </returns>
        public static byte[] ConvertUTF8String(string value)
        {
            byte[] ret = new byte[value.Length * 4];

            int writeIndex = 0;
            for (int i = 0; i < value.Length; i++)
            {
                uint character = value[i];

                if (character < 0x80)
                {
                    ret[writeIndex++] = (byte)character;
                }
                else if (character < 0x800)
                {
                    ret[writeIndex++] = (byte)(0b11000000 | ((character >> 6) & 0b11111));
                    ret[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
                else if (character < 0x10000)
                {
                    ret[writeIndex++] = (byte)(0b11100000 | ((character >> 12) & 0b1111));
                    ret[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
                    ret[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
                else
                {
                    ret[writeIndex++] = (byte)(0b11110000 | ((character >> 18) & 0b111));
                    ret[writeIndex++] = (byte)(0b10000000 | ((character >> 12) & 0b111111));
                    ret[writeIndex++] = (byte)(0b10000000 | ((character >> 6) & 0b111111));
                    ret[writeIndex++] = (byte)(0b10000000 | (character & 0b111111));
                }
            }

            ArrayUtilities.AddToArrayAtIndex(ref ret, (byte)value.Length, 0);
            ArrayUtilities.ResizeArray(ref ret, writeIndex + 1);
            return ret;
        }
        #endregion
    }

}