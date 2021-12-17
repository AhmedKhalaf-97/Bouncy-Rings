using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConesCollisionDetection : MonoBehaviour
{
    public MainMenu mainMenu;
    public Transform floatingScorePanel;
    public Text floatingTextObject;
    public int scoreToBeAdded;
    public float increasingTimeValue;

    public bool isRightOrMiddleCone;
    bool isSpecificCone;

    Color coneColor;

    public void CheckIfIsSpecificCone()
    {
        if(mainMenu.playModes.isSpecificRingWithConeMode && isRightOrMiddleCone)
        {
            isSpecificCone = true;

            coneColor = transform.parent.GetComponent<MeshRenderer>().material.color;
        }
        else
        {
            isSpecificCone = false;
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Floating"))
        {
            for (int i = 0; i < Player.floatingObjectsInstanceIds.Count; i++)
            {
                if(collider.gameObject.GetInstanceID() == Player.floatingObjectsInstanceIds[i])
                {
                    if (mainMenu.playModes.isSpecificRingWithConeMode)
                    {
                        Color ringColor = collider.gameObject.GetComponent<MeshRenderer>().material.color;

                        if (isSpecificCone)
                        {
                            if (ringColor == coneColor)
                            {
                                TriggerCoreFunction(collider, i);
                            }
                        }
                        else
                        {
                            if (!(ringColor == mainMenu.playModes.specificColor))
                            {
                                TriggerCoreFunction(collider, i);
                            }
                        }
                    }
                    else
                    {
                        TriggerCoreFunction(collider, i);
                    }

                }
            }
        }
    }

    void TriggerCoreFunction(Collider collider, int i)
    {
        collider.gameObject.GetComponent<FloatingObject>().RemoveThisObject();

        //for floating score text.
        var cam = Camera.main;
        Vector2 textPosition = cam.WorldToViewportPoint(collider.transform.position);
        Text _text = (Text)Instantiate(floatingTextObject, floatingScorePanel.transform, false);
        RectTransform rt = _text.GetComponent<RectTransform>();
        rt.anchorMax = textPosition;
        rt.anchorMin = textPosition;

        _text.text = scoreToBeAdded + "+";

        if (mainMenu.playModes.isTimeTrialGame)
        {
            Text _timeIncreasingText = (Text)Instantiate(floatingTextObject, floatingScorePanel.transform, false);
            RectTransform timeIncreasignRectTransform = _timeIncreasingText.GetComponent<RectTransform>();
            timeIncreasignRectTransform.anchorMax = textPosition - (new Vector2(0f, 0.1f));
            timeIncreasignRectTransform.anchorMin = textPosition - (new Vector2(0f, 0.1f));

            _timeIncreasingText.text = "+" + increasingTimeValue + "S";
        }

        mainMenu.AddScore(scoreToBeAdded, increasingTimeValue);

        Player.floatingObjectsInstanceIds.RemoveAt(i);
    }
}
