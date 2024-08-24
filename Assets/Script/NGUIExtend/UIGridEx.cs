using UnityEngine;
using System.Collections.Generic;

[AddComponentMenu("NGUI/Interaction/GridEx")]
public class UIGridEx : UIGrid
{
    public bool mIsDirectionAdd = false;

    public float mIntervalPixel = 0;




    protected override void ResetPosition(List<Transform> list)
    {
        mReposition = false;

        Transform myTrans = transform;

        Bounds bdLast = new Bounds();
        float fLocalPosLast = 0;
        //float fSizeTotal = 0;
        for (int indexChild = 0; indexChild < list.Count; ++indexChild)
        {
            Transform trChild = list[indexChild];

            Bounds bd = NGUIMath.CalculateRelativeWidgetBounds(trChild, false);
            
            switch (arrangement)
            {
                case Arrangement.Horizontal:
                    {
                        if (indexChild == 0)
                        {
                            trChild.localPosition = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            float fPos = fLocalPosLast;
                            fPos += bdLast.center.x;
                            if(!mIsDirectionAdd)
                            {
                                fPos -= bdLast.size.x / 2.0f;
                            }
                            else
                            {
                                fPos += bdLast.size.x / 2.0f;
                            }                            
                            fPos -= bd.center.x;
                            if (!mIsDirectionAdd)
                            {
                                fPos -= bd.size.x / 2.0f;
                                fPos -= mIntervalPixel;
                            }
                            else
                            {
                                fPos += bd.size.x / 2.0f;
                                fPos += mIntervalPixel;
                            }                           

                            trChild.localPosition = new Vector3(fPos, 0, 0);
                            fLocalPosLast = trChild.localPosition.x;
                        }
                    }
                    break;
                case Arrangement.Vertical:
                    {
                        if (indexChild == 0)
                        {
                            trChild.localPosition = new Vector3(0, 0, 0);
                        }
                        else
                        {
                            float fPos = fLocalPosLast;
                            fPos += bdLast.center.y;
                            if (!mIsDirectionAdd)
                            {
                                fPos -= bdLast.size.y / 2.0f;
                            }
                            else
                            {
                                fPos += bdLast.size.y / 2.0f;
                            }                            
                            fPos -= bd.center.y;
                            if (!mIsDirectionAdd)
                            {
                                fPos -= bd.size.y / 2.0f;
                                fPos -= mIntervalPixel;
                            }
                            else
                            {
                                fPos += bd.size.y / 2.0f;
                                fPos += mIntervalPixel;
                            }             

                            trChild.localPosition = new Vector3(0, fPos, 0);
                            fLocalPosLast = trChild.localPosition.y;
                        }
                    }
                    break;
                default:
                    {
                        Debug.LogError("UIGridEx.ResetPosition.arrangement: " + arrangement.ToString());
                    }
                    break;
            }

            bdLast = bd;

            //if (animateSmoothly && Application.isPlaying && Vector3.SqrMagnitude(t.localPosition - pos) >= 0.0001f)
            //{
            //    SpringPosition sp = SpringPosition.Begin(t.gameObject, pos, 15f);
            //    sp.updateScrollView = true;
            //    sp.ignoreTimeScale = true;
            //}
            //else t.localPosition = pos;
        }
    }
}