using AOSharp.Common.GameData;
using AOSharp.Core;
using AOSharp.Core.UI;
using SmokeLounge.AOtomation.Messaging.GameData;
using SmokeLounge.AOtomation.Messaging.Messages.N3Messages;
using AOSharp.Core.Inventory;
using SmokeLounge.AOtomation.Messaging.Messages;
using System.Linq;

namespace BagTrader
{
    public class Main : AOPluginEntry
    {
        private bool _tradeBag;
        public override void Run(string pluginDir)
        {

            Chat.WriteLine("BagTrader loaded.");
            Chat.WriteLine("Commands: '/tradebag' - ON trades any bag you right click.");


            Chat.RegisterCommand("tradebag", (command, args, window) =>
            {
                _tradeBag = !_tradeBag;

                Chat.WriteLine(
                    $"BagTrader: {(_tradeBag ? "ON" : "OFF")}");
            });

            Network.N3MessageSent += OnN3MessageSent;
        }

        private void OnN3MessageSent(object sender, N3Message n3Msg)
        {
            if (!_tradeBag)
                return;

            if (!(n3Msg is GenericCmdMessage cmd))
                return;

            if (cmd.Action != GenericCmdAction.Use)
                return;

            if (cmd.Target.Type != IdentityType.Inventory)
                return;

            if (!Trade.TradeTarget.HasValue)
                return;

            var bag = Inventory.Backpacks
                .FirstOrDefault(x => x.Slot == cmd.Target);

            if (bag == null)
                return;

            var player = DynelManager.LocalPlayer;

            if (player == null)
                return;

            int slotInstance = cmd.Target.Instance;

            Network.Send(new TradeMessage
            {
                Unknown1 = 2,
                Action = TradeAction.AddItem,

                Param1 = 50000,
                Param2 = player.Identity.Instance,

                Param3 = 101,
                Param4 = slotInstance
            });

            Chat.WriteLine(
                $"TradeBag: Added bag '{bag.Name}' to trade.");
        }
    }
}