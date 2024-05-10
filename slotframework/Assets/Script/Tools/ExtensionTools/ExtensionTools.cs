using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public static class ExtensionTools
{

    /// <summary>
    /// list�������
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="list"></param>
    public static void ListSortRandom<T>(this List<T> list)
    {
        int randomIndex;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            randomIndex = Random.Range(0, i);
            (list[randomIndex], list[i]) = (list[i], list[randomIndex]);
        }
    }

    /// <summary>
    /// ��ȡ�������
    /// </summary>
    /// <param name="list"></param>
    /// <param name="isRemove">�Ƿ���Ҫ��List���Ƴ�����</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetRandomItem<T>(this List<T> list, bool isRemove = true)
    {
        if (list.Count==0)
        {
            Debug.LogError("ListΪ���޷���ȡ����");
        }
        T item = list[Random.Range(0, list.Count)];
        if (isRemove)
        {
            list.Remove(item);
        }
        return item;
    }

    /// <summary>
    /// list<int>�������Ϊ�ַ���
    /// </summary>
    /// <param name="ints"></param>
    /// <returns></returns>
    public static string ListIntToString(this List<int> ints)
    {
        string str = "";
        foreach (var item in ints)
        {
            str += item.ToString();
        }
        return str;
    }

    /// <summary>
    /// ����ָ�����
    /// </summary>
    /// <param name="value"></param>
    /// <param name="num0fParts">�ָ����</param>
    /// <param name="factor">�ָ�鹫����</param>
    /// <returns></returns>
    public static List<int> SplitValue(this int value, int num0fParts, int factor = 1)
    {
        value /= factor;
        List<int> result = new List<int>();
        for (int i = 0; i < num0fParts; i++)
        {
            result.Add(0);
        }
        if (value >= num0fParts)
        {
            for (int i = 0; i < num0fParts; i++)
            {
                result[i] += 1 * factor;
            }
            value -= num0fParts;
        }
        else
        {
            Debug.LogError("�ָ�������ڷָ�ֵ/�������������0�ָ��");
        }
        for (int i = 0; i < value; i++)
        {
            result[Random.Range(0, num0fParts)] += 1 * factor;
        }
        return result;
    }
    
    /// <summary>
    /// ����ָ�List
    /// </summary>
    /// <param name="value"></param>
    /// <param name="numOfParts">�ָ����</param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<List<T>> SplitList<T>(this List<T> value,int numOfParts)
    {
        List<List<T>> result = new List<List<T>>(numOfParts);
        for (int i = 0; i < numOfParts; i++)
        {
            result.Add(new List<T>());
        }
        foreach (var item in value)
        {
            result[Random.Range(0,result.Count)].Add(item);
        }

        return result;
    }

    /// <summary>
    /// ȡList�����������ĺ�
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int ListIntSum(this List<int> value)
    {
        int sum = 0;
        foreach (var item in value)
        {
            sum += item;
        }

        return sum;
    }
    
    
}
