using UnityEngine;

public class Coin : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public string coinID;

    private void Start()
    {
        // Nếu coinID đã được lưu là đã ăn, thì tự huỷ
        if (PlayerPrefs.GetInt("Coin_" + coinID, 0) == 1)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
