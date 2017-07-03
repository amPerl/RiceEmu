using Rice.Server.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rice.Game;
using Rice.Server.Structures;
using Rice.Server.Structures.Resources;

namespace Rice.Server.Packets.Game
{
    public static class Dealership
    {
        [RicePacket(85, RiceServer.ServerType.Game)]
        public static void BuyCar(RicePacket packet)
        {
            Log.WriteDebug($"buycar {BitConverter.ToString(packet.Buffer)}");
            string charName = packet.Reader.ReadUnicodeStatic(21); // what the fug npludo
            uint carId = packet.Reader.ReadUInt32();
            uint color = packet.Reader.ReadUInt32();

            Log.WriteDebug($"char {charName} buying car {carId} color {color}");
            if (!VehicleTable.Vehicles.ContainsKey((int) carId))
            {
                packet.Sender.Error("fuckity fuck off");
                Log.WriteError("Attempted to purchase vehicle not in table");
                return;
            }

            var vehEntry = VehicleTable.Vehicles[(int) carId];
            if (vehEntry.HasCondition())
            {
                Log.WriteError("Attempted to purchase car that has not-null sell condition, not implemented");
                return;
            }

            int grade = vehEntry.Level;
            var vehUpgradeEntry = VehicleTable.VehicleUpgrades[(int) carId][grade - 1];
            int price = vehUpgradeEntry.Price;

            var character = packet.Sender.Player.ActiveCharacter;

            bool tookMoney = character.SpendMito((ulong) price);
            if (!tookMoney)
            {
                Log.WriteError($"Failed to spend mito for vehicle {carId}");
                return;
            }

            Vehicle givenVehicle;
            Item givenKey;
            bool gaveVehicle = character.GrantVehicle((int) carId, out givenVehicle, out givenKey, (int) color, grade);
            if (!gaveVehicle)
            {
                Log.WriteError($"Failed to grant vehicle {carId}");
                return;
            }

            bool selectedCar = character.SelectCar(givenVehicle.CarID);
            if (!selectedCar)
            {
                Log.WriteError($"Failed to select vehicle {carId}");
                return;
            }

            var ack = new RicePacket(86);
            ack.Writer.Write(givenVehicle.GetInfo());
            ack.Writer.Write(price);
            packet.Sender.Send(ack);

            character.FlushModInfo(packet.Sender);
        }

        [RicePacket(87, RiceServer.ServerType.Game)]
        public static void SellCar(RicePacket packet)
        {
            string charName = packet.Reader.ReadUnicodeStatic(21); // what the fug npludo
            uint carId = packet.Reader.ReadUInt32();

            var character = packet.Sender.Player.ActiveCharacter;
            if (carId == character.CurrentCarID)
            {
                packet.Sender.Error("dude you're driving that");
                return;
            }

            var vehicle = character.Garage.FirstOrDefault(v => v.CarID == carId);
            if (vehicle == null)
            {
                packet.Sender.Error("fuck you man this aint your car");
                Log.WriteError($"Client attempted to select invalid vehicle id {carId}");
                return;
            }

            var price = vehicle.VehicleUpgradeEntry.SellPrice;

            bool tookVehicle = character.RemoveVehicle((int) carId);
            if (!tookVehicle)
            {
                Log.WriteError($"Failed to take vehicle {carId} for SellCar");
                return;
            }

            bool gaveMoney = character.GrantMito((ulong) price);
            if (!gaveMoney)
            {
                packet.Sender.Error("took your car but you aint gettin any mito sry");
                Log.WriteError($"Failed to give mito for vehicle {carId} for SellCar");
                return;
            }

            var ack = new RicePacket(88);
            ack.Writer.Write(carId);
            ack.Writer.Write((int)price);
            packet.Sender.Send(ack);

            character.FlushModInfo(packet.Sender);
        }

        [RicePacket(89, RiceServer.ServerType.Game)]
        public static void SelectCar(RicePacket packet)
        {
            uint carId = packet.Reader.ReadUInt32();

            var character = packet.Sender.Player.ActiveCharacter;

            var vehicle = character.Garage.FirstOrDefault(v => v.CarID == carId);
            if (vehicle == null)
            {
                packet.Sender.Error("fuck you man this aint your car");
                Log.WriteError($"Client attempted to select invalid vehicle id {carId}");
                return;
            }

            character.SelectCar((int) carId);

            var ack = new RicePacket(90);
            ack.Writer.Write(vehicle.GetInfo());
            packet.Sender.Send(ack);
        }
    }
}
