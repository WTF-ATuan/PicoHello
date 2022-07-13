// Copyright © 2015-2021 Pico Technology Co., Ltd. All Rights Reserved.

using System;
using UnityEngine;
using UnityEngine.XR;

namespace Unity.XR.PXR
{
    public static class PXR_Input
    {
        public enum ControllerDevice
        {
            G2 = 3,
            Neo2,
            Neo3,
            Neo3_Phoenix,
            NewController = 10
        }

        public enum Controller
        {
            LeftController,
            RightController,
        }

        /// <summary>
        /// Get the current master control controller.
        /// </summary>
        public static Controller GetDominantHand()
        {
            return (Controller)PXR_Plugin.Controller.UPxr_GetControllerMainInputHandle();
        }

        /// <summary>
        /// Set the current master control controller.
        /// </summary>
        public static void SetDominantHand(Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerMainInputHandle((UInt32)controller);
        }

        /// <summary>
        /// Set the controller vibrate.
        /// </summary>
        public static void SetControllerVibration(float strength, int time, Controller controller)
        {
            PXR_Plugin.Controller.UPxr_SetControllerVibration((UInt32)controller, strength, time);
        }

        /// <summary>
        /// Get the controller device.
        /// </summary>
        public static ControllerDevice GetControllerDeviceType()
        {
            return (ControllerDevice)PXR_Plugin.Controller.UPxr_GetControllerType();
        }

        /// <summary>
        /// Get the connection status of Controller.
        /// </summary>
        public static bool IsControllerConnected(Controller controller)
        {
            var state = false;
            switch (controller)
            {
                case Controller.LeftController:
                    InputDevices.GetDeviceAtXRNode(XRNode.LeftHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
                case Controller.RightController:
                    InputDevices.GetDeviceAtXRNode(XRNode.RightHand).TryGetFeatureValue(PXR_Usages.controllerStatus, out state);
                    return state;
            }
            return state;
        }

        /// <summary>
        /// Set the controller origin offset data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="offset">m</param>
        public static void SetControllerOriginOffset(Controller controller, Vector3 offset)
        {
            PXR_Plugin.Controller.UPxr_SetControllerOriginOffset((int)controller, offset);
        }

        /// <summary>
        /// Get the controller predict rotation data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Quaternion GetControllerPredictRotation(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime,headData, ref pxrControllerTracking);

            return new Quaternion(
                pxrControllerTracking.localControllerPose.pose.orientation.x,
                pxrControllerTracking.localControllerPose.pose.orientation.y, 
                pxrControllerTracking.localControllerPose.pose.orientation.z,
                pxrControllerTracking.localControllerPose.pose.orientation.w);
        }

        /// <summary>
        /// Get the controller predict position data.
        /// </summary>
        /// <param name="hand">0,1</param>
        /// <param name="predictTime">ms</param>
        public static Vector3 GetControllerPredictPosition(Controller controller, double predictTime)
        {
            PxrControllerTracking pxrControllerTracking = new PxrControllerTracking();
            float[] headData = new float[7] { 0, 0, 0, 0, 0, 0, 0 };

            PXR_Plugin.Controller.UPxr_GetControllerTrackingState((uint)controller, predictTime, headData, ref pxrControllerTracking);

            return new Vector3(
                pxrControllerTracking.localControllerPose.pose.position.x, 
                pxrControllerTracking.localControllerPose.pose.position.y,
                pxrControllerTracking.localControllerPose.pose.position.z);
        }

        /// <summary>
        /// Set the controller vibrate.
        /// </summary>
        /// <param name="hand">0-Left 1-Right</param>
        /// <param name="frequency">30-500</param>
        /// <param name="strength">0-1</param>
        /// <param name="time">0-65535ms</param>
        /// <returns></returns>
        public static int SetControllerVibrationEvent(UInt32 hand, int frequency, float strength, int time) {
            return PXR_Plugin.Controller.UPxr_SetControllerVibrationEvent(hand, frequency, strength, time);
        }

        /// <summary>
        /// Stop sound vibration
        /// </summary>
        /// <param name="id">The default is 0</param>
        /// <returns></returns>
        public static int StopControllerVCMotor(int id) {
            return PXR_Plugin.Controller.UPxr_StopControllerVCMotor(id);
        }

        /// <summary>
        /// Start sound vibration
        /// </summary>
        /// <param name="file">Audio file path</param>
        /// <param name="slot">0-No 1-Left 2-Right 3-Left And Right</param>
        /// <param name="slotconfig">0-Reversal 1-No Reversal</param>
        /// <returns></returns>
        public static int StartControllerVCMotor(string file, int slot, int slotconfig) {
            return PXR_Plugin.Controller.UPxr_StartControllerVCMotor(file, slot, slotconfig);
        }

        /// <summary>
        /// Set vibration intensity
        /// </summary>
        /// <param name="mode">0-2</param>
        /// <returns></returns>
        public static int SetControllerAmp(float mode) {
            return PXR_Plugin.Controller.UPxr_SetControllerAmp(mode);
        }

        /// <summary>
        /// Start sound vibration
        /// </summary>
        /// <param name="audioClip"></param>
        /// <param name="slot"></param>
        /// <returns></returns>
        public static int StartVibrateBySharem(AudioClip audioClip, int slot, int slotconfig) {
            float[] data = new float[audioClip.samples * audioClip.channels];
            int buffersize = audioClip.samples * audioClip.channels;
            audioClip.GetData(data, 0);
            int sampleRate = audioClip.frequency;
            int channelMask = audioClip.channels;
            return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(data, slot, buffersize, sampleRate, channelMask, 32, slotconfig);
        }

        /// <summary>
        /// Start sound vibration
        /// </summary>
        /// <param name="data">PCM Data</param>
        /// <param name="slot">0-No 1-Left 2-Right 3-Left And Right</param>
        /// <param name="buffersize">buffersize</param>
        /// <param name="sampleRate">frequency</param>
        /// <param name="channelMask">channels</param>
        /// <returns></returns>
        public static int StartVibrateBySharem(float[] data, int slot, int buffersize, int frequency, int channelMask, int slotconfig)
        {
            return PXR_Plugin.Controller.UPxr_StartVibrateBySharem(data, slot, buffersize, frequency, channelMask, 32, slotconfig);
        }
    }
}

