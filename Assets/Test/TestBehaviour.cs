using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using PaperStag;
using UnityEngine;

public class TestBehaviour : MonoBehaviour
{
    [SerializeReference] [SelectImpl(typeof(TestBase))]
    protected TestBase _test1;

    [SerializeReference]
    [SelectImpl(typeof(TestBase))]
    [ReorderableList]
    protected List<TestBase> _test2;

    [SerializeReference] [SelectImpl(typeof(TestBase))]
    protected TestBase _test3;
}