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
            // 1 = poignet
            // 2 = paume de main
            // les autres sont les bouts de chaques doigts
            //if((int)handJoint == 1 || (int)handJoint == 2 
            //    || (int)handJoint == 6 || (int)handJoint == 11 
            //    || (int)handJoint == 16 || (int)handJoint == 21 
            //    || (int)handJoint == 26)
            //    Debug.Log(handJoint + " : " + eventData.InputData[handJoint].Position);
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
        StartCoroutine("SaveHandPose");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SaveHandPose()
    {
        while(true)
        {
            if(_handPos.Count > 0)
            {
                Debug.Log("SAVE");
                StreamWriter writer = new StreamWriter(Application.dataPath + "/Hand Positions.txt", true);
                writer.WriteLine("#START#");
                foreach(var position in _handPos)
                {
                    writer.WriteLine(position.x + "|" + position.y + "|" + position.z);
                }
                writer.WriteLine("#END#");
                writer.Close();

                writer = new StreamWriter(Application.persistentDataPath + "/Hand Positions.txt", true);
                writer.WriteLine("#START#");
                foreach(var position in _handPos)
                {
                    writer.WriteLine(position.x + "|" + position.y + "|" + position.z);
                }
                writer.WriteLine("#END#");
                writer.Close();
            }

            yield return new WaitForSeconds(1);
        }
    }
}
