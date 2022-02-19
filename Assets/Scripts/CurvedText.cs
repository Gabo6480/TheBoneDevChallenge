using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteAlways]
public class CurvedText : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] RectTransform _textRect;


    [SerializeField] float _curvature = 0;

    //float _oldCurvature = float.MaxValue;

    float _radius = 0;

    //bool _forceUpdate = false;

    private void OnValidate()
    {
        if(_text == null)
            _text = gameObject.GetComponent<TMP_Text>();
        if(_textRect == null)
            _textRect = gameObject.GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        // Subscribe to event fired when text object has been regenerated.
        TMPro_EventManager.TEXT_CHANGED_EVENT.Add(ON_TEXT_CHANGED);
    }
    private void OnDisable()
    {
        TMPro_EventManager.TEXT_CHANGED_EVENT.Remove(ON_TEXT_CHANGED);
    }

    void ON_TEXT_CHANGED(Object obj)
    {
        if (obj == _text)
            UpdateTextMesh();
    }

    //private void Update()
    //{
    //    //if the text and the parameters are the same of the old frame, don't waste time in re-computing everything
    //    if (!_forceUpdate && !ParametersHaveChanged())
    //    {
    //        return;
    //    }
    //
    //    _forceUpdate = false;
    //
    //    //UpdateTextMesh();
    //}

    //bool ParametersHaveChanged()
    //{
    //    //check if paramters have changed and update the old values for next frame iteration
    //    bool retVal = _curvature != _oldCurvature;
    //
    //    _oldCurvature = _curvature;
    //
    //    return retVal;
    //}

    [ContextMenu("Update Text Mesh")]
    void UpdateTextMesh()
    {
        //We use the curvature to calculate the radius of the circle with the arc length formula S = rA
        //using the text's bound's width as the arc lenght (S) and _curvature as the inner angle (A)
        if (_curvature != 0)
            _radius = _textRect.sizeDelta.x / _curvature;
        else
            _radius = 0;

        //Vertices represents the 4 vertices of a single character we're analyzing, 
        //while matrix is the roto-translation matrix that will rotate and scale the characters so that they will
        //follow the curve
        Vector3[] vertices;
        Matrix4x4 matrix;

        //Generate the mesh and get information about the text and the characters
        //_text.ForceMeshUpdate();
        TMP_TextInfo textInfo = _text.textInfo;

        //We can't work if the text info is null
        if (textInfo == null)
            return;

        int characterCount = textInfo.characterCount;

        //if the string is empty, no need to waste time
        if (characterCount == 0)
            return;

        //gets the bounds of the rectangle that contains the text 
        float boundsMinX = _text.bounds.min.x;
        float boundsMaxX = _text.bounds.max.x;

        //for each character
        for (int i = 0; i < characterCount; i++)
        {
            //skip if it is invisible
            if (!textInfo.characterInfo[i].isVisible)
                continue;

            //Get the index of the mesh used by this character, then the one of the material... and use all this data to get
            //the 4 vertices of the rect that encloses this character. Store them in vertices
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            vertices = textInfo.meshInfo[materialIndex].vertices;

            //Compute the baseline mid point for each character. This is the central point of the character.
            //we will use this as the point representing this character for the geometry transformations
            Vector3 charMidBaselinePos = new Vector2((vertices[vertexIndex + 0].x + vertices[vertexIndex + 2].x) / 2, textInfo.characterInfo[i].baseLine);

            //remove the central point from the vertices point. After this operation, every one of the four vertices 
            //will just have as coordinates the offset from the central position. This will come handy when will deal with the rotations
            vertices[vertexIndex + 0] -= charMidBaselinePos;
            vertices[vertexIndex + 1] -= charMidBaselinePos;
            vertices[vertexIndex + 2] -= charMidBaselinePos;
            vertices[vertexIndex + 3] -= charMidBaselinePos;

            //compute the horizontal position of the character relative to the bounds of the box, in a range [0, 1]
            //where 0 is the left border of the text and 1 is the right border
            float zeroToOnePos = (charMidBaselinePos.x - boundsMinX) / (boundsMaxX - boundsMinX);

            //get the transformation matrix, that maps the vertices, seen as offset from the central character point, to their final
            //position that follows the curve
            matrix = ComputeTransformationMatrix(charMidBaselinePos, zeroToOnePos, textInfo, i);

            //apply the transformation, and obtain the final position and orientation of the 4 vertices representing this char
            vertices[vertexIndex + 0] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 0]);
            vertices[vertexIndex + 1] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 1]);
            vertices[vertexIndex + 2] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 2]);
            vertices[vertexIndex + 3] = matrix.MultiplyPoint3x4(vertices[vertexIndex + 3]);
        }

        //Upload the mesh with the revised information
        _text.UpdateVertexData();
    }

    Matrix4x4 ComputeTransformationMatrix(Vector3 charMidBaselinePos, float zeroToOnePos, TMP_TextInfo textInfo, int charIdx)
    {
        if(_curvature == 0)
            return Matrix4x4.TRS(charMidBaselinePos, Quaternion.identity, Vector3.one);

        float r = _textRect.sizeDelta.x;
        float offset = _text.bounds.extents.x / r / 2f;
        //compute the angle at which to show this character.
        //We want the string to be centered at the top point of the circle, so we first convert the position from a range [0, 1]
        //to a [-0.5, 0.5], to make it centered on the desired point
        float angle = (((zeroToOnePos) - 0.5f) * _curvature * offset) * Mathf.Deg2Rad; //we need radians for sin and cos

        //compute the coordinates of the new position of the central point of the character. Use sin and cos since we are on a circle.
        //Notice that we have to do some extra calculations because we have to take in count that text may be on multiple lines
        float x0 = Mathf.Cos(angle);
        float y0 = Mathf.Sin(angle);
        float radius = r * _radius;
        float lineNumModif = textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber;
        Vector3 newMideBaselinePos = new Vector3(y0 * (radius - lineNumModif), x0 * (radius - lineNumModif) - 1 * radius, 0); //actual new position of the character

        newMideBaselinePos.y += charMidBaselinePos.y;

        return Matrix4x4.TRS(newMideBaselinePos, Quaternion.AngleAxis(-Mathf.Atan2(y0, x0) * Mathf.Rad2Deg, Vector3.forward), Vector3.one);
    }

    //private void OnDrawGizmos()
    //{
    //    //Gizmos.color = Color.red;
    //    //
    //    //var pos = transform.position;
    //    //pos.y -= _radius;
    //    //
    //    //Gizmos.DrawSphere(pos, 0.01f);
    //}
}
