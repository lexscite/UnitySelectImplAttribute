using System;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace PaperStag
{
    [CustomPropertyDrawer(typeof(SelectImplAttribute))]
    public class SelectImplDrawer : PropertyDrawer
    {
        private class Data
        {
            public SerializedProperty property;
            public List<Type> impls;
            public int index;
        }

        private readonly Dictionary<string, Data> _datas
            = new Dictionary<string, Data>();

        public override void OnGUI(Rect position,
            SerializedProperty property,
            GUIContent label)
        {
            if (!_datas.TryGetValue(property.propertyPath, out Data data))
            {
                data = new Data();
                _datas[property.propertyPath] = data;
                data.property = property;

                RefreshImpls(data);

                if (property.managedReferenceFullTypename != string.Empty)
                {
                    var names = property.managedReferenceFullTypename.Split(' ');
                    var fullType = Type.GetType($"{names[1]}, {names[0]}");

                    var refType = data.impls.FirstOrDefault(
                        impl => impl == fullType);

                    var index = data.impls.IndexOf(refType);
                    data.index = index;
                }
                else
                {
                    Choose(data, property);
                }
            }

            var y = position.y
                + position.height
                - EditorGUIUtility.singleLineHeight;
            var height = EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            data.index = EditorGUI.Popup(new Rect(
                new Vector2(position.x, y),
                new Vector2(position.width, height)),
                data.index,
                data.impls.Select(impl => impl.Name).ToArray());
            var popupValueChanged = EditorGUI.EndChangeCheck();

            if (popupValueChanged)
            {
                Choose(data, property);
            }

            EditorGUI.PropertyField(position, property, true);
        }

        public static List<Type> GetImplementations(Type interfaceType)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes());
            return types.Where(p =>interfaceType.IsAssignableFrom(p)
            && !p.IsAbstract && !p.IsInterface)
                .ToList();
        }

        private void Choose(Data data, SerializedProperty property)
        {
            property.managedReferenceValue = Activator.CreateInstance(
                data.impls[data.index]);
        }

        private void RefreshImpls(Data data)
        {
            data.impls = GetImplementations(
                (attribute as SelectImplAttribute).fieldType)
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