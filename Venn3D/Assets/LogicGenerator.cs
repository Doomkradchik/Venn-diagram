using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class LogicGenerator : MonoBehaviour
{

    public ThreeSetPresenter[] setPresenters;


    void Start()
    {
        //setPresenter.VisualizeLogic(SetKind.A);
    }

    public void GenerateRandomLogicThreeChain()
    {
        var orders = GenerateRandomOrder(3);
        var ops = GenerateFromOrder(orders);
        foreach (var presenter in setPresenters)
            presenter.VisualizeLogic((SetKind)orders[0], ops);
        Debug.Log(ToString((SetKind)orders[0], ops));
    }

    string ToString(SetKind x, params BinarySetOperation[] binarySetOperations)
    {
        
        string s = $"{x}";
        if(binarySetOperations != null)
            foreach(var b in binarySetOperations)
            {
                s += $" {b.o} {b.setKind}";
            }

        return s;
    }

    BinarySetOperation[] GenerateFromOrder(int[] ids)
    {
        if (ids.Length == 1) { return null; }
        var bops = new BinarySetOperation[ids.Length - 1];
        for (int i = 0; i < bops.Length; i++)
        {
            bops[i] = new BinarySetOperation
            {
                o = (Operator)Random.Range(0, 4),
                setKind = (SetKind)ids[i + 1]
            };
        }

        return bops;
    }

    int[] GenerateRandomOrder(int maxLength)
    {
        int[] numbers = { 0, 1, 2 };

        // Fisher-Yates Shuffle Algorithm
        for (int i = numbers.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);

            // Swap the elements at randomIndex and i
            int temp = numbers[i];
            numbers[i] = numbers[randomIndex];
            numbers[randomIndex] = temp;
        }

        // Take a random subset with a length between 1 and maxLength
        int subsetLength = Random.Range(1, Mathf.Min(maxLength, numbers.Length) + 1);
        int[] randomSubset = new int[subsetLength];

        for (int i = 0; i < subsetLength; i++)
        {
            randomSubset[i] = numbers[i];
        }

        return randomSubset;
    }
}
