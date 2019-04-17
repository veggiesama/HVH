Hey!

Here is how to make any of your scripts a singleton using Singleton Perfect:


	
	:: First, Inherit from Singleton class like so,
	public class YOURTYPE : Singleton<YOURTYPE> {}
	
	:: Second, include this variable so you can access the instance of your singleton.
	public static YOURTYPE Instance {
		get {
			return ((YOURTYPE)mInstance);
		} set {
			mInstance = value;
		}
	}
	
	:: Third, Voila! Now you can access the instance of your singleton with YOURTYPE.Instance 





Why use Singleton Perfect?

:: It's really easy to make anything a Singleton!

:: It handles nasty situations for you! Have multiple instances in a scene by accident? It'll remove the unnecessary ones and alert you!

:: It won't destroy when you load a new scene.

:: If you reference the Instance at all, it will create itself if it doesn't exist! It'll even name itself appropriately! That's something your mom doesn't teach you!


Don't waste time rewriting the same code... Just use Singleton Perfect... It's free.



P.S.
Singleton Perfect is used in SoundManagerPro and SoundManagerPro Free! available on the Unity Asset Store.  Check them out!
It's also used in countless games on the AppStore and Google Play.  Join the crowd.