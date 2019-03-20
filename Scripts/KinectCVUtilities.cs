using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OpenCvSharp;
using Windows.Kinect;
using System;
using System.Runtime.InteropServices;
using System.IO;


/// <summary>
/// Utilities for bridging Kinect, OpenCV and Unity together
/// </summary>
public static class KinectCVUtilities {

    public static byte[] ConvertMatToBytes(OpenCvSharp.Mat img)
    {
        int renderSize = img.Width * img.Height * img.Channels();
        byte[] ret = new byte[renderSize];
        Marshal.Copy(img.Data, ret, 0, renderSize);
        return ret;
    }


    /// <summary>
    /// Transforms a pixel point from a 2D cartesian grid to a point defined by a 3D plane
    /// </summary>
    /// <param name="plane">The 3D plane to transform the point to (assumes center of model is origin)</param>
    /// <param name="texSize">The size of the texture for point that it is referring to</param>
    /// <param name="pt">The 2D pixel point to transform</param>
    /// <returns></returns>
    public static Vector3 TransformTextureToUnity(Transform plane, Vector2 texSize, Vector2 pt)
    {
        Vector2 worldCoord = new Vector2();
        worldCoord.x = pt.x;
        worldCoord.y = pt.y;


        Vector2 planePos = new Vector2(plane.position.x, plane.position.y);

        float irWidth = texSize.x;
        float irHeight = texSize.y;

        float planeWidth = plane.localScale.x;
        float planeHeight = plane.localScale.y;



        //scale the local pixel system to the unity world system.
        Vector2 scaleTransform = new Vector2(planeWidth / irWidth, planeHeight / irHeight);
        worldCoord = Vector2.Scale(worldCoord, scaleTransform);

        //invert the y since y0 starts from bottom up
        worldCoord.y = planeHeight - worldCoord.y;

        //transform to real world, the pixel point is in the unity world coord system
        worldCoord += planePos;

        //convert to plane's coordinate system, plane's have their origins (world position) start at the center of the object
        worldCoord.x -= planeWidth / 2;
        worldCoord.y -= planeHeight / 2;

        //now apply the rotation of the plane
        Vector3 worldPt = new Vector3(worldCoord.x, worldCoord.y, plane.position.z);
        worldPt -= plane.position;
        worldPt = plane.rotation * worldPt;
        worldPt += plane.position;

        return worldPt;
    }


    /// <summary>
    /// Gets an angle between 3 points that form a connection:  p1---p2---p3
    /// Such that the vector v = p3-p2 defines the angle around p1
    /// </summary>
    /// <param name="p1">A position in space</param>
    /// <param name="p2">A position in space</param>
    /// <param name="p3">A position in space</param>
    /// <returns></returns>
    public static float VerticalWristRotation(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        Vector3 a = p2 - p1;
        Vector3 b = p2 - p3;
        a.Normalize();
        b.Normalize();
        Vector3 thetaVector = Vector3.ProjectOnPlane(b, a);
        thetaVector.Normalize();

        Vector3 rightOnPlane = Vector3.ProjectOnPlane(Vector3.right, a);
        return Vector3.SignedAngle(rightOnPlane, thetaVector, Vector3.up);
    }


    #region Debug Drawing functions
    //=================================================
    //Utility draw functions, dont forget to call Apply() on the texture when done.


    /// <summary>
    /// Draws a circle from center and radius. Expects points in the texture's coordinate system
    /// </summary>
    public static void DrawCircle(Texture2D tex, Vector2 pt, Color color, int radius)
    {
        int diameter = radius * 2;

        Vector2 center = new Vector2(diameter / 2, diameter / 2);
        int ptX = (int)pt.x;
        int ptY = (int)pt.y;


        for (int i = 0; i < diameter; i++)
        {
            for (int j = 0; j < diameter; j++)
            {
                Vector2 drawPt = new Vector2(i, j);

                if ((drawPt - center).sqrMagnitude <= (radius))
                {

                    tex.SetPixel(ptX + (i - radius), ptY + (j - radius), color);
                }
            }

        }


        tex.Apply();
    }

    /// <summary>
    /// Draws a line from start to end points. Expects points in the texture's coordinate system
    /// Source; http://wiki.unity3d.com/index.php?title=TextureDrawLine
    /// </summary>
    public static void DrawLine(Texture2D tex, Vector2 start, Vector2 end, Color color)
    {
        int x0 = (int)start.x;
        int y0 = (int)start.y;
        int x1 = (int)end.x;
        int y1 = (int)end.y;

        int dy = (int)(y1 - y0);
        int dx = (int)(x1 - x0);
        int stepx, stepy;

        if (dy < 0) { dy = -dy; stepy = -1; }
        else { stepy = 1; }
        if (dx < 0) { dx = -dx; stepx = -1; }
        else { stepx = 1; }
        dy <<= 1;
        dx <<= 1;

        float fraction = 0;

        tex.SetPixel(x0, y0, color);
        if (dx > dy)
        {
            fraction = dy - (dx >> 1);
            while (Mathf.Abs(x0 - x1) > 1)
            {
                if (fraction >= 0)
                {
                    y0 += stepy;
                    fraction -= dx;
                }
                x0 += stepx;
                fraction += dy;
                tex.SetPixel(x0, y0, color);
            }
        }
        else
        {
            fraction = dx - (dy >> 1);
            while (Mathf.Abs(y0 - y1) > 1)
            {
                if (fraction >= 0)
                {
                    x0 += stepx;
                    fraction -= dy;
                }
                y0 += stepy;
                fraction += dx;
                tex.SetPixel(x0, y0, color);
            }
        }

    }


    /// <summary>
    /// Draws a rectangle. An option to have it filled.
    /// </summary>
    /// <param name="tex"></param>
    /// <param name="topLeft"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="color"></param>
    /// <param name="filled"></param>
    public static void DrawRectangle(Texture2D tex, Vector2 topLeft, int width, int height, Color color, bool filled)
    {
        //Easy, draw the 4 sides
        DrawLine(tex, topLeft, new Vector2(topLeft.x + width, topLeft.y), color);
        DrawLine(tex, topLeft, new Vector2(topLeft.x, topLeft.y + height), color);

        Vector2 bottomRight = new Vector2(topLeft.x + width, topLeft.y + height);
        DrawLine(tex, bottomRight, new Vector2(bottomRight.x - width, bottomRight.y), color);
        DrawLine(tex, bottomRight, new Vector2(bottomRight.x, bottomRight.y - height), color);


        //fill the rest of the rectangle
        if (filled)
        {
            int topLeftX = (int)topLeft.x;
            int topLeftY = (int)topLeft.y;

            //fill
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    tex.SetPixel(topLeftX + i, topLeftY + j, color);
                }
            }
        }
    }
    #endregion

}
