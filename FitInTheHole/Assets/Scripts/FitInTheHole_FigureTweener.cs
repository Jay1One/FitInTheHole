using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitInTheHole_FigureTweener : MonoBehaviour
{
    private Vector3 fromPosition;
    private Vector3 toPosition;

    private Vector3 rotationPoint;
    private float rotationAngle;
    private float rotationDirection;
    private float speedMultiplayer = 4f;

    float timer;


    public void Tween(Vector3 from, Vector3 to, int arrayDirection)
    {
        fromPosition = from;
        toPosition = to;

        //проверяем нужно ли смещение по оси Х
        bool isX = Mathf.Abs(from.x - to.x) > 0.01f;
        //проверяем нужно ли смещение по оси Y
        bool isY = Mathf.Abs(from.y - to.y) > 0.01f;

        //Находим точку вращения
        rotationPoint = Vector3.Lerp(from, to, 0.5f);
 
        rotationAngle = 90f;
        //Если нужно смещение по обеим осям
        if (isX && isY)
        {
            rotationAngle = 180f;
            
            //если двигаемся влево по массиву позиций
            if (arrayDirection == 1)
            {
                rotationDirection = 1;
            }
            //если двигаемся влево по массиву позиций
            if (arrayDirection == -1)
            {
                rotationDirection = -1;
            }
            return;
        }
        
        //Если нужно смещение только по оси X
        if (isX)
        {
            //если двигаемся влево по массиву позиций
            if (arrayDirection == 1)
            {
                rotationDirection = 1;
                rotationPoint.y += from.x > to.x ? -0.5f : 0.5f;
            }
            //если двигаемся влево по массиву позиций
            if (arrayDirection == -1)
            { 
                rotationDirection = -1;
                rotationPoint.y += from.x > to.x ? 0.5f : -0.5f;
            }
        }

        //Если нужно смещение только по оси Y
        else
        {
            //если двигаемся влево по массиву позиций
            if (arrayDirection == 1)
            {
                rotationDirection = 1;
                rotationPoint.x += from.y > to.y ? 0.5f : -0.5f;
            }
            //если двигаемся вправо по массиву позиций
            if (arrayDirection == -1)
            {
                rotationDirection = -1;
                rotationPoint.x += from.y > to.y ? -0.5f : 0.5f;
            }
        }
    }

    void Update()
    {
        transform.RotateAround(rotationPoint, Vector3.forward,
            Time.deltaTime * speedMultiplayer * rotationAngle * rotationDirection);

        timer += Time.deltaTime * speedMultiplayer;
        timer = Mathf.Clamp01(timer);

        if (timer < 0.999f)
            return;
        transform.position = toPosition;
        transform.rotation = Quaternion.identity;

        Destroy(this);
    }
}