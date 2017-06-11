using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.iOS;

public class ARAnchorManager : MonoBehaviour
{
    public Transform SceneParent;
    public GameObject EmptyAnchorPrefab;
    public ARPlaneAnchorGameObject CurrentUsedAnchor { get; private set; }
    private Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

    void Start()
    {
        planeAnchorMap = new Dictionary<string, ARPlaneAnchorGameObject>();
        UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
        UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
        UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
    }
    public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        GameObject empty = Instantiate<GameObject>(this.EmptyAnchorPrefab);
        ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject();
        arpag.planeAnchor = arPlaneAnchor;
        arpag.gameObject = empty;
        planeAnchorMap.Add(arPlaneAnchor.identifier, arpag);

        positionSceneAtLargestAnchor();
    }

    public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
        {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            GameObject.Destroy(arpag.gameObject);
            planeAnchorMap.Remove(arPlaneAnchor.identifier);

            positionSceneAtLargestAnchor();
        }
    }

    public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
    {
        if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
        {
            ARPlaneAnchorGameObject arpag = planeAnchorMap[arPlaneAnchor.identifier];
            UnityARUtility.UpdatePlaneWithAnchorTransform(arpag.gameObject, arPlaneAnchor);
            arpag.planeAnchor = arPlaneAnchor;
            planeAnchorMap[arPlaneAnchor.identifier] = arpag;
        }
    }

    public void Destroy()
    {
        foreach (ARPlaneAnchorGameObject arpag in planeAnchorMap.Values)
        {
            GameObject.Destroy(arpag.gameObject);
        }

        planeAnchorMap.Clear();
    }

    private void positionSceneAtLargestAnchor()
    {
        ARPlaneAnchorGameObject largestAnchor = this.CurrentUsedAnchor;
        foreach (ARPlaneAnchorGameObject arpag in planeAnchorMap.Values)
        {
            if (largestAnchor == null || arpag.planeAnchor.extent.magnitude > largestAnchor.planeAnchor.extent.magnitude)
                largestAnchor = arpag;
        }

        if (largestAnchor != null && largestAnchor != this.CurrentUsedAnchor)
        {
            this.CurrentUsedAnchor = largestAnchor;
            SceneCreator creator = this.SceneParent.GetComponent<SceneCreator>();
            this.SceneParent.SetParent(largestAnchor.gameObject.transform, false);
            if (!creator.SceneCreated)
                creator.CreateScene();
        }
    }
}
