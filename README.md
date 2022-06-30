# HowToMakeSave_json_Unity
## Unityバージョン
2020.3.29f1

## はじめに
サルでもわかるUnityでのjson方式のセーブデータの作り方のチュートリアルです！著者はサルなので本当です！🐵  
とにかくわかりやすいように、Unityエディターのスクショは一部分でなく全体を映し、スクリプトに追記するときも全文記載しながらゲームを一緒に作っていきます。  
さっそくこのReadMeに沿って実践してみましょう。なお、リポジトリのファイルはゲームの完成品なので、このReadMeさえ見ればセーブシステムは作れます。万が一作り方がわからなければダウンロードして確認してみましょう。  *An Easy Unity Save Tutorial*  
（このプロジェクトはこちらを参考に作りました：https://unity-senpai.hatenablog.com/entry/2019/05/26/162323）  

## サルでもわかる制作の流れ
###  ①簡単なゲームを作ろう
まずボタンを押すと対応する四角（平面）が消えたり現れたりするゲームを作ります。  
それでは3Dテンプレートから新しいプロジェクトを作成しましょう。  
そしたら、カメラに映るようにシーンにこのように平面を3つ配置し、名前を左からPlane1,Plane2,Plane3にしましょう。  <p align="center">
  <img src="https://user-images.githubusercontent.com/82185511/176580013-473b78e5-2265-41ba-8021-b1806d8aeac5.png" alt="altテキスト" width="">
</p>  

