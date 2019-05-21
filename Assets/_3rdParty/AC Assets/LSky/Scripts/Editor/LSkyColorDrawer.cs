
/////////////////////////////////////////////
/// Custom property drawer for LSkyColor. ///
/////////////////////////////////////////////


using UnityEngine;
using UnityEditor;


namespace AC.LSky
{

	[CustomPropertyDrawer(typeof(LSkyColor))]
	public class LSkyColorDrawer : PropertyDrawer
	{


		string displayName;
		//----------------------------------

		SerializedProperty colorType;
		SerializedProperty inputColor;
		SerializedProperty gradient;
		//----------------------------------

		bool isCached = false;
		//----------------------------------

		enum ColorType{C, G} 
		ColorType ct;
		//----------------------------------


		public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
		{
			
			if (!isCached) 
			{

				displayName = property.displayName; 
				property.Next (true);

				colorType = property.Copy ();
				property.NextVisible (true);

				inputColor = property.Copy ();
				property.NextVisible (true);

				gradient = property.Copy ();
				property.NextVisible (true);

				isCached = true;
			}
			//-----------------------------------------------------------------------------------

			ct =  (ColorType)colorType.enumValueIndex;
			//-----------------------------------------------------------------------------------

			rect.height = 20f; rect.width *= 0.90f; 
			EditorGUI.indentLevel = 0;
			//-----------------------------------------------------------------------------------


			if(colorType.enumValueIndex == 0) // Input color.
			{

				EditorGUI.BeginProperty(rect, label, inputColor);
				{

					EditorGUI.BeginChangeCheck();
					Color inputColorValue = EditorGUI.ColorField(rect, new GUIContent(displayName), inputColor.colorValue);
					if(EditorGUI.EndChangeCheck()) 
					{
						inputColor.colorValue = inputColorValue;
					}
				}
				EditorGUI.EndProperty();
			} 
			else // Input gradient.
			{

				EditorGUI.BeginProperty(rect, label, gradient);
				{
					EditorGUI.BeginChangeCheck();
					EditorGUI.PropertyField(rect, gradient, new GUIContent(displayName));
					EditorGUI.EndChangeCheck();
				}
				EditorGUI.EndProperty();
			}
			//-----------------------------------------------------------------------------------

	

			Rect switchRect     = rect; 
			switchRect.x       += rect.width; //buttonRect.y     += 2.5f; 
			switchRect.height   = 20; switchRect.width *= 0.1f;
			//-----------------------------------------------------------------------------------

			// Switch color type.
			EditorGUI.BeginProperty(rect, label, colorType);
			{

				EditorGUI.BeginChangeCheck();

				ct = (ColorType)EditorGUI.EnumPopup(switchRect, new GUIContent("", "Switch Color/Gradient"), ct, EditorStyles.miniLabel); 

				if (EditorGUI.EndChangeCheck()) 
				{
					colorType.enumValueIndex = (int)ct;
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