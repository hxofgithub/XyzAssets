using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace XyzAssets.Editor
{
    public class TypeSelectorAttribute : PropertyAttribute
    {
        public readonly Type SelectedType;

        public TypeSelectorAttribute(Type type)
        {
            SelectedType = type;
        }
    }
}