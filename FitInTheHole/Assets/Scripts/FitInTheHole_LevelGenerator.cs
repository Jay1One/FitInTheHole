using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FitInTheHole_LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject m_CubePrefab;
    [SerializeField] private float m_BaseSpeed = 2f;

    [SerializeField]
    private float m_WallDistance = 15f; //рассояние от цетра на котором генерится новая стена и исчезает старая

    private float speed; //текущая скорость движения стены
    [SerializeField] private FitInTheHole_Template[] m_Templates;
    [SerializeField] private Transform m_FigurePoint;
    [SerializeField] private int m_DisableHintsAfterFigure =2;

    private FitInTheHole_Template[] templates; //храним экземпляры шаблонов, чтобы не инстансировать каждый раз

    private FitInTheHole_Template figure; // текущая фигура

    private FitInTheHole_Wall wall; //стена, существует в одном экземпляре
    private bool hintsEnabled = true;
    private int figuresPassed = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        templates = new FitInTheHole_Template[m_Templates.Length];
        for (int i = 0; i < m_Templates.Length; i++)
        {
            templates[i] = Instantiate(m_Templates[i]);
            templates[i].GeneratePositionVariants();
            templates[i].gameObject.SetActive(false);
            templates[i].transform.position = m_FigurePoint.transform.position;
        }

        wall = new FitInTheHole_Wall(5, 5, m_CubePrefab); //строим стену первый и единственный раз
        SetupTemplate();
        wall.SetupWall(figure, m_WallDistance);
        speed = m_BaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //проверяем условия поражения
        if (wall.Parent.position.z + 1f > m_FigurePoint.position.z &&
            wall.Parent.position.z - 1f < m_FigurePoint.position.z)
        {
            figure.CanMove = false;
            if (!figure.isPlayerInCorrectPosition())
            {
                print($"defeat, your score: {figuresPassed}");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        }
        //если фигура находится посередине стены и все ОКб запретить ей двигаться
        else
        {
            figure.CanMove = true;
        }
        
        

        wall.Parent.transform.Translate(Vector3.back * Time.deltaTime * speed);

        if (wall.Parent.transform.position.z > m_WallDistance * -1f)
            return;
        
        //увеличиваем скорость каждый раз когда появляется новая фигура
        speed += 1f;
        
        //отключаем подсказки после нескольких завершенных фигур
        figuresPassed++;
        if (figuresPassed >= m_DisableHintsAfterFigure)
            hintsEnabled = false;
        //если достигли конечной позиции - перезапускаем шаблон и перестраиваем стену
        SetupTemplate();
        wall.SetupWall(figure, m_WallDistance);
    }

    /// <summary>
    /// Настройка фигуры из шаблона
    /// </summary>
    private void SetupTemplate()
    {
        if (figure)
            figure.gameObject.SetActive(false);

        var rand = Random.Range(0, templates.Length);
        figure = templates[rand];
        figure.gameObject.SetActive(true);
        figure.SetupRandomFigure(hintsEnabled);
    }
}