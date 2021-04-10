using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stratus
{
	/// <summary>
	/// Modifies a <see cref="Material"/> at runtime
	/// </summary>
	public class StratusMaterialPropertyTriggerable : StratusTriggerableBehaviour
	{
		public enum PropertyType
		{
			SetColor,
			SetFloat,
			SetInteger,
			SetTexture,
			SetMaterial
		}

		public Material material;
		public PropertyType type;
		public string propertyName;
		[DrawIf(nameof(type), PropertyType.SetFloat, ComparisonType.Equals)] 
		public float floatValue;
		[DrawIf(nameof(type), PropertyType.SetInteger, ComparisonType.Equals)]
		public int integerValue;
		[DrawIf(nameof(type), PropertyType.SetColor, ComparisonType.Equals)]
		public Color color = Color.white;
		[DrawIf(nameof(type), PropertyType.SetTexture, ComparisonType.Equals)]
		public Texture texture;
		[DrawIf(nameof(type), PropertyType.SetMaterial, ComparisonType.Equals)]
		public Material material2;

		public float duration = 1.0f;

		private Material originalMaterial;
		public static Dictionary<string, Material> restoredMaterials = new Dictionary<string, Material>();

		protected override void OnAwake()
		{
			originalMaterial = new Material(material);
		}

		protected override void OnReset()
		{

		}

		private void OnDestroy()
		{
			// If the material has been previously restored, we don't want to do it again
			// This is so that we can ensure that we restore the material?
			bool hasBeenRestored = restoredMaterials.ContainsKey(material.name);
			if (hasBeenRestored)
			{
				return;
			}

			// Restore the original material
			material.CopyPropertiesFromMaterial(originalMaterial);
			restoredMaterials.Add(material.name, material);
		}

		protected override void OnTrigger(object data = null)
		{
			IEnumerator routine = null;
			switch (type)
			{
				case PropertyType.SetColor:
					routine = StratusRoutines.Lerp(material.color, color, duration, (Color val) => { material.color = val; }, Color.Lerp);
					break;
				case PropertyType.SetFloat:
					routine = StratusRoutines.Lerp(material.GetFloat(propertyName), floatValue, duration, (float val) => { material.SetFloat(propertyName, val); }, StratusRoutines.Lerp);
					break;
				case PropertyType.SetInteger:
					routine = StratusRoutines.Lerp(material.GetInt(propertyName), integerValue, duration, (float val) => { material.SetInt(propertyName, Mathf.CeilToInt(val)); }, StratusRoutines.Lerp);
					break;
				case PropertyType.SetTexture:
					routine = StratusRoutines.Call(() => { material.SetTexture(propertyName, texture); }, duration);
					break;
				case PropertyType.SetMaterial:
					routine = StratusRoutines.Lerp((float t) => { material.Lerp(material, material2, t); }, duration);
					break;
				default:
					break;
			}

			this.StartCoroutine(routine, nameof(StratusMaterialPropertyTriggerable));
		}
	}

}