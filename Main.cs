/*
 * 
 */
using System;
using LSPD_First_Response.Mod.API;
using Rage;

namespace OMD
{
	/// <summary>
	/// Description of Main.
	/// </summary>
	public class Main : Plugin
	{
		
		public static OMD.omd.utils.FileLogger Logger = new OMD.omd.utils.FileLogger("OMD", OMD.omd.utils.FileLogger.LogLevel.TRACE);
		
		public Main()
		{
			// Do nothing
		}
		
		public override void Initialize()
		{
			Functions.OnOnDutyStateChanged += OnOnDutyStateChangedHandler;
			Game.LogTrivial("OhMyDispatch " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
			OMD.Main.Logger.Info("OhMyDispatch " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString() + " has been initialised.");
			Game.DisplayNotification("~b~OhMyDispatch~s~ has been initialised.");
		}
		
		public override void Finally()
		{
			Game.LogTrivial("OhMyDispatch has been cleaned up.");
			OMD.Main.Logger.Info("OhMyDispatch has been cleaned up.");
		}
		
		private static void OnOnDutyStateChangedHandler(bool OnDuty)
		{
			if (OnDuty) {
				StartPlugin();
			}
		}
		
		private static void StartPlugin()
		{
			Game.LogTrivial("OhMyDispatch is starting...");
			OMD.Main.Logger.Info("OhMyDispatch is starting...");
			Game.DisplayNotification("~b~OhMyDispatch~s~ is starting...");
			
			GameFiber.StartNew(delegate{
	           	
			    bool IsRunning = true;
	           	Game.LogTrivial("Delegate thread.");
				OMD.Main.Logger.Info("Delegate thread.");
	           	
	           	try {
					
					// Getting the configuration
					OMD.omd.utils.IniFile iniFile = new OMD.omd.utils.IniFile("plugins/LSPDFR/OMD.ini");
					
	           		OMD.omd.domain.OhMyDispatch ohMyDispatch = new OMD.omd.domain.OhMyDispatch(iniFile);
	           		
	           		while (IsRunning)
		           	{
		           		GameFiber.Yield();
		           		try {
		           			ohMyDispatch.process();
			           	} catch (Exception e) {
			           		Game.LogTrivial("Got error on OhMyDispatch process... (" + e.Message + ")");
							OMD.Main.Logger.Error("Got error on OhMyDispatch process... (" + e.Message + ")", e);
							Game.DisplayNotification("Got an ~r~error~s~ with ~b~OhMyDispatch~s~");
							IsRunning = false;
			           	}
		           	}
	           		
	           	} catch (Exception e) {
	           		Game.LogTrivial("Got error on OhMyDispatch creation... (" + e.Message + ")");
					OMD.Main.Logger.Error("Got error on OhMyDispatch creation... (" + e.Message + ")", e);
					Game.DisplayNotification("Got an ~r~error~s~ when starting ~b~OhMyDispatch~s~");
	           	}
	           	
           });
			
		}
	}
}