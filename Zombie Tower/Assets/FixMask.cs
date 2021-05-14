using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixMask : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SpriteMask mask = GetComponent<SpriteMask>();
        int so = transform.parent.gameObject.GetComponent<SpriteRenderer>().sortingOrder;
        mask.isCustomRangeActive = true;
        mask.backSortingOrder = so;
        mask.frontSortingOrder = so;
    }

    // Update is called once per frame
    void Update()
    {

    }
}