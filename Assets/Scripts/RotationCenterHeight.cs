using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class RotationCenterHeight : MonoBehaviour
{
    public float height = 0.088f;

    public Transform roomCenter;
    public Transform screenCenter;

    [Header("UI")]
    public TMP_InputField heightInputField;

    // Start is called before the first frame update
    void Start()
    {
        SetHeight(height);

        heightInputField.SetTextWithoutNotify(height.ToString());
        heightInputField.onEndEdit.AddListener(value =>
        {
            SetHeight(float.Parse(value));
        });
    }

    private void SetHeight(float height)
    {
        var localRotationCenterPosition = new Vector3(0, height, 0);
        if (roomCenter != null)
        {
            roomCenter.localPosition = localRotationCenterPosition;
        }
        if (screenCenter != null)
        {
            screenCenter.localPosition = localRotationCenterPosition;
        }
        this.height = height;
    }
}
