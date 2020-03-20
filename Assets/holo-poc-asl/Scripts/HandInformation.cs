using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandInformation : MonoBehaviour, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler
{
    private List<Vector3> _handPos;

    private void OnEnable()
    {
        CoreServices.InputSystem?.RegisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.RegisterHandler<IMixedRealityHandJointHandler>(this);
    }

    private void OnDisable()
    {
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealitySourceStateHandler>(this);
        CoreServices.InputSystem?.UnregisterHandler<IMixedRealityHandJointHandler>(this);
    }

    public void OnHandJointsUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
    {
        _handPos = new List<Vector3>();
        foreach (TrackedHandJoint handJoint in eventData.InputData.Keys)
        {
            _handPos.Add(eventData.InputData[handJoint].Position);
        }
    }

    public void OnSourceDetected(SourceStateEventData eventData)
    {
        _handPos = new List<Vector3>();
        Debug.Log("Detect");
    }

    public void OnSourceLost(SourceStateEventData eventData)
    {
        _handPos = new List<Vector3>();
        Debug.Log("Lost");
    }

    // Start is called before the first frame update
    void Start()
    {
         _handPos = new List<Vector3>();
#if UNITY_EDITOR
        File.Delete(Application.dataPath + "/holo-poc-asl/Hand Positions.txt");
#else
        File.Delete(Application.persistentDataPath + "/Hand Positions.txt");
#endif
        StartCoroutine("SaveHandPose");
    }

    public IEnumerator SaveHandPose()
    {
        while(true)
        {
            if(_handPos.Count > 0)
            {
                Debug.Log("SAVE");
                StreamWriter writer;
#if UNITY_EDITOR
                writer = new StreamWriter(Application.dataPath + "/holo-poc-asl/Hand Positions.txt", true);
                writer.WriteLine("#START#");
                foreach(var position in _handPos)
                {
                    writer.WriteLine(position.x + "|" + position.y + "|" + position.z);
                }
                writer.WriteLine("#END#");
                writer.Close();
#else
                writer = new StreamWriter(Application.persistentDataPath + "/Hand Positions.txt", true);
                writer.WriteLine("#START#");
                foreach(var position in _handPos)
                {
                    writer.WriteLine(position.x + "|" + position.y + "|" + position.z);
                }
                writer.WriteLine("#END#");
                writer.Close();
#endif
            }

            yield return new WaitForSeconds(1);
        }
    }
}
