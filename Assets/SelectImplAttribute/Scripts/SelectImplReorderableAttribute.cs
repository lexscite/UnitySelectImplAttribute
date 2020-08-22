using System;
using UnityEngine;

namespace PaperStag
{
    public class SelectImplReorderableAttribute : PropertyAttribute
    {
        public Type fieldType;

        public SelectImplReorderableAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}