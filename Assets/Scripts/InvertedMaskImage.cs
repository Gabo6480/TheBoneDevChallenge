using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.Rendering;

public class InvertedMaskImage : Image
{
    //public override Material materialForRendering { 
    //    get
    //    {
    //        Material temp = new Material(base.materialForRendering);
    //        //temp.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
    //        return temp;
    //    }
    //}
    public override bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
    {
        bool result = base.IsRaycastLocationValid(screenPoint, eventCamera);

        //If the result is true and the image type is Filled, we redo the calculation to include the angle
        if (result && type == Image.Type.Filled)
        {
            Vector2 local;
            //Its unnecesary to validate if the point is inside the plane, since the base.IsRaycastLocationValid already did that validation
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, eventCamera, out local);

            //Right now I'll just implement the Radial360 since is the one I require for this implementation,
            //replace the ifs for a switch in the case of future implementations
            if(fillMethod == FillMethod.Radial360)
            {
                //Now the fun math begins
                //We can define the "filled" sprite as an ellipse and fillAmount as it's outter angle
                //refer to wikipedia.org/wiki/Elipse

                //Keep in mind the hit calculations are done in local space!

                //First we calculate the a and b semiaxis
                //Calculate the half of width (a) and half of the hight (b)
                var a = rectTransform.rect.size.x / 2f;
                var b = rectTransform.rect.size.y / 2f;

                Vector2 origin = Vector2.zero;
                float rotation = 0;
                Vector2 clockwise = Vector2.one;

                switch ((Origin360)fillOrigin)
                {
                    case Origin360.Bottom:
                        {
                            origin = Vector2.down;
                            rotation = 270;
                            //Swap a and b since the origin is oriented vertically
                            var c = a;
                            a = b;
                            b = c;
                            //If the fill is clockwise we invert the X axis
                            clockwise.x *= fillClockwise ? -1 : 1;
                        }
                        break;
                    case Origin360.Left:
                        {
                            origin = Vector2.left;
                            rotation = 180;
                            //If the fill is clockwise we invert the Y axis
                            clockwise.y *= fillClockwise ? -1 : 1;
                        }
                        break;
                    case Origin360.Right:
                        {
                            origin = Vector2.right;
                            rotation = 0;
                            //If the fill is clockwise we invert the Y axis
                            clockwise.y *= fillClockwise ? -1 : 1;
                        }
                        break;
                    case Origin360.Top:
                        {
                            origin = Vector2.up;
                            rotation = 90;
                            //Swap a and b since the origin is oriented vertically
                            var c = a;
                            a = b;
                            b = c;

                            //If the fill is clockwise we invert the X axis
                            clockwise.x *= fillClockwise ? -1 : 1;
                        }
                        break;
                    default:
                        Debug.LogWarning("You shouldn't be here... Something must have gone really wrong. " + ((Origin360)fillOrigin).ToString() + " fill origin.");
                        break;
                }

                //Now we transform fillAmount (a value from [0 , 1]) to an angle in radians
                var parametricalAngle = fillAmount * Mathf.PI * 2;

                //Using the elliptical parametrized formula (ie x = a * cos(t)  y = b * sin(t)) we can calculate the calculate the position of the fill edge
                Vector2 elipsePos = new Vector2(a * Mathf.Cos(parametricalAngle), b * Mathf.Sin(parametricalAngle));

                //Since the formula calculates the position starting from the right, we'll have to rotate the end result
                elipsePos = Quaternion.Euler(0, 0, rotation) * elipsePos;

                //We multiply the resulting coordinates by the auxiliar clockwise vector to fix for the fillClockwise orientation
                elipsePos *= clockwise;

                //We can calculate the angle from the fill origin to the the position of the fill edge (elipsePos)
                //And we invert the angle if the fill is clockwise since Vector2.SignedAngle returns a counter clockwise angle
                var fillEdgePointAngle = NormalizeAngle(Vector2.SignedAngle(origin, elipsePos) * (fillClockwise ? -1f : 1f));

                //We calculate the angle from the fill origin to the cursor (screenPoint)
                //And we invert the angle if the fill is clockwise since Vector2.SignedAngle returns a counter clockwise angle
                var screenPointAngle = NormalizeAngle(Vector2.SignedAngle(origin, local) * (fillClockwise ? -1f : 1f));


                //Now we just compare both angles to see if the angle of the cursor is inside of the fill edge angle
                result = screenPointAngle < fillEdgePointAngle && result;
            }
            else
            {
                Debug.LogWarning("No specific behaviour implemented for the " + fillMethod.ToString() + " fill method.");
            }

            
        }


        return result;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;

    //    var a = rectTransform.lossyScale.x * rectTransform.rect.size.x / 2f;
    //    var b = rectTransform.lossyScale.y * rectTransform.rect.size.y / 2f;

    //    var parametricalAngle = fillAmount * Mathf.PI * 2;

    //    float rotation = 0;
    //    Vector2 clockwise = Vector2.one;
    //    switch ((Origin360)fillOrigin)
    //    {
    //        case Origin360.Bottom:
    //            {
    //                rotation = 270;
    //                var c = a;
    //                a = b;
    //                b = c;

    //                clockwise.x *= fillClockwise ? -1 : 1;
    //            }
    //            break;
    //        case Origin360.Left:
    //            {
    //                rotation = 180;
    //                clockwise.y *= fillClockwise ? -1 : 1;
    //            }
    //            break;
    //        case Origin360.Right:
    //            {
    //                rotation = 0;
    //                clockwise.y *= fillClockwise ? -1 : 1;
    //            }
    //            break;
    //        case Origin360.Top:
    //            {
    //                rotation = 90;
    //                var c = a;
    //                a = b;
    //                b = c;

    //                clockwise.x *= fillClockwise ? -1 : 1;
    //            }
    //            break;
    //        default:
    //            Debug.LogWarning("You shouldn't be here... Something must have gone really wrong. " + ((Origin360)fillOrigin).ToString() + " fill origin.");
    //            break;
    //    }

    //    Vector2 elipsePos = new Vector2(a * Mathf.Cos(parametricalAngle), b * Mathf.Sin(parametricalAngle));
    //    elipsePos = Quaternion.Euler(0, 0, rotation) * elipsePos;

    //    elipsePos *= clockwise;

    //    var pos = transform.position;
    //    pos.x += elipsePos.x;
    //    pos.y += elipsePos.y;

    //    Gizmos.DrawSphere(transform.position, 0.05f);
    //    Gizmos.DrawSphere(pos, 0.01f);
    //}

    private float NormalizeAngle(float a) => (a + 360f) % 360f;
}
