namespace SJ_CommandInputManager
{
    
using System;
using System.Collections.Generic;
using System.Linq;

// 키 코드는 int 값으로 사용 (예: Unity KeyCode, Windows Virtual Key 등과 호환)

// 커맨드 입력 타입
public enum CommandInputType
{
    Sequential,    // 순차적 입력 (↓↘→ + P)
    Simultaneous   // 동시 입력 (A + B)
}

// 개별 입력 정보
public class InputStep
{
    public List<int> Keys { get; set; } = new List<int>();
    public CommandInputType Type { get; set; }
    public int TimeWindowMs { get; set; } // 이 스텝을 완료하기 위한 시간 창

    public InputStep(CommandInputType type, int timeWindowMs, params int[] keys)
    {
        Type = type;
        TimeWindowMs = timeWindowMs;
        Keys.AddRange(keys);
    }
}

// 전체 커맨드 정의
public class Command
{
    public string Name { get; set; }
    public List<InputStep> Steps { get; set; } = new List<InputStep>();
    public int TotalTimeoutMs { get; set; } // 전체 커맨드 완료 제한 시간

    public Command(string name, int totalTimeoutMs = 1000)
    {
        Name = name;
        TotalTimeoutMs = totalTimeoutMs;
    }

    public void AddStep(InputStep step)
    {
        Steps.Add(step);
    }
}

// 키 입력 이벤트
public class KeyInputEvent
{
    public int Key { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsPressed { get; set; } // true: 눌림, false: 떼어짐

    public KeyInputEvent(int key, bool isPressed)
    {
        Key = key;
        IsPressed = isPressed;
        Timestamp = DateTime.Now;
    }
}

// 커맨드 완료 이벤트
public class CommandCompletedEventArgs : EventArgs
{
    public string CommandName { get; set; }
    public DateTime CompletedAt { get; set; }

    public CommandCompletedEventArgs(string commandName)
    {
        CommandName = commandName;
        CompletedAt = DateTime.Now;
    }
}

// 진행 중인 커맨드 상태
public class CommandProgress
{
    public Command Command { get; set; }
    public int CurrentStep { get; set; } = 0;
    public DateTime StartTime { get; set; }
    public DateTime LastInputTime { get; set; }
    public HashSet<int> CurrentlyPressed { get; set; } = new HashSet<int>();

    public CommandProgress(Command command)
    {
        Command = command;
        StartTime = DateTime.Now;
        LastInputTime = DateTime.Now;
    }
}

// 메인 커맨드 입력 관리자
public class CommandInputManager
{
    private Dictionary<string, Command> _commands = new Dictionary<string, Command>();
    private List<CommandProgress> _activeCommands = new List<CommandProgress>();
    private Queue<KeyInputEvent> _inputHistory = new Queue<KeyInputEvent>();
    private HashSet<int> _currentlyPressed = new HashSet<int>();
    
    // 설정값
    public int SimultaneousInputToleranceMs { get; set; } = 50; // 동시 입력 허용 오차
    public int InputHistorySize { get; set; } = 100;

    // 이벤트
    public event EventHandler<CommandCompletedEventArgs> CommandCompleted;

    // 커맨드 등록
    public void RegisterCommand(Command command)
    {
        _commands[command.Name] = command;
    }

    // 빠른 커맨드 등록 헬퍼 메서드들
    public void RegisterSequentialCommand(string name, int timeoutMs, params int[] keys)
    {
        var command = new Command(name, timeoutMs);
        foreach (var key in keys)
        {
            command.AddStep(new InputStep(CommandInputType.Sequential, 200, key));
        }
        RegisterCommand(command);
    }

    public void RegisterSimultaneousCommand(string name, int timeoutMs, params int[] keys)
    {
        var command = new Command(name, timeoutMs);
        command.AddStep(new InputStep(CommandInputType.Simultaneous, SimultaneousInputToleranceMs, keys));
        RegisterCommand(command);
    }

