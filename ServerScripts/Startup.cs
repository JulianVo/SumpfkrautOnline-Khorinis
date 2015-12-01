using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using GUC.Server.WorldObjects;
using GUC.Server.Log;
using GUC.Server.Scripting.Listener;
using GUC.Server.Scripts.Sumpfkraut.VobSystem;
using GUC.Server.Scripts.Sumpfkraut.VobSystem.Definitions;
using GUC.Server.Scripts.Sumpfkraut.VobSystem.Instances;
using GUC.Server.Scripts.Sumpfkraut;

namespace GUC.Server.Scripts
{
	public class Startup : IServerStartup
	{

        //public delegate void MyEventHandler (Sumpfkraut.Utilities.Threading.Runnable runnable);
        //public MyEventHandler MyEvent;

		public void OnServerInit()
		{


            //Type bla = typeof(ScriptObject);
            ////Console.WriteLine(bla.GetProperty("_staticName"));
            ////Console.WriteLine(bla.GetProperty("_staticName").GetGetMethod());
            //Console.WriteLine(bla.GetMembers().Length);
            //Console.WriteLine(bla.GetMember("_staticName").Length);
            //Console.WriteLine(bla.GetMember("_staticName").GetValue(0));
            //System.Reflection.MemberInfo[] mInfo = bla.GetMember("_staticName");

            //for (int i = 0; i < mInfo.Length; i++)
            //{

            //    Console.WriteLine(mInfo[i]);
            //}

            //object temp;
            //int i = 0;
            //while (true)
            //{
            //    try
            //    {
            //        temp = mInfo.GetValue(i);
            //        Console.WriteLine(temp.GetType());
            //        Console.WriteLine(temp);
            //        Console.WriteLine("--------------------------");
            //    } catch
            //    {
            //        break;
            //    }
            //    i++;
            //}


            //Type type = typeof(ScriptObject); // MyClass is static class with static properties
            //foreach (var p in type.GetProperties())
            //{
            //    var v = p.GetValue(null, null); // static classes cannot be instanced, so use null...
            //    Console.WriteLine(v.GetType());
            //    Console.WriteLine(v);
            //    Console.WriteLine("--------------------------");
            //}

            //Type t = typeof(ScriptObject);
            //System.Reflection.PropertyInfo prop = t.GetProperty("_staticName");
            //object value = prop.GetValue(null, null);
            //Console.WriteLine(value);

            //Console.WriteLine(t.GetField("_staticName"));
            //Console.WriteLine(t.GetField("_staticName").GetValue(""));
            //Console.WriteLine(typeof(ScriptObject).GetField("_staticName").GetValue(""));
            //Console.WriteLine(typeof(VobHandler).GetField("_staticName").GetValue(""));

            //VobDef def = new VobDef();
            //def.Print("Hello World!");

            //VobDef.PrintStatic(typeof(VobHandler), "GOTCHA!");


            //ItemDef.MakeLogErrorStatic(typeof(ItemDef), 
            //    String.Format("Prevented attempt of adding a definition to the dictionaries:"
            //        + " The {0} {1} is already taken.", "id", 9999));

            //Func<bool, bool, bool> check = (x, y) => x && y;
            //Console.WriteLine(check.Invoke(true, false));

            //ItemDef def0 = new ItemDef(); def0.setId(0); ItemDef.Add(def0);
            //ItemDef def1 = new ItemDef(); def1.setId(1); ItemDef.Add(def1);
            //ItemDef def2 = new ItemDef(); def2.setId(2); ItemDef.Add(def2);
            //ItemDef def2_ = new ItemDef(); def2_.setId(2); ItemDef.Add(def2_);

            //ItemInstance ii = new ItemInstance("Schmarn");
            //ii.Name = "Schmarn (Ya mei!)";
            //ii.Material = Enumeration.ItemMaterial.Metal;
            //ii.Type = Enumeration.ItemType.Food_Small;
            //ii.Weight = 99;
            //ii.Description = String.Format("Lorem ipsum dolor sit amet, consectetur adipiscing elit. "
            //    +"Sed faucibus magna sem, at lobortis magna dignissim ac. " 
            //    + "Suspendisse vitae augue ultrices velit suscipit pellentesque et quis ligula. "
            //    + "Duis vitae pharetra nisl. Aenean id sollicitudin ligula, ut mattis arcu. "
            //    + "Maecenas varius euismod ornare. Etiam id leo sodales, facilisis leo ac, "
            //    + "tincidunt enim. Nulla eget efficitur sapien, eu egestas dolor. Nullam fermentum "
            //    + "tincidunt massa, non tempus magna lacinia a. In feugiat vel risus ac vestibulum.");
            //ii.Visual = "ItFo_FishSoup.3ds";
            //Item item = new Item(ii);
            //item.Amount = 9001;
            //item.Position = new GUC.Types.Vec3f(0.0f, 0.0f, 0.0f);
            ////item.CDDyn = false;
            ////item.CDStatic = true;
            ////World.NewWorld.ItemDict.Add(1, item);
            ////item.Spawn(World.NewWorld);
            //item.Drop(World.NewWorld, item.Position);

            //IGTime newTime = new IGTime();
            //newTime.day = 4; newTime.hour = 22; newTime.minute = 30;
            //Console.WriteLine(">>>> " + newTime.day + " " + newTime.hour + " " + newTime.minute);
            //World.NewWorld.ChangeTime(newTime.day, newTime.hour, newTime.minute);
            //Console.WriteLine(String.Format(">>> CHANGED TIME to day {0} {1}:{2}<<<",
            //    World.NewWorld.GetIGTime().day, 
            //    World.NewWorld.GetIGTime().hour, 
            //    World.NewWorld.GetIGTime().minute));


            //Sumpfkraut.Utilities.Threading.TestRunnable timeRunner =
            //    new Sumpfkraut.Utilities.Threading.TestRunnable(false, new TimeSpan(0, 0, 2), false);
            //timeRunner.SetObjName("timeRunner");
            //timeRunner.printStateControls = true;
            //timeRunner.Start();


            //Sumpfkraut.WeatherSystem.Weather weather_1 =
            //    new Sumpfkraut.WeatherSystem.Weather(false, "MyWeather");
            //weather_1.Start();


            //Dictionary<int, VobDef> myDict = new Dictionary<int, VobDef>();
            //ItemDef myDef = new ItemDef();
            //myDef.setEffect("myEffect");
            //myDict.Add(1, myDef);
            //ItemDef myDef_ = (ItemDef)myDict[1];
            //Console.WriteLine(">>> " + myDef_.getEffect());


            //ItemDef def1 = new ItemDef();
            //def1.SetId(1);
            //def1.SetCodeName("myCodeName");
            //def1.SetEffect("myEffect");
            //ItemDef.Add(def1);
            //ItemDef def1_;
            //if (ItemDef.TryGetValueById(1, out def1_))
            //{
            //    ItemDef.MakeLogStatic(typeof(ItemDef), "GOTCHA!");
            //    ItemDef.MakeLogStatic(typeof(ItemDef), def1 == def1_);
            //    ItemDef.MakeLogStatic(typeof(ItemDef), def1_.GetEffect());
            //}

            //List<String> commandQueue = new List<string> { "SELECT 1; SELECT 11; ", "SELECT 2;", "SELECT 3;" };
            //Sumpfkraut.Database.DBAgent agentBlack = new Sumpfkraut.Database.DBAgent(commandQueue, false);
            //agentBlack.ReceivedResults += delegate(object sender, 
            //    Sumpfkraut.Database.DBAgent.ReceivedResultsEventArgs e) 
            //{
            //    List<List<List<object>>> results = e.GetResults();
            //    Console.WriteLine("+++ " + results[0][0][0]);
            //};
            //agentBlack.FinishedQueue += delegate (object sender)
            //{
            //    Console.WriteLine("+++ FINISHED");
            //};

            //Sumpfkraut.Utilities.Threading.Runnable homeRun =
            //    new Sumpfkraut.Utilities.Threading.Runnable(false, new TimeSpan(0, 0, 0, 1), false);
            //homeRun.printStateControls = true;
            //homeRun.OnInit += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) { Console.WriteLine("Inuit 1"); };
            ////homeRun.OnInit += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) { Console.WriteLine("Inuit 2"); };
            //homeRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) { Console.WriteLine("Task 1"); };
            ////homeRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) { Console.WriteLine("Task 2"); };
            //homeRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable)
            //{
            //    runnable.Print("-----> " + (DateTime.Now.Second % 10));
            //    if ((DateTime.Now.Second % 10) == 0)
            //    {
            //        runnable.Suspend();
            //    }
            //};
            //homeRun.Start();
            ////homeRun.Suspend();



            //Sumpfkraut.Utilities.Threading.Runnable homeRun =
            //    new Sumpfkraut.Utilities.Threading.Runnable(false, new TimeSpan(0, 0, 0, 0, 200), false);
            //homeRun.printStateControls = true;
            //homeRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable sender)
            //{
            //    Console.WriteLine(DateTime.Now);
            //    sender.Suspend();
            //};
            //MyEvent += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) { runnable.Resume(); };
            //for (int i = 0; i < 20; i++)
            //{
            //    Console.WriteLine("--> " + i);
            //    MyEvent.Invoke(homeRun);
            //}
            //homeRun.Start();
            //homeRun.Suspend();


            //Sumpfkraut.Utilities.Threading.TestRunnable myRun =
            //    new Sumpfkraut.Utilities.Threading.TestRunnable(false, new TimeSpan(0, 0, 1), false);
            ////myRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable runnable) 
            ////{
            ////    Sumpfkraut.Utilities.Threading.TestRunnable _runnable = 
            ////        (Sumpfkraut.Utilities.Threading.TestRunnable) runnable;
            ////    //_runnable.TestEvent.Invoke(DateTime.Now);
            ////};
            //myRun.TestEvent += delegate (DateTime dt) { Console.WriteLine("~~~> " + dt); };
            //myRun.Start();



            //DateTime myResults_1 = DateTime.Now.AddHours(2);
            //int myResults_2 = -1;
            //Sumpfkraut.Utilities.Threading.TestRunnable homeRun =
            //    new Sumpfkraut.Utilities.Threading.TestRunnable(false, new TimeSpan(0, 0, 1), true);
            //homeRun.printStateControls = true;
            //System.Threading.AutoResetEvent waitHandle = new System.Threading.AutoResetEvent(false);
            //homeRun.waitHandle = waitHandle;
            //homeRun.OnRun += delegate (Sumpfkraut.Utilities.Threading.Runnable sender)
            //{
            //    int bla;
            //    for (int i = 0; i < 999999; i++)
            //    {
            //        bla = i - 999999;
            //    }
            //    myResults_1 = DateTime.Now;
            //    myResults_2 = 999;
            //};
            //homeRun.Start();
            //waitHandle.WaitOne();
            //Console.WriteLine("~~~> " + myResults_1 + " " + myResults_2);




            Logger.log(Logger.LogLevel.INFO, "######################## Initalise ########################");
            
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

            //Server.Network.Messages.AnimationMenuMessage.Init();
            
            Logger.log(Logger.LogLevel.INFO, "###################### End Initalise ######################");
		}

        private void AgentBlack_ReceivedResults(object sender, Sumpfkraut.Database.DBAgent.ReceivedResultsEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
