using Rice.Server.Core;
using Rice.Server.Structures;
using Rice.Server.Structures.Resources;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models = Rice.Server.Database.Models;

namespace Rice.Game
{
    public class Vehicle
    {
        public uint AuctionCount;
        public uint Color;
        public int CarID;
        public uint CarType;
        public uint Grade;
        public float Mitron;
        public float Kms;
        public uint SlotType;

        public VehicleUpgradeEntry VehicleUpgradeEntry;
        public VehicleListEntry VehicleListEntry;

        private Vehicle(int id, Models.Vehicle dbVehicle = null, int grade = 1)
        {
            int lookupId = id;
            int lookupGrade = grade;

            if (dbVehicle != null)
            {
                AuctionCount = (uint)dbVehicle.AuctionCount;
                Color = (uint)dbVehicle.Color;
                CarID = dbVehicle.CarID;
                CarType = (uint)dbVehicle.CarType;
                Grade = (uint)dbVehicle.Grade;
                Mitron = dbVehicle.Mitron;
                Kms = dbVehicle.Kms;
                SlotType = 0;

                lookupId = dbVehicle.CarType;
                lookupGrade = dbVehicle.Grade;
            }

            VehicleUpgradeEntry = VehicleTable.GetVehicleUpgrade(lookupId, lookupGrade);
            VehicleListEntry = VehicleTable.Vehicles[lookupId];
        }

        public static Vehicle FromDB(Models.Vehicle dbVehicle) => new Vehicle(dbVehicle.CarID, dbVehicle, dbVehicle.Grade);

        public static Vehicle CreateEntry(ulong CID, int carSort, int color = 0, int grade = 1)
        {
            using (var rc = Database.GetContext())
            {
                try
                {
                    VehicleTable.GetVehicleUpgrade(carSort, grade);
                }
                catch (Exception ex)
                {
                    Log.WriteError($"Failed to get vehicle {carSort} grade {grade} for Vehicle.CreateEntry: {ex.Message}");
                    return null;
                }

                var lastVehicle = rc.Vehicles.Where(i => i.CID == (long)CID).OrderByDescending(i => i.CarID).FirstOrDefault();

                var newVehicle = new Models.Vehicle
                {
                    CID = (long)CID,
                    CarID = (lastVehicle?.CarID ?? 0) + 1,
                    CarType = carSort,
                    Grade = grade,
                    AuctionCount = 0,
                    Color = color
                };

                rc.Vehicles.Add(newVehicle);
                rc.SaveChanges();

                return new Vehicle(newVehicle.CarType, newVehicle);
            }
        }

        public CarInfo GetInfo()
        {
            return new CarInfo
            {
                AuctionOn = false,
                CarUnit = new CarUnit
                {
                    AuctionCnt = AuctionCount,
                    BaseColor = 0,
                    CarID = CarID,
                    CarType = CarType,
                    Grade = Grade,
                    Mitron = Mitron,
                    Kmh = Kms,
                    SlotType = SlotType
                },
                Color = Color,
                MitronCapacity = VehicleUpgradeEntry.MitronCapacity,
                MitronEfficiency = VehicleUpgradeEntry.MitronEfficiency
            };
        }
    }
}
