

////////////////////
/// Time method. ///
////////////////////


using System;
using UnityEngine;


namespace AC.LSky
{

	public abstract class LSkyTime : MonoBehaviour 
	{


		public    bool  playTime      = true; // Progress time.
		public    float dayInSeconds  = 900;  // 60*15 = 900 (15 minutes).
		protected int   k_HoursPerDay = 24;   
		//-------------------------------------------------------------------

		[Range(0.0f, 24f)] public float timeline = 7.0f;
		//-------------------------------------------------------------------

		protected virtual void ProgressTime()
		{

			timeline = Mathf.Repeat(timeline, k_HoursPerDay);

			// Add time in timeline.
			if (playTime && Application.isPlaying && dayInSeconds != 0)
			{
				timeline += (Time.deltaTime / dayInSeconds) * k_HoursPerDay; 
			}
		}
		//-------------------------------------------------------------------
	}
}