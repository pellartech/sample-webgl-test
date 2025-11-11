using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using QuantumLeap;
using UnityEngine.Networking;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class TestQL : MonoBehaviour
{
    public GameStudioComponent gsComponent;
    public GameEventLogComponent gameEventLog;
    public UDIComponent udiComponent;

    public UDIStatsComponent udiStatsComponent;

    public UDIComponent udiComponentForFuse;
    public UDIFuseComponent udiFuseComponent;

    private void Start()
    {
        Debug.Log("Starting");
        Debug.Log($"QL: {gsComponent.GetInstanceID()}");
        if (gsComponent != null)
        {
            gsComponent.OnGameStudioReceived += HandleGameStudioReceived;
            gsComponent.GetGameStudioById("b51e5fdb-154d-4f31-a144-d0909f345408");
        }
        else
        {
            Debug.LogWarning("Can not find gsComponent");
        }

        // Test game event logging
        if (gameEventLog != null)
        {
            gameEventLog.OnGameEventLogReceived += HandleGameEventLogReceived;
            gameEventLog.OnGameEventLogError += HandleGameEventLogError;

            // Example: Log a car history event
            var eventData = new GameEventData
            {
                mileage = 100,
                circuit = "hockenheimring",
                rankedRace = true,
                raceWon = true,
                podiumFinish = true
            };

            gameEventLog.LogGameEvent(
                eventType: "car_history",
                category: "gameplay",
                categoryNumber: 1,
                brand: "lamborghini-test",
                model: "revuelto",
                tokenId: "60",
                eventData: eventData
            );
        }
        else
        {
            Debug.LogWarning("GameEventLogComponent not found");
        }

        // Test UDI functionality
        if (udiComponent != null)
        {
            Debug.Log("Testing UDI Component...");
            udiComponent.OnUDIReceived += HandleUDIReceived;
            udiComponent.OnUDIError += HandleUDIError;

            // Test 1: Create UDI
            Debug.Log("Test 1: Creating UDI...");
            udiComponent.CreateUDI(
                email: "test@gmail.com",
                brand: "lamborghini-test",
                model: "huracan"
            );
        }
        else
        {
            Debug.LogWarning("UDIComponent not found");
        }

        if (udiStatsComponent != null)
        {
            Debug.Log("Testing UDI Stats Component...");
            udiStatsComponent.OnUDIStatsReceived += HandleUDIStatsReceived;
            udiStatsComponent.OnUDIStatsError += HandleUDIStatsError;
        }
        else
        {
            Debug.LogWarning("UDIStatsComponent not found");
        }

        // Test UDI Fuse functionality
        if (udiFuseComponent != null && udiComponentForFuse != null)
        {
            Debug.Log("Testing UDI Fuse Component...");
            udiFuseComponent.OnUDIFuseReceived += HandleUDIFuseReceived;
            udiFuseComponent.OnUDIFuseError += HandleUDIFuseError;

            udiComponentForFuse.OnUDIReceived += HandleUDIReceivedForFuse;
            udiComponentForFuse.OnUDIError += HandleUDIError;

            System.Random random = new System.Random();
            int value = random.Next(99999999 + 1);

            udiComponentForFuse.CreateUDI(
                email: $"test{value}@gmail.com",
                brand: "lamborghini-test",
                model: "huracan"
            );
        }
        else
        {
            Debug.LogWarning("UDIFuseComponent not found");
        }
    }

    private void HandleGameStudioReceived(string action, GameStudio gameStudio)
    {
        Debug.Log("QL DATA: " + action + " " + gameStudio.ToJson());
    }

    private void HandleGameEventLogReceived(string action, GameEventLog gameEventLog)
    {
        Debug.Log("Game Event Log Received: " + action + " " + gameEventLog.ToJson());
    }

    private void HandleGameEventLogError(string error)
    {
        Debug.LogError("Game Event Log Error: " + error);
    }

    private void HandleUDIReceivedForFuse(string action, UDI udi)
    {
        Debug.Log("UDI Received for Fuse: " + action + " " + udi.ToJson());

        System.Random random = new System.Random();

        // Random integer (0-99999999)
        int value = random.Next(99999999 + 1);

        Debug.Log("Token ID: " + value.ToString());

        udiFuseComponent.FuseUDI(
            udiId: udi.id,
            tokenId: value.ToString(),
            contractAddress: "0x1234567890abcdef1234567890abcdef12345678",
            ownerAddress: "0xabcdef1234567890abcdef1234567890abcdef12",
            rarity: "legendary",
            attributes: new UDIAttribute[]
            {
                new UDIAttribute("color", "red"),
                new UDIAttribute("speed", "fast"),
                new UDIAttribute("rarity", "legendary")
            }
        );
    }

    private void HandleUDIReceived(string action, UDI udi)
    {
        if (action == udiComponent.ACTION_CREATE_UDI)
        {
            Debug.Log("UDI Received: " + action + " " + udi.ToJson());
            Debug.Log($"UDI Details - ID: {udi.id}, Brand: {udi.GetBrand()}, Model: {udi.GetModel()}");
            Debug.Log($"UDI Email: {udi.email}, Is Fused: {udi.IsFused()}");

            // Test utility methods
            if (udiComponent.HasUDI())
            {
                Debug.Log($"Current UDI ID: {udiComponent.GetCurrentUDIId()}");
                Debug.Log($"Current Brand: {udiComponent.GetCurrentBrand()}");
                Debug.Log($"Current Model: {udiComponent.GetCurrentModel()}");
                Debug.Log($"Current Email: {udiComponent.GetCurrentEmail()}");
                Debug.Log($"Is Current UDI Fused: {udiComponent.IsCurrentUDIFused()}");

                udiComponent.GetUDIById(udi.id);
            }
        }
        else if (action == udiComponent.ACTION_GET_UDI_BY_ID)
        {
            Debug.Log("UDI Received: " + action + " " + udi.ToJson());

            if (udiComponent.HasUDI())
            {
                Debug.Log($"Current UDI ID: {udiComponent.GetCurrentUDIId()}");
                Debug.Log($"Current Brand: {udiComponent.GetCurrentBrand()}");
                Debug.Log($"Current Model: {udiComponent.GetCurrentModel()}");
                Debug.Log($"Current Email: {udiComponent.GetCurrentEmail()}");
                Debug.Log($"Is Current UDI Fused: {udiComponent.IsCurrentUDIFused()}");

                udiStatsComponent.GetUDIStats(udiComponent.GetCurrentBrand(), udiComponent.GetCurrentModel(), udiComponent.GetCurrentSequentialId());
            }
        }
        else
        {
            Debug.LogError("UDI Received: " + action + " " + udi.ToJson());
            Debug.LogError("Unknown action: " + action);
        }
    }

    private void HandleUDIError(string error)
    {
        Debug.LogError("UDI Error: " + error);
    }

    private void HandleUDIStatsReceived(string action, UDIStats stats)
    {
        Debug.Log("UDI Stats Received: " + action + " " + stats.ToJson());
        Debug.Log($"UDI Stats Details - Brand: {stats.brand}, Model: {stats.model}, Sequential ID: {stats.sequentialId}");
        Debug.Log($"Total Races: {stats.totalRaces}, Total Mileage: {stats.totalMileage}");
        Debug.Log($"Wins: {stats.wins}, Podium Finishes: {stats.podiumFinishes}");
        Debug.Log($"Win Percentage: {stats.GetWinPercentage():F1}%, Podium Percentage: {stats.GetPodiumPercentage():F1}%");

        // Test UDIStats utility methods
        if (udiStatsComponent.HasUDIStats())
        {
            Debug.Log($"Current UDIStats Brand: {udiStatsComponent.GetCurrentUDIStatsBrand()}");
            Debug.Log($"Current UDIStats Model: {udiStatsComponent.GetCurrentUDIStatsModel()}");
            Debug.Log($"Current UDIStats Sequential ID: {udiStatsComponent.GetCurrentUDIStatsSequentialId()}");
            Debug.Log($"Current UDIStats Total Races: {udiStatsComponent.GetCurrentUDIStatsTotalRaces()}");
            Debug.Log($"Current UDIStats Total Mileage: {udiStatsComponent.GetCurrentUDIStatsTotalMileage()}");
            Debug.Log($"Current UDIStats Wins: {udiStatsComponent.GetCurrentUDIStatsWins()}");
            Debug.Log($"Current UDIStats Podium Finishes: {udiStatsComponent.GetCurrentUDIStatsPodiumFinishes()}");
            Debug.Log($"Current UDIStats Win Percentage: {udiStatsComponent.GetCurrentUDIStatsWinPercentage():F1}%");
            Debug.Log($"Current UDIStats Podium Percentage: {udiStatsComponent.GetCurrentUDIStatsPodiumPercentage():F1}%");
        }
    }

    private void HandleUDIStatsError(string error)
    {
        Debug.LogError("UDI Stats Error: " + error);
    }

    private void HandleUDIFuseReceived(string action, UDIFuseResponse response)
    {
        Debug.Log("UDI Fuse Received: " + response.ToJson());
        Debug.Log($"UDI Fuse Details - UDI ID: {response.data.udiId}, Token ID: {response.data.tokenId}");
        Debug.Log($"Type: {response.data.type}, Fused At: {response.data.fusedAt}, Fusion ID: {response.data.fusionId}");
        Debug.Log($"Is Fused: {response.data.IsFused()}, Type Display: {response.data.GetTypeDisplayName()}");

        // Test UDIFuse utility methods
        if (udiFuseComponent.HasFuseResponse())
        {
            Debug.Log($"Current Fuse UDI ID: {udiFuseComponent.GetCurrentFuseUDIId()}");
            Debug.Log($"Current Fuse Token ID: {udiFuseComponent.GetCurrentFuseTokenId()}");
            Debug.Log($"Current Fuse Type: {udiFuseComponent.GetCurrentFuseType()}");
            Debug.Log($"Current Fuse Date: {udiFuseComponent.GetCurrentFuseDate()}");
            Debug.Log($"Current Fusion ID: {udiFuseComponent.GetCurrentFusionId()}");
            Debug.Log($"Is Current UDI Fused: {udiFuseComponent.IsCurrentUDIFused()}");
            Debug.Log($"Current Fuse Type Display: {udiFuseComponent.GetCurrentFuseTypeDisplayName()}");
            Debug.Log($"Current Fuse Summary: {udiFuseComponent.GetCurrentFuseSummary()}");

            // Test DateTime parsing
            var fuseDateTime = udiFuseComponent.GetCurrentFuseDateTime();
            if (fuseDateTime.HasValue)
            {
                Debug.Log($"Current Fuse DateTime: {fuseDateTime.Value:yyyy-MM-dd HH:mm:ss}");
            }
        }
    }

    private void HandleUDIFuseError(string error)
    {
        Debug.LogError("UDI Fuse Error: " + error);
    }
}