次にUIボタンを追加します。まずオンオフボタンを3つ、分かりやすいよう右下、下真ん中、左下にします。（Alt+Shiftを押しながらアンカーを設定すると良いでしょう。）それからセーブ用,ロード用のボタンも上の方に追加しておいてください。
![スクリーンショット (69)](https://user-images.githubusercontent.com/82185511/176582489-daec488c-4f75-4f5d-9d97-dc9d416c7c75.png)  
ボタンのテキストの名前もそれぞれ分かりやすいように変えておいてください。  
・  
・  
・  
ではお待ちかね、スクリプトを書いていきます。とはいえまずは平面を消すギミックを記述していきます。  
スクリプトの名前は**PlaneSaveLoad**です。

```
using System.Collections;  
using System.Collections.Generic;  
using UnityEngine;  
  
  
public class PlaneSaveLoad : MonoBehaviour  
{  
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
}
```
これをボタンを押すたびに起動させるように設定していきます。  
まずこのスクリプトを何かしらのオブジェクトにアタッチします。今回はEventSystemにアタッチしておきましょう。（アタッチしたオブジェクトから参照せず、アセットから参照しようとすると次のステップでメソッドが選択できないからです。）  
そしたらButton1~3を選択し、クリック時()の＋を押し、オブジェクトにEvevtSystemを入れ、NoFunctionの場所はSwitchPlaneを選択。  
![スクリーンショット (74)](https://user-images.githubusercontent.com/82185511/176584198-0d8cdd0b-2e31-4b6b-83fc-76a2d87ed68b.png)  


最後に、それぞれのボタンを再度選択しなおし、それぞれのPlaneオブジェクトを入れます。  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 12_08_12](https://user-images.githubusercontent.com/82185511/176584591-cccbb548-7b0e-4a6e-a4dd-ee40817f2ea1.png)  
・  
・  
・  
ではプレイしてみましょう。  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 12_13_31](https://user-images.githubusercontent.com/82185511/176587148-21653347-fa32-40a6-a930-5c4ea02799c9.png)  
左下のボタンを押せば左の平面が消え、右下のボタンを押せば右の平面が消え……というようになりましたね。  


### ②フラグ管理してみよう
さて、次はそれぞれの平面が現れているのか消えているのかを変数に記録します。フラグ管理です。jsonに書き込むためにシリアライズ化というのが必要です。そのため、シリアライズ化する`PlayerData`というクラスを作り、その中でフラグとなる変数を宣言していきます。  
以下のようにスクリプトに追記します。
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlaneSaveLoad : MonoBehaviour
{
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

//追記開始
    [System.Serializable]
    public class PlayerData
    {
        public string flagToSave;
    }
    PlayerData myData = new PlayerData();
//追記終了
}
```
もしご自身でbool値やint値を変数として扱いたい場合、`PlayerData`内にどんどん追加していけば良い、というわけですね。…………。  

ここで勘のいい方はお気づきでしょうが、そうです、今回3つの平面があるのにフラグは1つしか定義していません。もちろん、それぞれの平面がアクティブかどうかを「plane1Active = true」というようなbool値で管理してもよいのです。しかし、もし平面を100個に増やした場合、bool値も100個になり、管理するのがあまりにも大変になります。ですから、配列というオシャンティなものを使い、すべてのフラグを１つのstring型にまとめようという訳です。「配列？難しいやろが！」という方、わかりやすく説明するのでどうぞお付き合いください。  


では続きです。スクリプトに２つの新しい変数の定義と、`OnClickToSave`というメソッドを追記していきます。
メソッド内ではPlane1~3が入ったGameObject型の配列から１つずつplaneオブジェクトを取り出し、それがアクティブかどうかをチェックします。それを1か0で判断し、string型として1文字ずつ記録していきます。
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSaveLoad : MonoBehaviour
{
//追記開始
    [SerializeField] GameObject[] planes = new GameObject[3];//インスペクター上でplane1~3を入れてください
    string flagStr;//flagStrToSave（シリアライズ化してjsonに書き込むための変数）に値を送るための変数
//追記終了

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

//追記開始
    public void OnClickToSave()
    {
        flagStr = "";//初期化
        for (int i = 0; i < planes.Length; i++)//「iというint型の変数を定義します、これを0から」;「配列の要素の数を超えるまで」;「1ずつ増やしていきます」
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
    }
//追記終了
}
```
コメントでそれぞれの文の意味を注釈しておきましたが、念のため何をしているのかもう一度日本語で書いておきます。  
インスペクター上で入れられたplane1~3を、for文で順番にアクティブかそうでないかを確認し、0か1をflagStrという変数の末尾に入れていく、ということが行われます。  

さて、このスクリプトを動かすためにEventSystemオブジェクトのインスペクターからそれぞれplane1~3を入れてください。  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 14_18_55](https://user-images.githubusercontent.com/82185511/176599010-984daf17-aed6-431a-ba6f-b196ecff37b4.png)  
スクショは省きますが、**Saveボタンのクリック時()にこのOnClickSaveメソッドをあてがう**のを忘れないでくださいね。  

ゲームをプレイしてみると、なんとそれぞれの平面の状態が3ケタの数字で記録されているのがログから確認できます！！！  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 14_30_15](https://user-images.githubusercontent.com/82185511/176600118-18e4da52-c20c-4246-8476-b728f95821f0.png)  

### ③セーブしてみよう
いよいよフラグ管理のための変数をjsonファイルに記述するためのコードを書いていきます。  
……といっても簡単なもので、先頭にusing、`OnClickToSave()`に数行を追記するだけです。 
```
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//追記開始
using System.IO;
//追記終了

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
        for (int i = 0; i < planes.Length; i++)//「iというint型の変数を定義します、これを0から」;「配列の要素の数を超えるまで」;「1ずつ増やしていきます」
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

//追記開始
        StreamWriter writer;
        myData.flagStrToSave = flagStr;//ここでflagStrToSaveにflagStrの文字を渡す

        string jsonStr = JsonUtility.ToJson(myData);
        Debug.Log("jsonstrは今" + jsonStr + "です");

        writer = new StreamWriter(Application.dataPath + "/save.json", false);
        writer.Write(jsonStr);
        writer.Flush();
        writer.Close();
//追記終了
    }
}
```
「using System.IO」はjson方式でのセーブに必須みたいです。  
ここまでの流れ、わかりましたでしょうか？`PlayerData`クラス内で宣言した`flagStrToSave`に、処理した`flagStr`を入れる。  
ご自身でセーブシステムを制作する際は、注釈した部分の`myData.flagStrToSave = flagStr;`を、その前段階で処理した変数で置き換えればよいという訳ですね。  


さて、プレイしてみましょう。平面を消して、Saveボタンを押すと……  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 15_56_23](https://user-images.githubusercontent.com/82185511/176612988-8be2a930-3f85-4f1d-a841-d8652215571c.png)  
コンソールには３ケタの変数と、jsonStrの中身が表示されます。そして、このUnityプロジェクトのファイルをエクスプローラーから見てみると……  
![Assets 2022_06_30 15_55_38](https://user-images.githubusercontent.com/82185511/176614752-6227d88d-584e-41b5-9808-210fcf79d4f9.png)  
はい。さっきまでなかったjsonファイルが追加されてます。🐵

### ④ロードしてみよう
じゃあロード機能もちゃちゃっと実装しちゃいましょう。
`OnClickToLoad`というメソッドを追加して、以下のように書いてください。
```
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
        for (int i = 0; i < planes.Length; i++)//「iというint型の変数を定義します、これを0から」;「配列の要素の数を超えるまで」;「1ずつ増やしていきます」
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
    
//追記開始
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
//追記終了
}
```
「サルでもわかるって言ったやないか！まったくわからんわ！」という怒号が飛んできそうですね。落ち着いてください。やっていることはセーブの逆です。セーブの手順は、1文字ずつ文字を格納していって、jsonファイルに書き込んでますよね。ロードではこの逆を行います。  
まずはjsonファイルにある文字列を`dataStr`という変数に読み込みます。次にfor文でまた一文字ずつ1か0か確認して、Plane1~3のアクティブと非アクティブを設定します。  
なお`if (File.Exists(Application.dataPath + "/save.json"))`というif文では、jsonファイルがまだ生成されていない、つまり一度もSaveしたことのない状態でのみ起こるnullチェックをしています。え？どういうことかって？では早速Loadボタンに`OnClickToLoad`メソッドをあてがい、プレイしてみましょう。  


今まで通りプレイしてもらうと、セーブした状態の平面がロードで復活するようになっています。  
ではjsonファイル（とそのmetaファイル）を消して、未プレイ時と同じ状態にしてみましょう。  
![Assets 2022_06_30 17_11_01](https://user-images.githubusercontent.com/82185511/176627153-bb7cd747-3e70-4c3c-a483-767ef812ce95.png)  
この状態で再度Loadボタンを押してみましょう。すると……  
![PlaneTest - SampleScene - PC, Mac   Linux Standalone - Unity 2020 3 29f1 Personal_ _DX11_ 2022_06_30 17_15_23](https://user-images.githubusercontent.com/82185511/176627979-3801da4e-c17b-4c3e-9136-283fec6f936e.png)  
「セーブデータはありません。」とコンソールに表示されます！これで完成です！！  
・  
・  
・  
ビルドしてもjsonでのセーブシステムは正常に動きますよ。ただ注意なのが、プラットフォームによって`Application.dataPath`の部分を変えた方がいいということです。Standalone（パソコンで動かす）ならこのままで大丈夫ですが、iOSでは`Application.persistentDataPath`、Androidではまた違ったパスがふさわしいようなので、プラットフォームごとに調べながらよさげな場所を指定した方がよさそうです。

## さいごに
「サルでもわかる」の謳い文句につられたあなたは不覚にもfor文を覚えてしまった訳ですが、今回のセーブ方式には良いところがあります。Saveで作成したjsonファイルを開いてみましょう。  
`{"flagStrToSave":"111"}`  
このように、我々はゲームの制作者ですからこの数字が意味するところは分かりますが、勘の悪いプレイヤーが見れば「111？なんのこっちゃ？」と思うことでしょう。つまり、セーブの変数をもっと複雑にすれば、jsonファイルをいじられてセーブデータを改造される恐れがなくなる、いわゆる**暗号化**処理が行えます。  

いかがでしたでしょうか。もしわからないところがあればコメントして頂ければ答えらえる範囲でお答えいたします。ではまた。🐒  
