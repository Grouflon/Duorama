using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace grouflon {

public class UnityTools
{
    static public void DestroyAllChildren(Transform _transform)
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in _transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        else
        {
            Transform[] children = new Transform[_transform.childCount];
            for (int i = 0; i < _transform.childCount; ++i)
            {
                children[i] = _transform.GetChild(i);
            }
            foreach (Transform child in children)
            {
                GameObject.DestroyImmediate(child.gameObject);
            }
        }
    }

    static public void SetLayerRecursively(Transform _transform, int _layer)
    {
        _transform.gameObject.layer = _layer;
        foreach (Transform child in _transform)
        {
            SetLayerRecursively(child, _layer);
        }
    }
}

} // namespace grouflon
