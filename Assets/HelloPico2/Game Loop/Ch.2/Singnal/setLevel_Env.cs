using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setLevel_Env : MonoBehaviour
{
    Level_FadeController _level_FadeController;
    public string setName;
    public GameObject setLevel;
    public GameObject hideLevel;
    public GameObject setTunnelDefault;
    public bool isfrist;
    // Start is called before the first frame update
    void Start()
    {
        if (isfrist)
        {
            setLevel.GetComponent<Level_FadeController>().Enable_Env = true;
        }
        SettingLevelEnv();
    }
    private void SettingLevelEnv()
    {
        hideLevel.SetActive(false);
        setTunnelDefault.transform.position = Vector3.zero;
    }

}
