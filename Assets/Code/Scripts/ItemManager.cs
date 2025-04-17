using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public GameObject ennemieManager;
    public GameObject collectibleManager;

    private GameObject[] ennemiesInGame;
    private GameObject[] collectiblesInGame;

    void Start()
    {
        ennemiesInGame = new GameObject[ennemieManager.transform.childCount];
        collectiblesInGame = new GameObject[collectibleManager.transform.childCount];

        for (int i = 0; i < ennemiesInGame.Length; i++)
        {
            ennemiesInGame[i] = ennemieManager.transform.GetChild(i).gameObject;
            Ennemie ennemie = ennemiesInGame[i].GetComponent<Ennemie>();

            for (int j = 0; j < collectiblesInGame.Length; j++)
            {
                collectiblesInGame[j] = collectibleManager.transform.GetChild(j).gameObject;
                Collectible collectible = collectiblesInGame[j].GetComponent<Collectible>();

                if (ennemie.beat == collectible.beat && ennemie.offset == collectible.offset)
                {
                    ennemiesInGame[i].SetActive(false);
                }

            }
        }
    }


}
