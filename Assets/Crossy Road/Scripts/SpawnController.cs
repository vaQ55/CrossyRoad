using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public bool goLeft = false;
    public bool goRight = false;

    public List<GameObject> items = new List<GameObject>();
    public List<Spawner> spawnersLeft = new List<Spawner>();
    public List<Spawner> spawnersRight = new List<Spawner>();

    void Start()
    {
        int itemId1 = Random.Range(0, items.Count);
        int itemId2 = Random.Range(0, items.Count);

        GameObject item1 = items[itemId1];
        GameObject item2 = items[itemId2];

        int direction = Random.Range(0, 2);

        if (direction > 0) { goLeft = false; goRight = true; } else { goLeft = true; goRight = false; }

        // ⭐ QUYẾT ĐỊNH KÍCH HOẠT: Chọn ngẫu nhiên xem sẽ kích hoạt các vị trí lẻ hay vị trí chẵn.
        // Điều này đảm bảo các vật thể cách nhau ít nhất một ô trống.
        bool activateOddPositions = Random.value < 0.5f; 

        // --- Xử lý SpawnersLeft ---
        for(int i = 0; i< spawnersLeft.Count; i++)
        {
            bool isOdd = (i % 2 != 0);
            bool shouldActivate = false;

            // 1. Gán Item (GIỮ NGUYÊN logic i % 2)
            if (isOdd)
            {
                spawnersLeft[i].item = item1;
                shouldActivate = (activateOddPositions == true); // Kích hoạt nếu là vị trí lẻ và ta chọn vị trí lẻ
            }
            else
            {
                spawnersLeft[i].item = item2;
                shouldActivate = (activateOddPositions == false); // Kích hoạt nếu là vị trí chẵn và ta chọn vị trí chẵn
            }
            
            // 2. Kích hoạt/Vô hiệu hóa (Chống chồng chéo)
            spawnersLeft[i].goLeft = goLeft;
            
            // Chỉ kích hoạt Spawner nếu hướng di chuyển đúng VÀ nó được chọn để spawn (shouldActivate).
            spawnersLeft[i].gameObject.SetActive(goRight && shouldActivate); 
            
            spawnersLeft[i].spawnLeftPos = spawnersLeft[i].transform.position.x;
        }


        // --- Xử lý SpawnersRight ---
        // Lặp lại quyết định kích hoạt tương tự cho đường ray bên phải.
        activateOddPositions = Random.value < 0.5f; 
        
        for (int i = 0; i < spawnersRight.Count; i++)
        {
            bool isOdd = (i % 2 != 0);
            bool shouldActivate = false;
            
            // 1. Gán Item (GIỮ NGUYÊN logic i % 2)
            if (isOdd)
            {
                spawnersRight[i].item = item1;
                shouldActivate = (activateOddPositions == true);
            }
            else
            {
                spawnersRight[i].item = item2;
                shouldActivate = (activateOddPositions == false);
            }
            
            // 2. Kích hoạt/Vô hiệu hóa (Chống chồng chéo)
            spawnersRight[i].goLeft = goLeft;
            
            // Chỉ kích hoạt Spawner nếu hướng di chuyển đúng VÀ nó được chọn để spawn (shouldActivate).
            spawnersRight[i].gameObject.SetActive(goLeft && shouldActivate);
            
            spawnersRight[i].spawnLeftPos = spawnersRight[i].transform.position.x;
        }
    }

    void Update()
    {
        
    }
}