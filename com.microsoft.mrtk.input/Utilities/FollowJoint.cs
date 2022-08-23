// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Subsystems;
using System;
using UnityEngine;
using UnityEngine.XR;

namespace Microsoft.MixedReality.Toolkit.Input
{
    /// <summary>
    /// Simple script that makes the attached GameObject follow the specified joint.
    /// Internal for now; TODO possibly made public in the future.
    /// </summary>
    /// <remarks>
    /// Packaging-wise, this should be considered part of the Hands feature. This also does
    /// not depend on XRI.
    /// </remarks>
    [AddComponentMenu("MRTK/Input/Follow Joint")]
    internal class FollowJoint : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The pose source representing the hand joint this interactor tracks")]
        private HandJointPoseSource jointPoseSource;

        /// <summary>
        /// The pose source representing the hand joint this interactor tracks
        /// </summary>
        protected HandJointPoseSource JointPoseSource { get => jointPoseSource; set => jointPoseSource = value; }

        [SerializeField]
        [HideInInspector]
        // A temporary variable used to migrate instnaces of FollowJoint to use the jointPoseSource class as the source of truth
        // rather than its own separately serialized values.
        // TODO: Remove this after some time to ensure users have successfully migrated.
        private bool migratedSuccessfully = false;


        [SerializeField]
        [HideInInspector]
        private Handedness hand;

        /// <summary>
        /// The hand on which to track the joint.
        /// </summary>
        [Obsolete("Please change the Hand value on the jointPoseSource instead")]
        protected Handedness Hand { get => JointPoseSource.Hand; set => JointPoseSource.Hand = value; }

        [SerializeField]
        [HideInInspector]
        private TrackedHandJoint joint;

        /// <summary>
        /// The specific joint to track.
        /// </summary>
        [Obsolete("Please change the Joint value on the jointPoseSource instead")]
        protected TrackedHandJoint Joint { get => JointPoseSource.Joint; set => JointPoseSource.Joint = value; }

        /// <summary>
        /// Using OnValidate to ensure that instances of FollowJoint are migrated to the new HandJointPoseSource
        /// Doesn't work perfectly due to complications with prefab variants :(
        ///
        /// TODO: Remove this after some time to ensure users have successfully migrated.
        /// </summary>
        private void OnValidate()
        {
            if (!migratedSuccessfully)
            {
                JointPoseSource.Hand = hand;
                JointPoseSource.Joint = joint;
                migratedSuccessfully = true;
            }
        }

        void Update()
        {
            if (JointPoseSource != null && JointPoseSource.TryGetPose(out Pose pose))
            {
                transform.SetPositionAndRotation(pose.position, pose.rotation);
            }
            else
            {
                // If we have no valid tracked joint, reset to local zero.
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
            }
        }
    }
}
