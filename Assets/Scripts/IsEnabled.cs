using UnityEngine;

public class IsEnabled : MonoBehaviour
{
    public int needToUnlock;
    public Material lockMaterial;
    private void Start()
    {
        if(PlayerPrefs.GetInt("score") < needToUnlock)
        {
            GetComponent<MeshRenderer>().material = lockMaterial;
        } 
    }
}
