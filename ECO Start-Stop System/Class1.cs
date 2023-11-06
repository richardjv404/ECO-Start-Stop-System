using System;
using System.Windows.Forms;
using GTA;
using GTA.Math;
using GTA.Native;

public class EcoModus : Script
{
    private Vehicle vehicle;
    private int lightToggleTimer = 0;
    private int vehicleIdleTimer = 0;
    private const int EngineIdleThresholdMilliseconds = 5000;
    private int lastRadioStation = -1;

    private bool LightsOn { get; set; } = true;

    public EcoModus()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
    }

    void OnTick(object sender, EventArgs e)
    {
        Ped playerPed = Game.Player.Character;

        if (playerPed.IsInVehicle() && CanUse(playerPed.CurrentVehicle))
        {
            vehicle = playerPed.CurrentVehicle;

            if (IsCarOrBike(vehicle) && IsPlayerDrivingOrRagdoll(playerPed))
            {
                vehicle.IsEngineRunning = true;
                vehicleIdleTimer = Game.GameTime;
            }
            else if (vehicle.Speed == 0.0f && IsVehicleIdleForThreshold())
            {
                vehicle.IsEngineRunning = false;
            }
        }

        if (playerPed.IsInVehicle() && CanUse(playerPed.CurrentVehicle))
        {
            vehicle = playerPed.CurrentVehicle;

            if (IsCarOrBike(vehicle))
            {
                if (!Function.Call<bool>(Hash.IS_MOBILE_PHONE_RADIO_ACTIVE))
                {
                    Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, true);

                    int randomStation = GetRandomRadioStation();
                    Function.Call(Hash.SET_RADIO_TO_STATION_NAME, GetRadioStationName(randomStation));
                    lastRadioStation = randomStation;
                }
            }
        }
        else
        {
            // Player is not in a vehicle, so disable the mobile radio
            if (Function.Call<bool>(Hash.IS_MOBILE_PHONE_RADIO_ACTIVE))
            {
                Function.Call(Hash.SET_MOBILE_RADIO_ENABLED_DURING_GAMEPLAY, false);
            }
        }

        UpdateVehicleLights();
    }



    void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.H)
        {
            ToggleLights();
        }
    }

    bool CanUse(Entity entity)
    {
        return entity != null && entity.Exists();
    }

    bool IsCarOrBike(Vehicle vehicle)
    {
        return vehicle.Model.IsCar || vehicle.Model.IsBike;
    }

    bool IsPlayerDrivingOrRagdoll(Ped playerPed)
    {
        return Game.IsControlPressed(GTA.Control.VehicleAccelerate) ||
               Game.IsControlPressed(GTA.Control.VehicleBrake) ||
               Game.IsControlPressed(GTA.Control.VehicleMoveLeft) ||
               Game.IsControlPressed(GTA.Control.VehicleMoveRight) ||
               playerPed.IsRagdoll;
    }

    bool IsVehicleIdleForThreshold()
    {
        return Game.GameTime - vehicleIdleTimer >= EngineIdleThresholdMilliseconds;
    }

    void UpdateVehicleLights()
    {
        int lightsSetting = LightsOn ? 2 : 1;
        Function.Call(Hash.SET_VEHICLE_LIGHTS, vehicle, lightsSetting);
    }

    void ToggleLights()
    {
        if (Game.GameTime > lightToggleTimer)
        {
            LightsOn = !LightsOn;
            lightToggleTimer = Game.GameTime + 800;
        }
    }


    int GetRandomRadioStation()
    {
        // You need to define the range of available radio stations
        // and generate a random station ID within that range.
        // For example, if you have 10 stations, use:
        return new Random().Next(0, 10);
    }

    string GetRadioStationName(int stationID)
    {
        switch (stationID)
        {
            case 0:
                return "RADIO_01_CLASS_ROCK";
            case 1:
                return "RADIO_02_POP";
            case 2:
                return "RADIO_03_HIPHOP_NEW";
            case 3:
                return "RADIO_04_PUNK";
            case 4:
                return "RADIO_05_TALK_01";
            case 5:
                return "RADIO_06_COUNTRY";
            case 6:
                return "RADIO_07_DANCE_01";
            case 7:
                return "RADIO_08_MEXICAN";
            case 8:
                return "RADIO_09_HIPHOP_OLD";
            case 9:
                return "RADIO_10_TALK_02";
            // Add more cases for other station IDs here

            default:
                return "RADIO_01_CLASS_ROCK";  // Default station if the ID is out of range
        }
    }
}