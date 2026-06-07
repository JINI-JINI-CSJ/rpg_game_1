using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using UnityEngine.Purchasing;
//using UnityEngine.Store;


//public class SJ_IAPBase : SJ_Singleton_Mono , IStoreListener
//{
//	static	public	SJ_IAPBase	g;
//	public override SJ_Singleton_Mono OnGetStatic(){return g;}
//	public override void OnSetStatic(SJ_Singleton_Mono s){g = s as SJ_IAPBase;}


//	[System.Serializable]
//	public	class _Product_Inf
//	{
//		public	string		ID;
//		public	string		Name;
//	}
//	public	List<_Product_Inf>	lt_Product_Inf;

//	public	IStoreController	storeController;
//	public	IExtensionProvider	extensionProvider;
//	//public	ITransactionHistoryExtensions transactionHistoryExtensions;

//	public	GameObject	go_recv;


//	public	bool	Check_Init()
//	{ 
//		return storeController != null && extensionProvider != null;
//	}

//	static	public	bool	Init( GameObject _recv )
//	{
//		return g._Init(_recv);
//	}

//	public	bool	_Init(GameObject _recv )
//	{
//		g.go_recv = _recv;
//		if(	Check_Init() ) return true;

		

//		var module	= StandardPurchasingModule.Instance();
//		var builder = ConfigurationBuilder.Instance(module);
//		foreach( _Product_Inf s in lt_Product_Inf )
//		{
//			builder.AddProduct( s.ID , ProductType.Consumable , new IDs{{ s.Name , GooglePlay.Name } } );
//		}
//		UnityPurchasing.Initialize( this , builder );
//		return true;
//	}

//	static	public	bool	Buy( string ID , GameObject recv )
//	{ 
//		g.go_recv = recv;
//		var prd = g.storeController.products.WithID( ID );
//		if( prd == null )
//		{
//			return false;
//		}
//		g.storeController.InitiatePurchase( prd );
//		return true;
//	}

//	public	void OnInitialized(IStoreController controller, IExtensionProvider extensions)
//	{
//		storeController = controller;
//		extensionProvider = extensions;
//		foreach( _Product_Inf s in lt_Product_Inf )
//		{
//			var prd = storeController.products.WithID( s.ID );
//			if( prd == null || prd.availableToPurchase == false )
//			{
//				// error
//				Debug.LogError( "Error! OnInitialized : " + s.ID );
//				SJ_Unity.SendMsg( go_recv , "OnIAP_Error_Product" , s.ID );
//			}
//		}
//	}
//	public	void OnInitializeFailed(InitializationFailureReason error)
//	{
//		Debug.LogError( "Error! OnInitializeFailed : " + error.ToString() );
//		SJ_Unity.SendMsg( go_recv , "OnIAP_Error_Initialize" , error );
//	}
//	public	void OnPurchaseFailed(Product i, PurchaseFailureReason p)
//	{
//		Debug.LogError( "Error! OnPurchaseFailed : " + p.ToString() );
//		SJ_Unity.SendMsg( go_recv , "OnIAP_PurchaseFailed" , p );
//	}
//	public	PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
//	{
//		Debug.LogError( "ProcessPurchase : " + e.ToString() );
//		SJ_Unity.SendMsg( go_recv , "ProcessPurchase" , e );
//		return PurchaseProcessingResult.Complete;
//	}


//}
