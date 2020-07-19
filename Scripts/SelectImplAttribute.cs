using System;
using UnityEngine;

namespace PaperStag
{
    public class SelectImplAttribute : PropertyAttribute
    {
        public Type fieldType;

        public SelectImplAttribute(Type fieldType)
        {
            this.fieldType = fieldType;
        }
    }
}