    // 키 입력 처리
    public void OnKeyPressed(int key)
    {
        _currentlyPressed.Add(key);
        ProcessInput(new KeyInputEvent(key, true));
    }

    public void OnKeyReleased(int key)
    {
        _currentlyPressed.Remove(key);
        ProcessInput(new KeyInputEvent(key, false));
    }

    private void ProcessInput(KeyInputEvent inputEvent)
    {
        // 입력 히스토리 관리
        _inputHistory.Enqueue(inputEvent);
        while (_inputHistory.Count > InputHistorySize)
        {
            _inputHistory.Dequeue();
        }

        // 키를 눌렀을 때만 커맨드 체크
        if (!inputEvent.IsPressed) return;

        // 새로운 커맨드 시작 체크
        CheckForNewCommands(inputEvent);

        // 진행 중인 커맨드 업데이트
        UpdateActiveCommands(inputEvent);

        // 타임아웃된 커맨드 정리
        CleanupTimedOutCommands();
    }

    private void CheckForNewCommands(KeyInputEvent inputEvent)
    {
        foreach (var command in _commands.Values)
        {
            if (command.Steps.Count == 0) continue;

            var firstStep = command.Steps[0];
            
            // 첫 번째 스텝이 현재 입력과 매치되는지 확인
            if (IsStepMatched(firstStep, inputEvent))
            {
                // 동일한 커맨드가 이미 진행 중인지 확인 (중복 방지)
                bool alreadyActive = _activeCommands.Any(p => p.Command.Name == command.Name);
                if (alreadyActive) continue;

                var progress = new CommandProgress(command);
                progress.LastInputTime = inputEvent.Timestamp;
                
                // 첫 번째 스텝이 완료되었는지 확인
                if (IsStepCompleted(firstStep, progress, inputEvent))
                {
                    progress.CurrentStep = 1;
                    
                    // 단일 스텝 커맨드인 경우 즉시 완료
                    if (progress.CurrentStep >= progress.Command.Steps.Count)
                    {
                        CommandCompleted?.Invoke(this, new CommandCompletedEventArgs(command.Name));
                        continue; // 진행 중인 리스트에 추가하지 않음
                    }
                }
                
                _activeCommands.Add(progress);
            }
        }
    }

    private void UpdateActiveCommands(KeyInputEvent inputEvent)
    {
        var completedCommands = new List<CommandProgress>();

        // 역순으로 순회하여 리스트 수정 중 안전성 확보
        for (int i = _activeCommands.Count - 1; i >= 0; i--)
        {
            var progress = _activeCommands[i];
            
            if (progress.CurrentStep >= progress.Command.Steps.Count)
            {
                // 이미 완료된 커맨드
                completedCommands.Add(progress);
                _activeCommands.RemoveAt(i);
                continue;
            }

            var currentStep = progress.Command.Steps[progress.CurrentStep];
            
            if (IsStepMatched(currentStep, inputEvent))
            {
                progress.LastInputTime = inputEvent.Timestamp;
                
                if (IsStepCompleted(currentStep, progress, inputEvent))
                {
                    progress.CurrentStep++;
                    
                    // 모든 스텝 완료 확인
                    if (progress.CurrentStep >= progress.Command.Steps.Count)
                    {
                        completedCommands.Add(progress);
                        _activeCommands.RemoveAt(i); // 즉시 제거하여 중복 방지
                    }
                }
            }
        }

        // 완료된 커맨드 이벤트 발생
        foreach (var completed in completedCommands)
        {
            CommandCompleted?.Invoke(this, new CommandCompletedEventArgs(completed.Command.Name));
        }
    }

    private bool IsStepMatched(InputStep step, KeyInputEvent inputEvent)
    {
        return step.Keys.Contains(inputEvent.Key);
    }

