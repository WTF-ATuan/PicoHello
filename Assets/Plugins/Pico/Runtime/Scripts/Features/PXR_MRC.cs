﻿/************************************************************************************
 【PXR SDK】
 Copyright 2015-2020 Pico Technology Co., Ltd. All Rights Reserved.

************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.IO;
using UnityEngine.UI;
using System;
using UnityEngine.XR;
using Unity.XR.PXR;
using Unity.XR;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace Unity.XR.PXR
{
    public struct CameraData
    {
        public string id;
        public string cameraName;
        public float imageWidth;
        public float imageHeight;
        public float[] matrix33;
        public float[] matrix81;
        public float[] translation;
        public float[] rotation;
        public float attachedDevice;
        public float camDelayMs;
        public float[] color;
        public float chromaKeySimilarity;
        public float chromaKeySmoothRange;
        public float chromaKeySpillRange;
        public float[] rawTranslation;
        public float[] rawRotation;
    }


    public class PXR_MRC : MonoBehaviour
    {
        public static PXR_MRC pxrmrc;

        public const int CAM_DEPTH = 9999;
        public const int CAM_LAYER = 0;

        public bool onlyBack = false;
        public LayerMask foregroundLayerMask = -1;
        public LayerMask backLayerMask = -1;
        public Color foregroundColor = new Color(0, 1, 0, 1);
        [HideInInspector]
        public GameObject backCameraObj = null;
        [HideInInspector]
        public GameObject foregroundCameraObj = null;
        [HideInInspector]
        public RenderTexture mrcRenderTexture = null;
        [HideInInspector]
        public RenderTexture foregroundMrcRenderTexture = null;

        private UInt64 mrcRtNumber;
        private UInt64 mrcRtNumberForeground;
        private bool mrcPlay = false;
        private CameraData xmlCameraData;
        private bool replacement = false;

        private float[] cameraAttribute;
        private float yFov = 53f;

        private float height;

        private Vector3 locationDeflection;
        private Vector3 angularDeflection ;

        private PxrLayerParam layerParam = new PxrLayerParam();
        private UInt32 imageCounts = 0;
        private IntPtr foregroundTexturePtr;
        private IntPtr backTexturePtr;
        private Texture[] swapChain = new Texture[2];
        private bool createMRCOverlaySucceed = false;
        private int imageIndex;

        private int num = 0;
        private bool init = false;

        private void Awake()
        {
            if (pxrmrc == null)
            {
                pxrmrc = this;
            }
            else {
                Destroy(gameObject);
            }

            DontDestroyOnLoad(gameObject);
        }

        public void Pxr_MRCPoseInitialize() {
            locationDeflection = Vector3.zero;
            angularDeflection = Vector3.zero;
            xmlCameraData = new CameraData();
            UPxr_ReadXML(out xmlCameraData);
            Invoke("Pxr_GetHeight", 0.5f);

            PxrPosef pose = new PxrPosef();
            pose.orientation.x = xmlCameraData.rotation[0];
            pose.orientation.y = xmlCameraData.rotation[1];
            pose.orientation.z = xmlCameraData.rotation[2];
            pose.orientation.w = xmlCameraData.rotation[3];
            pose.position.x = xmlCameraData.translation[0];
            pose.position.y = xmlCameraData.translation[1];
            pose.position.z = xmlCameraData.translation[2];
            PXR_Plugin.System.UPxr_SetMrcPose(ref pose);

            PXR_Plugin.System.UPxr_GetMrcPose(ref pose);
            xmlCameraData.rotation[0] = pose.orientation.x;
            xmlCameraData.rotation[1] = pose.orientation.y;
            xmlCameraData.rotation[2] = pose.orientation.z;
            xmlCameraData.rotation[3] = pose.orientation.w;
            xmlCameraData.translation[0] = pose.position.x;
            xmlCameraData.translation[1] = pose.position.y;
            xmlCameraData.translation[2] = pose.position.z;
            mrcPlay = false;
            UInt64 textureWidth = (UInt64)xmlCameraData.imageWidth;
            UInt64 textureHeight = (UInt64)xmlCameraData.imageHeight;
            PXR_Plugin.System.UPxr_SetMrcTextutrWidth(textureWidth);
            PXR_Plugin.System.UPxr_SetMrcTextutrHeight(textureHeight);
            UPxr_CreateMRCOverlay((uint)xmlCameraData.imageWidth, (uint)xmlCameraData.imageHeight);
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginCameraRendering += BeginRendering;
            }
            else
            {
                Camera.onPostRender += UPxr_CopyMRCTexture;
            }
            Debug.Log("PXR_MRCInit Succeed");
        }

        public void Pxr_GetHeight() {
            height = Camera.main.transform.localPosition.y - PXR_Plugin.System.UPxr_GetMrcY();
            Debug.Log("Pxr_GetMrcY+:" + PXR_Plugin.System.UPxr_GetMrcY().ToString());
        }

        private void OnDestroy()
        {
            Debug.Log("Pxr_DestroyMRC");
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                RenderPipelineManager.beginCameraRendering -= BeginRendering;
            }
            else
            {
                Camera.onPostRender -= UPxr_CopyMRCTexture;
            }
            PXR_Plugin.Render.UPxr_DestroyLayer(9999);
        }

        void OnApplicationPause(bool pause) {
            if (pause)
            {

            }
            else {
                if (PXR_Plugin.System.UPxr_GetMRCEnable())
                {
                    UPxr_Calibration();
                }
            }
        }

        public void UPxr_ReadXML(out CameraData cameradata) {
            CameraData cameraDataNew = new CameraData();
            string path = Application.persistentDataPath + "/mrc.xml";
            cameraAttribute = PXR_Plugin.PlatformSetting.UPxr_MRCCalibration(path);
            Debug.Log("cameraAttributeLength: " + cameraAttribute.Length);
            for (int i = 0; i < cameraAttribute.Length; i++)
            {
                Debug.Log("cameraAttribute: " + i.ToString() + cameraAttribute[i].ToString());
            }
            cameraDataNew.imageWidth = cameraAttribute[0];
            cameraDataNew.imageHeight = cameraAttribute[1];
            yFov = cameraAttribute[2];
            cameraDataNew.translation = new float[3];
            cameraDataNew.rotation = new float[4];
            for (int i = 0; i < 3; i++) {
                cameraDataNew.translation[i] = cameraAttribute[3+i];
            }
            for (int i = 0; i < 4; i++)
            {
                cameraDataNew.rotation[i] = cameraAttribute[6 + i];
            }
            cameradata = cameraDataNew;
        }

        public void UPxr_CreateCamera(CameraData cameradata)
        {
            if (backCameraObj == null)
            {
                backCameraObj = new GameObject("myBackCamera");
                backCameraObj.transform.parent = Camera.main.transform.parent;
                Camera camera = backCameraObj.AddComponent<Camera>();
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                camera.transform.localScale = Vector3.one;
                camera.depth = CAM_DEPTH;
                camera.gameObject.layer = CAM_LAYER;
                camera.clearFlags = CameraClearFlags.Skybox;
                camera.orthographic = false;
                camera.fieldOfView = UPxr_GetYFOV();
                camera.aspect = cameradata.imageWidth / cameradata.imageHeight;
                camera.transform.localPosition = UPxr_ToVector3(cameradata.translation);
                camera.transform.localEulerAngles = UPxr_ToRotation(cameradata.rotation);
                camera.allowMSAA = true;
                camera.cullingMask = backLayerMask;
                if (mrcRenderTexture == null)
                {
                    mrcRenderTexture = new RenderTexture((int)cameradata.imageWidth, (int)cameradata.imageHeight, 24, RenderTextureFormat.ARGB32);
                }
                mrcRenderTexture.name = "mrcRenderTexture";
                camera.targetTexture = mrcRenderTexture;
            }
            else
            {
                backCameraObj.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
                backCameraObj.GetComponent<Camera>().transform.localPosition = UPxr_ToVector3(cameradata.translation);
                backCameraObj.GetComponent<Camera>().transform.localEulerAngles = UPxr_ToRotation(cameradata.rotation);
                backCameraObj.GetComponent<Camera>().transform.localScale = Vector3.one;
                backCameraObj.GetComponent<Camera>().depth = CAM_DEPTH;
                backCameraObj.GetComponent<Camera>().gameObject.layer = CAM_LAYER;
                backCameraObj.GetComponent<Camera>().clearFlags = CameraClearFlags.Skybox;
                backCameraObj.GetComponent<Camera>().orthographic = false;
                backCameraObj.GetComponent<Camera>().fieldOfView = UPxr_GetYFOV();
                backCameraObj.GetComponent<Camera>().aspect = cameradata.imageWidth / cameradata.imageHeight;
                backCameraObj.GetComponent<Camera>().allowMSAA = true;
                backCameraObj.GetComponent<Camera>().cullingMask = backLayerMask;
                if (mrcRenderTexture == null)
                {
                    mrcRenderTexture = new RenderTexture((int)cameradata.imageWidth, (int)cameradata.imageHeight, 24, RenderTextureFormat.ARGB32);
                }
                backCameraObj.GetComponent<Camera>().targetTexture = mrcRenderTexture;
                backCameraObj.SetActive(true);
            }
            if (foregroundCameraObj == null)
            {
                foregroundCameraObj = new GameObject("myForegroundCamera");
                foregroundCameraObj.transform.parent = Camera.main.transform.parent;
                Camera camera = foregroundCameraObj.AddComponent<Camera>();
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = foregroundColor;
                camera.stereoTargetEye = StereoTargetEyeMask.None;
                camera.transform.localScale = Vector3.one;
                camera.depth = CAM_DEPTH + 1;
                camera.gameObject.layer = CAM_LAYER;
                camera.orthographic = false;
                camera.fieldOfView = UPxr_GetYFOV();
                camera.aspect = cameradata.imageWidth / cameradata.imageHeight;
                camera.transform.localPosition = UPxr_ToVector3(cameradata.translation);
                camera.transform.localEulerAngles = UPxr_ToRotation(cameradata.rotation);
                camera.allowMSAA = true;
                if (onlyBack == true)
                {
                    camera.cullingMask = 0;
                }
                else
                {
                    camera.cullingMask = foregroundLayerMask;
                }
                if (foregroundMrcRenderTexture == null)
                {
                    foregroundMrcRenderTexture = new RenderTexture((int)cameradata.imageWidth, (int)cameradata.imageHeight, 24, RenderTextureFormat.ARGB32);
                }
                mrcRenderTexture.name = "foregroundMrcRenderTexture";
                camera.targetTexture = foregroundMrcRenderTexture;
            }
            else
            {
                foregroundCameraObj.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                foregroundCameraObj.GetComponent<Camera>().backgroundColor = foregroundColor;
                foregroundCameraObj.GetComponent<Camera>().stereoTargetEye = StereoTargetEyeMask.None;
                foregroundCameraObj.GetComponent<Camera>().transform.localPosition = UPxr_ToVector3(cameradata.translation);
                foregroundCameraObj.GetComponent<Camera>().transform.localEulerAngles = UPxr_ToRotation(cameradata.rotation);
                foregroundCameraObj.GetComponent<Camera>().transform.localScale = Vector3.one;
                foregroundCameraObj.GetComponent<Camera>().depth = CAM_DEPTH + 1;
                foregroundCameraObj.GetComponent<Camera>().gameObject.layer = CAM_LAYER;
                foregroundCameraObj.GetComponent<Camera>().orthographic = false;
                foregroundCameraObj.GetComponent<Camera>().fieldOfView = UPxr_GetYFOV();
                foregroundCameraObj.GetComponent<Camera>().aspect = cameradata.imageWidth / cameradata.imageHeight;
                foregroundCameraObj.GetComponent<Camera>().allowMSAA = true;
                if (onlyBack == true)
                {
                    foregroundCameraObj.GetComponent<Camera>().cullingMask = 0;
                }
                else
                {
                    foregroundCameraObj.GetComponent<Camera>().cullingMask = foregroundLayerMask;
                }
                if (foregroundMrcRenderTexture == null)
                {
                    foregroundMrcRenderTexture = new RenderTexture((int)cameradata.imageWidth, (int)cameradata.imageHeight, 24, RenderTextureFormat.ARGB32);
                }
                foregroundCameraObj.GetComponent<Camera>().targetTexture = foregroundMrcRenderTexture;
                foregroundCameraObj.SetActive(true);
            }
        }

        public void UPxr_CreateMRCOverlay(uint width,uint height) {
            layerParam.layerId = 9999;
            layerParam.layerShape = PXR_OverLay.OverlayShape.Quad;
            layerParam.layerType = PXR_OverLay.OverlayType.Overlay;
            layerParam.layerLayout = PXR_OverLay.LayerLayout.Stereo;
            layerParam.format = (UInt64)RenderTextureFormat.Default;
            layerParam.width = width;
            layerParam.height = height;
            layerParam.sampleCount = 1;
            layerParam.faceCount = 1;
            layerParam.arraySize = 1;
            layerParam.mipmapCount = 0;
            layerParam.layerFlags = (UInt32)PxrLayerCreateFlags.PxrLayerFlagStaticImage;
            IntPtr layerParamPtr = Marshal.AllocHGlobal(Marshal.SizeOf(layerParam));
            Marshal.StructureToPtr(layerParam, layerParamPtr, false);
            PXR_Plugin.Render.UPxr_CreateLayerParam(layerParam);
            Marshal.FreeHGlobal(layerParamPtr);
        }

        public void UPxr_GetLayerImage() {
            if (createMRCOverlaySucceed == false)
            {
                if (PXR_Plugin.Render.UPxr_GetLayerImageCount(9999, EyeType.EyeLeft, ref imageCounts) == 0 && imageCounts > 0)
                {
                    PXR_Plugin.Render.UPxr_GetLayerImagePtr(9999, EyeType.EyeLeft, 0, ref foregroundTexturePtr);
                    IntPtr ptr = (IntPtr)foregroundTexturePtr;
                    Texture sc = Texture2D.CreateExternalTexture((int)layerParam.width, (int)layerParam.height, TextureFormat.RGBA32, false, true, ptr);
                    swapChain[0] = sc;
                }
                if (PXR_Plugin.Render.UPxr_GetLayerImageCount(9999, EyeType.EyeRight, ref imageCounts) == 0 && imageCounts > 0)
                {
                    PXR_Plugin.Render.UPxr_GetLayerImagePtr(9999, EyeType.EyeRight, 0, ref backTexturePtr);
                    IntPtr ptr = (IntPtr)backTexturePtr;
                    Texture sc = Texture2D.CreateExternalTexture((int)layerParam.width, (int)layerParam.height, TextureFormat.RGBA32, false, true, ptr);
                    swapChain[1] = sc;
                    createMRCOverlaySucceed = true;
                    Debug.Log("Pxr_GetMrcLayerImage : true");
                }
            }
        }

        private void BeginRendering(ScriptableRenderContext arg1, Camera arg2)
        {
            UPxr_CopyMRCTexture(arg2);
        }

        public void UPxr_CopyMRCTexture(Camera cam) {
            if (cam.tag != Camera.main.tag || cam.stereoActiveEye == Camera.MonoOrStereoscopicEye.Right) return;
            if (createMRCOverlaySucceed)
            {
                PXR_Plugin.Render.UPxr_GetLayerNextImageIndex(9999, ref imageIndex);
                Graphics.CopyTexture(foregroundMrcRenderTexture, 0, 0, swapChain[1], 0, 0);
                Graphics.CopyTexture(mrcRenderTexture, 0, 0, swapChain[0], 0, 0);
                PxrLayerQuad layerSubmit = new PxrLayerQuad();
                layerSubmit.header.layerId = 9999;
                layerSubmit.header.layerFlags = (UInt32)PxrLayerSubmitFlagsEXT.PxrLayerFlagMRCComposition;
                layerSubmit.width = 1.0f;
                layerSubmit.height = 1.0f;
                layerSubmit.header.colorScaleX = 1.0f;
                layerSubmit.header.colorScaleY = 1.0f;
                layerSubmit.header.colorScaleZ = 1.0f;
                layerSubmit.header.colorScaleW = 1.0f;
                layerSubmit.header.headPose.orientation.x = 0;
                layerSubmit.header.headPose.orientation.y = 0;
                layerSubmit.header.headPose.orientation.z = 0;
                layerSubmit.header.headPose.orientation.w = 1;
                PXR_Plugin.Render.UPxr_SubmitLayerQuad(layerSubmit);
            }
        }

        private void Update()
        {
            if (num < 3)
            {
                num++;

            }
            else if (init == false)
            {
                init = true;
                Pxr_MRCPoseInitialize();
            }
            if (PXR_Plugin.System.UPxr_GetMRCEnable() && num >= 3)
            {
                if (!mrcPlay)
                {
                    mrcPlay = true;
                    UPxr_CreateCamera(xmlCameraData);
                    //mrcRtNumber = (UInt64)mrcRenderTexture.GetNativeTexturePtr().ToInt64();
                    //PXR_Plugin.System.UPxr_SetMRCTextureID(mrcRtNumber);
                    //if(onlyBack == false)
                    //{
                    //    mrcRtNumberForeground = (UInt64)foregroundMrcRenderTexture.GetNativeTexturePtr().ToInt64();
                    //    PXR_Plugin.System.UPxr_SetMRCTextureID2(mrcRtNumberForeground);
                    //}
                    //else {
                    //    PXR_Plugin.System.UPxr_SetMRCTextureID2(0);
                    //}
                }
                else if (backCameraObj == null)
                {
                    mrcPlay = true;
                    UPxr_CreateCamera(xmlCameraData);
                    UPxr_Calibration();
                    //mrcRtNumber = (UInt64)mrcRenderTexture.GetNativeTexturePtr().ToInt64();
                    //PXR_Plugin.System.UPxr_SetMRCTextureID(mrcRtNumber);
                    //if (onlyBack == false)
                    //{
                    //    mrcRtNumberForeground = (UInt64)foregroundMrcRenderTexture.GetNativeTexturePtr().ToInt64();
                    //    PXR_Plugin.System.UPxr_SetMRCTextureID2(mrcRtNumberForeground);
                    //}
                    //else
                    //{
                    //    PXR_Plugin.System.UPxr_SetMRCTextureID2(0);
                    //}
                }
                if (PXR_Plugin.System.UPxr_GetHomeKey())
                {
                    replacement = true;
                }


                Vector3 headToExternalCameraVec = Camera.main.transform.position - foregroundCameraObj.transform.position;
                float clipDistance = Vector3.Dot(headToExternalCameraVec, foregroundCameraObj.transform.forward);
                foregroundCameraObj.GetComponent<Camera>().farClipPlane = Mathf.Max(foregroundCameraObj.GetComponent<Camera>().nearClipPlane + 0.001f, clipDistance);
                UPxr_GetLayerImage();
            }
            else
            {
                if (mrcPlay == true)
                {
                    mrcPlay = false;
                    backCameraObj.SetActive(false);
                    foregroundCameraObj.SetActive(false);
                }
            }

        }

        public void LateUpdate()
        {
            if (replacement && PXR_Plugin.System.UPxr_GetMRCEnable())
            {
                UPxr_Calibration();
                PXR_Plugin.System.UPxr_InitHomeKey();
                replacement = false;
            }
        }

        public float UPxr_GetYFOV()
        {
            return yFov;
        }

        public void UPxr_Calibration() {
            PxrPosef pose = new PxrPosef();
            pose.orientation.x = 0;
            pose.orientation.y = 0;
            pose.orientation.z = 0;
            pose.orientation.w = 0;
            pose.position.x = 0;
            pose.position.y = 0;
            pose.position.z = 0;
            PXR_Plugin.System.UPxr_GetMrcPose(ref pose);
            backCameraObj.transform.localPosition = new Vector3(pose.position.x + locationDeflection.x, pose.position.y + locationDeflection.y + height, (-pose.position.z) * 1f);
            foregroundCameraObj.transform.localPosition = new Vector3(pose.position.x + locationDeflection.x, pose.position.y + locationDeflection.y + height, (-pose.position.z) * 1f);
            Vector3 rototion = new Quaternion(pose.orientation.x, pose.orientation.y, pose.orientation.z, pose.orientation.w).eulerAngles;
            backCameraObj.transform.localEulerAngles = new Vector3(-rototion.x + angularDeflection.x, -rototion.y + angularDeflection.y, -rototion.z + angularDeflection.z);
            foregroundCameraObj.transform.localEulerAngles = new Vector3(-rototion.x + angularDeflection.x, -rototion.y + angularDeflection.y, -rototion.z + angularDeflection.z);
        }

        public Vector3 UPxr_ToVector3(float[] translation)
        {
            Debug.Log("translation:" + new Vector3(translation[0], translation[1], -translation[2]).ToString());
            return new Vector3(translation[0] + locationDeflection.x, translation[1] + locationDeflection.y + height, (-translation[2]) * 1f);
        }

        public Vector3 UPxr_ToRotation(float[] rotation)
        {
            Quaternion quaternion = new Quaternion(rotation[0], rotation[1], rotation[2], rotation[3]);
            Vector3 vector3 = quaternion.eulerAngles;
            Debug.Log("rotation:" + vector3.ToString());
            return new Vector3(-vector3.x + angularDeflection.x, -vector3.y + angularDeflection.y, -vector3.z + angularDeflection.z);
        }
    }
}