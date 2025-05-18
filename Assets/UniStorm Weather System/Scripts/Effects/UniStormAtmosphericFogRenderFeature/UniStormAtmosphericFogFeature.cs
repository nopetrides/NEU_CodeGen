﻿using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.Universal.Internal;

public class UniStormAtmosphericFogFeature : ScriptableRendererFeature
{
    public enum DitheringControl { Enabled, Disabled };

    [System.Serializable]
    public class Settings
    {
        public bool isEnabled = true;
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingSkybox;

        public Texture2D NoiseTexture;
        
        public DitheringControl Dither = DitheringControl.Enabled;    
        [System.NonSerialized]
        public Light SunSource;        
        [System.NonSerialized]
        public Light MoonSource;        
        public bool distanceFog = true;
        public bool useRadialDistance;        
        public bool heightFog;        
        public float height = 1.0f;        
        public float heightDensity = 2.0f;
        public float startDistance;
    
        public Color SunColor = new Color(1, 0.63529f, 0);        
        public Color MoonColor = new Color(1, 0.63529f, 0);        
        public Color TopColor;        
        public Color BottomColor;
        public float BlendHeight = 0.03f;        
        public float FogGradientHeight = 0.5f;
        public float SunIntensity = 2;
        public float MoonIntensity = 1;
        public float SunFalloffIntensity = 9.4f;
        public float SunControl = 1;
        public float MoonControl = 1;

    }

    // UniStormAtmosphericFog Pass
    public Settings settings = new Settings();
    UniStormAtmosphericFogPass fogPass;

    // CopyDepth Pass
    Shader copyDepthShader;
    Material copyDepthPassMaterial;
    CopyDepthPass copyDepthPass;

    public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
    {
        fogPass.Setup(renderer.cameraColorTargetHandle);  //Use of target after allocation
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!settings.isEnabled)
            return;

        //UniStormAtmosphericFog Pass
        fogPass.settings = settings;
        renderer.EnqueuePass(fogPass);
    }

    public override void Create()
    {
        //UniStormAtmosphericFog Pass
        fogPass = new UniStormAtmosphericFogPass(name);

        //CopyDepth Pass
        if (Application.isEditor && !Application.isPlaying) return;
        copyDepthShader = Shader.Find("Hidden/Universal Render Pipeline/CopyDepth");
        copyDepthPassMaterial = new Material(copyDepthShader);
        copyDepthPass = new CopyDepthPass(settings.renderPassEvent, copyDepthShader);
    }
}