    private bool IsStepCompleted(InputStep step, CommandProgress progress, KeyInputEvent inputEvent)
    {
        switch (step.Type)
        {
            case CommandInputType.Sequential:
                // 순차적 입력은 단일 키 입력으로 완료
                return step.Keys.Contains(inputEvent.Key);

            case CommandInputType.Simultaneous:
                // 현재 입력된 키가 필요한 키 중 하나인지 확인
                if (!step.Keys.Contains(inputEvent.Key)) return false;
                
                // 동시 입력은 모든 키가 허용 시간 내에 눌려야 함
                var pressedKeys = GetRecentlyPressedKeys(step.TimeWindowMs);
                bool allKeysPressed = step.Keys.All(key => pressedKeys.Contains(key));
                
                return allKeysPressed;

            default:
                return false;
        }
    }

    private HashSet<int> GetRecentlyPressedKeys(int timeWindowMs)
    {
        var cutoffTime = DateTime.Now.AddMilliseconds(-timeWindowMs);
        var recentKeys = new HashSet<int>();

        foreach (var input in _inputHistory.Reverse())
        {
            if (input.Timestamp < cutoffTime) break;
            if (input.IsPressed)
            {
                recentKeys.Add(input.Key);
            }
        }

        return recentKeys;
    }

    private void CleanupTimedOutCommands()
    {
        var now = DateTime.Now;
        _activeCommands.RemoveAll(progress =>
        {
            // 전체 타임아웃 체크
            if ((now - progress.StartTime).TotalMilliseconds > progress.Command.TotalTimeoutMs)
                return true;

            // 현재 스텝 타임아웃 체크
            if (progress.CurrentStep < progress.Command.Steps.Count)
            {
                var currentStep = progress.Command.Steps[progress.CurrentStep];
                if ((now - progress.LastInputTime).TotalMilliseconds > currentStep.TimeWindowMs)
                    return true;
            }

            return false;
        });
    }

    // 디버그/모니터링용 메서드들
    public List<string> GetActiveCommandNames()
    {
        return _activeCommands.Select(p => p.Command.Name).ToList();
    }

    public void ClearAllCommands()
    {
        _commands.Clear();
        _activeCommands.Clear();
    }

    public bool IsCommandRegistered(string commandName)
    {
        return _commands.ContainsKey(commandName);
    }
}

// 사용 예제 (키 코드 상수 정의)
public class GameCommandExample
{
    // 키 코드 상수들 (Unity KeyCode, Windows Virtual Key 등에 맞게 조정 가능)
    public const int KEY_UP = 38;
    public const int KEY_DOWN = 40;
    public const int KEY_LEFT = 37;
    public const int KEY_RIGHT = 39;
    public const int KEY_A = 65;
    public const int KEY_B = 66;
    public const int KEY_X = 88;
    public const int KEY_Y = 89;
    public const int KEY_PUNCH = 80;
    public const int KEY_KICK = 75;
    public const int KEY_BLOCK = 66;

