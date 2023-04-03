using UnityEngine;
using System;

public static class ArrayUtilities
{
    public static void SwapValuesInArray<T>(ref T[] _array, int _indexToSwap, int _indexToSwapTo)
    {
        if (_indexToSwap == _indexToSwapTo ||
            !(_indexToSwap < _array.Length && _indexToSwap >= 0) ||
            !(_indexToSwapTo < _array.Length && _indexToSwapTo >= 0)) return;

        T _toSwap = _array[_indexToSwap];
        _array[_indexToSwap] = _array[_indexToSwapTo];
        _array[_indexToSwapTo] = _toSwap;
    }

    //Move and rearange objects in an array
    public static void MoveValueInArray<T>(ref T[] _array, int _indexToMove, int _indexToMoveTo)
    {
        if (_indexToMoveTo == _indexToMove ||
            !(_indexToMove < _array.Length && _indexToMove >= 0) ||
            !(_indexToMoveTo < _array.Length && _indexToMoveTo >= 0)) return;

        T _toMove = _array[_indexToMove];

        if (_indexToMoveTo > _indexToMove)
        {
            Array.Copy(_array, _indexToMove + 1, _array, _indexToMove, _indexToMoveTo - _indexToMove - 1);
        }
        else
        {
            Array.Copy(_array, _indexToMoveTo, _array, _indexToMoveTo + 1, _indexToMove - _indexToMoveTo - 1);
        }

        _array[_indexToMoveTo] = _toMove;
    }

    public static void AddToArray<T>(ref T[] _array, T _value)
    {
        ResizeArray(ref _array, _array.Length + 1);
        _array[_array.Length - 1] = _value;
    }

    public static void AddRangeToArray<T>(ref T[] _array, T[] _values)
    {
        ResizeArray(ref _array, _array.Length + _values.Length);
        Array.Copy(_values, 0, _array, _array.Length - _values.Length, _values.Length);
    }

    public static void AddToArrayAtIndex<T>(ref T[] _array, T _value, int _index)
    {
        if (_index > _array.Length || _index < 0) { Debug.LogError("_index is not valid: " + _index); return; }

        T[] tempArray = new T[_array.Length + 1];
        if (_index == 0)
        {
            Array.Copy(_array, 0, tempArray, 1, _array.Length);
        }
        else if (_index == _array.Length)
        {
            Array.Copy(_array, 0, tempArray, 0, _array.Length);
        }
        else
        {
            Array.Copy(_array, 0, tempArray, 0, _index);
            Array.Copy(_array, _index, tempArray, _index + 1, tempArray.Length - _index);
        }

        tempArray[_index] = _value;
        _array = tempArray;
    }

    public static void AddRangeToArrayAtIndex<T>(ref T[] _array, T[] _values, int _index)
    {
        if (_index > _array.Length || _index < 0) { Debug.LogError("_index is not valid: " + _index); return; }

        T[] tempArray = new T[_array.Length + _values.Length];
        if (_index == 0)
        {
            Array.Copy(_values, 0, tempArray, 0, _index);
            Array.Copy(_array, 0, tempArray, _values.Length, _array.Length);
        }
        else if (_index == _array.Length)
        {
            Array.Copy(_array, 0, tempArray, 0, _array.Length);
            Array.Copy(_values, 0, tempArray, _values.Length, _values.Length);
        }
        else
        {
            Array.Copy(_array, 0, tempArray, 0, _index);
            Array.Copy(_values, 0, tempArray, _index, _values.Length);
            Array.Copy(_array, _index, tempArray, _index + _values.Length, tempArray.Length - _values.Length - _index);
        }

        _array = tempArray;
    }

    public static void RemoveValueFromArrayAtIndex<T>(ref T[] _array, int _index)
    {
        if (_index < 0 || _index > _array.Length - 1) return;

        T[] tempArray = new T[_array.Length - 1];
        Array.Copy(_array, 0, tempArray, 0, _index);
        Array.Copy(_array, _index + 1, tempArray, _index, _array.Length - 1 - _index);
        _array = tempArray;
    }

