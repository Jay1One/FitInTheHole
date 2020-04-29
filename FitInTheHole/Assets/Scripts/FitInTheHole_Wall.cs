using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FitInTheHole_Wall : MonoBehaviour
{
    private List<Transform> cubes;
    private Transform parent;

    public Transform Parent
    {
        get { return parent; }
    }

    public FitInTheHole_Wall(int sizeX, int sizeY, GameObject prefab)
    {
        GenerateWall(sizeX, sizeY, prefab);
    }

    public void SetupWall(FitInTheHole_Template template, float position)
    {
        parent.transform.position = new Vector3(0f, 0.5f, position);
        foreach (var cube in cubes)
        {
            cube.gameObject.SetActive(true);
        }

        var figure = template.GetFigure();

        for (int f = 0; f < figure.Length; f++)
        {
            for (int c = 0; c < cubes.Count; c++)
            {
                if (!figure[f] || !cubes[c]) //тут потенциально может быть баг
                {
                    continue;
                }

                if (Mathf.Abs(figure[f].position.x - cubes[c].position.x) > 0.1f)
                    continue;

                if (Mathf.Abs(figure[f].position.y - cubes[c].position.y) > 0.1f)
                    continue;

                cubes[c].gameObject.SetActive(false);
            }
        }
    }


    private void GenerateWall(int sizeX, int sizeY, GameObject prefab)
    {
        cubes = new List<Transform>();

        parent = new GameObject("Wall").transform;

        for (int x = -sizeX + 1; x < sizeX; x++)
        {
            for (int y = 0; y < sizeY; y++)
            {
                var obj = Object.Instantiate(prefab, new Vector3(x, y, 0f), Quaternion.identity);
                obj.transform.parent = parent;
                cubes.Add(obj.transform);
            }
        }

        parent.position = new Vector3(0f, 0.5f, 0f);
    }
}