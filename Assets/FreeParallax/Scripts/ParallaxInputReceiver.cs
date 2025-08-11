// License: https://en.wikipedia.org/wiki/MIT_License
// Code created by Jeff Johnson & Digital Ruby, LLC - http://www.digitalruby.com
// Code is from the Free Parallax asset on the Unity asset store: http://u3d.as/bvv
// Code may be redistributed in source form, provided all the comments at the top here are kept intact

using System;
using UnityEngine;
using System.Collections;

public class ParallaxInputReceiver : MonoBehaviour
{

    public FreeParallax parallax;

    private float scrollValue = 0;

    private float lastScrollValue = 0;

    private int direction = 0;
    
    private bool isScrolling;

    // Use this for initialization
    void Start()
    {
        
    }

    public void OnScrollValueChange(Vector2 value)
    {
        if (Mathf.Abs(lastScrollValue - value.y) > 0.00005)
        {
            if (lastScrollValue < value.y)
            {
                direction = -1;
            
            }
            else if(lastScrollValue > value.y)
            {
                direction = 1;
            }
        }
        else
        {
            direction = 0;
        }

        lastScrollValue = value.y;
        isScrolling = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (parallax != null)
        {
            if (isScrolling)
            {
                parallax.Speed = 10.0f * direction;
                isScrolling = false;
            }
            else
            {
                parallax.Speed = 0.0f;
            }
        }
    }
}
