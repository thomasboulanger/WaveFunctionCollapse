using NaughtyAttributes;
using NaughtyAttributes.Editor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SocketAttribute))]
public class SocketAttributeDrawer : PropertyDrawerBase
{

    protected override float GetPropertyHeight_Internal (SerializedProperty property, GUIContent label)
    {
        return (property.propertyType == SerializedPropertyType.String)
            ? GetPropertyHeight(property)
            : GetPropertyHeight(property) + GetHelpBoxHeight();
    }


    protected override void OnGUI_Internal (Rect rect, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(rect, label, property)) {

            if (property.propertyType == SerializedPropertyType.String) {

                SocketAttribute socketAttribute = (SocketAttribute)attribute;
                SocketAttribute.ESocketMode mode = socketAttribute.mode;

                // Only show sockets that are valid & in the same mode (vertical/horizontal)
                List<string> socketList = SocketLUT.Editor_GetInstance().Sockets.ToList()
                    .Where(s =>
                    SocketUtility.IsValid(s) &&
                    (mode == SocketAttribute.ESocketMode.Vertical|| !SocketUtility.IsDirectional(s)))
                    .ToList();

                string propertyString = property.stringValue;
                
                // if value was previousely outdated,
                // remove the tag and check if it is available once more
                if (propertyString.EndsWith(" OUTDATED")) {
                    propertyString = propertyString.Remove(propertyString.Length - " OUTDATED".Length);
                }

                int index = -1;
                // check if there is an entry that matches the entry and get the index
                for (int i = 0; i < socketList.Count; i++) {
                    if (socketList[i].Equals(propertyString, System.StringComparison.Ordinal)) {
                        index = i;
                        break;
                    }
                }
                // If the socket corresponding index wasn't found
                // keeps the oldest value and add '*' at the end to highlight the issue
                if (index == -1) {
                    if (propertyString == "") {
                        index = 0;
                    }
                    else {
                        if (!propertyString.EndsWith(" OUTDATED"))
                            propertyString+= " OUTDATED";

                        socketList.Add(propertyString);
                        index = socketList.Count-1;
                    }
                }


                socketList = socketList.Select(s => SocketUtility.IsWildCard(s) ? "?" : s).ToList();

                // Draw the popup box with the current selected index
                int newIndex = EditorGUI.Popup(rect, label.text, index, socketList.ToArray());

                socketList = socketList.Select(s => s == "?" ? SocketUtility.WILDCARD : s).ToList();

                // Adjust the actual string value of the property based on the selection
                string newValue = (newIndex < socketList.Count) ? socketList[newIndex] : propertyString;

                if (!property.stringValue.Equals(newValue, System.StringComparison.Ordinal)) {
                    property.stringValue = newValue;
                }

            } else {

                string message = $"{typeof(SocketAttribute).Name} supports only string fields";
                DrawDefaultPropertyAndHelpBox(rect, property, message, MessageType.Warning);
            }

        }
    }
}