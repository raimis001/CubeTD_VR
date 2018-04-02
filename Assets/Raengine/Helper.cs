#define ANATOMY_ANALYTICS1

using System;
using UnityEngine;

namespace Simulator.Scripts
{
	public static class Helper
	{
		public static void Reset(this Transform transform)
		{
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.zero;
			transform.localScale = Vector3.one;
		}

		public static void Reset(this Transform transform, Transform position)
		{
			transform.localEulerAngles = position.localEulerAngles;
			transform.localPosition = position.localPosition;
			transform.localScale = position.localScale;
		}

		public static string join(this string[] array, string delimiter, int startIndex = 0)
		{
			string result = "";
			for (int i = startIndex; i < array.Length; i++)
			{
				if (!String.IsNullOrEmpty(result)) result = result + delimiter;
				result = result + array[i];
			}

			return result;
		}

//		public static bool IsInternetReachable()
//		{
//			if (Application.internetReachability == NetworkReachability.NotReachable)
//			{
//				return false;
//			}
//
////			try
////			{
////				using (var client = new WebClient())
////				{
////					using (client.OpenRead("https://www.google.com"))
////					{
////						return true;
////					}
////				}
////			}
////			catch
////			{
////				return false;
////			}
//			return true;
//		}

		public static Vector3 ClampAngle(Vector3 angle)
		{
			Vector3 res = angle;
			res.x = ClampAngle(res.x);
			res.y = ClampAngle(res.y);
			res.z = ClampAngle(res.z);
			return res;
		}

		public static float ClampAngle(float angle)
		{
			if (angle < 0) angle = angle + 360f * (int) (-angle / 360f + 1);
			if (angle > 360) angle = angle - 360f * (int) (angle / 360f);

			return angle;
		}

		public static Vector3 RotateAroundPivot(this Vector3 point, Vector3 pivot, Vector3 angles)
		{
			Vector3 dir = point - pivot;
			dir = Quaternion.Euler(angles) * dir;
			point = dir + pivot;
			return point;
		}
	}

	public static class Log
	{
		public static bool devMode =
#if UNITY_EDITOR
			true;
#else
        false;
#endif

		public static bool force = false;
		private static LogType handleLog = LogType.Exception;


		public static LogType HandleLog
		{
			set
			{
				handleLog = value;
				Application.logMessageReceived += HandleLogF;
			}
		}

		static void HandleLogF(string logString, string stackTrace, LogType type)
		{

			if (logString.Contains("Failed to load")) return;
			if ((int) type > (int) handleLog) return;

			file(logString);
		}

		public static void l(object[] message, string delimiter = ";")
		{
			string result = "";
			foreach (object s in message)
			{
				result += (!string.IsNullOrEmpty(result) ? delimiter : "") + s;
			}

			Debug.Log("array: " + result);
		}

		public static void f(string message, params object[] args)
		{
			Debug.Log(string.Format(message, args));
		}

		public static void r(string message, params object[] args)
		{
			Debug.Log("ra001: " + string.Format(message, args));
		}

		public static void a(string message, params object[] args)
		{
			string m = string.Format(message, args);
			Debug.Log("analytic: " + m);
#if ANATOMY_ANALYTICS
		Analytics.SendLog(EventKind.Info,m);
#endif
		}

		public static void e(string message, params object[] args)
		{
			Debug.LogError(string.Format(message, args));
		}

		public static void w(string message, params object[] args)
		{
			Debug.LogWarning(string.Format(message, args));
		}

		public static void file(string message, params object[] args)
		{
			file(string.Format(message, args));
		}

//		public static void file(string message)
//		{
//			if (!force && !devMode) return;
//			Debug.Log("file: " + message);
//			string filePath = "unity.txt";
////			using (System.IO.StreamWriter fileWriter = new System.IO.StreamWriter(filePath, true))
////			{
////				fileWriter.AutoFlush = true;
////				fileWriter.WriteLine("[{0:yyyy.MM.dd HH.mm.ss}]{1}", DateTime.Now, message);
////				fileWriter.Close();
////			}
//
//			force = false;
//		}


		public static void t(string message, float startTime, params object[] args)
		{
			TimeSpan ts = TimeSpan.FromSeconds(Time.time - startTime);
			string tss = message + string.Format(" time:{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
			f(tss, args);
		}

		public static void tr(string message, float startTime, params object[] args)
		{
			TimeSpan ts = TimeSpan.FromSeconds(Time.time - startTime);
			string tss = message + string.Format(" time:{0:00}:{1:00}:{2:00}", ts.Minutes, ts.Seconds, ts.Milliseconds);
			r(tss, args);
		}
	}
}