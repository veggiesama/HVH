

using UnityEngine;
using UnityEditor;

namespace AC.LSky
{

	[CustomPropertyDrawer(typeof(LSkyFloatAttribute))]
	public class LSkyCustomFloatDrawer : PropertyDrawer
	{


		string displayName;
		//----------------------------------

		SerializedProperty valueType;
		SerializedProperty inputValue;
		SerializedProperty curve;
		//----------------------------------

		bool isCached = false;
		//----------------------------------

		enum ValueType{V, C} ValueType vt;
		//----------------------------------


		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{

			if (!isCached) 
			{

				displayName = property.displayName;
				property.Next (true);

				valueType = property.Copy ();
				property.NextVisible (true);

				inputValue = property.Copy ();
				property.NextVisible (true);

				curve = property.Copy ();
				property.NextVisible (true);

				isCached = true;
			}
			//------------------------------------------------------------------------

			vt = (ValueType)valueType.enumValueIndex;
			//------------------------------------------------------------------------

			rect.height = 20f; rect.width *= 0.90f; 
			EditorGUI.indentLevel = 0;
			//------------------------------------------------------------------------

			LSkyFloatAttribute attr = attribute as LSkyFloatAttribute;
			//------------------------------------------------------------------------

			if(valueType.enumValueIndex == 0) // Input value.
			{

				EditorGUI.BeginProperty(rect, label, inputValue);
				{

					EditorGUI.BeginChangeCheck();
					float inVal = EditorGUI.Slider(rect, new GUIContent(displayName), inputValue.floatValue, attr.minValue, attr.maxValue);

					if(EditorGUI.EndChangeCheck())
					{
						inputValue.floatValue = inVal;
					}
				}
				EditorGUI.EndProperty();
			} 
			else // Input curve.
			{

				EditorGUI.BeginProperty(rect, label, curve);
				{

					EditorGUI.BeginChangeCheck();
					Color curveColor = attr.GetCurveColor();

					AnimationCurve c = EditorGUI.CurveField(rect, new GUIContent(displayName), curve.animationCurveValue, curveColor, new Rect(attr.timeStart, attr.valueStart, attr.timeEnd, attr.valueEnd));

					if (EditorGUI.EndChangeCheck()) 
					{
						curve.animationCurveValue = c;
					}

				}
				EditorGUI.EndProperty();
			}
			//------------------------------------------------------------------------

			Rect switchRect     = rect; 
			switchRect.x       += rect.width; //buttonRect.y     += 2.5f; 
			switchRect.height   = 20;  switchRect.width *= 0.1f;
			//------------------------------------------------------------------------

			// Switch color.
			EditorGUI.BeginProperty(rect, label, valueType);
			{

				EditorGUI.BeginChangeCheck();
				vt = (ValueType)EditorGUI.EnumPopup(switchRect, new GUIContent(""), vt, EditorStyles.miniLabel); 

				if (EditorGUI.EndChangeCheck()) 
				{
					valueType.enumValueIndex = (int)vt;
				}
			}
			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label) 
		{
			return base.GetPropertyHeight(property, label) + 5;
		}
	}
}