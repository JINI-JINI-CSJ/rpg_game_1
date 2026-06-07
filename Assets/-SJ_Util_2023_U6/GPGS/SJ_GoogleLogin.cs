// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using GooglePlayGames.BasicApi; //conf
// using GooglePlayGames;  //platfom
// using GooglePlayGames.BasicApi.SavedGame;
// using System;
// using System.Text;
// //using Newtonsoft.Json;
// using System.IO;

// // 안드로이드 빌드 아니면 에러

// public class SJ_GoogleLogin : MonoBehaviour
// {


//     public bool         noGoogleLogin;
//     public bool         state_init;
//     public int          state_login;

//     public Action<int>  OnEndLogin;
//     public Action<int>  OnEndSave;
//     public Action<int>  OnEndLoad;

//     private bool isSaving;
//     public string FILE_NAME = "default_save.bin";
//     public string   save_json_data = "";
//     public byte[]   save_bin_data = null;

//     public void     Init( Action<int> func_login ,  Action<int> func_save = null , Action<int> func_load = null )
//     {
//         if( state_init ) return;
//         state_init = true;

//         OnEndLogin = func_login;
//         if( func_save != null )OnEndSave += func_save;
//         if( func_load != null )OnEndLoad += func_load;           

// #if UNITY_ANDROID 

//         // GPGS 플러그인 설정
//         PlayGamesClientConfiguration config = new PlayGamesClientConfiguration
//             .Builder()
//             .EnableSavedGames()
//             .RequestServerAuthCode(false)
//             //.RequestEmail() // 이메일 권한을 얻고 싶지 않다면 해당 줄(RequestEmail)을 지워주세요.
//             .RequestIdToken()
//             .Build();
//         //커스텀 된 정보로 GPGS 초기화
//         PlayGamesPlatform.InitializeInstance(config);
//         PlayGamesPlatform.DebugLogEnabled = true; // 디버그 로그를 보고 싶지 않다면 false로 바꿔주세요.
//         //GPGS 시작.
//         PlayGamesPlatform.Activate();
// #endif
//     }


//     public void GPGSLogin()
//     {
//         if( noGoogleLogin )
//         {
//             state_login = 1;
//             //func_login_OK.Func();
//             OnEndLogin(1);
//             return;
//         }

// #if UNITY_ANDROID
//         if( state_login == 1 ) 
//         {
//             //func_login_OK.Func();
//             OnEndLogin(1);
//             return;
//         }

//         // 이미 로그인 된 경우
//         if (Social.localUser.authenticated == true)
//         {
//             state_login = 1;        
//             //func_login_OK.Func();
//             OnEndLogin(1);

//         }
//         else
//         {
//             Social.localUser.Authenticate((bool success) => {
//                 if (success)
//                 {
//                     state_login = 1;
//                     //LoadData();
//                     Debug.Log("Login success !!!");
//                     OnEndLogin(1);

//                     LoadData();
//                 }
//                 else
//                 {
//                     state_login = -1;
//                     // 로그인 실패
//                     Debug.Log("Login failed for some reason~~~~");
//                     OnEndLogin(0);
//                 }
//             });
//         }
// #else
//         state_login = 1;
//         OnEndLogin(1);
// #endif

//     }

//     // 구글 토큰 받아옴
//     public string GetTokens()
//     {
// #if UNITY_ANDROID

//         if (PlayGamesPlatform.Instance.localUser.authenticated)
//         {
//             // 유저 토큰 받기 첫 번째 방법
//             string _IDtoken = PlayGamesPlatform.Instance.GetIdToken();
//             // 두 번째 방법
//             // string _IDtoken = ((PlayGamesLocalUser)Social.localUser).GetIdToken();
//             return _IDtoken;
//         }
//         else
//         {
//             Debug.Log("접속되어 있지 않습니다. PlayGamesPlatform.Instance.localUser.authenticated :  fail");
//             return "";
//         }
// #else
//         return "";
// #endif
//     }


