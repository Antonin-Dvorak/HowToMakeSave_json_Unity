using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlaneSaveLoad : MonoBehaviour
{
    [SerializeField] GameObject[] planes = new GameObject[3]; //インスペクター上でplane1~3を入れてください   
    string flagStr;//flagStrToSave（シリアライズ化してjsonに書き込むための変数）に値を送るための変数  


    public void SwitchPlane(GameObject obj)
    {
        if (obj.activeSelf == true)
        {
            obj.SetActive(false);
        }
        else
        {
            obj.SetActive(true);
        }
    }


    [System.Serializable]
    public class PlayerData
    {
        public string flagStrToSave;
    }
    PlayerData myData = new PlayerData();


    public void OnClickToSave()
    {
        flagStr = "";//初期化
        for (int i = 0; i < planes.Length; i++)//「iというint型を定義します、これを0から」;「配列の要素の数を超えるまで」;「1ずつ増やしていきます」
        {
            if (planes[i].activeSelf == true)//もし「planesという配列のi番目がアクティブ」==「である」なら
            {
                flagStr += 1.ToString();//末尾に1を追加
            }
            else if (planes[i].activeSelf == false)//もし「planesという配列のi番目がアクティブ」==「でない」なら
            {
                flagStr += 0.ToString();//末尾に0を追加
            }
        }
        Debug.Log(flagStr + "をセーブ");


        StreamWriter writer;
        myData.flagStrToSave = flagStr;//ここでflagStrToSaveにflagStrの文字を渡す

        string jsonStr = JsonUtility.ToJson(myData);
        Debug.Log("jsonstrは今" + jsonStr + "です");

        writer = new StreamWriter(Application.dataPath + "/save.json", false);
        writer.Write(jsonStr);
        writer.Flush();
        writer.Close();
    }
    

    public void OnClickToLoad()
    {
        if (File.Exists(Application.dataPath + "/save.json"))
        {
            string dataStr = "";
            StreamReader reader;
            reader = new StreamReader(Application.dataPath + "/save.json");
            dataStr = reader.ReadToEnd();
            reader.Close();

            myData = JsonUtility.FromJson<PlayerData>(dataStr); // ロードしたデータで上書き
            Debug.Log(myData.flagStrToSave + "のデータをロードしました");
        }
        else
        {
            Debug.Log("データがありません");
            return;
        }


        for (int i = 0; i < planes.Length; i++)
        {
            if (myData.flagStrToSave == null)
            {
                Debug.Log("セーブデータはありません。");
                break;
            }
            string x = myData.flagStrToSave[i].ToString();
            GameObject nowGameObj = planes[i];
            if (x == "1")
            {
                nowGameObj.SetActive(true);
            }
            else if (x == "0")
            {
                nowGameObj.SetActive(false);
            }
        }
    }
}