    public static void RemoveRangeFromArray<T>(ref T[] _array, int _rangeStart, int _rangeEnd)
    {
        _rangeStart = _rangeStart < 0 ? 0 : _rangeStart;
        _rangeEnd = _rangeEnd >= _array.Length ? _array.Length - 1 : _rangeEnd;
        if (_rangeStart > _array.Length - 1 || _rangeEnd < 0) return;

        if (_rangeEnd < _rangeStart)
        {
            int _newRangeStart = _rangeEnd;
            _rangeEnd = _rangeStart;
            _rangeStart = _newRangeStart;
        }

        int _rangeSize = _rangeEnd - _rangeStart;
        if (_rangeSize == _array.Length)
        {
            _array = new T[0];
            return;
        }

        T[] tempArray = new T[_array.Length - _rangeSize];
        Array.Copy(_array, 0, tempArray, 0, _rangeStart);
        Array.Copy(_array, _rangeEnd, tempArray, _rangeStart, _array.Length - _rangeEnd);
        _array = tempArray;
    }

    public static void RemoveFirstValueFromArray<T>(ref T[] _array, T _value)
    {
        RemoveValueFromArrayAtIndex(ref _array, ReturnFirstIndexOfValueFromArray(_array, _value));
    }

    public static void RemoveAllValuesFromArray<T>(ref T[] _array, T _value, bool _keepOrdering)
    {
        int[] indices = ReturnAllIndicesOfValueFromArray(_array, _value);

        for (int a = 0; a < indices.Length; a++)
        {
            _array[indices[a]] = default(T);
        }

        SortAndRemoveNullsFromArray(ref _array);
    }

    public static int ReturnFirstIndexOfValueFromArray<T>(T[] _array, T _value)
    {
        for (int a = 0; a < _array.Length; a++)
        {
            if (_array[a].Equals(_value)) return a;
        }
        return -1;
    }

    public static int[] ReturnAllIndicesOfValueFromArray<T>(T[] _array, T _value)
    {
        int[] ret = new int[_array.Length];
        int currentArrayIndex = 0;
        for (int a = 0; a < _array.Length; a++)
        {
            if (_array[a].Equals(_value))
            {
                ret[currentArrayIndex] = a;
                currentArrayIndex++;
            }
        }
        ResizeArray(ref ret, currentArrayIndex - 1);
        return ret;
    }

    public static bool CheckObjectInArray<T>(T[] _array, T _value)
    {
        return ReturnFirstIndexOfValueFromArray(_array, _value) != -1;
    }

    public static void SortAndRemoveNullsFromArray<T>(ref T[] _array)
    {
        if (_array == null || _array.Length == 0) { _array = new T[0]; return; }

        int _arrayNullOffset = 0;
        for (int i = 0; i < _array.Length; i++)
        {
            if (_array[i] == null)
            {
                _arrayNullOffset++;
            }
            else
            {
                _array[i - _arrayNullOffset] = _array[i];
            }
        }

        ResizeArray(ref _array, _array.Length - _arrayNullOffset);
    }

    public static void ResizeArray<T>(ref T[] _array, int _size)
    {
        T[] tmp = new T[_size];
        if (_array != null) Array.Copy(_array, 0, tmp, 0, _size > _array.Length ? _array.Length : _size);
        _array = tmp;
    }

    //Conversion Functions For Basic Types
    public static T[] ConvertTypeArrayToOtherTypeArray<T, t>(t[] _array)
    {
        T[] ret = new T[_array.Length];
        for (int a = 0; a < ret.Length; a++)
        {
            ret[a] = (T)Convert.ChangeType(_array[a], typeof(T));
        }
        return ret;
    }

    public static T[] ReturnAllTypesInArray<T>(T[] _array, object value, string compare)
    {
        T[] ret = new T[0];
        for (int a = 0; a < _array.Length; a++)
        {
            if (_array[a] == null) continue;
            object cmp = _array[a].GetType().GetField(compare).GetValue(_array[a]);
            if (cmp == value) AddToArray(ref ret, _array[a]);
        }
        return ret;
    }
}