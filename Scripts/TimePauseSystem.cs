using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimePauseSystem : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

//using Unity.Entities;
//using Unity.Mathematics;

//public class TimeSystem : SystemBase
//{
//	private GameStateSystem gameStateSystem;

//	protected override void OnCreate()
//	{
//		base.OnCreate();
//		gameStateSystem = World.GetOrCreateSystem<GameStateSystem>();
//		gameStateSystem.GameLoadedEvent += OnGameLoaded;
//		gameStateSystem.GameEndEvent += OnGameEnded;
//		gameStateSystem.GameEndingPreConfirmEvent += OnGameEndingPreConfirm;
//		gameStateSystem.GameStartingToProgressEvent += OnGameStartingToPrgress;

//		RequireSingletonForUpdate<GameTimeComponent>();
//		CreateTimeEntity();
//	}

//	protected override void OnDestroy()
//	{
//		base.OnDestroy();
//		gameStateSystem.GameLoadedEvent -= OnGameLoaded;
//		gameStateSystem.GameEndEvent -= OnGameEnded;
//		gameStateSystem.GameEndingPreConfirmEvent -= OnGameEndingPreConfirm;
//		gameStateSystem.GameStartingToProgressEvent -= OnGameStartingToPrgress;
//	}

//	protected override void OnUpdate()
//	{
//		GameTimeComponent gameTimeComponent = GetSingleton<GameTimeComponent>();

//		if (this.HasSingleton<TimeWarpComponent>())
//		{
//			TimeWarpComponent timeWarpComponent = this.GetSingleton<TimeWarpComponent>();
//			timeWarpComponent.CurrentLerpAmount = math.min(timeWarpComponent.CurrentLerpAmount + Time.DeltaTime / timeWarpComponent.TimeWarpDuration, 1f);
//			gameTimeComponent.TimeScale = math.lerp(timeWarpComponent.LerpFromTimeScale, timeWarpComponent.LerpToTimeScale, timeWarpComponent.CurrentLerpAmount);
//			if (timeWarpComponent.CurrentLerpAmount >= 1f)
//			{
//				DestroyTimeWarpEntity();
//			}
//			else
//			{
//				this.SetSingleton(timeWarpComponent);
//			}
//		}
//		else if (this.HasSingleton<PlayerEnemyProximityTimeWarpComponent>() && !this.HasSingleton<PauseTimeComponent>())
//		{
//			PlayerEnemyProximityTimeWarpComponent playerEnemyProximityTimeWarpComponent = GetSingleton<PlayerEnemyProximityTimeWarpComponent>();
//			gameTimeComponent.TimeScale = math.clamp(playerEnemyProximityTimeWarpComponent.ProximityFraction, 0f, 1f);
//		}

//		gameTimeComponent.GameElapsedTime += Time.DeltaTime * gameTimeComponent.TimeScale * gameTimeComponent.GameInProgress;
//		gameTimeComponent.DeltaTime = math.min(Time.DeltaTime, 0.05f);
//		gameTimeComponent.ScaledDeltaTime = gameTimeComponent.DeltaTime * gameTimeComponent.TimeScale;
//		this.SetSingleton(gameTimeComponent);
//	}

//	public void CreateTimeEntity()
//	{
//		EntityManager.CreateEntity(ComponentType.ReadWrite<GameTimeComponent>());
//		this.SetSingleton(new GameTimeComponent
//		{
//			TimeScale = 1f,
//			GameElapsedTime = 0f,
//			GameInProgress = 0,
//		});
//	}

//	public void CreateTimeWarpEntity(float targetTimeScale = 0f, float duration = 1f)
//	{
//		DestroyTimeWarpEntity();

//		GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();

//		duration = math.clamp(duration, 0.1f, 5f);
//		TimeWarpComponent timeWarpComponent = new TimeWarpComponent
//		{
//			LerpToTimeScale = targetTimeScale,
//			TimeWarpDuration = duration,
//			LerpFromTimeScale = gameTimeComponent.TimeScale,
//			CurrentLerpAmount = 0f
//		};

//		EntityManager.CreateEntity(ComponentType.ReadWrite<TimeWarpComponent>());
//		this.SetSingleton(timeWarpComponent);
//	}

//	public void CreateOrUpdatePlayerEnemyProximityTimeWarpEntity(float initialFraction)
//	{
//		if (!this.HasSingleton<GameTimeComponent>())
//		{
//			UnityEngine.Debug.LogError("Attempt to create PlayerEnemyProximityTimeWarpComponent without GameTimeComponent present. Aborting.");
//			return;
//		}

