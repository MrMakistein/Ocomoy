using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;




public class customCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public float CursorTextureDivider = 4;

    public GameObject LineRender;
    
    public Vector2 objectPoint2D = new Vector2();
    public Vector3 objectPosition = new Vector3();
    Camera c;
    float Distance;
    float relDistance;
    Color dragColor;
    float LineWidth;

    public static customCursor instance;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        Vector2 hotSpot = new Vector2(0, 0); //new Vector2(cursorTexture.width / 2, cursorTexture.height / 2);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        c = Camera.main;
    }

    public void UpdateCursor()
    {
        Vector2 hotSpot = new Vector2(0, 0);
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    private void OnGUI()
    {
        if(dnd.draggingObject != null && dnd.draggingObject.activeInHierarchy) { 
            objectPosition = c.WorldToScreenPoint(dnd.draggingObject.transform.position);
            objectPoint2D = new Vector2(objectPosition.x, objectPosition.y);

            Distance = Vector2.Distance(Input.mousePosition, objectPoint2D);
            //0 - 1 Value for the distance from the cursor to the object. used to display the "strenght" of the connection
            relDistance = Distance/dnd.DropDistance;
            dragColor = Color.Lerp(Color.green, Color.red, relDistance);

            //thickness from 1 - 10
            LineWidth = 1 + 9.0f * (1.0f - relDistance);

         
            //update params
            LineRender.SetActive(true);
            LineRender.GetComponent<UILineRenderer>().LineThickness = LineWidth;
            LineRender.GetComponent<UILineRenderer>().color = dragColor;
            LineRender.GetComponent<UILineRenderer>().Points[0] = objectPoint2D;
            LineRender.GetComponent<UILineRenderer>().Points[1] = Input.mousePosition;
            
            if (relDistance <= 0.003)
            {
                LineRender.SetActive(false);
            }
            else
            {
                LineRender.SetActive(true);
            }
        }
        else
        {
            LineRender.SetActive(false);
        }       
    }
}

