using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Nobi.UiRoundedCorners
{
	[ExecuteInEditMode]                             //Required to check the OnEnable function
	[DisallowMultipleComponent]                     //You can only have one of these in every object.
	[RequireComponent(typeof(RectTransform))]
	public class ImageWithRoundedCorners_resizable : MonoBehaviour
	{


		public float rectangle1CornerRadius = 40f;
		public RectTransform rectangle1;
		public float rectangle2CornerRadius = 40f;
		public RectTransform rectangle2;
		[Range(0f, 1f)]
		public float progress = 0f;
		private Material material;
		private Vector4 outerUV = new Vector4(0, 0, 1, 1);

		[HideInInspector, SerializeField] private MaskableGraphic image;

		void Update()
		{
			updateDimensions();

		}
		private void OnValidate()
		{
			Validate();
			Refresh();
		}

		private void OnDestroy()
		{
			if (image != null)
			{
				image.material = null;      //This makes so that when the component is removed, the UI material returns to null
			}

			DestroyHelper.Destroy(material);
			image = null;
			material = null;
		}

		private void OnEnable()
		{
			//You can only add either ImageWithRoundedCorners or ImageWithIndependentRoundedCorners
			//It will replace the other component when added into the object.
			// var other = GetComponent<ImageWithIndependentRoundedCorners>();
			// if (other != null)
			// {
			// 	radius = other.r.x;                 //When it does, transfer the radius value to this script
			// 	DestroyHelper.Destroy(other);
			// }

			Validate();
			Refresh();
		}

		private void OnRectTransformDimensionsChange()
		{
			if (enabled && material != null)
			{
				Refresh();
			}
		}

		public void Validate()
		{
			if (material == null)
			{
				material = new Material(Shader.Find("UI/RoundedCorners/RoundedCorners_resizable"));

			}

			if (image == null)
			{
				TryGetComponent(out image);
			}

			if (image != null)
			{
				image.material = material;

			}

			if (image is Image uiImage && uiImage.sprite != null)
			{
				outerUV = UnityEngine.Sprites.DataUtility.GetOuterUV(uiImage.sprite);
			}
		}

		float LERP(float a, float b, float t)
		{
			return a + (b - a) * t;
		}
		Vector3 LERP(Vector3 a, Vector3 b, float t)
		{
			return a + (b - a) * t;
		}

		public void Refresh()
		{
			int Props1 = Shader.PropertyToID("_WidthHeightRadius");
			int prop_OuterUV = Shader.PropertyToID("_OuterUV");
			var rect = ((RectTransform)transform).rect;
			var radius = LERP(rectangle1CornerRadius, rectangle2CornerRadius, progress);
			//Multiply radius value by 2 to make the radius value appear consistent with ImageWithIndependentRoundedCorners script.
			//Right now, the ImageWithIndependentRoundedCorners appears to have double the radius than this.
			var mater = image.materialForRendering;
			mater.SetVector(Props1, new Vector4(rect.width, rect.height, radius * 2, 0));
			mater.SetVector(prop_OuterUV, outerUV);
		}


		private void updateDimensions()
		{
			var w = LERP(rectangle1.rect.width, rectangle2.rect.width, progress);
			var h = LERP(rectangle1.rect.height, rectangle2.rect.height, progress);	
			// resize
			var myTransform = transform.GetComponent<RectTransform>();
			myTransform.sizeDelta = new Vector2(w, h);
			// center
			var center = LERP(rectangle1.position, rectangle2.position, progress);
			var deltaCenter = center - transform.position;
			// move children the opposite direction of the parent
			foreach (Transform child in transform)
			{
				var tranylvania = child.GetComponent<RectTransform>();
				tranylvania.position = tranylvania.position - deltaCenter;
			}
			// move the parent
			myTransform.position = myTransform.position + deltaCenter;
		}
	}
}