//		GameTimeComponent gameTimeComponent = GetSingleton<GameTimeComponent>();

//		PlayerEnemyProximityTimeWarpComponent playerEnemyProximityTimeWarpComponent;

//		if (this.HasSingleton<PlayerEnemyProximityTimeWarpComponent>())
//		{
//			playerEnemyProximityTimeWarpComponent = GetSingleton<PlayerEnemyProximityTimeWarpComponent>();
//			playerEnemyProximityTimeWarpComponent.ProximityFraction = initialFraction;
//			EntityManager.SetComponentData(this.GetSingletonEntity<PlayerEnemyProximityTimeWarpComponent>(), playerEnemyProximityTimeWarpComponent);
//		}
//		else
//		{
//			playerEnemyProximityTimeWarpComponent = new PlayerEnemyProximityTimeWarpComponent
//			{
//				ProximityFraction = initialFraction,
//				PreviousTimeScale = gameTimeComponent.TimeScale
//			};

//			EntityManager.CreateEntity(ComponentType.ReadWrite<PlayerEnemyProximityTimeWarpComponent>());
//			this.SetSingleton(playerEnemyProximityTimeWarpComponent);
//		}
//	}

//	public void DestroyPlayerEnemyProximityTimeWarpEntity()
//	{
//		if (this.HasSingleton<PlayerEnemyProximityTimeWarpComponent>())
//		{
//			if (!this.HasSingleton<GameTimeComponent>())
//			{
//				UnityEngine.Debug.LogError("Attempt to Destroy PlayerEnemyProximityTimeWarpComponent without GameTimeComponent present. Aborting.");
//				return;
//			}

//			if (!this.HasSingleton<TimeWarpComponent>())
//			{
//				GameTimeComponent gameTimeComponent = GetSingleton<GameTimeComponent>();
//				PlayerEnemyProximityTimeWarpComponent playerEnemyProximityTimeWarpComponent = this.GetSingleton<PlayerEnemyProximityTimeWarpComponent>();
//				gameTimeComponent.TimeScale = playerEnemyProximityTimeWarpComponent.PreviousTimeScale;
//				EntityManager.SetComponentData(GetSingletonEntity<GameTimeComponent>(), gameTimeComponent);
//			}

//			EntityManager.DestroyEntity(GetSingletonEntity<PlayerEnemyProximityTimeWarpComponent>());
//		}
//	}

//	private void DestroyTimeWarpEntity()
//	{
//		if (this.HasSingleton<TimeWarpComponent>())
//		{
//			EntityManager.DestroyEntity(GetSingletonEntity<TimeWarpComponent>());
//		}
//	}

//	public void SetGamePaused(bool paused = true)
//	{
//		//GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();
//		//gameTimeComponent.TimeScale = paused ? 0f : 1f;
//		//this.SetSingleton(gameTimeComponent);
//		CreateTimeWarpEntity(paused ? 0f : 1f, 0.2f);

//		if (this.HasSingleton<PauseTimeComponent>())
//			EntityManager.DestroyEntity(this.GetSingletonEntity<PauseTimeComponent>());

//		if (paused)
//		{
//			EntityManager.CreateEntity(ComponentType.ReadOnly<PauseTimeComponent>());
//			this.SetSingleton(new PauseTimeComponent { });
//		}
//	}

//	private void OnGameEnded(GameEndData gameEndData)
//	{
//		GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();
//		gameTimeComponent.GameInProgress = 0;
//		gameTimeComponent.TimeScale = 1f;
//		this.SetSingleton(gameTimeComponent);
//	}

//	private void OnGameLoaded(float3 position)
//	{
//		GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();
//		gameTimeComponent.GameElapsedTime = 0f;
//		gameTimeComponent.TimeScale = 1f;
//		this.SetSingleton(gameTimeComponent);
//	}


//	private void OnGameEndingPreConfirm(GameEndData blah)
//	{
//		DestroyTimeWarpEntity();
//		DestroyPlayerEnemyProximityTimeWarpEntity();
//		GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();
//		gameTimeComponent.GameInProgress = 0;
//		gameTimeComponent.TimeScale = 1f;
//		this.SetSingleton(gameTimeComponent);
//	}

//	private void OnGameStartingToPrgress()
//	{
//		GameTimeComponent gameTimeComponent = this.GetSingleton<GameTimeComponent>();
//		gameTimeComponent.GameInProgress = 1;
//		gameTimeComponent.TimeScale = 1f;
//		this.SetSingleton(gameTimeComponent);
//	}
//}