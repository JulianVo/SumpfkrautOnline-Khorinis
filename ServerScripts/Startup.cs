using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GUC.Server.WorldObjects;
using GUC.Server.Log;
using GUC.Server.Scripting.Listener;

namespace GUC.Server.Scripts
{
	public class Startup : IServerStartup
	{
		public void OnServerInit()
		{
            Log.Logger.log("######################## Initalise ########################");

            Animations.AniCtrl.InitAnimations();
            //DefaultItems.Init();
            

            //DayTime.Init();
            //DayTime.setTime(0, 12, 0);

#if SSM_AI
            //AI.AISystem.Init();
#endif
            //DefaultWorld.Init();


            

            //DamageScript.Init();

            

#if SSM_CHAT
            //chat = new Chat();
            //chat.Init();

            //important: register notification types for notification areas!
            /*NotificationManager.GetNotificationManager().AddNotificationArea(100, 100, 50, 8,
              new NotificationType[] { NotificationType.ChatMessage, NotificationType.ServerMessage,
                NotificationType.PlayerStatusMessage, NotificationType.MobsiMessage, NotificationType.Sound });*/
            //CommandInterpreter.GetCommandInterpreter();
            //Chat.GetChat();
            //EventNotifier.GetEventNotifier();

      

#endif

            

            //Modules.Init();



            //Modules.addModule(new Test.ListTestModule());


#if SSM_ACCOUNT
            //AccountSystem accSystem = new AccountSystem();
            //accSystem.Init();
#endif
            
#if SSM_WEB
            //Web.http_server.Init();
#endif

            //Sumpfkraut.SOKChat.SOKChat SOKChat = new Sumpfkraut.SOKChat.SOKChat();

            //Server.Sumpfkraut.Trade trade = new Server.Sumpfkraut.Trade();

            Instances.VobInstances.Init();

            Accounts.AccountSystem.Get(); //init

            DamageScript.Init();

            CmdHandler.Init();

            //Server.Network.Messages.AnimationMenuMessage.Init();
            
            Logger.log(Logger.LogLevel.INFO, "###################### End Initalise ######################");
		}
    }
}