    public static void SetupCommands()
    {
        var commandManager = new CommandInputManager();

        // === 다양한 이벤트 핸들러 등록 방법들 ===

        // 1. 람다 표현식으로 등록
        commandManager.CommandCompleted += (sender, e) =>
        {
            Console.WriteLine($"🎯 [{e.CompletedAt:HH:mm:ss.fff}] 커맨드 '{e.CommandName}' 완료!");
            
            // 커맨드별 처리
            switch (e.CommandName)
            {
                case "하둥권":
                    Console.WriteLine("💥 파이어볼 발사!");
                    // 게임 로직: 파이어볼 생성, 사운드 재생 등
                    break;
                case "승룡권":
                    Console.WriteLine("🔥 승룡권 발동!");
                    // 게임 로직: 승룡권 애니메이션, 데미지 처리 등
                    break;
                case "던지기":
                    Console.WriteLine("🤼 적을 던집니다!");
                    break;
                case "강공격":
                    Console.WriteLine("💪 강력한 공격!");
                    break;
            }
        };

        // 2. 별도 메서드로 등록
        commandManager.CommandCompleted += OnCommandCompleted;

        // 3. 델리게이트 체인으로 여러 핸들러 등록
        commandManager.CommandCompleted += LogCommandToFile;
        commandManager.CommandCompleted += UpdateGameStats;
        commandManager.CommandCompleted += PlaySoundEffect;

        // === 커맨드 등록 ===
        
        // 하둥권: ↓↘→ + P
        var hadoken = new Command("하둥권", 800);
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_DOWN));
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_DOWN, KEY_RIGHT)); // ↘
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_RIGHT));
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 300, KEY_PUNCH));
        commandManager.RegisterCommand(hadoken);

        // 승룡권: →↓↘ + P  
        var shoryuken = new Command("승룡권", 800);
        shoryuken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_RIGHT));
        shoryuken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_DOWN));
        shoryuken.AddStep(new InputStep(CommandInputType.Sequential, 200, KEY_DOWN, KEY_RIGHT)); // ↘
        shoryuken.AddStep(new InputStep(CommandInputType.Sequential, 300, KEY_PUNCH));
        commandManager.RegisterCommand(shoryuken);

        // 던지기: A + B (동시 입력)
        commandManager.RegisterSimultaneousCommand("던지기", 500, KEY_A, KEY_B);

        // 강공격: X + Y (동시 입력)
        commandManager.RegisterSimultaneousCommand("강공격", 500, KEY_X, KEY_Y);

        // 간단한 연속기: A → B → X
        commandManager.RegisterSequentialCommand("기본콤보", 1000, KEY_A, KEY_B, KEY_X);

        // === 입력 시뮬레이션 테스트 ===
        Console.WriteLine("=== 커맨드 입력 시스템 테스트 시작 ===\n");
        
        // 1. 하둥권 입력 테스트
        Console.WriteLine("1️⃣ 하둥권 입력 테스트...");
        TestHadokenInput(commandManager);
        
        System.Threading.Thread.Sleep(1000);
        
        // 2. 동시 입력 테스트
        Console.WriteLine("\n2️⃣ 던지기 입력 테스트...");
        TestSimultaneousInput(commandManager);
        
        System.Threading.Thread.Sleep(1000);
        
        // 3. 기본 콤보 테스트
        Console.WriteLine("\n3️⃣ 기본콤보 입력 테스트...");
        TestBasicCombo(commandManager);
    }

    // === 이벤트 핸들러 메서드들 ===
    
    private static void OnCommandCompleted(object sender, CommandCompletedEventArgs e)
    {
        Console.WriteLine($"📋 메서드 핸들러: {e.CommandName} 처리 완료");
    }

    private static void LogCommandToFile(object sender, CommandCompletedEventArgs e)
    {
        // 파일 로깅 로직 (예시)
        Console.WriteLine($"📝 로그: {e.CommandName} - {e.CompletedAt}");
    }

    private static void UpdateGameStats(object sender, CommandCompletedEventArgs e)
    {
        // 게임 통계 업데이트 (예시)
        Console.WriteLine($"📊 통계 업데이트: {e.CommandName} 사용 횟수 +1");
    }

    private static void PlaySoundEffect(object sender, CommandCompletedEventArgs e)
    {
        // 사운드 재생 (예시)
        Console.WriteLine($"🔊 사운드: {e.CommandName}.wav 재생");
    }

    // === 입력 테스트 헬퍼 메서드들 ===
    
    private static void TestHadokenInput(CommandInputManager manager)
    {
        // ↓
        manager.OnKeyPressed(KEY_DOWN);
        manager.OnKeyReleased(KEY_DOWN);
        System.Threading.Thread.Sleep(100);
        
        // ↘ (DOWN + RIGHT 동시)
        manager.OnKeyPressed(KEY_DOWN);
        manager.OnKeyPressed(KEY_RIGHT);
        manager.OnKeyReleased(KEY_DOWN);
        manager.OnKeyReleased(KEY_RIGHT);
        System.Threading.Thread.Sleep(100);
        
        // →
        manager.OnKeyPressed(KEY_RIGHT);
        manager.OnKeyReleased(KEY_RIGHT);
        System.Threading.Thread.Sleep(100);
        
        // P
        manager.OnKeyPressed(KEY_PUNCH);
        manager.OnKeyReleased(KEY_PUNCH);
    }

    private static void TestSimultaneousInput(CommandInputManager manager)
    {
        Console.WriteLine("   A와 B를 거의 동시에 입력...");
        
        // A + B 거의 동시에 (30ms 차이)
        manager.OnKeyPressed(KEY_A);
        System.Threading.Thread.Sleep(30);
        manager.OnKeyPressed(KEY_B);
        
        System.Threading.Thread.Sleep(100); // 잠시 대기
        
        manager.OnKeyReleased(KEY_A);
        manager.OnKeyReleased(KEY_B);
        
        Console.WriteLine("   동시 입력 테스트 완료");
    }

    private static void TestBasicCombo(CommandInputManager manager)
    {
        // A → B → X 순차 입력
        manager.OnKeyPressed(KEY_A);
        manager.OnKeyReleased(KEY_A);
        System.Threading.Thread.Sleep(200);
        
        manager.OnKeyPressed(KEY_B);
        manager.OnKeyReleased(KEY_B);
        System.Threading.Thread.Sleep(200);
        
        manager.OnKeyPressed(KEY_X);
        manager.OnKeyReleased(KEY_X);
    }
}

