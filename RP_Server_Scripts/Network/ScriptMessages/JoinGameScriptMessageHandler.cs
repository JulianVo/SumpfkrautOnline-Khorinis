//using System;
//using GUC.Network;
//using GUC.Scripts.Visuals;
//using GUC.Scripts.VobSystem.Instances;
//using GUC.Scripts.WorldSystem;
//using GUC.Types;
//using GUC.Utilities;
//using RP_Server_Scripts.VobSystem.Definitions;
//using RP_Shared_Script;

//namespace GUC.Scripts.Network.ScriptMessages
//{
//    internal class JoinGameScriptMessageHandler : IScriptMessageHandler
//    {
//        private readonly INpcDefList _NpcDefList;

//        public JoinGameScriptMessageHandler(INpcDefList npcDefList)
//        {
//            _NpcDefList = npcDefList ?? throw new ArgumentNullException(nameof(npcDefList));
//        }


//        public void HandleMessage(Client.Client sender, PacketReader stream)
//        {

//            KillCharacter(sender);

//            var charInfo = sender.CharInfo;
//            NpcDef def = _NpcDefList.GetByCode(charInfo.BodyMesh == HumBodyMeshs.HUM_BODY_NAKED0 ? "maleplayer" : "femaleplayer");
//            NpcInst npc = new NpcInst(def)
//            {
//                UseCustoms = true,
//                CustomBodyTex = charInfo.BodyTex,
//                CustomHeadMesh = charInfo.HeadMesh,
//                CustomHeadTex = charInfo.HeadTex,
//                CustomVoice = charInfo.Voice,
//                CustomFatness = charInfo.Fatness,
//                CustomScale = new Vec3f(charInfo.BodyWidth, 1.0f, charInfo.BodyWidth),
//                CustomName = charInfo.Name,
//                DropUnconsciousOnDeath = true,
//                UnconsciousDuration = 15 * TimeSpan.TicksPerSecond,
//            };


//            if (npc.ModelDef.TryGetOverlay("1HST1", out ScriptOverlay ov))
//                npc.ModelInst.ApplyOverlay(ov);
//            if (npc.ModelDef.TryGetOverlay("2HST1", out ov))
//                npc.ModelInst.ApplyOverlay(ov);

//            var pair = spawnPositions.GetRandom();
//            npc.Spawn(WorldInst.List[0], pair.Item1, pair.Item2);
//            sender.BaseClient.SetControl(npc.BaseInst);
//        }

//        public RP_Shared_Script.ScriptMessages SupportedMessage => RP_Shared_Script.ScriptMessages.JoinGame;


//        static List<Vec3f, Angles> spawnPositions = new List<Vec3f, Angles>()
//        {
//            { new Vec3f(0,0,50), new Angles(0f, -2.81347f, 0f) },

//        };

//        public void KillCharacter(Client.Client client)
//        {
//            if (client.Character == null || client.Character.IsDead)
//                return;

//            client.Character.SetHealth(0);
//        }

//    }
//}
