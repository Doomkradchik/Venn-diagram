using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum Operator : int
{
    Union,
    Inter,
    DiffL,
    DiffR
}

public enum SetKind : short
{
    A,
    B,
    C
}

public struct BinarySetOperation
{
    public Operator o;
    public SetKind setKind;
}

[System.Serializable]
public struct Set
{
    public Renderer visualRenderer;
    public Renderer maskRenderer;
}

[System.Serializable]
public struct ActionSet
{
    public Vector2Int mOp;
   // public Action<> comp;
}

public class ThreeSetPresenter : MonoBehaviour
{
    public Set setA;
    public Set setB;
    public Set setC;


    Dictionary<SetKind, Set> sets;
    Dictionary<Operator, ActionSet> actionSets;
    int q;

    private void Awake()
    {
        q = setA.maskRenderer.material.renderQueue;
        //actionSets = new Dictionary<Operator, ActionSet>()
        //{
        //    { Operator.DiffL, new ActionSet { mOp = new Vector2Int(0,5), comp = (r, d) =>  } }
        //}

        draw = new Dictionary<SetKind, bool>()
        {
            {SetKind.A, false},
            {SetKind.B, false},
            {SetKind.C, false},
        };

        sets = new Dictionary<SetKind, Set>
        {
            {SetKind.A, setA},
            {SetKind.B, setB},
            {SetKind.C, setC},
        };
    }

    private void Start()
    {
      //  ImplementLogic7Abstract();

        //ImplementLogic2();
    }