// === 게임에서 실제 사용 예제 ===
public class FightingGameController
{
    private CommandInputManager _commandManager;
    private PlayerCharacter _player;

    public FightingGameController(PlayerCharacter player)
    {
        _player = player;
        _commandManager = new CommandInputManager();
        SetupGameCommands();
    }

    private void SetupGameCommands()
    {
        // 커맨드 완료 이벤트 연결
        _commandManager.CommandCompleted += ExecutePlayerAction;

        // 게임 커맨드들 등록
        RegisterFightingCommands();
    }

    private void RegisterFightingCommands()
    {
        // 필살기들
        var hadoken = new Command("하둥권", 800);
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, 40)); // DOWN
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, 40, 39)); // DOWN+RIGHT
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 200, 39)); // RIGHT
        hadoken.AddStep(new InputStep(CommandInputType.Sequential, 300, 80)); // PUNCH
        _commandManager.RegisterCommand(hadoken);

        // 동시 입력 커맨드
        _commandManager.RegisterSimultaneousCommand("던지기", 500, 65, 66); // A+B
        _commandManager.RegisterSimultaneousCommand("가드캔슬", 300, 66, 88); // B+X
    }

    private void ExecutePlayerAction(object sender, CommandCompletedEventArgs e)
    {
        // 실제 게임 액션 실행
        switch (e.CommandName)
        {
            case "하둥권":
                _player.ExecuteSpecialMove("Hadoken");
                break;
            case "던지기":
                _player.ExecuteThrow();
                break;
            case "가드캔슬":
                _player.ExecuteGuardCancel();
                break;
        }
    }

    // 게임 루프에서 호출
    public void HandleKeyInput(int keyCode, bool isPressed)
    {
        if (isPressed)
            _commandManager.OnKeyPressed(keyCode);
        else
            _commandManager.OnKeyReleased(keyCode);
    }
}

// 플레이어 캐릭터 예시 클래스
public class PlayerCharacter
{
    public void ExecuteSpecialMove(string moveName)
    {
        Console.WriteLine($"🥋 {moveName} 필살기 발동!");
        // 애니메이션, 이펙트, 데미지 처리 등
    }

    public void ExecuteThrow()
    {
        Console.WriteLine("🤼 던지기 실행!");
        // 던지기 로직
    }

    public void ExecuteGuardCancel()
    {
        Console.WriteLine("🛡️ 가드 캔슬!");
        // 가드 캔슬 로직
    }
}
}