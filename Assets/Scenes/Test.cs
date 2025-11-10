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

            // Test 2: Create UDI with validation (after a delay)
            StartCoroutine(TestCreateUDI());

            // Test 3: Get UDI by ID (after a delay)
            StartCoroutine(TestGetUDIById());
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

            udiStatsComponent.GetUDIStats("lamborghini-test", "huracan", 12);
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

    private void HandleGameStudioReceived(GameStudio gameStudio)
    {
        Debug.Log("QL DATA: " + gameStudio.ToJson());
    }

    private void HandleGameEventLogReceived(GameEventLog gameEventLog)
    {
        Debug.Log("Game Event Log Received: " + gameEventLog.ToJson());
    }

    private void HandleGameEventLogError(string error)
    {
        Debug.LogError("Game Event Log Error: " + error);
    }

    private void HandleUDIReceivedForFuse(UDI udi)
    {
        Debug.Log("UDI Received for Fuse: " + udi.ToJson());

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

    private void HandleUDIReceived(UDI udi)
    {
        Debug.Log("UDI Received: " + udi.ToJson());
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
        }
    }

    private void HandleUDIError(string error)
    {
        Debug.LogError("UDI Error: " + error);
    }

    private void HandleUDIStatsReceived(UDIStats stats)
    {
        Debug.Log("UDI Stats Received: " + stats.ToJson());
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

    private void HandleUDIFuseReceived(UDIFuseResponse response)
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

    private IEnumerator TestCreateUDI()
    {
        yield return new WaitForSeconds(2f);

        Debug.Log("Test 2: Creating UDI with validation...");

        // Test valid data
        udiComponent.CreateUDI(
            email: "test@example.com",
            brand: "ferrari-test",
            model: "f40"
        );

        yield return new WaitForSeconds(2f);

        // Test invalid email
        Debug.Log("Test 2b: Testing invalid email validation...");
        udiComponent.CreateUDI(
            email: "invalid-email",
            brand: "ferrari-test",
            model: "f40"
        );

        yield return new WaitForSeconds(2f);

        // Test short brand
        Debug.Log("Test 2c: Testing short brand validation...");
        udiComponent.CreateUDI(
            email: "test@example.com",
            brand: null,
            model: "f40"
        );
    }

    private IEnumerator TestGetUDIById()
    {
        yield return new WaitForSeconds(5f);

        Debug.Log("Test 3: Getting UDI by ID...");

        // Test with a sample UDI ID (replace with actual ID from previous tests)
        string sampleUDIId = "0963718a-d9d4-4381-8448-e8b18ac72482";
        udiComponent.GetUDIById(sampleUDIId);

        yield return new WaitForSeconds(2f);

        // Test with empty ID (should trigger error)
        Debug.Log("Test 3b: Testing empty UDI ID validation...");
        udiComponent.GetUDIById("");

        yield return new WaitForSeconds(2f);

        // Test UDI utility methods
        Debug.Log("Test 3c: Testing UDI utility methods...");
        TestUDIUtilityMethods();
    }

    private void TestUDIUtilityMethods()
    {
        Debug.Log("=== Testing UDI Utility Methods ===");

        // Test when no UDI is loaded
        Debug.Log($"Has UDI: {udiComponent.HasUDI()}");
        Debug.Log($"Current UDI ID: {udiComponent.GetCurrentUDIId()}");
        Debug.Log($"Current Brand: {udiComponent.GetCurrentBrand()}");
        Debug.Log($"Current Model: {udiComponent.GetCurrentModel()}");
        Debug.Log($"Current Email: {udiComponent.GetCurrentEmail()}");
        Debug.Log($"Is Current UDI Fused: {udiComponent.IsCurrentUDIFused()}");

        // Test clearing UDI
        Debug.Log("Clearing UDI...");
        udiComponent.ClearUDI();
        Debug.Log($"Has UDI after clear: {udiComponent.HasUDI()}");

        Debug.Log("=== UDI Utility Methods Test Complete ===");
    }
}
