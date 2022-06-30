using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PlaneSaveLoad : MonoBehaviour
{
    [SerializeField] GameObject[] planes = new GameObject[3]; //�C���X�y�N�^�[���plane1~3�����Ă�������   
    string flagStr;//flagStrToSave�i�V���A���C�Y������json�ɏ������ނ��߂̕ϐ��j�ɒl�𑗂邽�߂̕ϐ�  


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
        flagStr = "";//������
        for (int i = 0; i < planes.Length; i++)//�ui�Ƃ���int�^���`���܂��A�����0����v;�u�z��̗v�f�̐��𒴂���܂Łv;�u1�����₵�Ă����܂��v
        {
            if (planes[i].activeSelf == true)//�����uplanes�Ƃ����z���i�Ԗڂ��A�N�e�B�u�v==�u�ł���v�Ȃ�
            {
                flagStr += 1.ToString();//������1��ǉ�
            }
            else if (planes[i].activeSelf == false)//�����uplanes�Ƃ����z���i�Ԗڂ��A�N�e�B�u�v==�u�łȂ��v�Ȃ�
            {
                flagStr += 0.ToString();//������0��ǉ�
            }
        }
        Debug.Log(flagStr + "���Z�[�u");


        StreamWriter writer;
        myData.flagStrToSave = flagStr;//������flagStrToSave��flagStr�̕�����n��

        string jsonStr = JsonUtility.ToJson(myData);
        Debug.Log("jsonstr�͍�" + jsonStr + "�ł�");

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

            myData = JsonUtility.FromJson<PlayerData>(dataStr); // ���[�h�����f�[�^�ŏ㏑��
            Debug.Log(myData.flagStrToSave + "�̃f�[�^�����[�h���܂���");
        }
        else
        {
            Debug.Log("�f�[�^������܂���");
            return;
        }


        for (int i = 0; i < planes.Length; i++)
        {
            if (myData.flagStrToSave == null)
            {
                Debug.Log("�Z�[�u�f�[�^�͂���܂���B");
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