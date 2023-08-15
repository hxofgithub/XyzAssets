using System;
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