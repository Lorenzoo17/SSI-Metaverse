using UnityEngine;
using MixedReality.Toolkit;
using MixedReality.Toolkit.SpatialManipulation;
using UnityEngine.XR;

public class GeneralUtilities : MonoBehaviour {
    [SerializeField] private GameObject indexObject;
    [SerializeField] private GameObject thumbObject;
    [SerializeField] private GameObject middleObject;
    private void Update() {
        if (XRSubsystemHelpers.HandsAggregator != null && (XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.IndexTip, XRNode.LeftHand, out HandJointPose index) &&
            XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.ThumbTip, XRNode.LeftHand, out HandJointPose thumb) && XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.MiddleIntermediate, XRNode.LeftHand, out HandJointPose middleInter))) {
            Vector3 indexTipPose = index.Pose.position;
            Vector3 thumbTipPose = thumb.Pose.position;
            Vector3 middleInterPose = middleInter.Pose.position;

            indexObject.transform.position = indexTipPose;
            thumbObject.transform.position = thumbTipPose;
            middleObject.transform.position = middleInterPose;

            // Debug.Log(Vector3.Angle(indexTipPose, thumbTipPose));
            // Debug.Log($"IndexTip : {indexTipPose} -- ThumbTip : {thumbTipPose}");
            // Debug.Log("Index radius : " + index.Pose.rotation); //Mediante questa rotation e' sicuramente possibile capire se il dito e' abbassato o meno
        }
        if (XRSubsystemHelpers.HandsAggregator != null && XRSubsystemHelpers.HandsAggregator.TryGetJoint(TrackedHandJoint.Palm, XRNode.RightHand, out HandJointPose palm)) {
            Vector3 palmPosition = palm.Pose.position;

            Debug.Log("Palm position: " + palmPosition);
        }
    }
}