//     string getFileName( string fileName )
//     {
//         if( string.IsNullOrEmpty(fileName) == false ) return fileName;
//         return FILE_NAME;
//     }


// #region 저장
//     public void SaveData( string fileName = "" )
//     {
        

//         if (Social.localUser.authenticated)
//         {
//             Debug.Log( "SaveData 1");
//             this.isSaving = true;

//             ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;
            
//             // 데이터 접근
//             saveGameClient.OpenWithAutomaticConflictResolution(getFileName(fileName),
//                 DataSource.ReadCacheOrNetwork,
//                 ConflictResolutionStrategy.UseLastKnownGood,
//                 onsavedGameOpend);

//             Debug.Log( "SaveData 2");
//         }
//         else
//         {
//             this.SaveLocal();
//         }
//     }
 
//     private void onsavedGameOpend(SavedGameRequestStatus status, ISavedGameMetadata game)
//     {
//         ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;
//         if (status == SavedGameRequestStatus.Success)
//         {
//             var update = new SavedGameMetadataUpdate.Builder().Build();

//             //json
//             var json = JsonUtility.ToJson("저장하려는 데이터!");
//             byte[] data = Encoding.UTF8.GetBytes(json);
            
//             // 저장 함수 실행
//             saveGameClient.CommitUpdate(game, update, data, OnSavedGameWritten);
//         }
//         else
//         {
//             Debug.Log("Save No.....");
//         }
//     }

//     // 저장 확인 
//     private void OnSavedGameWritten(SavedGameRequestStatus status, ISavedGameMetadata data)
//     {
//         if (status == SavedGameRequestStatus.Success)
//         {
//         // 저장완료부분
//             Debug.Log("Save End");
//             OnEndSave(1);
//         }
//         else
//         {
//             Debug.Log("Save nonononononono...");
//             OnEndSave(0);
//         }
//     }

//     private void SaveLocal()
//     {
//         File.WriteAllText( "inGame_save.txt" , save_json_data );
//     }

//     #endregion
 
//     #region 불러오기 
//     public void LoadData(string fileName = "")
//     {
//         if (Social.localUser.authenticated)
//         {
//             this.isSaving = false;
//             ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;
//             saveGameClient.OpenWithAutomaticConflictResolution(getFileName(fileName),
//                 DataSource.ReadCacheOrNetwork,
//                 ConflictResolutionStrategy.UseLastKnownGood,
//                 LoadGameData);
//         }
//         else
//         {
//             this.LoadLocal();
//         }
//     }

//     private void LoadGameData(SavedGameRequestStatus status, ISavedGameMetadata data)
//     {
//         ISavedGameClient saveGameClient = PlayGamesPlatform.Instance.SavedGame;
//         if (status == SavedGameRequestStatus.Success)
//         {
//             Debug.Log("!! GoodLee");
//             // 데이터 로드
//             saveGameClient.ReadBinaryData(data, onSavedGameDataRead);
//         }
//         else
//         {
//             Debug.Log("?? no");
//             OnEndLoad(0);
//         }
//     }

// 	// 불러온 데이터 처리 
//     private void onSavedGameDataRead(SavedGameRequestStatus status, byte[] loadedData)
//     {
//         string data = System.Text.Encoding.UTF8.GetString(loadedData);
//         if (data == "")
//         {
//             //SaveData();
//             OnEndLoad(0);
//         }
//         else
//         {
// 			// 불러온 데이터를 따로 처리해주는 부분 필요!
//             save_json_data = data;
//             OnEndLoad(1);
//         }
//     }
 
//     private void LoadLocal()
//     {
//         if( File.Exists( "inGame_save.txt" ) )
//         {
//             string str = File.ReadAllText("inGame_save.txt");
//             if( string.IsNullOrEmpty(str) == false )
//             {
//                 //json = JSON.Parse( str ) as JSONClass;
//                 save_json_data = str;
//                 OnEndLoad(1);
//             }
//         }
//         OnEndLoad(0);
//     }
 
//     private string GameInfoToString()
//     {
//         return save_json_data;
//     }

//     #endregion

// }
