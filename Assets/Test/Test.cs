using System;
using UnityEngine;

[Serializable]
public abstract class TestBase
{
    [SerializeField] protected int _test;
}

[Serializable]
public class Test1 : TestBase
{
    [SerializeField] protected int _test1;
}

[Serializable]
public class Test2 : TestBase
{
    [SerializeField] protected int _test2;
}