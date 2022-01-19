using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeManager : MonoBehaviour
{
    public MiniGameMgr MGM;
    public GameObject GuidText, GuidPanel;
    private Vector2 TextTarget;

    public GameObject Wall, Road;

    private GameObject[,] maze;
    private float x,y;
    public float blockSize;
    public int mazesize;

    public float resolutionScale;
    public float heightScale;
    private bool isFirst = true;
    // Start is called before the first frame update
    void Start()
    {
        MGM = GameObject.Find("MiniGameMgr").GetComponent<MiniGameMgr>();
        InGameMgr.Instance.EnterMiniGame("Stage_2_ruins");

        maze = new GameObject[mazesize,mazesize];

        SetUp();
        StartCoroutine(FirstStart());
        MakeMaze();
    }

    void SetUp()
    {
        resolutionScale = Screen.width / 1920f;
        heightScale = Screen.height / 1080f;
    
        TextTarget = new Vector2(GuidText.GetComponent<RectTransform>().anchoredPosition.x, GuidText.GetComponent<RectTransform>().anchoredPosition.y + MGM.textPosition);
    }

    void MakeMaze()
    {
        GameObject select, start, block;
        x = 0;
        y = 0;
        int i, j, rnd;
        int death;
        List<int> mazelist = new List<int>();
        Stack<int> mazeStackX = new Stack<int>();
        Stack<int> mazeStackY = new Stack<int>();
        //0:상 , 1:우, 2:하, 3:좌
        for(i=0;i<mazesize;i++)
        {
            for(j=0;j<mazesize;j++)
            {
                if((i%2==1 && j%2==1) || (i==mazesize-1 && j==1) || (i==0 && j==mazesize-2))
                {
                    select = Road;
                }
                else
                {
                    select = Wall;
                }
                
                
                maze[i,j] = Instantiate(select, transform.position , Quaternion.identity,GameObject.Find("MazeStructure").transform);
                maze[i,j].GetComponent<RectTransform>().anchoredPosition = new Vector2(x,y);
                
                
                x+=blockSize;
            }

            x = 0;
            y -= blockSize;
        }

        
        start = maze[1,1];
        select = start;
        i = 1;
        j = 1;
        death = 0;
        while(!start.GetComponent<MazeBlock>().isPop)
        {
            select.GetComponent<MazeBlock>().isVisit = true;
            if(!select.GetComponent<MazeBlock>().isPop)
            {
                mazeStackX.Push(i);
                mazeStackY.Push(j);
            }
            
            mazelist.Clear();
            if(i-2 > 0)
            {
                if(!maze[i-2,j].GetComponent<MazeBlock>().isVisit)
                {
                    mazelist.Add(0);
                }
            }

            if(j+2 < mazesize)
            {
                if(!maze[i,j+2].GetComponent<MazeBlock>().isVisit)
                {
                    mazelist.Add(1);
                }
            }

            if(i+ 2 < mazesize)
            {
                if(!maze[i+2,j].GetComponent<MazeBlock>().isVisit)
                {
                    mazelist.Add(2);
                }
            }

            if(j-2 > 0)
            {
                if(!maze[i,j-2].GetComponent<MazeBlock>().isVisit)
                {
                    mazelist.Add(3);
                }
            }

            if(mazelist.Count == 0)
            {
                //pop
                select.GetComponent<MazeBlock>().isPop = true;

                if(mazeStackX.Count != 0)
                    i = mazeStackX.Pop();
                
                if(mazeStackY.Count != 0)
                    j = mazeStackY.Pop();

            }
            else
            {
                rnd = Random.Range(0,mazelist.Count);
                if(mazelist[rnd] == 0) //상
                {
                    Destroy(maze[i-1,j]);
                    block = Instantiate(Road, select.transform.position , Quaternion.identity,GameObject.Find("MazeStructure").transform);
                    block.GetComponent<RectTransform>().anchoredPosition = new Vector2(select.GetComponent<RectTransform>().anchoredPosition.x,select.GetComponent<RectTransform>().anchoredPosition.y+blockSize);
                    maze[i-1,j] = block;
                    i -= 2;
                }
                else if(mazelist[rnd] == 1) //우
                {
                    Destroy(maze[i,j+1]);
                    block = Instantiate(Road, select.transform.position , Quaternion.identity,GameObject.Find("MazeStructure").transform);
                    block.GetComponent<RectTransform>().anchoredPosition = new Vector2(select.GetComponent<RectTransform>().anchoredPosition.x+blockSize,select.GetComponent<RectTransform>().anchoredPosition.y);
                    maze[i,j+1] = block;
                    j += 2;
                }
                else if(mazelist[rnd] == 2) // 하
                {
                    Destroy(maze[i+1,j]);
                    block = Instantiate(Road, select.transform.position , Quaternion.identity,GameObject.Find("MazeStructure").transform);
                    block.GetComponent<RectTransform>().anchoredPosition = new Vector2(select.GetComponent<RectTransform>().anchoredPosition.x,select.GetComponent<RectTransform>().anchoredPosition.y-blockSize);
                    maze[i+1,j] = block;
                    i += 2;
                }
                else if(mazelist[rnd] == 3) //좌
                {
                    Destroy(maze[i,j-1]);
                    block = Instantiate(Road, select.transform.position , Quaternion.identity,GameObject.Find("MazeStructure").transform);
                    block.GetComponent<RectTransform>().anchoredPosition = new Vector2(select.GetComponent<RectTransform>().anchoredPosition.x-blockSize,select.GetComponent<RectTransform>().anchoredPosition.y);
                    maze[i,j-1] = block;
                    j -= 2;
                }
            }
            select = maze[i,j];
            death+=1;
        }
        
    }

    void ResetMaze()
    {
        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        watch.Start();
        
        // 기존 미로 삭제
        for(int i=0;i<mazesize;i++)
        {
            for(int j=0;j<mazesize;j++)
            {
                Destroy(maze[i,j]);
            }
        }

        MakeMaze();
        watch.Stop();
        Debug.Log(watch.ElapsedMilliseconds + "ms");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
           ResetMaze();
        }
    }

    public void StartGame()
    {
        if(!isFirst)
            StartCoroutine(ReStart());
    }

    IEnumerator ReStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        ResetMaze();
        //StartCoroutine(SpawnObject());
    }
    IEnumerator FirstStart()
    {
        GuidPanel.SetActive(true);
        GuidText.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,0);
        
        yield return new WaitForSeconds(MGM.firstTime);
        GuidPanel.SetActive(false);
        isFirst = false;

        while (Vector2.Distance(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget) >= 0.1f)
        {
            GuidText.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(GuidText.GetComponent<RectTransform>().anchoredPosition, TextTarget, Time.deltaTime * MGM.textSpeed);

            yield return null;
        }

        
    }
}
