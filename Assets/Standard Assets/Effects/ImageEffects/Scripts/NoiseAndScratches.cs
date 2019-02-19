using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UnityStandardAssets.ImageEffects
{
    [ExecuteInEditMode]
    [RequireComponent (typeof(Camera))]
    [AddComponentMenu("Image Effects/Noise/Noise and Scratches")]
    public class NoiseAndScratches : MonoBehaviour
    {
        /// Monochrome noise just adds grain. Non-monochrome noise
        /// more resembles VCR as it adds noise in YUV color space,
        /// thus introducing magenta/green colors.
        public bool monochrome = true;
        private bool rgbFallback = false;

        // Noise grain takes random intensity from Min to Max.
        [Range(0.0f,5.0f)]
        public float grainIntensityMin = 0.1f;
        [Range(0.0f, 5.0f)]
        public float grainIntensityMax = 0.2f;

        /// The size of the noise grains (1 = one pixel).
        [Range(0.1f, 50.0f)]
        public float grainSize = 2.0f;

        public Texture grainTexture;
        public Shader   shaderRGB;
        public Shader   shaderYUV;
        private Material m_MaterialRGB;
        private Material m_MaterialYUV;

        protected void Start ()
        {
            // Disable if we don't support image effects
            if (!SystemInfo.supportsImageEffects) {
                enabled = false;
                return;
            }

            if ( shaderRGB == null || shaderYUV == null )
            {
                Debug.Log( "Noise shaders are not set up! Disabling noise effect." );
                enabled = false;
            }
            else
            {
                if ( !shaderRGB.isSupported ) // disable effect if RGB shader is not supported
                    enabled = false;
                else if ( !shaderYUV.isSupported ) // fallback to RGB if YUV is not supported
                    rgbFallback = true;
            }
        }

        protected Material material {
            get {
                if ( m_MaterialRGB == null ) {
                    m_MaterialRGB = new Material( shaderRGB );
                    m_MaterialRGB.hideFlags = HideFlags.HideAndDontSave;
                }
                if ( m_MaterialYUV == null && !rgbFallback ) {
                    m_MaterialYUV = new Material( shaderYUV );
                    m_MaterialYUV.hideFlags = HideFlags.HideAndDontSave;
                }
                return (!rgbFallback && !monochrome) ? m_MaterialYUV : m_MaterialRGB;
            }
        }

        protected void OnDisable() {
            if ( m_MaterialRGB )
                DestroyImmediate( m_MaterialRGB );
            if ( m_MaterialYUV )
                DestroyImmediate( m_MaterialYUV );
        }

        private void SanitizeParameters()
        {
            grainIntensityMin = Mathf.Clamp( grainIntensityMin, 0.0f, 5.0f );
            grainIntensityMax = Mathf.Clamp( grainIntensityMax, 0.0f, 5.0f );
            grainSize = Mathf.Clamp( grainSize, 0.1f, 50.0f );
        }

        // Called by the camera to apply the image effect
        void OnRenderImage (RenderTexture source, RenderTexture destination)
        {
            SanitizeParameters();

            Material mat = material;

            mat.SetTexture("_GrainTex", grainTexture);
            float grainScale = 1.0f / grainSize; // we have sanitized it earlier, won't be zero
            mat.SetVector("_GrainOffsetScale", new Vector4(
                                                   Random.value,
                                                   Random.value,
                                                   (float)Screen.width / (float)grainTexture.width * grainScale,
                                                   (float)Screen.height / (float)grainTexture.height * grainScale
                                                   ));
			mat.SetVector ("_Intensity", new Vector4 (Random.Range (grainIntensityMin, grainIntensityMax), 0, 0, 0));
            Graphics.Blit (source, destination, mat);
        }
    }
}
