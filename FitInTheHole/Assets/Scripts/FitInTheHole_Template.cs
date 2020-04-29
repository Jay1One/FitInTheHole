using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FitInTheHole_Template : MonoBehaviour
{
    [SerializeField] private Transform[] m_Cubes;
    [SerializeField] private GameObject m_PositionVariantPrefab;
    [SerializeField] private GameObject m_PlayerCubePrefab;
    
    private Transform playerPosition;
    private Transform[] positionVariants;
    private int currentPosition;
    private FitInTheHole_FigureTweener tweener; //экземпляр класса, отвечающий за повороты кубика
    public bool CanMove = true;

    public Transform CurrentTarget { get; private set; }

    /// <summary>
    /// Получение массива со всеми блоками фигуры
    /// </summary>
    /// <returns></returns>
    public Transform[] GetFigure()
    {
        var figure = new Transform[m_Cubes.Length + 1];
        m_Cubes.CopyTo(figure, 0);
        figure[figure.Length - 1] = CurrentTarget;
        return figure;
    }

    /// <summary>
    /// Постройка случайной фигуры
    /// </summary>
    public void SetupRandomFigure(bool hintsEnabled)
    {
        int rand = Random.Range(0, positionVariants.Length);
        for (int i = 0; i < positionVariants.Length; i++)
        {
            if (i == rand)
            {
                //если подсказки выключены то не показываем красный квадрат
                if (hintsEnabled) 
                    positionVariants[i].gameObject.SetActive(true);
                else
                    positionVariants[i].gameObject.SetActive(false);

                CurrentTarget = positionVariants[i].transform;
                continue;
            }

            positionVariants[i].gameObject.SetActive(false);
        }
        //Выставляем случайное стартовое значение позиции
        currentPosition = Random.Range(0, positionVariants.Length);
        playerPosition.position = positionVariants[currentPosition].position;
    }



    private void Update()
    {
        if (tweener)
            return;
        
        if (!CanMove)
            return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.RightArrow))
            MoveRight();
    }


    private void MoveLeft()
    {
        if (!IsMovementPossible(1))
            return;

        currentPosition += 1;
        tweener = playerPosition.gameObject.AddComponent<FitInTheHole_FigureTweener>();
        tweener.Tween(playerPosition.position, positionVariants[currentPosition].position, 1);
    }


    private void MoveRight()
    {
        if (!IsMovementPossible(-1))
            return;

        currentPosition -= 1;
        tweener = playerPosition.gameObject.AddComponent<FitInTheHole_FigureTweener>();
        tweener.Tween(playerPosition.position, positionVariants[currentPosition].position, -1);
    }

    private bool IsMovementPossible(int dir)
    {
        return currentPosition + dir >= 0 && currentPosition + dir < positionVariants.Length;
    }

    public bool isPlayerInCorrectPosition()
    {
        return CurrentTarget.position == playerPosition.position;
    }
    

    public void GeneratePositionVariants()
    {
        //заполняем массив позициями  белых кубов
        Vector3[] cubesPositions=new Vector3[m_Cubes.Length];
        for (int i = 0; i < m_Cubes.Length; i++)
        {
            cubesPositions[i] = m_Cubes[i].transform.position;
        }

        
        //сюда будем складывать позиции красных кубов
        HashSet<Vector3> RedCubePositions=new HashSet<Vector3>();


        for (int i = 0; i < cubesPositions.Length; i++)
        {
            //проходимся по всем направлениям от текущего куба
            Vector3 v = cubesPositions[i] + Vector3.up;
            RedCubePositions.Add(v);
            v = cubesPositions[i] + Vector3.left;
            RedCubePositions.Add(v);
            v = cubesPositions[i] + Vector3.right;
            RedCubePositions.Add(v);
            v = cubesPositions[i] + Vector3.down;
            if (v.y>=0)
            {
                RedCubePositions.Add(v);
            }

        }
        //убираем позиции, занятые белыми кубами
        foreach (var position in cubesPositions)
        {
            RedCubePositions.Remove(position);
        }

        

        //находим первую  позицию красных кубов
            Vector3 bottomRightPosition=new Vector3();
            foreach (var position in RedCubePositions)
            {
                if (position.y!=0)
                {
                    continue;
                }

                if (position.x>bottomRightPosition.x)
                {
                    bottomRightPosition = position;
                }
            }

            
            //ставим точки в нужном порядке
            Vector3[] result=new Vector3[RedCubePositions.Count];
            result[0] = bottomRightPosition;
            for (int i = 1; i < RedCubePositions.Count; i++)
            {
                float distance = 1.5f;
                foreach (var positionVariant in RedCubePositions)
                {
                    if (positionVariant == result[i - 1])
                    {
                        continue;
                    }
                    if ((positionVariant - result[i-1]).magnitude>1.5f)
                    {
                        continue;
                    }
                    
                    if (i > 1)
                    {
                        if ( positionVariant==result[i-2])
                        {
                            continue;
                        }
                    }

                    if ((positionVariant - result[i-1]).magnitude<distance)
                    {
                        distance = (positionVariant - result[i - 1]).magnitude;
                        result[i] = positionVariant;
                    }

                }
            }
            print(result.Length);
            //создаем красные кубы
            positionVariants=new Transform[result.Length];
            for (int i = 0; i < result.Length; i++)
            {
                positionVariants[i] = Instantiate(m_PositionVariantPrefab, result[i], Quaternion.identity, this.transform).transform;
            }

            //создаем желтый куб игрока
            playerPosition = Instantiate(m_PlayerCubePrefab, this.transform).transform;

    }
}