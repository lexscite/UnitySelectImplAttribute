using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PaperStag
{
[CustomPropertyDrawer(typeof(SelectImplReorderableAttribute))]
public class SelectImplReorderableDrawer : PropertyDrawer
{
    private class Data
    {
        public SerializedProperty Property;
        public List<Type> Impls;
        public int Index;
    }

    private readonly Dictionary<string, Data> _dataDict
        = new Dictionary<string, Data>();

    public override void OnGUI(Rect position,
        SerializedProperty property,
        GUIContent label)
    {
        if (!_dataDict.TryGetValue(property.propertyPath, out Data data))
        {
            data = new Data();
            _dataDict[property.propertyPath] = data;
            data.Property = property;

            RefreshImpls(data);

            if (data.Property.managedReferenceFullTypename
                != string.Empty)
            {
                var names =
                    property.managedReferenceFullTypename.Split(' ');
                var fullType = Type.GetType($"{names[1]}, {names[0]}");

                var refType = data.Impls.FirstOrDefault(
                    impl => impl == fullType);

                var index = data.Impls.IndexOf(refType);
                data.Index = index;
            }
            else
            {
                Choose(data, property);
            }
        }

        var y = position.y
            + position.height;
        var height = EditorGUIUtility.singleLineHeight;

        EditorGUI.BeginChangeCheck();
        data.Index = EditorGUI.Popup(new Rect(new Vector2(position.x, y),
                new Vector2(position.width, height)),
            data.Index,
            data.Impls.Select(impl => impl.Name).ToArray());
        var popupValueChanged = EditorGUI.EndChangeCheck();

        if (popupValueChanged)
        {
            Choose(data, property);
        }

        position.y += EditorGUIUtility.singleLineHeight;

        EditorGUI.PropertyField(position, property, true);
    }

    private static IEnumerable<Type> GetImplementations(
        Type interfaceType)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes());
        return types.Where(p => interfaceType.IsAssignableFrom(p)
                && !p.IsAbstract
                && !p.IsInterface)
            .ToList();
    }

    private void Choose(Data data, SerializedProperty property)
    {
        property.managedReferenceValue =
            Activator.CreateInstance(data.Impls[data.Index]);
    }

    private void RefreshImpls(Data data)
    {
        data.Impls = GetImplementations(
                (attribute as SelectImplReorderableAttribute)?.fieldType)
            .Where(impl =>
                !impl.IsSubclassOf(typeof(UnityEngine.Object)))
            .ToList();
    }

    public override float GetPropertyHeight(SerializedProperty property,
        GUIContent label)
    {
        var result = EditorGUI.GetPropertyHeight(property, label, true);
        result += EditorGUIUtility.singleLineHeight;
        result += EditorGUIUtility.standardVerticalSpacing;
        return result;
    }
}
}