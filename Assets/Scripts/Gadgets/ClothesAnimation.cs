using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClothesAnimation : MonoBehaviour
{
    public SkinnedMeshRenderer TargetMeshRenderer;
    Dictionary<string, Transform> boneMap = new Dictionary<string, Transform>();
    SkinnedMeshRenderer myRenderer;
    void Start()
    {
        Bind();
    }

    public void Bind()
    {
        myRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        foreach (Transform bone in TargetMeshRenderer.bones)
            boneMap[bone.gameObject.name] = bone;

        Transform[] newBones = new Transform[myRenderer.bones.Length];
        for (int i = 0; i < myRenderer.bones.Length; ++i)
        {
            if (myRenderer.bones[i] == null)
            {
                continue;
            }
            GameObject bone = myRenderer.bones[i].gameObject;
            if (!boneMap.TryGetValue(bone.name, out newBones[i]))
            {

            }
            else
            {

            }
        }
        myRenderer.bones = newBones;
    }
    void Update()
    {

    }
}