    void ResetPassStencils()
    {
        foreach (var set in sets)
        {
            set.Value.maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 0);
            set.Value.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 0);
            // set.Value.mask.gameObject.SetActive(draw[set.Key]);
        }
    }

    public void VisualizeLogic(SetKind x, params BinarySetOperation[] binarySetOperations)
    {
        if (binarySetOperations != null && binarySetOperations.Length > 2)
        { throw new System.InvalidOperationException(); }
        int r = 0;
        int d = 0;
        int order = 3;
        ResetPassStencils();
        StartLogic(x, ref r, ref d, ref order);

        if(binarySetOperations != null)
            foreach (var bso in binarySetOperations)
            {
                switch (bso.o)
                {
                    case Operator.Union:
                        Union(bso.setKind, ref r, ref d, ref order);
                        break;
                    case Operator.DiffL:
                        DifferenceLeft(bso.setKind, ref r, ref d, ref order);
                        break;
                    case Operator.Inter:
                        Intersect(bso.setKind, ref r, ref d, ref order);
                        break;
                    case Operator.DiffR:
                        DifferenceRight(bso.setKind, ref r, ref d, ref order);
                        break;
                }
            }

        Visualize(r, d);

        
    }

    /// <summary>
    /// A difference (C intersection B)
    /// </summary>
    /// 


    Dictionary<SetKind, bool> draw;

    void ResetDraw(SetKind ignore)
    {
        foreach (var key in draw.Keys.ToArray())
        {
            if (key == ignore) { continue; }
            draw[key] = false;
        }
    }

    void ResetDraw()
    {
        foreach (var key in draw.Keys.ToArray())
        {
            draw[key] = false;
        }
    }


    void StartLogic(SetKind setKind, ref int r, ref int d,ref int order)
    {
        sets[setKind].maskRenderer.material.renderQueue = q - order;
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);
        draw[setKind] = true;
        r = 1;
        d = 3;
        order--;
    }

    void Intersect(SetKind setKind, ref int r, ref int d, ref int order)
    {
        sets[setKind].maskRenderer.material.renderQueue = q - order;
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
        draw[setKind] = true;
        r++;
        d = 3;
        order--;
    }

    void Union(SetKind setKind, ref int r, ref int d, ref int order)
    {
        sets[setKind].maskRenderer.material.renderQueue = q - order;
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), r);
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);
        draw[setKind] = true;
        order--;
    }

    void DifferenceLeft(SetKind setKind, ref int r, ref int d, ref int order)
    {
        sets[setKind].maskRenderer.material.renderQueue = q - order;
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 0);
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 5);
        draw[setKind] = true;

        ResetDraw(setKind);
        r = 255 - r + 1;
        d = 4;
        order--;
    }

    void DifferenceRight(SetKind setKind, ref int r, ref int d, ref int order)
    {
        sets[setKind].maskRenderer.material.renderQueue = q - order;
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 0);
        sets[setKind].maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 1);
        draw[setKind] = false;
        order--;
    }


    void Visualize(int r, int d)
    {
        foreach(var set in sets)
        {
            set.Value.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), r);
            set.Value.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"),draw[set.Key] ? d : 1);
           // set.Value.visualRenderer.gameObject.SetActive(draw[set.Key]);
        }

        ResetDraw();
    }

    //private void Clean()
    //{
    //    foreach (var set in sets)
    //    {
    //        set.Value.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), r);
    //        set.Value.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"), draw[set.Key] ? d : 1);
    //        //set.Value.visualRenderer.gameObject.SetActive(draw[set.Key]);
    //    }
    //}

    //void ImplementLogic6Abstract()
    //{
    //    int r = 0;
    //    int d = 0;
    //    int order = 3;

    //    StartLogic(SetKind.A,ref r,ref d, ref order);
    //    DifferenceLeft(SetKind.C, ref r, ref d, ref order);
    //    DifferenceLeft(SetKind.B, ref r, ref d, ref order);

    //    Visualize(r, d);
    //}

    ///// <summary>
    /////  A un B / C
    ///// </summary>
    //void ImplementLogic7Abstract()
    //{
    //    int r = 0;
    //    int d = 0;
    //    int order = 3;

    //    StartLogic(SetKind.A, ref r, ref d, ref order);
    //    Union(SetKind.B, ref r, ref d, ref order);
    //    Intersect(SetKind.C, ref r, ref d, ref order);


    //    Visualize(r, d);
    //    //OP1()
    //    //Op2()
    //    //Comp()
    //    //View

    //}

    ///// <summary>
    ///// A inter B inter C
    ///// </summary>
    //void ImplementLogic5Abstract()
    //{
    //    int r = 0;
    //    int d;
    //    setB.maskRenderer.material.renderQueue -= 1;
    //    setA.maskRenderer.material.renderQueue -= 2;
    //    setC.maskRenderer.material.renderQueue -= 3;


    //    setA.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    //    r++;
    //    d = 3;
    //    setB.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    //    r++;
    //    d = 3;
    //    setC.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    //    r++;
    //    d = 3;


    //    setA.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), r);
    //    setA.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"), d);
    //    setB.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), r);
    //    setB.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"),d);
    //    setC.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), r);
    //    setC.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"), d);
    //}


    //private void ImplementLogic4()
    //{
    //    setB.maskRenderer.material.renderQueue -= 1;
    //    setA.maskRenderer.material.renderQueue -= 2;
    //    setA.maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setA.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);
    //    setB.maskRenderer.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setB.maskRenderer.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 1);

    //    setA.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 1);
    //    setA.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 3);
    //    setB.visualRenderer.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    //    setB.visualRenderer.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);
    //}

    ///// <summary>
    ///// A / B
    ///// </summary>
    ////void ImplementLogic4()
    ////{
    ////    setA.material.renderQueue -= 1000;
    ////    setA.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    ////    setA.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);
    ////    setA.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 1);
    ////    setA.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 3);

    ////    setB.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    ////    setB.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 1);
    ////    setB.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    ////    setB.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);
    ////}


    /////// <summary>
    /////// A / B inter C
    /////// </summary>
    ////void ImplementLogic3()
    ////{
    ////    int rn = 0;

    ////    MatrixOperations3(ref rn);
    ////    DrawRes3(rn);
    ////}

    ////void MatrixOperations3(ref int rn)
    ////{
    ////    setA.material.renderQueue -= 2;
    ////    setA.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    ////    setA.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);

    ////    setB.material.renderQueue -= 1;
    ////    setB.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    ////    setB.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 4);

    ////    setC.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    ////    setC.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    ////}

    ////void DrawRes3(int rn)
    ////{
    ////    setA.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 2);
    ////    setA.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 5);

    ////    setB.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    ////    setB.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);

    ////    setC.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    ////    setC.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);
    ////}


    /////// <summary>
    /////// A difference (C intersection B)
    /////// </summary>
    ////void ImplementLogic1()
    //{
    //    setB.material.renderQueue -= 2;
    //    setB.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setB.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    //    setB.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    //    setB.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);

    //    setC.material.renderQueue -= 1;
    //    setC.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setC.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 3);
    //    setC.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    //    setC.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);

    //    setA.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 0);
    //    setA.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 0);
    //    setA.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 2);
    //    setA.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 5);
    //}

    ///// union, inter - commutative, differ - non-commutative


    ///// <summary>
    ///// B union ( A / C )
    ///// </summary>
    //void ImplementLogic2()
    //{
    //    setA.material.renderQueue -= 2;
    //    setA.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setA.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);

    //    setC.material.renderQueue -= 1;
    //    setC.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setC.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 4);

    //    setB.material.SetInteger(Shader.PropertyToID("_Ref1Code"), 1);
    //    setB.material.SetInteger(Shader.PropertyToID("_Pass1Code"), 2);

    //    /// renderer conditions

    //    setA.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 1);
    //    setA.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 3);

    //    setB.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 1);
    //    setB.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 3);

    //    setC.material.SetInteger(Shader.PropertyToID("_Ref2Code"), 0);
    //    setC.material.SetInteger(Shader.PropertyToID("_Comp2Code"), 1);

    //}


    //public void PerfromLogic(SetKind x, params BinarySetOperation[] operations)
    //{

    //}
}
